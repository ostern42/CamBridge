// src\CamBridge.Infrastructure\Services\PacsUploadQueue.cs
// Version: 0.8.0
// Description: Queue for PACS uploads with retry logic
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CamBridge.Core;
using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Item for PACS upload queue
    /// </summary>
    public class PacsUploadItem
    {
        public string DicomFilePath { get; init; } = string.Empty;
        public DateTime EnqueuedAt { get; init; } = DateTime.UtcNow;
        public int RetryCount { get; set; }
        public string? LastError { get; set; }
    }

    /// <summary>
    /// Error information for failed PACS uploads
    /// </summary>
    public class PacsUploadError
    {
        public string DicomFilePath { get; init; } = string.Empty;
        public string ErrorMessage { get; init; } = string.Empty;
        public int AttemptCount { get; init; }
        public DateTime FailedAt { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Queue for PACS uploads per pipeline with retry logic
    /// </summary>
    public class PacsUploadQueue
    {
        private readonly PipelineConfiguration _pipeline;
        private readonly DicomStoreService _storeService;
        private readonly ILogger<PacsUploadQueue> _logger;
        private readonly Channel<PacsUploadItem> _channel;
        private readonly List<PacsUploadError> _errorList = new();
        private readonly object _errorListLock = new();
        private readonly SemaphoreSlim _concurrencyLimit;

        // Statistics
        private int _totalUploaded;
        private int _totalFailed;
        private DateTime _startTime = DateTime.UtcNow;

        public int QueueLength => _channel.Reader.Count;
        public int TotalUploaded => _totalUploaded;
        public int TotalFailed => _totalFailed;
        public List<PacsUploadError> GetErrors()
        {
            lock (_errorListLock)
            {
                return _errorList.ToList();
            }
        }

        public PacsUploadQueue(
            PipelineConfiguration pipeline,
            DicomStoreService storeService,
            ILogger<PacsUploadQueue> logger)
        {
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Create unbounded channel for queue
            _channel = Channel.CreateUnbounded<PacsUploadItem>();

            // Create concurrency limiter
            var maxConcurrent = _pipeline.PacsConfiguration?.MaxConcurrentUploads ?? 1;
            _concurrencyLimit = new SemaphoreSlim(maxConcurrent, maxConcurrent);

            _logger.LogInformation("Created PACS upload queue for pipeline {Pipeline} with concurrency {Max}",
                _pipeline.Name, maxConcurrent);
        }

        /// <summary>
        /// Enqueue DICOM file for upload
        /// </summary>
        public async Task EnqueueAsync(string dicomFilePath)
        {
            if (string.IsNullOrEmpty(dicomFilePath))
                throw new ArgumentException("Path required", nameof(dicomFilePath));

            if (!File.Exists(dicomFilePath))
            {
                _logger.LogWarning("Cannot enqueue non-existent file: {Path}", dicomFilePath);
                return;
            }

            var item = new PacsUploadItem { DicomFilePath = dicomFilePath };
            await _channel.Writer.WriteAsync(item);

            _logger.LogDebug("Enqueued for PACS upload: {File} (queue depth: {Depth})",
                Path.GetFileName(dicomFilePath), _channel.Reader.Count);
        }

        /// <summary>
        /// Process upload queue
        /// </summary>
        public async Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("PACS upload queue started for pipeline {Pipeline}", _pipeline.Name);

            var uploadTasks = new List<Task>();

            try
            {
                await foreach (var item in _channel.Reader.ReadAllAsync(cancellationToken))
                {
                    // Wait for available slot
                    await _concurrencyLimit.WaitAsync(cancellationToken);

                    // Start upload task
                    var uploadTask = Task.Run(async () =>
                    {
                        try
                        {
                            await ProcessUploadAsync(item, cancellationToken);
                        }
                        finally
                        {
                            _concurrencyLimit.Release();
                        }
                    }, cancellationToken);

                    uploadTasks.Add(uploadTask);

                    // Clean up completed tasks
                    uploadTasks.RemoveAll(t => t.IsCompleted);
                }

                // Wait for remaining uploads
                await Task.WhenAll(uploadTasks);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("PACS upload queue cancelled for pipeline {Pipeline}", _pipeline.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PACS upload queue error for pipeline {Pipeline}", _pipeline.Name);
            }
            finally
            {
                _logger.LogInformation("PACS upload queue stopped for pipeline {Pipeline}. " +
                    "Uploaded: {Success}, Failed: {Failed}",
                    _pipeline.Name, _totalUploaded, _totalFailed);
            }
        }

        private async Task ProcessUploadAsync(PacsUploadItem item, CancellationToken cancellationToken)
        {
            var pacsConfig = _pipeline.PacsConfiguration;
            if (pacsConfig == null || !pacsConfig.Enabled)
            {
                _logger.LogWarning("PACS configuration missing or disabled for pipeline {Pipeline}",
                    _pipeline.Name);
                return;
            }

            var fileName = Path.GetFileName(item.DicomFilePath);
            _logger.LogInformation("Processing PACS upload for {File} (attempt {Attempt})",
                fileName, item.RetryCount + 1);

            try
            {
                // Perform upload with retry
                var result = await _storeService.StoreFileWithRetryAsync(
                    item.DicomFilePath,
                    pacsConfig,
                    cancellationToken);

                if (result.Success)
                {
                    Interlocked.Increment(ref _totalUploaded);
                    _logger.LogInformation("Successfully uploaded {File} to PACS. Transaction: {Uid}",
                        fileName, result.TransactionUid);
                }
                else
                {
                    // Max retries reached
                    Interlocked.Increment(ref _totalFailed);
                    AddToErrorList(item, result.ErrorMessage ?? "Unknown error");

                    _logger.LogError("Failed to upload {File} to PACS after retries: {Error}",
                        fileName, result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _totalFailed);
                AddToErrorList(item, ex.Message);

                _logger.LogError(ex, "Exception during PACS upload for {File}", fileName);
            }
        }

        private void AddToErrorList(PacsUploadItem item, string error)
        {
            var errorEntry = new PacsUploadError
            {
                DicomFilePath = item.DicomFilePath,
                ErrorMessage = error,
                AttemptCount = item.RetryCount + 1,
                FailedAt = DateTime.UtcNow
            };

            lock (_errorListLock)
            {
                _errorList.Add(errorEntry);

                // Keep list size reasonable
                if (_errorList.Count > 1000)
                {
                    _logger.LogWarning("PACS error list exceeded 1000 entries, removing oldest 500");
                    _errorList.RemoveRange(0, 500);
                }
            }
        }

        /// <summary>
        /// Clear error list
        /// </summary>
        public void ClearErrors()
        {
            lock (_errorListLock)
            {
                _logger.LogInformation("Clearing {Count} PACS upload errors", _errorList.Count);
                _errorList.Clear();
            }
        }

        /// <summary>
        /// Save error list to file for persistence
        /// </summary>
        public async Task SaveErrorsToFileAsync(string filePath)
        {
            try
            {
                List<PacsUploadError> errors;
                lock (_errorListLock)
                {
                    errors = _errorList.ToList();
                }

                var lines = new List<string>
                {
                    $"PACS Upload Errors for Pipeline: {_pipeline.Name}",
                    $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    $"Total Errors: {errors.Count}",
                    new string('-', 80)
                };

                foreach (var error in errors)
                {
                    lines.Add($"File: {error.DicomFilePath}");
                    lines.Add($"Error: {error.ErrorMessage}");
                    lines.Add($"Attempts: {error.AttemptCount}");
                    lines.Add($"Failed At: {error.FailedAt:yyyy-MM-dd HH:mm:ss}");
                    lines.Add(string.Empty);
                }

                await File.WriteAllLinesAsync(filePath, lines);
                _logger.LogInformation("Saved {Count} PACS errors to {Path}", errors.Count, filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save PACS errors to file");
            }
        }
    }
}
