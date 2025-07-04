// src/CamBridge.Infrastructure/Services/ProcessingQueue.cs
// Version: 0.8.21
// Description: Thread-safe queue with PROPER correlation ID usage
// Session: 125 - Fixed to accept and use pipeline init correlation ID
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Thread-safe queue for managing file processing with retry logic
    /// FIXED: Now accepts pipeline correlation ID for proper hierarchy
    /// </summary>
    public class ProcessingQueue
    {
        private readonly ILogger<ProcessingQueue> _logger;
        private readonly FileProcessor _fileProcessor;
        private readonly ProcessingOptions _options;
        private readonly string _pipelineName;
        private readonly string _pipelineCorrelationId;  // NEW: Store pipeline init ID
        private readonly ConcurrentQueue<ProcessingItem> _queue = new();
        private readonly ConcurrentDictionary<string, ProcessingItem> _activeItems = new();
        private readonly SemaphoreSlim _processingSlots;
        private CancellationTokenSource? _cancellationSource;
        private Task? _processingTask;

        // Track processed and enqueued files to prevent duplicates
        private readonly HashSet<string> _processedFiles = new();
        private readonly HashSet<string> _enqueuedFiles = new();
        private readonly object _trackingLock = new object();

        // Statistics
        private int _totalProcessed;
        private int _totalSuccessful;
        private int _totalFailed;
        private readonly DateTime _startTime = DateTime.UtcNow;
        private readonly ConcurrentDictionary<string, int> _errorCounts = new();
        private readonly object _statsLock = new object();

        // Public statistics properties
        public int QueueLength => _queue.Count;
        public int ActiveCount => _activeItems.Count;
        public int TotalProcessed => _totalProcessed;
        public int TotalSuccessful => _totalSuccessful;
        public int TotalFailed => _totalFailed;
        public TimeSpan UpTime => DateTime.UtcNow - _startTime;

        /// <summary>
        /// Creates a ProcessingQueue with a specific FileProcessor for this pipeline
        /// </summary>
        public ProcessingQueue(
            ILogger<ProcessingQueue> logger,
            FileProcessor fileProcessor,
            IOptions<ProcessingOptions> options,
            string pipelineName,
            string pipelineCorrelationId)  // NEW PARAMETER!
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileProcessor = fileProcessor ?? throw new ArgumentNullException(nameof(fileProcessor));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _pipelineName = pipelineName ?? "Unknown";
            _pipelineCorrelationId = pipelineCorrelationId ?? throw new ArgumentNullException(nameof(pipelineCorrelationId));

            _processingSlots = new SemaphoreSlim(
                _options.MaxConcurrentProcessing,
                _options.MaxConcurrentProcessing);

            // Log initialization with the pipeline correlation ID
            _logger.LogInformation("[{CorrelationId}] [QueueInit] Processing queue created for pipeline {Pipeline} with max concurrent: {MaxConcurrent}",
                _pipelineCorrelationId, _pipelineName, _options.MaxConcurrentProcessing);
        }

        /// <summary>
        /// Enqueues a file for processing with its own correlation ID
        /// </summary>
        public bool TryEnqueue(string filePath, string fileCorrelationId = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            // If no file correlation ID provided, generate one
            if (string.IsNullOrEmpty(fileCorrelationId))
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var filePrefix = fileName.Length > 8 ? fileName.Substring(0, 8) : fileName;
                fileCorrelationId = $"F{DateTime.Now:HHmmssff}-{filePrefix}";
            }

            lock (_trackingLock)
            {
                // Check if already successfully processed
                if (_processedFiles.Contains(filePath))
                {
                    _logger.LogDebug("[{CorrelationId}] [QueueDuplicate] File {FilePath} already processed, ignoring duplicate event [{Pipeline}]",
                        fileCorrelationId, filePath, _pipelineName);
                    return false;
                }

                // Check if already in queue
                if (_enqueuedFiles.Contains(filePath))
                {
                    _logger.LogDebug("[{CorrelationId}] [QueueDuplicate] File {FilePath} already in queue, ignoring duplicate event [{Pipeline}]",
                        fileCorrelationId, filePath, _pipelineName);
                    return false;
                }

                // Check if already being processed
                if (_activeItems.ContainsKey(filePath))
                {
                    _logger.LogDebug("[{CorrelationId}] [QueueActive] File {FilePath} is currently being processed [{Pipeline}]",
                        fileCorrelationId, filePath, _pipelineName);
                    return false;
                }

                // Add to tracking
                _enqueuedFiles.Add(filePath);
            }

            var item = new ProcessingItem(filePath, fileCorrelationId);
            _queue.Enqueue(item);

            _logger.LogInformation("[{CorrelationId}] [QueueEnqueue] Enqueued {FilePath} for processing (queue length: {QueueLength}) [{Pipeline}]",
                fileCorrelationId, filePath, _queue.Count, _pipelineName);

            return true;
        }

        /// <summary>
        /// Async wrapper for TryEnqueue to match PipelineManager expectations
        /// </summary>
        public Task<bool> EnqueueAsync(string filePath, string fileCorrelationId = null, CancellationToken cancellationToken = default)
        {
            var result = TryEnqueue(filePath, fileCorrelationId);
            return Task.FromResult(result);
        }

        /// <summary>
        /// Processes items from the queue
        /// </summary>
        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            // Use the pipeline correlation ID for queue lifecycle events
            _logger.LogInformation("[{CorrelationId}] [QueueStart] Processing queue started [{Pipeline}]",
                _pipelineCorrelationId, _pipelineName);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_queue.TryDequeue(out var item))
                    {
                        await _processingSlots.WaitAsync(cancellationToken);

                        // Fire and forget - process in background
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await ProcessItemAsync(item, cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "[{CorrelationId}] [QueueError] Unexpected error in background processing [{Pipeline}]",
                                    item.CorrelationId, _pipelineName);
                            }
                        }, cancellationToken);
                    }
                    else
                    {
                        // No items to process, wait a bit
                        await Task.Delay(100, cancellationToken);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // This is normal during shutdown - use pipeline correlation ID
                _logger.LogInformation("[{CorrelationId}] [QueueCancelled] Processing queue cancelled [{Pipeline}]",
                    _pipelineCorrelationId, _pipelineName);
                throw;  // Re-throw so PipelineManager can handle it
            }
            finally
            {
                // Use pipeline correlation ID for lifecycle events
                _logger.LogInformation("[{CorrelationId}] [QueueStop] Processing queue stopped [{Pipeline}]",
                    _pipelineCorrelationId, _pipelineName);
            }
        }

        /// <summary>
        /// Compatibility wrapper for PipelineManager that expects ProcessQueueAsync
        /// </summary>
        public Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            _cancellationSource = new CancellationTokenSource();
            return ProcessAsync(cancellationToken);
        }

        /// <summary>
        /// Starts the background processing
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationSource = new CancellationTokenSource();
            _processingTask = ProcessAsync(_cancellationSource.Token);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the background processing
        /// </summary>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationSource?.Cancel();

            if (_processingTask != null)
            {
                try
                {
                    await _processingTask;
                }
                catch (OperationCanceledException)
                {
                    // Expected when canceling
                }
            }

            // Dispose cancellation source
            _cancellationSource?.Dispose();
            _cancellationSource = null;
        }

        /// <summary>
        /// Gets current statistics
        /// </summary>
        public ProcessingStatistics GetStatistics()
        {
            lock (_statsLock)
            {
                return new ProcessingStatistics
                {
                    QueueLength = QueueLength,
                    ActiveCount = ActiveCount,
                    TotalProcessed = TotalProcessed,
                    TotalSuccessful = TotalSuccessful,
                    TotalFailed = TotalFailed,
                    UpTime = UpTime,
                    ProcessingRate = TotalProcessed > 0 ? TotalProcessed / UpTime.TotalMinutes : 0,
                    TopErrors = _errorCounts
                        .OrderByDescending(x => x.Value)
                        .Take(5)
                        .ToDictionary(x => x.Key, x => x.Value)
                };
            }
        }

        private async Task ProcessItemAsync(ProcessingItem item, CancellationToken cancellationToken)
        {
            try
            {
                // Mark as active
                _activeItems.TryAdd(item.FilePath, item);
                item.StartTime = DateTime.UtcNow;
                item.AttemptCount++;

                _logger.LogInformation("[{CorrelationId}] [ProcessStart] Starting processing of {FilePath} (attempt {Attempt}) [{Pipeline}]",
                    item.CorrelationId, item.FilePath, item.AttemptCount, _pipelineName);

                // Process the file with THIS pipeline's FileProcessor!
                // FileProcessor will use the file's correlation ID
                var result = await _fileProcessor.ProcessFileAsync(item.FilePath);

                // Update statistics
                lock (_statsLock)
                {
                    _totalProcessed++;
                    if (result.Success)
                    {
                        _totalSuccessful++;
                    }
                    else
                    {
                        _totalFailed++;
                        TrackError(result.ErrorMessage ?? "Unknown error");
                    }
                }

                if (result.Success)
                {
                    // Mark as successfully processed
                    lock (_trackingLock)
                    {
                        _processedFiles.Add(item.FilePath);
                        _enqueuedFiles.Remove(item.FilePath);

                        // Cleanup old entries if too many (prevent memory leak)
                        if (_processedFiles.Count > 10000)
                        {
                            _logger.LogInformation("[{CorrelationId}] [QueueMaintenance] Cleaning up processed files tracking (>10000 entries) [{Pipeline}]",
                                _pipelineCorrelationId, _pipelineName);
                            _processedFiles.Clear();
                        }
                    }

                    _logger.LogInformation("[{CorrelationId}] [ProcessComplete] Successfully processed {FilePath} [{Pipeline}]",
                        item.CorrelationId, item.FilePath, _pipelineName);
                }
                else if (ShouldRetry(item))
                {
                    // Schedule retry
                    await ScheduleRetryAsync(item, cancellationToken);
                }
                else
                {
                    // Final failure - remove from tracking
                    lock (_trackingLock)
                    {
                        _enqueuedFiles.Remove(item.FilePath);
                    }

                    _logger.LogError("[{CorrelationId}] [ProcessFailed] Failed to process {FilePath} after {Attempts} attempts: {Error} [{Pipeline}]",
                        item.CorrelationId, item.FilePath, item.AttemptCount, result.ErrorMessage, _pipelineName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{CorrelationId}] [ProcessError] Unexpected error processing {FilePath} [{Pipeline}]",
                    item.CorrelationId, item.FilePath, _pipelineName);

                lock (_statsLock)
                {
                    _totalProcessed++;
                    _totalFailed++;
                    TrackError(ex.Message);
                }

                if (ShouldRetry(item))
                {
                    await ScheduleRetryAsync(item, cancellationToken);
                }
                else
                {
                    // Final failure - remove from tracking
                    lock (_trackingLock)
                    {
                        _enqueuedFiles.Remove(item.FilePath);
                    }

                    _logger.LogError("[{CorrelationId}] [ProcessAbandoned] Failed to process {FilePath} after {Attempts} attempts [{Pipeline}]",
                        item.CorrelationId, item.FilePath, item.AttemptCount, _pipelineName);
                }
            }
            finally
            {
                // Remove from active items
                _activeItems.TryRemove(item.FilePath, out _);

                // Release processing slot
                _processingSlots.Release();
            }
        }

        private bool ShouldRetry(ProcessingItem item)
        {
            return _options.RetryOnFailure &&
                   item.AttemptCount < _options.MaxRetryAttempts;
        }

        private async Task ScheduleRetryAsync(ProcessingItem item, CancellationToken cancellationToken)
        {
            var delay = TimeSpan.FromSeconds(_options.RetryDelaySeconds * item.AttemptCount);

            _logger.LogInformation("[{CorrelationId}] [RetryScheduled] Scheduling retry for {FilePath} in {Delay} seconds [{Pipeline}]",
                item.CorrelationId, item.FilePath, delay.TotalSeconds, _pipelineName);

            // Wait before re-enqueueing
            await Task.Delay(delay, cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
            {
                item.StartTime = null; // Reset start time
                _queue.Enqueue(item);
            }
        }

        private void TrackError(string error)
        {
            var category = CategorizeError(error);
            _errorCounts.TryGetValue(category, out var count);
            _errorCounts[category] = count + 1;
        }

        private string CategorizeError(string error)
        {
            if (error.Contains("EXIF", StringComparison.OrdinalIgnoreCase))
                return "EXIF extraction failed";
            if (error.Contains("DICOM", StringComparison.OrdinalIgnoreCase))
                return "DICOM conversion failed";
            if (error.Contains("Patient", StringComparison.OrdinalIgnoreCase))
                return "Patient data missing";
            if (error.Contains("File", StringComparison.OrdinalIgnoreCase))
                return "File access error";
            if (error.Contains("Memory", StringComparison.OrdinalIgnoreCase))
                return "Memory error";
            return "Other error";
        }

        /// <summary>
        /// Processing item with retry tracking and correlation ID
        /// </summary>
        private class ProcessingItem
        {
            public string FilePath { get; }
            public string CorrelationId { get; }
            public int AttemptCount { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime EnqueuedTime { get; }

            public ProcessingItem(string filePath, string correlationId)
            {
                FilePath = filePath;
                CorrelationId = correlationId;
                EnqueuedTime = DateTime.UtcNow;
            }
        }
    }

    // Statistics classes remain the same...
    public class ProcessingStatistics
    {
        public int QueueLength { get; set; }
        public int ActiveCount { get; set; }
        public int TotalProcessed { get; set; }
        public int TotalSuccessful { get; set; }
        public int TotalFailed { get; set; }
        public TimeSpan UpTime { get; set; }
        public double ProcessingRate { get; set; }
        public Dictionary<string, int> TopErrors { get; set; } = new();
    }

    public class ProcessingItemStatus
    {
        public string FilePath { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public int AttemptCount { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
