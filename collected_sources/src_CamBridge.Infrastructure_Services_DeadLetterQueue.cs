using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Queue for managing files that failed processing after all retry attempts
    /// </summary>
    public class DeadLetterQueue
    {
        private readonly ILogger<DeadLetterQueue> _logger;
        private readonly ConcurrentDictionary<Guid, DeadLetterItem> _items = new();
        private readonly string _persistencePath;
        private readonly SemaphoreSlim _persistenceLock = new(1, 1);
        private readonly Timer _persistenceTimer;
        private bool _isDirty = false;

        public event EventHandler<DeadLetterEventArgs>? ItemAdded;
        public event EventHandler<DeadLetterEventArgs>? ItemRemoved;
        public event EventHandler<DeadLetterEventArgs>? ThresholdExceeded;

        public int Count => _items.Count;
        public int ThresholdLimit { get; set; } = 50;

        public DeadLetterQueue(ILogger<DeadLetterQueue> logger, string? persistencePath = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _persistencePath = persistencePath ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "CamBridge",
                "dead-letters.json");

            // Ensure directory exists
            var directory = Path.GetDirectoryName(_persistencePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Load existing items
            _ = LoadFromDiskAsync();

            // Start persistence timer (save every 30 seconds if dirty)
            _persistenceTimer = new Timer(async _ => await PersistIfDirtyAsync(), null,
                TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Adds a failed item to the dead letter queue
        /// </summary>
        public async Task<DeadLetterItem> AddAsync(string filePath, string error, int attemptCount)
        {
            var item = new DeadLetterItem
            {
                Id = Guid.NewGuid(),
                FilePath = filePath,
                Error = error,
                FailedAt = DateTime.UtcNow,
                AttemptCount = attemptCount,
                FileSize = File.Exists(filePath) ? new FileInfo(filePath).Length : 0
            };

            _items.TryAdd(item.Id, item);
            _isDirty = true;

            _logger.LogWarning("Added file to dead letter queue: {FilePath}, Error: {Error}",
                filePath, error);

            // Raise events
            ItemAdded?.Invoke(this, new DeadLetterEventArgs(item));

            if (_items.Count >= ThresholdLimit)
            {
                _logger.LogError("Dead letter queue threshold exceeded: {Count} items", _items.Count);
                ThresholdExceeded?.Invoke(this, new DeadLetterEventArgs(item) { TotalCount = _items.Count });
            }

            // Persist immediately for dead letters
            await PersistToDiskAsync();

            return item;
        }

        /// <summary>
        /// Removes an item from the dead letter queue
        /// </summary>
        public async Task<bool> RemoveAsync(Guid id)
        {
            if (_items.TryRemove(id, out var item))
            {
                _isDirty = true;
                _logger.LogInformation("Removed item from dead letter queue: {FilePath}", item.FilePath);

                ItemRemoved?.Invoke(this, new DeadLetterEventArgs(item));
                await PersistToDiskAsync();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets all items in the dead letter queue
        /// </summary>
        public IReadOnlyList<DeadLetterItem> GetAllItems()
        {
            return _items.Values.OrderByDescending(x => x.FailedAt).ToList();
        }

        /// <summary>
        /// Gets a specific item by ID
        /// </summary>
        public DeadLetterItem? GetItem(Guid id)
        {
            return _items.TryGetValue(id, out var item) ? item : null;
        }

        /// <summary>
        /// Clears all items from the queue
        /// </summary>
        public async Task ClearAsync()
        {
            _items.Clear();
            _isDirty = true;

            _logger.LogInformation("Cleared all items from dead letter queue");
            await PersistToDiskAsync();
        }

        /// <summary>
        /// Gets statistics about the dead letter queue
        /// </summary>
        public DeadLetterStatistics GetStatistics()
        {
            var items = _items.Values.ToList();

            return new DeadLetterStatistics
            {
                TotalCount = items.Count,
                OldestItem = items.MinBy(x => x.FailedAt)?.FailedAt,
                NewestItem = items.MaxBy(x => x.FailedAt)?.FailedAt,
                TotalSizeBytes = items.Sum(x => x.FileSize),
                ErrorCategories = items
                    .GroupBy(x => CategorizeError(x.Error))
                    .ToDictionary(g => g.Key, g => g.Count()),
                AverageAttempts = items.Count > 0 ? items.Average(x => x.AttemptCount) : 0
            };
        }

        /// <summary>
        /// Persists the queue to disk
        /// </summary>
        private async Task PersistToDiskAsync()
        {
            await _persistenceLock.WaitAsync();
            try
            {
                var items = _items.Values.ToList();
                var json = JsonSerializer.Serialize(items, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(_persistencePath, json);
                _isDirty = false;

                _logger.LogDebug("Persisted {Count} dead letter items to disk", items.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist dead letter queue to disk");
            }
            finally
            {
                _persistenceLock.Release();
            }
        }

        /// <summary>
        /// Persists only if there are unsaved changes
        /// </summary>
        private Task PersistIfDirtyAsync()
        {
            if (_isDirty)
            {
                return PersistToDiskAsync();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Loads the queue from disk
        /// </summary>
        private async Task LoadFromDiskAsync()
        {
            if (!File.Exists(_persistencePath))
                return;

            await _persistenceLock.WaitAsync();
            try
            {
                var json = await File.ReadAllTextAsync(_persistencePath);
                var items = JsonSerializer.Deserialize<List<DeadLetterItem>>(json);

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        _items.TryAdd(item.Id, item);
                    }

                    _logger.LogInformation("Loaded {Count} dead letter items from disk", items.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load dead letter queue from disk");
            }
            finally
            {
                _persistenceLock.Release();
            }
        }

        private string CategorizeError(string error)
        {
            if (error.Contains("EXIF", StringComparison.OrdinalIgnoreCase))
                return "EXIF Extraction";
            if (error.Contains("DICOM", StringComparison.OrdinalIgnoreCase))
                return "DICOM Conversion";
            if (error.Contains("Patient", StringComparison.OrdinalIgnoreCase))
                return "Patient Data";
            if (error.Contains("File", StringComparison.OrdinalIgnoreCase))
                return "File Access";
            if (error.Contains("Memory", StringComparison.OrdinalIgnoreCase))
                return "Memory";
            return "Other";
        }

        public void Dispose()
        {
            _persistenceTimer?.Dispose();

            // Final save
            if (_isDirty)
            {
                PersistToDiskAsync().GetAwaiter().GetResult();
            }

            _persistenceLock?.Dispose();
        }
    }

    /// <summary>
    /// Represents an item in the dead letter queue
    /// </summary>
    public class DeadLetterItem
    {
        public Guid Id { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public DateTime FailedAt { get; set; }
        public int AttemptCount { get; set; }
        public long FileSize { get; set; }

        public string FileName => Path.GetFileName(FilePath);
    }

    /// <summary>
    /// Event args for dead letter queue events
    /// </summary>
    public class DeadLetterEventArgs : EventArgs
    {
        public DeadLetterItem Item { get; }
        public int TotalCount { get; set; }

        public DeadLetterEventArgs(DeadLetterItem item)
        {
            Item = item;
        }
    }

    /// <summary>
    /// Statistics about the dead letter queue
    /// </summary>
    public class DeadLetterStatistics
    {
        public int TotalCount { get; set; }
        public DateTime? OldestItem { get; set; }
        public DateTime? NewestItem { get; set; }
        public long TotalSizeBytes { get; set; }
        public Dictionary<string, int> ErrorCategories { get; set; } = new();
        public double AverageAttempts { get; set; }

        public string GetFormattedSize()
        {
            if (TotalSizeBytes < 1024)
                return $"{TotalSizeBytes} B";
            if (TotalSizeBytes < 1024 * 1024)
                return $"{TotalSizeBytes / 1024.0:F2} KB";
            if (TotalSizeBytes < 1024 * 1024 * 1024)
                return $"{TotalSizeBytes / (1024.0 * 1024):F2} MB";
            return $"{TotalSizeBytes / (1024.0 * 1024 * 1024):F2} GB";
        }
    }
}
