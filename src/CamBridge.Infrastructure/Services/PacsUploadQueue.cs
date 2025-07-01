// src/CamBridge.Infrastructure/Services/PacsUploadQueue.cs
// Version: 0.8.10
// Modified: Session 110 - Fixed correlation ID usage
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
        private readonly string _correlationId;
        private int _queueLength = 0;

        /// <summary>
        /// Current number of items in the upload queue
        /// </summary>
        public int QueueLength => _queueLength;

        public PacsUploadQueue(
            PipelineConfiguration pipelineConfig,
            DicomStoreService storeService,
            ILogger<PacsUploadQueue> logger,
            string correlationId)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            _pipelineConfig = pipelineConfig ?? throw new ArgumentNullException(nameof(pipelineConfig));
            _correlationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));

            if (_pipelineConfig.PacsConfiguration == null)
                throw new ArgumentException("Pipeline must have PACS configuration", nameof(pipelineConfig));

            // Create unbounded channel for queue
            _queue = Channel.CreateUnbounded<PacsUploadItem>(new UnboundedChannelOptions
            {
                SingleReader = true
            });

            _cts = new CancellationTokenSource();
            _processingTask = ProcessQueueAsync(_cts.Token);
        }

        /// <summary>
        /// Queue a DICOM file for upload to PACS
        /// </summary>
        public async Task<bool> EnqueueAsync(string dicomFilePath, string correlationId)
        {
            if (!File.Exists(dicomFilePath))
            {
                _logger.LogWarning(
                    "[{CorrelationId}] [PacsUpload] File not found for upload: {Path} [{Pipeline}]",
                    correlationId, dicomFilePath, _pipelineConfig.Name);
                return false;
            }

            var item = new PacsUploadItem
            {
                DicomFilePath = dicomFilePath,
                CorrelationId = correlationId,
                QueuedAt = DateTime.UtcNow
            };

            try
            {
                await _queue.Writer.WriteAsync(item);
                Interlocked.Increment(ref _queueLength);

                _logger.LogInformation(
                    "[{CorrelationId}] [PacsUpload] File queued for upload (queue depth: {QueueDepth}) [{Pipeline}]",
                    correlationId, _queueLength, _pipelineConfig.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[{CorrelationId}] [PacsUpload] Failed to queue file [{Pipeline}]",
                    correlationId, _pipelineConfig.Name);
                return false;
            }
        }

        private async Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            var pacsConfig = _pipelineConfig.PacsConfiguration!;
            var maxConcurrent = Math.Max(1, Math.Min(pacsConfig.MaxConcurrentUploads, 10));

            // FIXED: Don't use _correlationId here! This is a one-time startup log
            // We create a specific correlation ID for PACS initialization
            var pacsInitCorrelationId = $"PM{DateTime.Now:HHmmssff}-PACS-{_pipelineConfig.Name}";

            _logger.LogInformation(
                "[{CorrelationId}] [PacsInit] Starting upload processor with {MaxConcurrent} concurrent uploads [{Pipeline}]",
                pacsInitCorrelationId, maxConcurrent, _pipelineConfig.Name);

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
                // Use a shutdown-specific correlation ID
                var shutdownCorrelationId = $"PM{DateTime.Now:HHmmssff}-PACS-SHUTDOWN-{_pipelineConfig.Name}";
                _logger.LogInformation(
                    "[{CorrelationId}] [PacsShutdown] Upload processor cancelled [{Pipeline}]",
                    shutdownCorrelationId, _pipelineConfig.Name);
            }
            catch (Exception ex)
            {
                // Use an error-specific correlation ID
                var errorCorrelationId = $"PM{DateTime.Now:HHmmssff}-PACS-ERROR-{_pipelineConfig.Name}";
                _logger.LogError(ex,
                    "[{CorrelationId}] [PacsError] Upload processor failed [{Pipeline}]",
                    errorCorrelationId, _pipelineConfig.Name);
            }
        }

        private async Task ProcessUploadAsync(PacsUploadItem item, PacsConfiguration pacsConfig, CancellationToken cancellationToken)
        {
            var correlationId = item.CorrelationId;
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
                    // Use a disposal-specific correlation ID
                    var disposeCorrelationId = $"PM{DateTime.Now:HHmmssff}-PACS-DISPOSE-{_pipelineConfig.Name}";
                    _logger.LogWarning(
                        "[{CorrelationId}] [PacsShutdown] Upload processor did not complete within timeout [{Pipeline}]",
                        disposeCorrelationId, _pipelineConfig.Name);
                }
            }
            catch (Exception ex)
            {
                // Use an error-specific correlation ID
                var errorCorrelationId = $"PM{DateTime.Now:HHmmssff}-PACS-DISPOSE-ERROR-{_pipelineConfig.Name}";
                _logger.LogError(ex,
                    "[{CorrelationId}] [PacsShutdown] Error during disposal [{Pipeline}]",
                    errorCorrelationId, _pipelineConfig.Name);
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
