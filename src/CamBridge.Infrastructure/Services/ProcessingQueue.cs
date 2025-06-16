// src/CamBridge.Infrastructure/Services/ProcessingQueue.cs
// Version: 0.7.20
// Description: Thread-safe queue with pipeline-specific FileProcessor!
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
    /// PIPELINE UPDATE: Uses pipeline-specific FileProcessor directly!
    /// </summary>
    public class ProcessingQueue
    {
        private readonly ILogger<ProcessingQueue> _logger;
        private readonly FileProcessor _fileProcessor; // Direct dependency!
        private readonly ProcessingOptions _options;
        private readonly ConcurrentQueue<ProcessingItem> _queue = new();
        private readonly ConcurrentDictionary<string, ProcessingItem> _activeItems = new();
        private readonly SemaphoreSlim _processingSlots;
        private readonly object _statsLock = new();
        private readonly Dictionary<string, int> _errorCounts = new();

        private int _totalProcessed;
        private int _totalSuccessful;
        private int _totalFailed;
        private DateTime _startTime = DateTime.UtcNow;

        public int QueueLength => _queue.Count;
        public int ActiveProcessing => _activeItems.Count;
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

            // Check if already in queue or being processed
            if (_activeItems.ContainsKey(filePath))
            {
                _logger.LogDebug("File {FilePath} is already being processed", filePath);
                return false;
            }

            if (_queue.Any(item => item.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogDebug("File {FilePath} is already in queue", filePath);
                return false;
            }

            // Check if file should be processed using THIS pipeline's FileProcessor
            if (!_fileProcessor.ShouldProcessFile(filePath))
            {
                _logger.LogDebug("File {FilePath} does not meet processing criteria", filePath);
                return false;
            }

            var item = new ProcessingItem(filePath);
            _queue.Enqueue(item);

            _logger.LogInformation("Enqueued {FilePath} for processing (queue length: {QueueLength})",
                filePath, _queue.Count);

            return true;
        }

        /// <summary>
        /// Processes items from the queue
        /// </summary>
        public async Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            while (!cancellationToken.IsCancellationRequested)
            {
                // Clean up completed tasks
                tasks.RemoveAll(t => t.IsCompleted);

                // Try to dequeue and process
                if (_queue.TryDequeue(out var item))
                {
                    // Wait for available processing slot
                    await _processingSlots.WaitAsync(cancellationToken);

                    // Start processing task
                    var task = ProcessItemAsync(item, cancellationToken);
                    tasks.Add(task);
                }
                else
                {
                    // No items in queue, wait a bit
                    await Task.Delay(100, cancellationToken);
                }
            }

            // Wait for all remaining tasks to complete on shutdown
            if (tasks.Count > 0)
            {
                _logger.LogInformation("Waiting for {Count} active processing tasks to complete", tasks.Count);
                await Task.WhenAll(tasks);
            }
        }

        /// <summary>
        /// Gets current queue statistics
        /// </summary>
        public QueueStatistics GetStatistics()
        {
            lock (_statsLock)
            {
                return new QueueStatistics
                {
                    QueueLength = QueueLength,
                    ActiveProcessing = ActiveProcessing,
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

        /// <summary>
        /// Gets daily summary for notifications
        /// </summary>
        public ProcessingSummary GetDailySummary()
        {
            var stats = GetStatistics();

            return new ProcessingSummary
            {
                Date = DateTime.Today,
                TotalProcessed = stats.TotalProcessed,
                Successful = stats.TotalSuccessful,
                Failed = stats.TotalFailed,
                ProcessingTimeSeconds = stats.UpTime.TotalSeconds,
                TopErrors = stats.TopErrors,
                Uptime = stats.UpTime
            };
        }

        /// <summary>
        /// Gets items currently being processed
        /// </summary>
        public IReadOnlyList<ProcessingItemStatus> GetActiveItems()
        {
            return _activeItems.Values
                .Select(item => new ProcessingItemStatus
                {
                    FilePath = item.FilePath,
                    StartTime = item.StartTime,
                    AttemptCount = item.AttemptCount,
                    Duration = item.StartTime.HasValue
                        ? DateTime.UtcNow - item.StartTime.Value
                        : TimeSpan.Zero
                })
                .ToList()
                .AsReadOnly();
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

                if (!result.Success && ShouldRetry(item))
                {
                    // Schedule retry
                    await ScheduleRetryAsync(item, cancellationToken);
                }
                else if (!result.Success)
                {
                    // KISS: File already moved to error folder by FileProcessor!
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
                    // Log final failure - file should already be in error folder
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
    public class QueueStatistics
    {
        public int QueueLength { get; init; }
        public int ActiveProcessing { get; init; }
        public int TotalProcessed { get; init; }
        public int TotalSuccessful { get; init; }
        public int TotalFailed { get; init; }
        public TimeSpan UpTime { get; init; }
        public double ProcessingRate { get; init; }
        public Dictionary<string, int> TopErrors { get; init; } = new();

        public double SuccessRate => TotalProcessed > 0
            ? (double)TotalSuccessful / TotalProcessed * 100
            : 0;
    }

    public class ProcessingItemStatus
    {
        public string FilePath { get; init; } = string.Empty;
        public DateTime? StartTime { get; init; }
        public int AttemptCount { get; init; }
        public TimeSpan Duration { get; init; }
    }
}
