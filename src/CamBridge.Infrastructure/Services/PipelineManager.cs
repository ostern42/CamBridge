// src/CamBridge.Infrastructure/Services/PipelineManager.cs
// Version: 0.7.28
// Description: Orchestrates multiple processing pipelines with isolated logging and corrected log levels
// Copyright: Â© 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Manages multiple processing pipelines with independent configurations, queues, watchers, FileProcessors, AND Loggers!
    /// PIPELINE UPDATE: Each pipeline gets its own FileProcessor instance AND Logger!
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
                        context.Watcher.EnableRaisingEvents = true;
                        context.IsActive = true;
                        _logger.LogInformation("Pipeline {PipelineName} enabled", context.Configuration.Name);
                    }
                }
                else
                {
                    // Try to find in configuration and create
                    var config = _settingsMonitor.CurrentValue.Pipelines
                        .FirstOrDefault(p => p.Id.ToString() == pipelineId);

                    if (config != null)
                    {
                        config.Enabled = true;
                        await CreateAndStartPipelineAsync(config, cancellationToken);
                    }
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
                    await StopPipelineAsync(context, cancellationToken);
                }
            }
            finally
            {
                _pipelineLock.Release();
            }
        }

        /// <summary>
        /// Gets the current status of all pipelines
        /// </summary>
        public List<PipelineStatus> GetPipelineStatuses()
        {
            return _pipelines.Values.Select(p => new PipelineStatus
            {
                Id = p.Configuration.Id,
                Name = p.Configuration.Name,
                IsActive = p.IsActive,
                QueueDepth = p.Queue.QueueLength,
                ProcessedCount = p.ProcessedCount,
                ErrorCount = p.ErrorCount,
                LastProcessed = p.LastProcessed,
                WatchPath = p.Configuration.WatchSettings.Path,
                OutputPath = p.Configuration.WatchSettings.OutputPath ?? ""
            }).ToList();
        }

        /// <summary>
        /// Gets detailed information about a specific pipeline
        /// </summary>
        public PipelineStatus? GetPipelineStatus(string pipelineId)
        {
            if (_pipelines.TryGetValue(pipelineId, out var context))
            {
                return new PipelineStatus
                {
                    Id = context.Configuration.Id,
                    Name = context.Configuration.Name,
                    IsActive = context.IsActive,
                    QueueDepth = context.Queue.QueueLength,
                    ProcessedCount = context.ProcessedCount,
                    ErrorCount = context.ErrorCount,
                    LastProcessed = context.LastProcessed,
                    WatchPath = context.Configuration.WatchSettings.Path,
                    OutputPath = context.Configuration.WatchSettings.OutputPath ?? ""
                };
            }
            return null;
        }

        private async Task CreateAndStartPipelineAsync(PipelineConfiguration config, CancellationToken cancellationToken)
        {
            if (_pipelines.ContainsKey(config.Id.ToString()))
            {
                _logger.LogWarning("Pipeline {PipelineName} ({Id}) already exists", config.Name, config.Id);
                return;
            }

            // Changed to DEBUG level - technical initialization detail
            _logger.LogDebug("Creating pipeline: {PipelineName} ({Id})", config.Name, config.Id);

            try
            {
                // Get services from DI
                var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
                var exifReader = _serviceProvider.GetRequiredService<ExifToolReader>();
                var dicomConverter = _serviceProvider.GetRequiredService<DicomConverter>();
                var globalDicomSettings = _settingsMonitor.CurrentValue.GlobalDicomSettings ?? new DicomSettings();

                // CRITICAL: Create pipeline-specific logger with sanitized name
                var sanitizedName = SanitizeForFileName(config.Name);
                var pipelineLogger = loggerFactory.CreateLogger($"Pipeline.{sanitizedName}");

                // Create FileProcessor for THIS pipeline with pipeline-specific logger
                var fileProcessor = new FileProcessor(
                    pipelineLogger,  // Use pipeline-specific logger!
                    exifReader,
                    dicomConverter,
                    config,
                    globalDicomSettings
                );

                // Create processing queue for this pipeline
                var processingOptions = Microsoft.Extensions.Options.Options.Create(config.ProcessingOptions);
                var queue = new ProcessingQueue(
                    loggerFactory.CreateLogger<ProcessingQueue>(),
                    fileProcessor,
                    processingOptions
                );

                // Create file system watcher
                var watcher = CreateFileSystemWatcher(config.WatchSettings);

                // Create pipeline context
                var context = new PipelineContext(
                    config,
                    fileProcessor,
                    queue,
                    watcher,
                    pipelineLogger  // Store pipeline logger in context
                );

                // Wire up file processor events
                fileProcessor.ProcessingCompleted += (sender, args) =>
                {
                    context.ProcessedCount++;
                    context.LastProcessed = DateTime.UtcNow;
                };

                fileProcessor.ProcessingError += (sender, args) =>
                {
                    context.ErrorCount++;
                };

                // Register pipeline
                if (_pipelines.TryAdd(config.Id.ToString(), context))
                {
                    // Wire up watcher events
                    watcher.Created += async (sender, e) =>
                    {
                        if (IsValidImageFile(e.FullPath, config.WatchSettings.FilePattern))
                        {
                            // Changed to DEBUG - file detection is technical detail
                            pipelineLogger.LogDebug("New file detected: {FilePath}", e.FullPath);
                            await queue.EnqueueAsync(e.FullPath, cancellationToken);
                        }
                    };

                    // Start processing queue (fire and forget!)
                    _ = Task.Run(async () =>
                    {
                        pipelineLogger.LogDebug("Starting processing queue for pipeline: {PipelineName}", config.Name);
                        await queue.ProcessQueueAsync(cancellationToken);
                    }, cancellationToken);

                    // Enable watcher
                    watcher.EnableRaisingEvents = true;
                    context.IsActive = true;

                    // Keep as INFO - important business event
                    pipelineLogger.LogInformation("Pipeline {PipelineName} started successfully. Watching: {WatchPath}",
                        config.Name, config.WatchSettings.Path);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create pipeline: {PipelineName}", config.Name);
                throw;
            }
        }

        private async Task StopPipelineAsync(PipelineContext context, CancellationToken cancellationToken)
        {
            try
            {
                context.PipelineLogger.LogInformation("Stopping pipeline: {PipelineName}", context.Configuration.Name);

                context.IsActive = false;
                context.Watcher.EnableRaisingEvents = false;

                // Give queue time to finish current work
                await Task.Delay(1000, cancellationToken);

                context.PipelineLogger.LogInformation("Pipeline {PipelineName} stopped", context.Configuration.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping pipeline: {PipelineName}", context.Configuration.Name);
            }
        }

        private async Task ReconfigurePipelinesAsync(CamBridgeSettingsV2 newSettings)
        {
            await _pipelineLock.WaitAsync();
            try
            {
                // Stop pipelines that are no longer in config
                var configuredIds = newSettings.Pipelines.Select(p => p.Id.ToString()).ToHashSet();
                var toRemove = _pipelines.Keys.Where(id => !configuredIds.Contains(id)).ToList();

                foreach (var id in toRemove)
                {
                    if (_pipelines.TryRemove(id, out var context))
                    {
                        await StopPipelineAsync(context, CancellationToken.None);
                        context.Dispose();
                    }
                }

                // Start or update pipelines from config
                foreach (var pipelineConfig in newSettings.Pipelines)
                {
                    if (pipelineConfig.Enabled)
                    {
                        if (!_pipelines.ContainsKey(pipelineConfig.Id.ToString()))
                        {
                            await CreateAndStartPipelineAsync(pipelineConfig, CancellationToken.None);
                        }
                    }
                    else if (_pipelines.TryGetValue(pipelineConfig.Id.ToString(), out var context))
                    {
                        await StopPipelineAsync(context, CancellationToken.None);
                    }
                }
            }
            finally
            {
                _pipelineLock.Release();
            }
        }

        private FileSystemWatcher CreateFileSystemWatcher(PipelineWatchSettings settings)
        {
            // Ensure watch directory exists
            Directory.CreateDirectory(settings.Path);

            var watcher = new FileSystemWatcher(settings.Path)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime,
                IncludeSubdirectories = settings.IncludeSubdirectories
            };

            // Apply file pattern filter(s)
            // Note: FileSystemWatcher only supports single pattern, so we use *.* and filter in event
            watcher.Filter = "*.*";

            return watcher;
        }

        private bool IsValidImageFile(string filePath, string filePattern)
        {
            var fileName = Path.GetFileName(filePath);

            // Default to JPEG patterns if not specified
            if (string.IsNullOrWhiteSpace(filePattern))
                filePattern = "*.jpg;*.jpeg";

            // Split patterns by semicolon and check each
            var patterns = filePattern.Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim());

            foreach (var pattern in patterns)
            {
                // Convert wildcard pattern to regex
                var regexPattern = "^" + Regex.Escape(pattern)
                    .Replace("\\*", ".*")
                    .Replace("\\?", ".") + "$";

                if (Regex.IsMatch(fileName, regexPattern, RegexOptions.IgnoreCase))
                    return true;
            }

            return false;
        }

        private string SanitizeForFileName(string pipelineName)
        {
            // Same logic as in LogViewerViewModel
            var invalid = Path.GetInvalidFileNameChars()
                .Concat(new[] { ' ', '.', ',', '/', '\\', ':', '-' })
                .Distinct()
                .ToArray();

            var sanitized = string.Join("_", pipelineName.Split(invalid, StringSplitOptions.RemoveEmptyEntries));

            if (sanitized.Length > 100)
            {
                sanitized = sanitized.Substring(0, 97) + "...";
            }

            return sanitized;
        }

        public void Dispose()
        {
            if (_disposed) return;

            try
            {
                StopAsync(CancellationToken.None).GetAwaiter().GetResult();

                foreach (var context in _pipelines.Values)
                {
                    context.Dispose();
                }

                _pipelines.Clear();
                _pipelineLock?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during PipelineManager disposal");
            }

            _disposed = true;
        }

        /// <summary>
        /// Internal class to hold pipeline runtime context
        /// </summary>
        private class PipelineContext : IDisposable
        {
            public PipelineConfiguration Configuration { get; }
            public FileProcessor FileProcessor { get; }
            public ProcessingQueue Queue { get; }
            public FileSystemWatcher Watcher { get; }
            public ILogger PipelineLogger { get; }
            public bool IsActive { get; set; }
            public DateTime LastProcessed { get; set; }
            public int ProcessedCount { get; set; }
            public int ErrorCount { get; set; }

            public PipelineContext(
                PipelineConfiguration configuration,
                FileProcessor fileProcessor,
                ProcessingQueue queue,
                FileSystemWatcher watcher,
                ILogger pipelineLogger)
            {
                Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
                FileProcessor = fileProcessor ?? throw new ArgumentNullException(nameof(fileProcessor));
                Queue = queue ?? throw new ArgumentNullException(nameof(queue));
                Watcher = watcher ?? throw new ArgumentNullException(nameof(watcher));
                PipelineLogger = pipelineLogger ?? throw new ArgumentNullException(nameof(pipelineLogger));
                LastProcessed = DateTime.MinValue;
            }

            public void Dispose()
            {
                Watcher?.Dispose();
            }
        }
    }

    /// <summary>
    /// Pipeline status information for API responses
    /// </summary>
    public class PipelineStatus
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int QueueDepth { get; set; }
        public int ProcessedCount { get; set; }
        public int ErrorCount { get; set; }
        public DateTime LastProcessed { get; set; }
        public string WatchPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
    }
}
