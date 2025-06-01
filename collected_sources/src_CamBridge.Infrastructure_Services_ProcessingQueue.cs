using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Thread-safe queue for managing file processing with retry logic and dead letter support
    /// </summary>
    public class ProcessingQueue
    {
        private readonly ILogger<ProcessingQueue> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ProcessingOptions _options;
        private readonly ConcurrentQueue<ProcessingItem> _queue = new();
        private readonly ConcurrentDictionary<string, ProcessingItem> _activeItems = new();
        private readonly SemaphoreSlim _processingSlots;
        private readonly DeadLetterQueue _deadLetterQueue;
        private readonly INotificationService? _notificationService;
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

        public ProcessingQueue(
            ILogger<ProcessingQueue> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<ProcessingOptions> options,
            DeadLetterQueue deadLetterQueue,
            INotificationService? notificationService = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _deadLetterQueue = deadLetterQueue ?? throw new ArgumentNullException(nameof(deadLetterQueue));
            _notificationService = notificationService;

            _processingSlots = new SemaphoreSlim(
                _options.MaxConcurrentProcessing,
                _options.MaxConcurrentProcessing);

            // Subscribe to dead letter events
            _deadLetterQueue.ThresholdExceeded += OnDeadLetterThresholdExceeded;
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

            // Check if file should be processed using a new scope
            using (var scope = _scopeFactory.CreateScope())
            {
                var fileProcessor = scope.ServiceProvider.GetRequiredService<IFileProcessor>();
                if (!fileProcessor.ShouldProcessFile(filePath))
                {
                    _logger.LogDebug("File {FilePath} does not meet processing criteria", filePath);
                    return false;
                }
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
            var deadLetterStats = _deadLetterQueue.GetStatistics();

            return new ProcessingSummary
            {
                Date = DateTime.Today,
                TotalProcessed = stats.TotalProcessed,
                Successful = stats.TotalSuccessful,
                Failed = stats.TotalFailed,
                ProcessingTimeSeconds = stats.UpTime.TotalSeconds,
                TopErrors = stats.TopErrors,
                DeadLetterCount = deadLetterStats.TotalCount,
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

                // Process the file with a new scope
                ProcessingResult result;
                using (var scope = _scopeFactory.CreateScope())
                {
                    var fileProcessor = scope.ServiceProvider.GetRequiredService<IFileProcessor>();
                    result = await fileProcessor.ProcessFileAsync(item.FilePath);
                }

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
                    // Add to dead letter queue
                    _logger.LogError("Failed to process {FilePath} after {Attempts} attempts: {Error}",
                        item.FilePath, item.AttemptCount, result.ErrorMessage);

                    await _deadLetterQueue.AddAsync(
                        item.FilePath,
                        result.ErrorMessage ?? "Processing failed",
                        item.AttemptCount);

                    // Send notification for critical errors
                    if (IsCriticalError(result.ErrorMessage))
                    {
                        await NotifyCriticalErrorAsync(item.FilePath, result.ErrorMessage);
                    }
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
                    // Add to dead letter queue
                    await _deadLetterQueue.AddAsync(
                        item.FilePath,
                        $"Unexpected error: {ex.Message}",
                        item.AttemptCount);

                    // Notify about unexpected errors
                    await NotifyCriticalErrorAsync(item.FilePath, ex.Message, ex);
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

        private bool IsCriticalError(string? error)
        {
            if (string.IsNullOrEmpty(error)) return false;

            var criticalKeywords = new[] { "OutOfMemory", "AccessDenied", "Unauthorized", "Fatal" };
            return criticalKeywords.Any(keyword =>
                error.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        private async Task NotifyCriticalErrorAsync(string filePath, string? error, Exception? ex = null)
        {
            if (_notificationService == null) return;

            try
            {
                await _notificationService.NotifyCriticalErrorAsync(
                    $"Processing failed for {Path.GetFileName(filePath)}",
                    $"File: {filePath}\nError: {error}",
                    ex);
            }
            catch (Exception notifyEx)
            {
                _logger.LogError(notifyEx, "Failed to send critical error notification");
            }
        }

        private async void OnDeadLetterThresholdExceeded(object? sender, DeadLetterEventArgs e)
        {
            if (_notificationService == null) return;

            try
            {
                var stats = _deadLetterQueue.GetStatistics();
                await _notificationService.NotifyDeadLetterThresholdAsync(e.TotalCount, stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send dead letter threshold notification");
            }
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

    /// <summary>
    /// Queue statistics
    /// </summary>
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

    /// <summary>
    /// Status of an item being processed
    /// </summary>
    public class ProcessingItemStatus
    {
        public string FilePath { get; init; } = string.Empty;
        public DateTime? StartTime { get; init; }
        public int AttemptCount { get; init; }
        public TimeSpan Duration { get; init; }
    }
}
