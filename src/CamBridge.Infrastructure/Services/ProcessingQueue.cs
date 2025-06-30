// src/CamBridge.Infrastructure/Services/ProcessingQueue.cs
// Version: 0.8.8
// Description: Thread-safe queue with duplicate detection and NULL check fix
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
    /// FIXED: Prevents duplicate processing of successfully completed files
    /// FIXED: NullReferenceException in StopAsync
    /// </summary>
    public class ProcessingQueue
    {
        private readonly ILogger<ProcessingQueue> _logger;
        private readonly FileProcessor _fileProcessor;
        private readonly ProcessingOptions _options;
        private readonly ConcurrentQueue<ProcessingItem> _queue = new();
        private readonly ConcurrentDictionary<string, ProcessingItem> _activeItems = new();
        private readonly SemaphoreSlim _processingSlots;
        private CancellationTokenSource? _cancellationSource;  // FIXED: Nullable
        private Task? _processingTask;

        // FIX: Track processed and enqueued files to prevent duplicates
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
            IOptions<ProcessingOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileProcessor = fileProcessor ?? throw new ArgumentNullException(nameof(fileProcessor));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

            _processingSlots = new SemaphoreSlim(
                _options.MaxConcurrentProcessing,
                _options.MaxConcurrentProcessing);
        }

        /// <summary>
        /// Enqueues a file for processing
        /// </summary>
        public bool TryEnqueue(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            lock (_trackingLock)
            {
                // FIX: Check if already successfully processed
                if (_processedFiles.Contains(filePath))
                {
                    _logger.LogDebug("File {FilePath} already processed, ignoring duplicate event", filePath);
                    return false;
                }

                // FIX: Check if already in queue
                if (_enqueuedFiles.Contains(filePath))
                {
                    _logger.LogDebug("File {FilePath} already in queue, ignoring duplicate event", filePath);
                    return false;
                }

                // Check if already being processed
                if (_activeItems.ContainsKey(filePath))
                {
                    _logger.LogDebug("File {FilePath} is currently being processed", filePath);
                    return false;
                }

                // Add to tracking
                _enqueuedFiles.Add(filePath);
            }

            // Check file size constraints if needed
            // TODO: Add MinFileSizeBytes/MaxFileSizeBytes to ProcessingOptions if needed

            var item = new ProcessingItem(filePath);
            _queue.Enqueue(item);

            _logger.LogInformation("Enqueued {FilePath} for processing (queue length: {QueueLength})",
                filePath, _queue.Count);

            return true;
        }

        /// <summary>
        /// Async wrapper for TryEnqueue to match PipelineManager expectations
        /// </summary>
        public Task<bool> EnqueueAsync(string filePath, CancellationToken cancellationToken = default)
        {
            var result = TryEnqueue(filePath);
            return Task.FromResult(result);
        }

        /// <summary>
        /// Processes items from the queue
        /// </summary>
        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing queue started");

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
                                _logger.LogError(ex, "Unexpected error in background processing");
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
            finally
            {
                _logger.LogInformation("Processing queue stopped");
            }
        }

        /// <summary>
        /// Compatibility wrapper for PipelineManager that expects ProcessQueueAsync
        /// </summary>
        public Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            _cancellationSource = new CancellationTokenSource();  // FIXED: Create here
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
            // FIXED: Null checks added!
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

                _logger.LogInformation("Starting processing of {FilePath} (attempt {Attempt})",
                    item.FilePath, item.AttemptCount);

                // Process the file with THIS pipeline's FileProcessor!
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
                    // FIX: Mark as successfully processed
                    lock (_trackingLock)
                    {
                        _processedFiles.Add(item.FilePath);
                        _enqueuedFiles.Remove(item.FilePath);

                        // Cleanup old entries if too many (prevent memory leak)
                        if (_processedFiles.Count > 10000)
                        {
                            _logger.LogInformation("Cleaning up processed files tracking (>10000 entries)");
                            _processedFiles.Clear();
                        }
                    }
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

                    _logger.LogError("Failed to process {FilePath} after {Attempts} attempts: {Error}",
                        item.FilePath, item.AttemptCount, result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing {FilePath}", item.FilePath);

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

                    _logger.LogError("Failed to process {FilePath} after {Attempts} attempts",
                        item.FilePath, item.AttemptCount);
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

            _logger.LogInformation("Scheduling retry for {FilePath} in {Delay} seconds",
                item.FilePath, delay.TotalSeconds);

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
        /// Processing item with retry tracking
        /// </summary>
        private class ProcessingItem
        {
            public string FilePath { get; }
            public int AttemptCount { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime EnqueuedTime { get; }

            public ProcessingItem(string filePath)
            {
                FilePath = filePath;
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
