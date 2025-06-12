// src/CamBridge.Infrastructure/Services/PipelineManager.cs
// Version: 0.7.8
// Description: Orchestrates multiple processing pipelines with independent queues and watchers
// Copyright: Â© 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Manages multiple processing pipelines with independent configurations, queues, and watchers
    /// </summary>
    public class PipelineManager : IDisposable
    {
        private readonly ILogger<PipelineManager> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptionsMonitor<CamBridgeSettingsV2> _settingsMonitor;
        private readonly ConcurrentDictionary<string, PipelineContext> _pipelines = new();
        private readonly SemaphoreSlim _pipelineLock = new(1, 1);
        private bool _disposed;

        public PipelineManager(
            ILogger<PipelineManager> logger,
            IServiceProvider serviceProvider,
            IOptionsMonitor<CamBridgeSettingsV2> settingsMonitor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _settingsMonitor = settingsMonitor ?? throw new ArgumentNullException(nameof(settingsMonitor));

            // React to settings changes
            _settingsMonitor.OnChange(async settings =>
            {
                _logger.LogInformation("Settings changed, reconfiguring pipelines");
                await ReconfigurePipelinesAsync(settings);
            });
        }

        /// <summary>
        /// Initializes and starts all configured pipelines
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting Pipeline Manager");

            var settings = _settingsMonitor.CurrentValue;

            // Initialize pipelines from configuration
            foreach (var pipelineConfig in settings.Pipelines.Where(p => p.Enabled))
            {
                try
                {
                    await CreateAndStartPipelineAsync(pipelineConfig, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to start pipeline: {PipelineName}", pipelineConfig.Name);
                }
            }

            _logger.LogInformation("Pipeline Manager started with {Count} active pipelines",
                _pipelines.Count(p => p.Value.IsActive));
        }

        /// <summary>
        /// Stops all pipelines gracefully
        /// </summary>
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Stopping Pipeline Manager");

            var stopTasks = _pipelines.Values
                .Where(p => p.IsActive)
                .Select(p => StopPipelineAsync(p, cancellationToken))
                .ToArray();

            await Task.WhenAll(stopTasks);

            _logger.LogInformation("Pipeline Manager stopped");
        }

        /// <summary>
        /// Enables a specific pipeline at runtime
        /// </summary>
        public async Task EnablePipelineAsync(string pipelineId, CancellationToken cancellationToken = default)
        {
            await _pipelineLock.WaitAsync(cancellationToken);
            try
            {
                if (_pipelines.TryGetValue(pipelineId, out var context))
                {
                    if (!context.IsActive)
                    {
                        await StartPipelineAsync(context, cancellationToken);
                        _logger.LogInformation("Pipeline {PipelineId} enabled", pipelineId);
                    }
                }
                else
                {
                    _logger.LogWarning("Pipeline {PipelineId} not found", pipelineId);
                }
            }
            finally
            {
                _pipelineLock.Release();
            }
        }

        /// <summary>
        /// Disables a specific pipeline at runtime
        /// </summary>
        public async Task DisablePipelineAsync(string pipelineId, CancellationToken cancellationToken = default)
        {
            await _pipelineLock.WaitAsync(cancellationToken);
            try
            {
                if (_pipelines.TryGetValue(pipelineId, out var context))
                {
                    if (context.IsActive)
                    {
                        await StopPipelineAsync(context, cancellationToken);
                        _logger.LogInformation("Pipeline {PipelineId} disabled", pipelineId);
                    }
                }
                else
                {
                    _logger.LogWarning("Pipeline {PipelineId} not found", pipelineId);
                }
            }
            finally
            {
                _pipelineLock.Release();
            }
        }

        /// <summary>
        /// Gets the status of all pipelines
        /// </summary>
        public Dictionary<string, PipelineStatus> GetPipelineStatuses()
        {
            return _pipelines.ToDictionary(
                kvp => kvp.Key,
                kvp => new PipelineStatus
                {
                    Id = kvp.Value.Configuration.Id.ToString(),
                    Name = kvp.Value.Configuration.Name,
                    IsActive = kvp.Value.IsActive,
                    QueueLength = kvp.Value.Queue?.QueueLength ?? 0,
                    ActiveProcessing = kvp.Value.Queue?.ActiveProcessing ?? 0,
                    TotalProcessed = kvp.Value.Queue?.TotalProcessed ?? 0,
                    TotalSuccessful = kvp.Value.Queue?.TotalSuccessful ?? 0,
                    TotalFailed = kvp.Value.Queue?.TotalFailed ?? 0,
                    WatchedFolders = new List<string> { kvp.Value.Configuration.WatchSettings.Path }
                });
        }

        /// <summary>
        /// Gets detailed statistics for a specific pipeline
        /// </summary>
        public PipelineDetailedStatus? GetPipelineDetails(string pipelineId)
        {
            if (!_pipelines.TryGetValue(pipelineId, out var context))
                return null;

            var queueStats = context.Queue?.GetStatistics();
            var deadLetterStats = context.DeadLetterQueue?.GetStatistics();

            return new PipelineDetailedStatus
            {
                Id = context.Configuration.Id.ToString(),
                Name = context.Configuration.Name,
                Description = context.Configuration.Description,
                IsActive = context.IsActive,
                Configuration = context.Configuration,
                QueueStatistics = queueStats,
                DeadLetterStatistics = deadLetterStats,
                ActiveItems = context.Queue?.GetActiveItems() ?? new List<ProcessingItemStatus>()
            };
        }

        private async Task CreateAndStartPipelineAsync(PipelineConfiguration config, CancellationToken cancellationToken)
        {
            await _pipelineLock.WaitAsync(cancellationToken);
            try
            {
                // Check if pipeline already exists
                if (_pipelines.ContainsKey(config.Id.ToString()))
                {
                    _logger.LogWarning("Pipeline {PipelineId} already exists", config.Id);
                    return;
                }

                // Create pipeline context
                var context = CreatePipelineContextAsync(config);

                // Add to collection
                _pipelines[config.Id.ToString()] = context;

                // Start the pipeline
                await StartPipelineAsync(context, cancellationToken);

                _logger.LogInformation("Created and started pipeline: {PipelineName} ({PipelineId})",
                    config.Name, config.Id);
            }
            finally
            {
                _pipelineLock.Release();
            }
        }

        private PipelineContext CreatePipelineContextAsync(PipelineConfiguration config)
        {
            // Create pipeline-specific services
            var scope = _serviceProvider.CreateScope();


            var deadLetterQueue = new DeadLetterQueue(
                scope.ServiceProvider.GetRequiredService<ILogger<DeadLetterQueue>>(),
                deadLetterPath);

            // Create processing queue for this pipeline
            // Clone ProcessingOptions and set DeadLetterFolder
            var processingOptions = new ProcessingOptions
            {
                SuccessAction = config.ProcessingOptions.SuccessAction,
                FailureAction = config.ProcessingOptions.FailureAction,
                ArchiveFolder = config.ProcessingOptions.ArchiveFolder,
                ErrorFolder = config.ProcessingOptions.ErrorFolder,
                BackupFolder = config.ProcessingOptions.BackupFolder,
                DeadLetterFolder = deadLetterPath, // Set the pipeline-specific dead letter path
                CreateBackup = config.ProcessingOptions.CreateBackup,
                MaxConcurrentProcessing = config.ProcessingOptions.MaxConcurrentProcessing,
                RetryOnFailure = config.ProcessingOptions.RetryOnFailure,
                MaxRetryAttempts = config.ProcessingOptions.MaxRetryAttempts,
                OutputOrganization = config.ProcessingOptions.OutputOrganization,
                ProcessExistingOnStartup = config.ProcessingOptions.ProcessExistingOnStartup,
                MaxFileAge = config.ProcessingOptions.MaxFileAge,
                MinimumFileSizeBytes = config.ProcessingOptions.MinimumFileSizeBytes,
                MaximumFileSizeBytes = config.ProcessingOptions.MaximumFileSizeBytes,
                OutputFilePattern = config.ProcessingOptions.OutputFilePattern,
                RetryDelaySeconds = config.ProcessingOptions.RetryDelaySeconds
            };

            var processingQueue = new ProcessingQueue(
                scope.ServiceProvider.GetRequiredService<ILogger<ProcessingQueue>>(),
                scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>(),
                Options.Create(processingOptions),
                deadLetterQueue,
                scope.ServiceProvider.GetService<INotificationService>());

            // Create context
            var context = new PipelineContext
            {
                Id = config.Id.ToString(),
                Configuration = config,
                ServiceScope = scope,
                Queue = processingQueue,
                Watchers = new List<FileSystemWatcher>(),
                ProcessingTask = null,
                IsActive = false
            };

            return context;
        }

        private async Task StartPipelineAsync(PipelineContext context, CancellationToken cancellationToken)
        {
            if (context.IsActive)
                return;

            _logger.LogInformation("Starting pipeline: {PipelineName}", context.Configuration.Name);

            // Start processing queue
            context.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            context.ProcessingTask = Task.Run(async () =>
            {
                try
                {
                    await context.Queue.ProcessQueueAsync(context.CancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    // Expected during shutdown
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fatal error in pipeline {PipelineId} processing queue", context.Id);
                }
            }, context.CancellationTokenSource.Token);

            // Create watcher for the pipeline's watch folder
            if (context.Configuration.WatchSettings.IsValid)
            {
                try
                {
                    var watcher = CreateWatcher(context.Configuration.WatchSettings, context);
                    context.Watchers.Add(watcher);

                    _logger.LogInformation("Started watching folder: {Path} for pipeline {PipelineName}",
                        context.Configuration.WatchSettings.Path, context.Configuration.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create watcher for folder: {Path} in pipeline {PipelineId}",
                        context.Configuration.WatchSettings.Path, context.Id);
                }
            }

            // Process existing files if configured
            if (context.Configuration.ProcessingOptions.ProcessExistingOnStartup)
            {
                await ProcessExistingFilesAsync(context, cancellationToken);
            }

            context.IsActive = true;
        }

        private async Task StopPipelineAsync(PipelineContext context, CancellationToken cancellationToken)
        {
            if (!context.IsActive)
                return;

            _logger.LogInformation("Stopping pipeline: {PipelineName}", context.Configuration.Name);

            // Stop watchers
            foreach (var watcher in context.Watchers)
            {
                try
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing watcher in pipeline {PipelineId}", context.Id);
                }
            }
            context.Watchers.Clear();

            // Stop processing
            context.CancellationTokenSource?.Cancel();

            if (context.ProcessingTask != null)
            {
                try
                {
                    await context.ProcessingTask.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);
                }
                catch (TimeoutException)
                {
                    _logger.LogWarning("Pipeline {PipelineId} processing did not complete within timeout", context.Id);
                }
            }

            context.IsActive = false;
        }

        private FileSystemWatcher CreateWatcher(PipelineWatchSettings watchSettings, PipelineContext context)
        {
            var watcher = new FileSystemWatcher(watchSettings.Path)
            {
                IncludeSubdirectories = watchSettings.IncludeSubdirectories,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size
            };

            var patterns = watchSettings.FilePattern.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (patterns.Length == 1)
            {
                watcher.Filter = patterns[0].Trim();
            }
            else
            {
                watcher.Filter = "*.*";
            }

            // Wire up events to enqueue in the pipeline's specific queue
            watcher.Created += (sender, e) => OnFileEvent(e.FullPath, watchSettings, patterns, context);
            watcher.Changed += (sender, e) => OnFileEvent(e.FullPath, watchSettings, patterns, context);
            watcher.Renamed += (sender, e) => OnFileEvent(e.FullPath, watchSettings, patterns, context);

            watcher.Error += (sender, e) =>
            {
                var ex = e.GetException();
                _logger.LogError(ex, "FileSystemWatcher error for path: {Path} in pipeline {PipelineId}",
                    watchSettings.Path, context.Id);
            };

            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        private void OnFileEvent(string filePath, PipelineWatchSettings watchSettings, string[] patterns, PipelineContext context)
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
                            return fileName.EndsWith(cleanPattern.Substring(1), StringComparison.OrdinalIgnoreCase);
                        }
                        return fileName.Equals(cleanPattern, StringComparison.OrdinalIgnoreCase);
                    });

                    if (!matchesPattern)
                        return;
                }

                // Enqueue in the pipeline's specific queue
                if (context.Queue.TryEnqueue(filePath))
                {
                    _logger.LogInformation("Enqueued {FilePath} in pipeline {PipelineName}",
                        filePath, context.Configuration.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling file event for: {FilePath} in pipeline {PipelineId}",
                    filePath, context.Id);
            }
        }

        private Task ProcessExistingFilesAsync(PipelineContext context, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing existing files for pipeline: {PipelineName}",
                context.Configuration.Name);

            var processedCount = 0;
            var watchSettings = context.Configuration.WatchSettings;

            if (watchSettings.IsValid)
            {
                try
                {
                    var patterns = watchSettings.FilePattern
                        .Split(';', StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => p.Trim())
                        .ToArray();

                    var searchOption = watchSettings.IncludeSubdirectories
                        ? SearchOption.AllDirectories
                        : SearchOption.TopDirectoryOnly;

                    foreach (var pattern in patterns)
                    {
                        var files = Directory.GetFiles(watchSettings.Path, pattern, searchOption);
                        foreach (var file in files)
                        {
                            if (cancellationToken.IsCancellationRequested)
                                break;

                            if (context.Queue.TryEnqueue(file))
                            {
                                processedCount++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing existing files in folder: {Path}", watchSettings.Path);
                }
            }

            _logger.LogInformation("Enqueued {Count} existing files for pipeline {PipelineName}",
                processedCount, context.Configuration.Name);

            return Task.CompletedTask;
        }

        private async Task ReconfigurePipelinesAsync(CamBridgeSettingsV2 newSettings)
        {
            await _pipelineLock.WaitAsync();
            try
            {
                // Find pipelines to remove
                var toRemove = _pipelines.Keys
                    .Where(id => !newSettings.Pipelines.Any(p => p.Id.ToString() == id))
                    .ToList();

                // Remove deleted pipelines
                foreach (var id in toRemove)
                {
                    if (_pipelines.TryRemove(id, out var context))
                    {
                        await StopPipelineAsync(context, CancellationToken.None);
                        context.ServiceScope?.Dispose();
                        _logger.LogInformation("Removed pipeline: {PipelineId}", id);
                    }
                }

                // Update or add pipelines
                foreach (var pipelineConfig in newSettings.Pipelines)
                {
                    if (_pipelines.TryGetValue(pipelineConfig.Id.ToString(), out var existingContext))
                    {
                        // Update existing pipeline
                        if (pipelineConfig.Enabled && !existingContext.IsActive)
                        {
                            existingContext.Configuration = pipelineConfig;
                            await StartPipelineAsync(existingContext, CancellationToken.None);
                        }
                        else if (!pipelineConfig.Enabled && existingContext.IsActive)
                        {
                            await StopPipelineAsync(existingContext, CancellationToken.None);
                        }
                        else
                        {
                            // Update configuration
                            existingContext.Configuration = pipelineConfig;
                        }
                    }
                    else if (pipelineConfig.Enabled)
                    {
                        // Add new pipeline
                        await CreateAndStartPipelineAsync(pipelineConfig, CancellationToken.None);
                    }
                }
            }
            finally
            {
                _pipelineLock.Release();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _pipelineLock?.Wait();
            try
            {
                foreach (var context in _pipelines.Values)
                {
                    StopPipelineAsync(context, CancellationToken.None).GetAwaiter().GetResult();
                    context.ServiceScope?.Dispose();
                }
                _pipelines.Clear();
            }
            finally
            {
                _pipelineLock?.Release();
                _pipelineLock?.Dispose();
            }

            _disposed = true;
        }

        /// <summary>
        /// Internal context for managing a single pipeline
        /// </summary>
        private class PipelineContext
        {
            public string Id { get; set; } = string.Empty;
            public PipelineConfiguration Configuration { get; set; } = new();
            public ProcessingQueue Queue { get; set; } = null!;
                        public List<FileSystemWatcher> Watchers { get; set; } = new();
            public Task? ProcessingTask { get; set; }
            public CancellationTokenSource? CancellationTokenSource { get; set; }
            public IServiceScope ServiceScope { get; set; } = null!;
            public bool IsActive { get; set; }
        }
    }

    /// <summary>
    /// Pipeline status summary
    /// </summary>
    public class PipelineStatus
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int QueueLength { get; set; }
        public int ActiveProcessing { get; set; }
        public int TotalProcessed { get; set; }
        public int TotalSuccessful { get; set; }
        public int TotalFailed { get; set; }
        public List<string> WatchedFolders { get; set; } = new();
    }

    /// <summary>
    /// Detailed pipeline status including configuration and statistics
    /// </summary>
    public class PipelineDetailedStatus
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public PipelineConfiguration Configuration { get; set; } = new();
        public QueueStatistics? QueueStatistics { get; set; }
        public object? DeadLetterStatistics { get; set; }
        public IReadOnlyList<ProcessingItemStatus> ActiveItems { get; set; } = new List<ProcessingItemStatus>();
    }
}

