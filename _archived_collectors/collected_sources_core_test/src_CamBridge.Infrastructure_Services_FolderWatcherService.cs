using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Background service that monitors multiple folders for new JPEG files
    /// </summary>
    public class FolderWatcherService : IHostedService, IDisposable
    {
        private readonly ILogger<FolderWatcherService> _logger;
        private readonly ProcessingQueue _processingQueue;
        private readonly CamBridgeSettings _settings;
        private readonly List<FileSystemWatcher> _watchers = new();
        private readonly Dictionary<string, DateTime> _fileDebounce = new();
        private readonly object _debounceLock = new();
        private readonly TimeSpan _debounceInterval = TimeSpan.FromSeconds(2);
        private Timer? _debounceTimer;
        private bool _disposed;

        public FolderWatcherService(
            ILogger<FolderWatcherService> logger,
            ProcessingQueue processingQueue,
            IOptions<CamBridgeSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _processingQueue = processingQueue ?? throw new ArgumentNullException(nameof(processingQueue));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting folder watcher service");

            // Initialize watchers for each configured folder
            foreach (var folderConfig in _settings.WatchFolders.Where(f => f.Enabled && f.IsValid))
            {
                try
                {
                    var watcher = CreateWatcher(folderConfig);
                    _watchers.Add(watcher);

                    _logger.LogInformation("Started watching folder: {Path} (Pattern: {Pattern}, Subdirectories: {IncludeSubdirs})",
                        folderConfig.Path, folderConfig.FilePattern, folderConfig.IncludeSubdirectories);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create watcher for folder: {Path}", folderConfig.Path);
                }
            }

            if (_watchers.Count == 0)
            {
                _logger.LogWarning("No valid folders configured for watching");
            }

            // Start debounce timer
            _debounceTimer = new Timer(
                ProcessDebounceQueue,
                null,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1));

            // Process existing files if configured
            if (_settings.Processing.ProcessExistingOnStartup)
            {
                await ProcessExistingFilesAsync(cancellationToken);
            }

            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping folder watcher service");

            // Stop all watchers
            foreach (var watcher in _watchers)
            {
                try
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing watcher");
                }
            }

            _watchers.Clear();

            // Stop debounce timer
            _debounceTimer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _debounceTimer?.Dispose();

            foreach (var watcher in _watchers)
            {
                try
                {
                    watcher.Dispose();
                }
                catch { }
            }

            _disposed = true;
        }

        private FileSystemWatcher CreateWatcher(FolderConfiguration folderConfig)
        {
            var watcher = new FileSystemWatcher(folderConfig.Path)
            {
                IncludeSubdirectories = folderConfig.IncludeSubdirectories,
                NotifyFilter = NotifyFilters.FileName |
                              NotifyFilters.LastWrite |
                              NotifyFilters.Size
            };

            // Set up filters for multiple extensions
            var patterns = folderConfig.FilePattern.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (patterns.Length == 1)
            {
                watcher.Filter = patterns[0].Trim();
            }
            else
            {
                // For multiple patterns, we'll filter in the event handler
                watcher.Filter = "*.*";
            }

            // Wire up events
            watcher.Created += (sender, e) => OnFileEvent(e.FullPath, folderConfig, patterns);
            watcher.Changed += (sender, e) => OnFileEvent(e.FullPath, folderConfig, patterns);
            watcher.Renamed += (sender, e) => OnFileEvent(e.FullPath, folderConfig, patterns);

            // Error handling
            watcher.Error += (sender, e) =>
            {
                var ex = e.GetException();
                _logger.LogError(ex, "FileSystemWatcher error for path: {Path}", folderConfig.Path);

                // Try to recreate the watcher
                Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    try
                    {
                        var index = _watchers.IndexOf((FileSystemWatcher)sender);
                        if (index >= 0)
                        {
                            _watchers[index].Dispose();
                            _watchers[index] = CreateWatcher(folderConfig);
                            _logger.LogInformation("Recreated watcher for path: {Path}", folderConfig.Path);
                        }
                    }
                    catch (Exception recreateEx)
                    {
                        _logger.LogError(recreateEx, "Failed to recreate watcher for path: {Path}",
                            folderConfig.Path);
                    }
                });
            };

            // Start watching
            watcher.EnableRaisingEvents = true;

            return watcher;
        }

        private void OnFileEvent(string filePath, FolderConfiguration folderConfig, string[] patterns)
        {
            try
            {
                // Check if file matches any pattern
                if (patterns.Length > 1)
                {
                    var fileName = Path.GetFileName(filePath);
                    var matchesPattern = patterns.Any(pattern =>
                    {
                        var cleanPattern = pattern.Trim();
                        if (cleanPattern.StartsWith("*"))
                        {
                            return fileName.EndsWith(cleanPattern.Substring(1),
                                StringComparison.OrdinalIgnoreCase);
                        }
                        return fileName.Equals(cleanPattern, StringComparison.OrdinalIgnoreCase);
                    });

                    if (!matchesPattern)
                        return;
                }

                // Add to debounce queue
                lock (_debounceLock)
                {
                    _fileDebounce[filePath] = DateTime.UtcNow;
                }

                _logger.LogDebug("File event detected: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling file event for: {FilePath}", filePath);
            }
        }

        private void ProcessDebounceQueue(object? state)
        {
            try
            {
                List<string> filesToProcess;

                lock (_debounceLock)
                {
                    var cutoffTime = DateTime.UtcNow - _debounceInterval;

                    filesToProcess = _fileDebounce
                        .Where(kvp => kvp.Value < cutoffTime)
                        .Select(kvp => kvp.Key)
                        .ToList();

                    foreach (var file in filesToProcess)
                    {
                        _fileDebounce.Remove(file);
                    }
                }

                // Enqueue files for processing
                foreach (var filePath in filesToProcess)
                {
                    if (File.Exists(filePath))
                    {
                        if (_processingQueue.TryEnqueue(filePath))
                        {
                            _logger.LogInformation("Enqueued new file: {FilePath}", filePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing debounce queue");
            }
        }

        private async Task ProcessExistingFilesAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing existing files in watched folders");

            var processedCount = 0;
            var skippedCount = 0;

            foreach (var folderConfig in _settings.WatchFolders.Where(f => f.Enabled && f.IsValid))
            {
                try
                {
                    var patterns = folderConfig.FilePattern
                        .Split(';', StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => p.Trim())
                        .ToArray();

                    var searchOption = folderConfig.IncludeSubdirectories
                        ? SearchOption.AllDirectories
                        : SearchOption.TopDirectoryOnly;

                    foreach (var pattern in patterns)
                    {
                        var files = Directory.GetFiles(folderConfig.Path, pattern, searchOption);

                        foreach (var file in files)
                        {
                            if (cancellationToken.IsCancellationRequested)
                                break;

                            if (_processingQueue.TryEnqueue(file))
                            {
                                processedCount++;
                            }
                            else
                            {
                                skippedCount++;
                            }

                            // Small delay to avoid overwhelming the system
                            if (processedCount % 10 == 0)
                            {
                                await Task.Delay(100, cancellationToken);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing existing files in folder: {Path}",
                        folderConfig.Path);
                }
            }

            _logger.LogInformation("Existing file scan complete. Enqueued: {ProcessedCount}, Skipped: {SkippedCount}",
                processedCount, skippedCount);
        }
    }
}
