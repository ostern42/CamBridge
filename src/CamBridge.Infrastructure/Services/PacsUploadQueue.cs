// src/CamBridge.Infrastructure/Services/PacsUploadQueue.cs
// Version: 0.8.6
// Modified: Session 96 - Fixed StoreFileAsync call signature
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
        private int _queueLength = 0;

        /// <summary>
        /// Current number of items in the upload queue
        /// </summary>
        public int QueueLength => _queueLength;

        public PacsUploadQueue(
            PipelineConfiguration pipelineConfig,
            DicomStoreService storeService,
            ILogger<PacsUploadQueue> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            _pipelineConfig = pipelineConfig ?? throw new ArgumentNullException(nameof(pipelineConfig));

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

            _logger.LogInformation(
                "[PacsUpload] Starting upload processor with {MaxConcurrent} concurrent uploads [{Pipeline}]",
                maxConcurrent, _pipelineConfig.Name);

            // Use semaphore to limit concurrent uploads
            using var semaphore = new SemaphoreSlim(maxConcurrent, maxConcurrent);

            await foreach (var item in _queue.Reader.ReadAllAsync(cancellationToken))
            {
                Interlocked.Decrement(ref _queueLength);
                // Don't await - process concurrently up to limit
                _ = ProcessUploadAsync(item, pacsConfig, semaphore, cancellationToken);
            }
        }

        private async Task ProcessUploadAsync(
            PacsUploadItem item,
            PacsConfiguration pacsConfig,
            SemaphoreSlim semaphore,
            CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                await UploadWithRetryAsync(item, pacsConfig, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task UploadWithRetryAsync(
            PacsUploadItem item,
            PacsConfiguration pacsConfig,
            CancellationToken cancellationToken)
        {
            var startTime = DateTime.UtcNow;
            var maxAttempts = pacsConfig.RetryOnFailure ? pacsConfig.MaxRetryAttempts : 1;
            StoreResult? lastResult = null;

            // Check if file still exists
            if (!File.Exists(item.DicomFilePath))
            {
                _logger.LogWarning(
                    "[{CorrelationId}] [PacsUpload] File no longer exists: {Path} [{Pipeline}]",
                    item.CorrelationId, item.DicomFilePath, _pipelineConfig.Name);
                return;
            }

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogInformation(
                            "[{CorrelationId}] [PacsUpload] Upload cancelled [{Pipeline}]",
                            item.CorrelationId, _pipelineConfig.Name);
                        return;
                    }

                    if (attempt > 1)
                    {
                        _logger.LogInformation(
                            "[{CorrelationId}] [PacsUpload] Retry attempt {Attempt}/{Max} [{Pipeline}]",
                            item.CorrelationId, attempt, maxAttempts, _pipelineConfig.Name);
                        // Wait before retry
                        await Task.Delay(TimeSpan.FromSeconds(pacsConfig.RetryDelaySeconds), cancellationToken);
                    }

                    // FIXED: Use the correct method signature with PacsConfiguration
                    var result = await _storeService.StoreFileAsync(
                        item.DicomFilePath,
                        pacsConfig);

                    if (result.Success)
                    {
                        var duration = DateTime.UtcNow - startTime;
                        _logger.LogInformation(
                            "[{CorrelationId}] [PacsUpload] Upload successful to {Host}:{Port} ({Duration:F1}s) [{Pipeline}]",
                            item.CorrelationId, pacsConfig.Host, pacsConfig.Port, duration.TotalSeconds, _pipelineConfig.Name);
                        return;
                    }

                    lastResult = result;
                    _logger.LogWarning(
                        "[{CorrelationId}] [PacsUpload] Upload failed (attempt {Attempt}/{Max}): {Error} [{Pipeline}]",
                        item.CorrelationId, attempt, maxAttempts, result.ErrorMessage, _pipelineConfig.Name);
                }
                catch (Exception ex)
                {
                    lastResult = StoreResult.CreateFailure(
                        ex.Message,
                        "Upload failed with exception",
                        DicomErrorType.Unknown);

                    _logger.LogError(ex,
                        "[{CorrelationId}] [PacsUpload] Upload exception (attempt {Attempt}/{Max}) [{Pipeline}]",
                        item.CorrelationId, attempt, maxAttempts, _pipelineConfig.Name);
                }
            }

            // All attempts failed
            var totalDuration = DateTime.UtcNow - startTime;
            _logger.LogError(
                "[{CorrelationId}] [PacsUpload] Upload failed after {Attempts} attempts ({Duration:F1}s): {Error} [{Pipeline}]",
                item.CorrelationId, maxAttempts, totalDuration.TotalSeconds,
                lastResult?.ErrorMessage ?? "Unknown error", _pipelineConfig.Name);

            // Move to error folder if configured
            await HandleFailedUploadAsync(item, lastResult);
        }

        private async Task HandleFailedUploadAsync(PacsUploadItem item, StoreResult? lastResult)
        {
            try
            {
                // TODO: Move to PACS error folder if configured
                // For now, just log
                _logger.LogInformation(
                    "[{CorrelationId}] [PacsUpload] Failed upload handling complete [{Pipeline}]",
                    item.CorrelationId, _pipelineConfig.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[{CorrelationId}] [PacsUpload] Error handling failed upload [{Pipeline}]",
                    item.CorrelationId, _pipelineConfig.Name);
            }
        }

        public void Dispose()
        {
            try
            {
                // Signal completion and wait for processing to finish
                _queue.Writer.TryComplete();
                _cts.Cancel();

                // Give some time for graceful shutdown
                if (!_processingTask.Wait(TimeSpan.FromSeconds(30)))
                {
                    _logger.LogWarning(
                        "[PacsUpload] Upload queue did not shut down gracefully [{Pipeline}]",
                        _pipelineConfig.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[PacsUpload] Error during queue disposal [{Pipeline}]",
                    _pipelineConfig.Name);
            }
            finally
            {
                _cts.Dispose();
            }
        }

        private class PacsUploadItem
        {
            public required string DicomFilePath { get; set; }
            public required string CorrelationId { get; set; }
            public DateTime QueuedAt { get; set; }
        }
    }
}
