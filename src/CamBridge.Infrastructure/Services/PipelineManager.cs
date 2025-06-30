// src/CamBridge.Infrastructure/Services/PipelineManager.cs
// Version: 0.8.8
// Last Modified: 2025-06-30
// Description: Manages multiple processing pipelines with hierarchical logging
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Core;
using CamBridge.Core.Enums;
using CamBridge.Core.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Manages multiple file processing pipelines
    /// </summary>
    public class PipelineManager : IHostedService, IDisposable
    {
        private readonly ILogger<PipelineManager> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IOptionsMonitor<CamBridgeSettingsV2> _settingsMonitor;  // FIXED: Use IOptionsMonitor
        private readonly MappingConfigurationLoader _mappingLoader;
        private readonly NotificationService _notificationService;
        private readonly DicomStoreService? _dicomStoreService;

        private readonly ConcurrentDictionary<string, PipelineStatus> _pipelines;
        private readonly SemaphoreSlim _startupSemaphore;
        private CancellationTokenSource? _shutdownTokenSource;

        public PipelineManager(
            ILogger<PipelineManager> logger,
            ILoggerFactory loggerFactory,
            IOptionsMonitor<CamBridgeSettingsV2> settingsMonitor,  // FIXED: Accept IOptionsMonitor
            MappingConfigurationLoader mappingLoader,
            NotificationService notificationService,
            DicomStoreService? dicomStoreService = null)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
            _settingsMonitor = settingsMonitor;  // FIXED: Store IOptionsMonitor
            _mappingLoader = mappingLoader;
            _notificationService = notificationService;
            _dicomStoreService = dicomStoreService;

            _pipelines = new ConcurrentDictionary<string, PipelineStatus>();
            _startupSemaphore = new SemaphoreSlim(1, 1);
        }

        /// <summary>
        /// Starts all configured pipelines
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _startupSemaphore.WaitAsync(cancellationToken);
            try
            {
                _logger.LogInformation("Starting PipelineManager");
                _shutdownTokenSource = new CancellationTokenSource();

                // FIXED: Get current settings value
                var settings = _settingsMonitor.CurrentValue;

                // Validate settings
                if (settings?.Pipelines == null || !settings.Pipelines.Any())
                {
                    _logger.LogWarning("No pipelines configured");
                    return;
                }

                // DEBUG: Log what we actually loaded
                foreach (var pipeline in settings.Pipelines)
                {
                    _logger.LogDebug("Pipeline {Name}: Enabled={Enabled}, WatchPath={Watch}, ArchiveFolder={Archive}",
                        pipeline.Name,
                        pipeline.Enabled,
                        pipeline.WatchSettings?.Path ?? "(null)",
                        pipeline.ProcessingOptions?.ArchiveFolder ?? "(null)");
                }

                // Start each pipeline
                var startTasks = settings.Pipelines
                    .Where(p => p.Enabled)
                    .Select(p => CreateAndStartPipelineAsync(p, _shutdownTokenSource.Token));

                await Task.WhenAll(startTasks);

                _logger.LogInformation("Started {Count} pipelines", _pipelines.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start pipelines");
                await _notificationService.NotifyErrorAsync($"Failed to start pipelines: {ex.Message}", ex);
                throw;
            }
            finally
            {
                _startupSemaphore.Release();
            }
        }

        /// <summary>
        /// Stops all running pipelines
        /// </summary>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping PipelineManager");

            // Signal shutdown to all pipelines
            _shutdownTokenSource?.Cancel();

            // Stop all pipelines
            var stopTasks = _pipelines.Values.Select(p => StopPipelineAsync(p, cancellationToken));
            await Task.WhenAll(stopTasks);

            // Clear pipeline dictionary
            _pipelines.Clear();

            _logger.LogInformation("PipelineManager stopped");
        }

        /// <summary>
        /// Creates and starts a single pipeline
        /// </summary>
        private async Task CreateAndStartPipelineAsync(PipelineConfiguration config, CancellationToken cancellationToken)
        {
            try
            {
                // Generate correlation ID for this pipeline session
                var pipelineCorrelationId = $"P{DateTime.Now:yyyyMMddHHmmss}-{SanitizeForFileName(config.Name)}";

                // FIXED: Get current settings value
                var settings = _settingsMonitor.CurrentValue;
                var logVerbosity = settings.Service?.LogVerbosity ?? LogVerbosity.Detailed;  // Add null check with default

                // Create named logger for this pipeline
                var pipelineLogger = _loggerFactory.CreateLogger($"Pipeline.{config.Name}");
                var logContext = pipelineLogger.CreateContext(pipelineCorrelationId, config.Name, logVerbosity);

                // Note: LogContext is not IDisposable, use BeginStage for timing
                var stageContext = logContext.BeginStage(ProcessingStage.PipelineInitialization, $"Starting pipeline {config.Name}");
                try
                {
                    // Validate configuration
                    ValidatePipelineConfiguration(config);

                    // Extra validation logging
                    if (config.ProcessingOptions == null)
                    {
                        var ex = new InvalidOperationException($"ProcessingOptions is null for pipeline {config.Name}");
                        logContext.LogError(ex, "ProcessingOptions is NULL for pipeline {Pipeline}!", config.Name);
                        throw ex;
                    }

                    // Log pipeline configuration
                    logContext.LogInformation("Watch folder: {WatchFolder}", config.WatchSettings.Path);

                    // Debug output path resolution
                    var outputPath = config.WatchSettings.OutputPath ?? config.ProcessingOptions.ArchiveFolder;
                    logContext.LogInformation("Output path resolution: WatchSettings.OutputPath={OutputPath}, ArchiveFolder={ArchiveFolder}, Final={Final}",
                        config.WatchSettings.OutputPath ?? "(null)",
                        config.ProcessingOptions.ArchiveFolder ?? "(null)",
                        outputPath ?? "(null)");

                    logContext.LogInformation("Output folder: {OutputFolder}", outputPath);
                    logContext.LogInformation("File pattern: {Pattern}", config.WatchSettings.FilePattern);
                    logContext.LogInformation("Max concurrent: {MaxConcurrent}", config.ProcessingOptions.MaxConcurrentProcessing);

                    if (config.PacsConfiguration?.Enabled == true)
                    {
                        logContext.LogInformation("PACS upload enabled for pipeline {Pipeline} -> {Host}:{Port}",
                            config.Name, config.PacsConfiguration.Host, config.PacsConfiguration.Port);
                    }

                    // Create output directory structure
                    // EXTRA VALIDATION: Make sure we have a valid path
                    if (string.IsNullOrWhiteSpace(outputPath))
                    {
                        outputPath = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            "CamBridge",
                            "Output",
                            config.Name
                        );
                        logContext.LogWarning("Neither OutputPath nor ArchiveFolder configured for pipeline {Pipeline}, using default: {Path}",
                            config.Name, outputPath);
                    }

                    CreateOutputDirectoryStructure(outputPath, config.ProcessingOptions, logContext);

                    // Load mapping configuration from the mapping set
                    var mappingConfigPath = GetMappingConfigPath(config);

                    try
                    {
                        // Load mapping configuration - returns bool
                        var loadSuccess = await _mappingLoader.LoadConfigurationAsync(mappingConfigPath);
                        if (loadSuccess)
                        {
                            logContext.LogInformation("Mapping configuration loaded successfully");
                        }
                        else
                        {
                            logContext.LogWarning("Failed to load mapping configuration, using defaults");
                        }
                    }
                    catch (Exception ex)
                    {
                        logContext.LogWarning("Error loading mapping configuration, using defaults: {Message}", ex.Message);
                    }

                    // Create processing components
                    var components = CreateProcessingComponents(config, pipelineLogger);

                    // Create PACS upload queue if enabled
                    PacsUploadQueue? pacsQueue = null;
                    if (config.PacsConfiguration?.Enabled == true && _dicomStoreService != null)
                    {
                        pacsQueue = new PacsUploadQueue(config, _dicomStoreService,
                            _loggerFactory.CreateLogger<PacsUploadQueue>());
                        logContext.LogInformation("PACS upload queue created for {AeTitle}",
                            config.PacsConfiguration.CallingAeTitle);
                    }

                    // Create file processor with all components
                    // FIXED: Use _mappingLoader which implements IMappingConfiguration
                    var fileProcessor = new FileProcessor(
                        pipelineLogger,
                        components.ExifTool,
                        components.DicomConverter,
                        config,
                        settings.GlobalDicomSettings,  // FIXED: Use settings from IOptionsMonitor
                        components.TagMapper,
                        _mappingLoader,  // FIXED: _mappingLoader implements IMappingConfiguration
                        pacsQueue,
                        logVerbosity);

                    // Create processing queue with options wrapper
                    var processingOptions = Options.Create(config.ProcessingOptions);
                    var queue = new ProcessingQueue(
                        _loggerFactory.CreateLogger<ProcessingQueue>(),
                        fileProcessor,
                        processingOptions);

                    logContext.LogInformation("Created processing queue with max concurrent: {MaxConcurrent}",
                        config.ProcessingOptions.MaxConcurrentProcessing);

                    // Create file watcher
                    var watcher = CreateFileWatcher(config, queue);
                    logContext.LogInformation("Created file watcher for pattern {Pattern}",
                        config.WatchSettings.FilePattern);

                    // Start queue processing
                    var queueProcessingTask = queue.ProcessQueueAsync(cancellationToken);

                    // Create pipeline status
                    var status = new PipelineStatus
                    {
                        Configuration = config,
                        Queue = queue,
                        Watcher = watcher,
                        ProcessingTask = queueProcessingTask,
                        StartTime = DateTime.UtcNow,
                        LastActivityTime = DateTime.UtcNow,
                        IsRunning = true,
                        PacsQueue = pacsQueue
                    };

                    // Register pipeline
                    if (!_pipelines.TryAdd(config.Name, status))
                    {
                        throw new InvalidOperationException($"Pipeline {config.Name} already exists");
                    }

                    // Start watching for files
                    watcher.EnableRaisingEvents = true;
                    logContext.LogInformation("Pipeline {Name} started successfully", config.Name);
                }
                finally
                {
                    stageContext?.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start pipeline {Name}", config.Name);
                await _notificationService.NotifyErrorAsync($"Pipeline {config.Name} failed to start: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the mapping configuration path for a pipeline
        /// </summary>
        private string GetMappingConfigPath(PipelineConfiguration config)
        {
            // For now, use a default path - later this will use MappingSetId
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mappings.json");
        }

        /// <summary>
        /// Creates the output directory structure for a pipeline
        /// </summary>
        private void CreateOutputDirectoryStructure(string outputFolder, ProcessingOptions options, LogContext logContext)
        {
            // FIXED: Validate outputFolder and provide sensible default
            if (string.IsNullOrWhiteSpace(outputFolder))
            {
                // Don't crash - use a default instead!
                outputFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "CamBridge",
                    "Output"
                );
                logContext.LogWarning("No output folder configured, using default: {Path}", outputFolder);
            }

            // Create main output directory
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
                logContext.LogDebug("Created output directory: {Path}", outputFolder);
            }

            // FIXED: Create subdirectories based on actual configured paths
            // NOTE: BackupFolder has no UI control in PipelineConfigPage!
            // This is documented in WISDOM_DEBT.md as missing GUI element
            var subdirectories = new Dictionary<string, string?>
            {
                { "Archive", options.ArchiveFolder },
                { "Error", options.ErrorFolder }
                // Don't create Backup subdirectory - BackupFolder should be absolute path
            };

            // Handle BackupFolder separately if configured
            if (!string.IsNullOrWhiteSpace(options.BackupFolder))
            {
                try
                {
                    if (!Directory.Exists(options.BackupFolder))
                    {
                        Directory.CreateDirectory(options.BackupFolder);
                        logContext.LogDebug("Created backup directory: {Path}", options.BackupFolder);
                    }
                }
                catch (Exception ex)
                {
                    logContext.LogWarning("Failed to create backup directory at {Path}: {Error}",
                        options.BackupFolder, ex.Message);
                }
            }

            // Create other directories (these are absolute paths, not subdirectories!)
            foreach (var kvp in subdirectories)
            {
                if (!string.IsNullOrWhiteSpace(kvp.Value))
                {
                    try
                    {
                        if (!Directory.Exists(kvp.Value))
                        {
                            Directory.CreateDirectory(kvp.Value);
                            logContext.LogDebug("Created {Type} directory: {Path}", kvp.Key, kvp.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        logContext.LogWarning("Failed to create {Type} directory at {Path}: {Error}",
                            kvp.Key, kvp.Value, ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Creates processing components for a pipeline
        /// </summary>
        private (ExifToolReader ExifTool, DicomConverter DicomConverter, DicomTagMapper TagMapper)
            CreateProcessingComponents(PipelineConfiguration config, ILogger pipelineLogger)
        {
            // Create DICOM converter
            var dicomConverter = new DicomConverter(_loggerFactory.CreateLogger<DicomConverter>());

            // Create tag mapper
            var tagMapper = new DicomTagMapper(_loggerFactory.CreateLogger<DicomTagMapper>());

            // Create ExifTool reader - get path from configuration
            var exifToolPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Tools",
                "exiftool.exe");

            var exifTool = new ExifToolReader(
                _loggerFactory.CreateLogger<ExifToolReader>(),
                exifToolPath);

            return (exifTool, dicomConverter, tagMapper);
        }

        /// <summary>
        /// Validates pipeline configuration
        /// </summary>
        private void ValidatePipelineConfiguration(PipelineConfiguration config)
        {
            if (string.IsNullOrWhiteSpace(config.Name))
                throw new ArgumentException("Pipeline name is required");

            if (config.WatchSettings == null)
                throw new ArgumentException($"Watch settings are required for pipeline {config.Name}");

            if (string.IsNullOrWhiteSpace(config.WatchSettings.Path))
                throw new ArgumentException($"Watch folder is required for pipeline {config.Name}");

            if (!Directory.Exists(config.WatchSettings.Path))
                throw new DirectoryNotFoundException($"Watch folder not found for pipeline {config.Name}: {config.WatchSettings.Path}");

            if (config.ProcessingOptions == null)
                throw new ArgumentException($"Processing options are required for pipeline {config.Name}");

            // Don't require ArchiveFolder if OutputPath is set
            if (string.IsNullOrWhiteSpace(config.WatchSettings.OutputPath) &&
                string.IsNullOrWhiteSpace(config.ProcessingOptions.ArchiveFolder))
            {
                _logger.LogWarning("Pipeline {Name} has no output path configured, will use default", config.Name);
            }

            if (string.IsNullOrWhiteSpace(config.WatchSettings.FilePattern))
                throw new ArgumentException($"File pattern is required for pipeline {config.Name}");

            if (config.ProcessingOptions.MaxConcurrentProcessing <= 0)
                throw new ArgumentException($"Max concurrent processing must be positive for pipeline {config.Name}");
        }

        /// <summary>
        /// Creates a file watcher for the pipeline
        /// </summary>
        private FileSystemWatcher CreateFileWatcher(PipelineConfiguration config, ProcessingQueue queue)
        {
            var watcher = new FileSystemWatcher(config.WatchSettings.Path)
            {
                Filter = GetFirstPattern(config.WatchSettings.FilePattern),
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime,
                IncludeSubdirectories = config.WatchSettings.IncludeSubdirectories
            };

            // Handle file events
            watcher.Created += async (s, e) => await OnFileDetectedAsync(config.Name, e.FullPath, queue);
            watcher.Changed += async (s, e) => await OnFileDetectedAsync(config.Name, e.FullPath, queue);
            watcher.Error += (s, e) => OnWatcherError(config.Name, e);

            return watcher;
        }

        /// <summary>
        /// Gets the first pattern from a semicolon-separated list
        /// </summary>
        private string GetFirstPattern(string patterns)
        {
            var parts = patterns.Split(';', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[0].Trim() : "*.*";
        }

        /// <summary>
        /// Handles file detection events
        /// </summary>
        private async Task OnFileDetectedAsync(string pipelineName, string filePath, ProcessingQueue queue)
        {
            try
            {
                // Skip temporary files
                if (Path.GetFileName(filePath).StartsWith("~") ||
                    Path.GetFileName(filePath).StartsWith("."))
                {
                    return;
                }

                // Check file pattern (if multiple patterns)
                if (!IsFilePatternMatch(filePath, pipelineName))
                {
                    return;
                }

                // Add to queue
                var added = await queue.EnqueueAsync(filePath);
                if (added)
                {
                    _logger.LogDebug("[{Pipeline}] File queued: {File}", pipelineName, Path.GetFileName(filePath));

                    // Update last activity time
                    if (_pipelines.TryGetValue(pipelineName, out var status))
                    {
                        status.LastActivityTime = DateTime.UtcNow;
                    }
                }
                else
                {
                    _logger.LogWarning("[{Pipeline}] Queue full or duplicate, file skipped: {File}",
                        pipelineName, Path.GetFileName(filePath));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{Pipeline}] Error handling file detection for {File}",
                    pipelineName, filePath);
            }
        }

        /// <summary>
        /// Checks if file matches any of the pipeline's patterns
        /// </summary>
        private bool IsFilePatternMatch(string filePath, string pipelineName)
        {
            if (_pipelines.TryGetValue(pipelineName, out var status))
            {
                var patterns = status.Configuration.WatchSettings.FilePattern
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim());

                var fileName = Path.GetFileName(filePath);
                foreach (var pattern in patterns)
                {
                    if (FileMatchesPattern(fileName, pattern))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Simple pattern matching
        /// </summary>
        private bool FileMatchesPattern(string fileName, string pattern)
        {
            // Simple implementation - can be enhanced
            if (pattern == "*.*") return true;

            var extension = Path.GetExtension(fileName);
            var patternExt = pattern.StartsWith("*.") ? pattern.Substring(1) : pattern;

            return string.Equals(extension, patternExt, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Handles watcher errors
        /// </summary>
        private void OnWatcherError(string pipelineName, ErrorEventArgs e)
        {
            var ex = e.GetException();
            _logger.LogError(ex, "[{Pipeline}] File watcher error", pipelineName);

            // Try to recover by recreating the watcher
            Task.Run(async () =>
            {
                await Task.Delay(5000);
                await RecoverPipelineAsync(pipelineName);
            });
        }

        /// <summary>
        /// Attempts to recover a failed pipeline
        /// </summary>
        private async Task RecoverPipelineAsync(string pipelineName)
        {
            try
            {
                _logger.LogInformation("[{Pipeline}] Attempting to recover pipeline", pipelineName);

                if (_pipelines.TryGetValue(pipelineName, out var status))
                {
                    // Stop the current pipeline
                    await StopPipelineAsync(status, CancellationToken.None);

                    // Remove from dictionary
                    _pipelines.TryRemove(pipelineName, out _);

                    // Restart if we're not shutting down
                    if (_shutdownTokenSource != null && !_shutdownTokenSource.Token.IsCancellationRequested)
                    {
                        await CreateAndStartPipelineAsync(status.Configuration, _shutdownTokenSource.Token);
                        _logger.LogInformation("[{Pipeline}] Pipeline recovered successfully", pipelineName);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{Pipeline}] Failed to recover pipeline", pipelineName);
            }
        }

        /// <summary>
        /// Stops a single pipeline
        /// </summary>
        private async Task StopPipelineAsync(PipelineStatus status, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Stopping pipeline {Name}", status.Configuration.Name);

                // Stop watching for new files
                status.Watcher.EnableRaisingEvents = false;
                status.Watcher.Dispose();

                // Stop queue processing
                await status.Queue.StopAsync(cancellationToken);

                // Wait for processing to complete
                if (status.ProcessingTask != null)
                {
                    try
                    {
                        await status.ProcessingTask.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);
                    }
                    catch (TimeoutException)
                    {
                        _logger.LogWarning("Pipeline {Name} processing task did not complete in time",
                            status.Configuration.Name);
                    }
                }

                // Stop PACS queue if present
                if (status.PacsQueue != null)
                {
                    status.PacsQueue.Dispose();
                }

                status.IsRunning = false;
                status.StopTime = DateTime.UtcNow;

                _logger.LogInformation("Pipeline {Name} stopped", status.Configuration.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping pipeline {Name}", status.Configuration.Name);
            }
        }

        /// <summary>
        /// Gets the current status of all pipelines
        /// </summary>
        public IReadOnlyDictionary<string, PipelineInfo> GetPipelineStatus()
        {
            var result = new Dictionary<string, PipelineInfo>();

            foreach (var kvp in _pipelines)
            {
                var status = kvp.Value;
                var stats = status.Queue.GetStatistics();

                result[kvp.Key] = new PipelineInfo
                {
                    Name = kvp.Key,
                    IsRunning = status.IsRunning,
                    StartTime = status.StartTime,
                    LastActivityTime = status.LastActivityTime,
                    ProcessedCount = stats.TotalProcessed,
                    ErrorCount = stats.TotalFailed,
                    QueueLength = stats.QueueLength,
                    WatchFolder = status.Configuration.WatchSettings.Path,
                    OutputFolder = status.Configuration.WatchSettings.OutputPath ??
                                   status.Configuration.ProcessingOptions.ArchiveFolder
                };
            }

            return result;
        }

        /// <summary>
        /// Gets detailed information about a specific pipeline
        /// </summary>
        public PipelineInfo? GetPipelineInfo(string pipelineName)
        {
            if (_pipelines.TryGetValue(pipelineName, out var status))
            {
                var stats = status.Queue.GetStatistics();

                return new PipelineInfo
                {
                    Name = pipelineName,
                    IsRunning = status.IsRunning,
                    StartTime = status.StartTime,
                    LastActivityTime = status.LastActivityTime,
                    ProcessedCount = stats.TotalProcessed,
                    ErrorCount = stats.TotalFailed,
                    QueueLength = stats.QueueLength,
                    WatchFolder = status.Configuration.WatchSettings.Path,
                    OutputFolder = status.Configuration.WatchSettings.OutputPath ??
                                   status.Configuration.ProcessingOptions.ArchiveFolder
                };
            }

            return null;
        }

        /// <summary>
        /// Gets pipeline statuses for Worker.cs
        /// </summary>
        public List<PipelineStatusInfo> GetPipelineStatuses()
        {
            var result = new List<PipelineStatusInfo>();

            foreach (var kvp in _pipelines)
            {
                var status = kvp.Value;
                var stats = status.Queue.GetStatistics();

                result.Add(new PipelineStatusInfo
                {
                    Name = kvp.Key,
                    IsActive = status.IsRunning,
                    WatchPath = status.Configuration.WatchSettings.Path,
                    QueueDepth = stats.QueueLength,
                    ProcessedCount = stats.TotalProcessed,
                    ErrorCount = stats.TotalFailed
                });
            }

            return result;
        }

        /// <summary>
        /// Stops all pipelines (called by Worker.cs)
        /// </summary>
        public async Task StopAsync()
        {
            await StopAsync(CancellationToken.None);
        }

        /// <summary>
        /// Sanitizes a string for use in file names
        /// </summary>
        private string SanitizeForFileName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars()
                .Concat(new[] { ' ', '.', ',', '/', '\\', ':', '-' })
                .Distinct()
                .ToArray();

            return string.Join("_", name.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
        }

        public void Dispose()
        {
            _shutdownTokenSource?.Cancel();
            _shutdownTokenSource?.Dispose();
            _startupSemaphore?.Dispose();

            // Dispose all pipeline resources
            foreach (var status in _pipelines.Values)
            {
                status.Watcher?.Dispose();
                status.PacsQueue?.Dispose();
            }

            _pipelines.Clear();
        }
    }

    /// <summary>
    /// Internal class to track pipeline status
    /// </summary>
    internal class PipelineStatus
    {
        public required PipelineConfiguration Configuration { get; init; }
        public required ProcessingQueue Queue { get; init; }
        public required FileSystemWatcher Watcher { get; init; }
        public Task? ProcessingTask { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime? StopTime { get; set; }
        public DateTime LastActivityTime { get; set; }
        public bool IsRunning { get; set; }
        public PacsUploadQueue? PacsQueue { get; init; }
    }

    /// <summary>
    /// Public information about a pipeline
    /// </summary>
    public class PipelineInfo
    {
        public string Name { get; init; } = string.Empty;
        public bool IsRunning { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime LastActivityTime { get; init; }
        public int ProcessedCount { get; init; }
        public int ErrorCount { get; init; }
        public int QueueLength { get; init; }
        public string WatchFolder { get; init; } = string.Empty;
        public string OutputFolder { get; init; } = string.Empty;
    }

    /// <summary>
    /// Status info for Worker.cs compatibility
    /// </summary>
    public class PipelineStatusInfo
    {
        public string Name { get; init; } = string.Empty;
        public bool IsActive { get; init; }
        public string WatchPath { get; init; } = string.Empty;
        public int QueueDepth { get; init; }
        public int ProcessedCount { get; init; }
        public int ErrorCount { get; init; }
    }
}
