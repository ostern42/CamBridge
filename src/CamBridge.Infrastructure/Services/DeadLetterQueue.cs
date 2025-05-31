using CamBridge.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Manages files that could not be processed after all retry attempts
    /// </summary>
    public class DeadLetterQueue
    {
        private readonly ILogger<DeadLetterQueue> _logger;
        private readonly ProcessingOptions _options;
        private readonly ConcurrentDictionary<string, DeadLetterItem> _deadLetters = new();
        private readonly string _deadLetterPath;
        private readonly SemaphoreSlim _persistenceLock = new(1, 1);
        private Timer? _persistenceTimer;

        public int Count => _deadLetters.Count;

        public event EventHandler<DeadLetterEventArgs>? ItemAdded;
        public event EventHandler<DeadLetterEventArgs>? ThresholdExceeded;

        public DeadLetterQueue(
            ILogger<DeadLetterQueue> logger,
            IOptions<ProcessingOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

            _deadLetterPath = Path.Combine(_options.ErrorFolder, "dead-letters");
            Directory.CreateDirectory(_deadLetterPath);

            // Load existing dead letters
            LoadDeadLetters();

            // Start persistence timer
            _persistenceTimer = new Timer(
                PersistDeadLetters,
                null,
                TimeSpan.FromMinutes(5),
                TimeSpan.FromMinutes(5));
        }

        /// <summary>
        /// Adds a file to the dead letter queue
        /// </summary>
        public async Task AddAsync(string filePath, string error, int attemptCount)
        {
            var item = new DeadLetterItem
            {
                FilePath = filePath,
                FileName = Path.GetFileName(filePath),
                Error = error,
                AttemptCount = attemptCount,
                AddedAt = DateTime.UtcNow,
                FileSize = new FileInfo(filePath).Length,
                Id = Guid.NewGuid().ToString()
            };

            _deadLetters.TryAdd(item.Id, item);

            _logger.LogWarning(
                "Added {FileName} to dead letter queue after {Attempts} attempts. Error: {Error}",
                item.FileName, attemptCount, error);

            // Move file to dead letter folder
            await MoveToDeadLetterFolderAsync(item);

            // Raise events
            ItemAdded?.Invoke(this, new DeadLetterEventArgs(item));

            // Check threshold
            if (_deadLetters.Count > 50) // Configurable threshold
            {
                ThresholdExceeded?.Invoke(this, new DeadLetterEventArgs(item)
                {
                    TotalCount = _deadLetters.Count
                });
            }

            // Persist changes
            await PersistDeadLettersAsync();
        }

        /// <summary>
        /// Retrieves all dead letter items
        /// </summary>
        public IReadOnlyList<DeadLetterItem> GetAll()
        {
            return _deadLetters.Values
                .OrderByDescending(x => x.AddedAt)
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Gets dead letters from the last N hours
        /// </summary>
        public IReadOnlyList<DeadLetterItem> GetRecent(int hours = 24)
        {
            var cutoff = DateTime.UtcNow.AddHours(-hours);
            return _deadLetters.Values
                .Where(x => x.AddedAt >= cutoff)
                .OrderByDescending(x => x.AddedAt)
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Attempts to reprocess a dead letter item
        /// </summary>
        public async Task<bool> ReprocessAsync(string id)
        {
            if (!_deadLetters.TryGetValue(id, out var item))
            {
                _logger.LogWarning("Dead letter item {Id} not found", id);
                return false;
            }

            _logger.LogInformation("Attempting to reprocess dead letter {Id} ({FileName})",
                id, item.FileName);

            // Move file back to processing folder
            var targetPath = Path.Combine(_options.ErrorFolder, "..", "Reprocess", item.FileName);
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);

            var deadLetterFilePath = GetDeadLetterFilePath(item);
            if (File.Exists(deadLetterFilePath))
            {
                File.Move(deadLetterFilePath, targetPath, true);

                // Remove from dead letters
                _deadLetters.TryRemove(id, out _);
                await PersistDeadLettersAsync();

                _logger.LogInformation("Moved {FileName} to reprocessing folder", item.FileName);
                return true;
            }

            _logger.LogError("Dead letter file not found: {FilePath}", deadLetterFilePath);
            return false;
        }

        /// <summary>
        /// Removes a dead letter item permanently
        /// </summary>
        public async Task<bool> RemoveAsync(string id)
        {
            if (!_deadLetters.TryRemove(id, out var item))
            {
                return false;
            }

            // Delete the file if it exists
            var filePath = GetDeadLetterFilePath(item);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            await PersistDeadLettersAsync();
            _logger.LogInformation("Removed dead letter {Id} ({FileName})", id, item.FileName);

            return true;
        }

        /// <summary>
        /// Gets statistics about dead letters
        /// </summary>
        public DeadLetterStatistics GetStatistics()
        {
            var items = _deadLetters.Values.ToList();

            return new DeadLetterStatistics
            {
                TotalCount = items.Count,
                Last24HoursCount = items.Count(x => x.AddedAt >= DateTime.UtcNow.AddHours(-24)),
                LastWeekCount = items.Count(x => x.AddedAt >= DateTime.UtcNow.AddDays(-7)),
                TotalSizeBytes = items.Sum(x => x.FileSize),
                OldestItem = items.OrderBy(x => x.AddedAt).FirstOrDefault(),
                NewestItem = items.OrderByDescending(x => x.AddedAt).FirstOrDefault(),
                ErrorCategories = items
                    .GroupBy(x => CategorizeError(x.Error))
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }

        private async Task MoveToDeadLetterFolderAsync(DeadLetterItem item)
        {
            try
            {
                var targetPath = GetDeadLetterFilePath(item);
                var targetDir = Path.GetDirectoryName(targetPath)!;
                Directory.CreateDirectory(targetDir);

                if (File.Exists(item.FilePath))
                {
                    File.Move(item.FilePath, targetPath, true);
                    item.DeadLetterPath = targetPath;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to move file to dead letter folder: {FilePath}",
                    item.FilePath);
            }
        }

        private string GetDeadLetterFilePath(DeadLetterItem item)
        {
            var date = item.AddedAt.ToString("yyyy-MM-dd");
            var fileName = $"{Path.GetFileNameWithoutExtension(item.FileName)}_{item.Id}{Path.GetExtension(item.FileName)}";
            return Path.Combine(_deadLetterPath, date, fileName);
        }

        private async Task PersistDeadLettersAsync()
        {
            await _persistenceLock.WaitAsync();
            try
            {
                var manifestPath = Path.Combine(_deadLetterPath, "dead-letters.json");
                var json = JsonSerializer.Serialize(_deadLetters.Values, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                await File.WriteAllTextAsync(manifestPath, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist dead letters");
            }
            finally
            {
                _persistenceLock.Release();
            }
        }

        private void PersistDeadLetters(object? state)
        {
            Task.Run(async () => await PersistDeadLettersAsync());
        }

        private void LoadDeadLetters()
        {
            try
            {
                var manifestPath = Path.Combine(_deadLetterPath, "dead-letters.json");
                if (File.Exists(manifestPath))
                {
                    var json = File.ReadAllText(manifestPath);
                    var items = JsonSerializer.Deserialize<List<DeadLetterItem>>(json);

                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            _deadLetters.TryAdd(item.Id, item);
                        }

                        _logger.LogInformation("Loaded {Count} dead letter items", items.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load dead letters");
            }
        }

        private string CategorizeError(string error)
        {
            if (error.Contains("EXIF", StringComparison.OrdinalIgnoreCase))
                return "EXIF_ERROR";
            if (error.Contains("DICOM", StringComparison.OrdinalIgnoreCase))
                return "DICOM_ERROR";
            if (error.Contains("File", StringComparison.OrdinalIgnoreCase))
                return "FILE_ERROR";
            if (error.Contains("Patient", StringComparison.OrdinalIgnoreCase))
                return "DATA_ERROR";
            return "OTHER_ERROR";
        }

        public void Dispose()
        {
            _persistenceTimer?.Dispose();
            PersistDeadLettersAsync().GetAwaiter().GetResult();
        }
    }

    /// <summary>
    /// Represents an item in the dead letter queue
    /// </summary>
    public class DeadLetterItem
    {
        public string Id { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string? DeadLetterPath { get; set; }
        public string Error { get; set; } = string.Empty;
        public int AttemptCount { get; set; }
        public DateTime AddedAt { get; set; }
        public long FileSize { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Statistics about the dead letter queue
    /// </summary>
    public class DeadLetterStatistics
    {
        public int TotalCount { get; set; }
        public int Last24HoursCount { get; set; }
        public int LastWeekCount { get; set; }
        public long TotalSizeBytes { get; set; }
        public DeadLetterItem? OldestItem { get; set; }
        public DeadLetterItem? NewestItem { get; set; }
        public Dictionary<string, int> ErrorCategories { get; set; } = new();

        public string TotalSizeFormatted => FormatBytes(TotalSizeBytes);

        private static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }
    }

    /// <summary>
    /// Event args for dead letter events
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
}
