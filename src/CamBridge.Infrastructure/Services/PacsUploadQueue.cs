// src/CamBridge.Infrastructure/Services/PacsUploadQueue.cs
// Version: 0.8.21
// Modified: Session 125 - Fixed to use provided correlation ID consistently
// Purpose: Per-pipeline queue for PACS uploads with retry logic

using System;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CamBridge.Core;
using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Queue for PACS uploads with retry logic per pipeline
    /// </summary>
    public class PacsUploadQueue : IDisposable
    {
        private readonly Channel<PacsUploadItem> _queue;
        private readonly ILogger<PacsUploadQueue> _logger;
        private readonly DicomStoreService _storeService;
        private readonly PipelineConfiguration _pipelineConfig;
        private readonly CancellationTokenSource _cts;
        private readonly Task _processingTask;
        private readonly string _pipelineCorrelationId;  // This is the PIPELINE INIT ID!
        private int _queueLength = 0;

        /// <summary>
        /// Current number of items in the upload queue
        /// </summary>
        public int QueueLength => _queueLength;

        public PacsUploadQueue(
            PipelineConfiguration pipelineConfig,
            DicomStoreService storeService,
            ILogger<PacsUploadQueue> logger,
            string pipelineCorrelationId)  // This should be the PIPELINE INIT ID
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            _pipelineConfig = pipelineConfig ?? throw new ArgumentNullException(nameof(pipelineConfig));
            _pipelineCorrelationId = pipelineCorrelationId ?? throw new ArgumentNullException(nameof(pipelineCorrelationId));

            if (_pipelineConfig.PacsConfiguration == null)
                throw new ArgumentException("Pipeline must have PACS configuration", nameof(pipelineConfig));

            // Create unbounded channel for queue
            _queue = Channel.CreateUnbounded<PacsUploadItem>(new UnboundedChannelOptions
            {
                SingleReader = true
            });

            _cts = new CancellationTokenSource();

            // Log initialization with the PROVIDED pipeline correlation ID
            _logger.LogInformation(
                "[{CorrelationId}] [PacsInit] Starting PACS upload processor for pipeline {Pipeline} with {MaxConcurrent} concurrent uploads",
                _pipelineCorrelationId, _pipelineConfig.Name,
                Math.Max(1, Math.Min(_pipelineConfig.PacsConfiguration.MaxConcurrentUploads, 10)));

            _processingTask = ProcessQueueAsync(_cts.Token);
        }

        /// <summary>
        /// Queue a DICOM file for upload to PACS
        /// </summary>
        public async Task<bool> EnqueueAsync(string dicomFilePath, string fileCorrelationId)
        {
            if (!File.Exists(dicomFilePath))
            {
                _logger.LogWarning(
                    "[{CorrelationId}] [PacsUpload] File not found for upload: {Path} [{Pipeline}]",
                    fileCorrelationId, dicomFilePath, _pipelineConfig.Name);
                return false;
            }

            var item = new PacsUploadItem
            {
                DicomFilePath = dicomFilePath,
                CorrelationId = fileCorrelationId,  // This is the FILE correlation ID
                QueuedAt = DateTime.UtcNow
            };

            try
            {
                await _queue.Writer.WriteAsync(item);
                Interlocked.Increment(ref _queueLength);

                // Log with the FILE correlation ID
                _logger.LogInformation(
                    "[{CorrelationId}] [PacsUpload] File queued for upload (queue depth: {QueueDepth}) [{Pipeline}]",
                    fileCorrelationId, _queueLength, _pipelineConfig.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[{CorrelationId}] [PacsUpload] Failed to queue file [{Pipeline}]",
                    fileCorrelationId, _pipelineConfig.Name);
                return false;
            }
        }

        private async Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            var pacsConfig = _pipelineConfig.PacsConfiguration!;
            var maxConcurrent = Math.Max(1, Math.Min(pacsConfig.MaxConcurrentUploads, 10));

            // Already logged in constructor with pipeline correlation ID

            // Use semaphore to limit concurrent uploads
            using var semaphore = new SemaphoreSlim(maxConcurrent, maxConcurrent);

            try
            {
                await foreach (var item in _queue.Reader.ReadAllAsync(cancellationToken))
                {
                    await semaphore.WaitAsync(cancellationToken);

                    // Don't await - let it run in background within concurrency limit
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await ProcessUploadAsync(item, pacsConfig, cancellationToken);
                        }
                        finally
                        {
                            Interlocked.Decrement(ref _queueLength);
                            semaphore.Release();
                        }
                    }, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown - use the pipeline correlation ID
                _logger.LogInformation(
                    "[{CorrelationId}] [PacsShutdown] Upload processor cancelled [{Pipeline}]",
                    _pipelineCorrelationId, _pipelineConfig.Name);
            }
            catch (Exception ex)
            {
                // Error - use the pipeline correlation ID
                _logger.LogError(ex,
                    "[{CorrelationId}] [PacsError] Upload processor failed [{Pipeline}]",
                    _pipelineCorrelationId, _pipelineConfig.Name);
            }
        }

        private async Task ProcessUploadAsync(PacsUploadItem item, PacsConfiguration pacsConfig, CancellationToken cancellationToken)
        {
            var correlationId = item.CorrelationId;  // This is the FILE correlation ID
            var queueTime = DateTime.UtcNow - item.QueuedAt;

            _logger.LogInformation(
                "[{CorrelationId}] [PacsUpload] Starting upload after {QueueTime:F1}s queue time [{Pipeline}]",
                correlationId, queueTime.TotalSeconds, _pipelineConfig.Name);

            var result = await _storeService.StoreFileWithRetryAsync(
                item.DicomFilePath,
                pacsConfig,
                correlationId,
                cancellationToken);

            if (result.Success)
            {
                _logger.LogInformation(
                    "[{CorrelationId}] [PacsUpload] Successfully uploaded to PACS [{Pipeline}]",
                    correlationId, _pipelineConfig.Name);
            }
            else
            {
                _logger.LogError(
                    "[{CorrelationId}] [PacsUpload] Failed to upload: {ErrorMessage} [{Pipeline}]",
                    correlationId, result.ErrorMessage, _pipelineConfig.Name);
            }
        }

        public void Dispose()
        {
            try
            {
                _cts.Cancel();
                _queue.Writer.TryComplete();

                if (!_processingTask.Wait(TimeSpan.FromSeconds(30)))
                {
                    // Use the pipeline correlation ID for disposal
                    _logger.LogWarning(
                        "[{CorrelationId}] [PacsShutdown] Upload processor did not complete within timeout [{Pipeline}]",
                        _pipelineCorrelationId, _pipelineConfig.Name);
                }
            }
            catch (Exception ex)
            {
                // Use the pipeline correlation ID for disposal errors
                _logger.LogError(ex,
                    "[{CorrelationId}] [PacsShutdown] Error during disposal [{Pipeline}]",
                    _pipelineCorrelationId, _pipelineConfig.Name);
            }
            finally
            {
                _cts.Dispose();
            }
        }

        private class PacsUploadItem
        {
            public required string DicomFilePath { get; init; }
            public required string CorrelationId { get; init; }
            public DateTime QueuedAt { get; init; }
        }
    }
}
