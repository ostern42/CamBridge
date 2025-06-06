using System.Diagnostics;
using CamBridge.Core;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace CamBridge.Service;

/// <summary>
/// Main worker service that orchestrates file processing
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ProcessingQueue _processingQueue;
    private readonly IHostedService _folderWatcher;
    private readonly CamBridgeSettings _settings;
    private readonly IHostApplicationLifetime _lifetime;
    private Task? _processingTask;
    private Timer? _statisticsTimer;

    public Worker(
        ILogger<Worker> logger,
        ProcessingQueue processingQueue,
        FolderWatcherService folderWatcher,
        IOptions<CamBridgeSettings> settings,
        IHostApplicationLifetime lifetime)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _processingQueue = processingQueue ?? throw new ArgumentNullException(nameof(processingQueue));
        _folderWatcher = folderWatcher ?? throw new ArgumentNullException(nameof(folderWatcher));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("CamBridge Service v{Version} starting",
                FileVersionInfo.GetVersionInfo(typeof(Worker).Assembly.Location).ProductVersion);

            // Startup delay
            if (_settings.Service.StartupDelaySeconds > 0)
            {
                _logger.LogInformation("Waiting {Delay} seconds before starting...",
                    _settings.Service.StartupDelaySeconds);
                await Task.Delay(TimeSpan.FromSeconds(_settings.Service.StartupDelaySeconds), stoppingToken);
            }

            // Start folder watcher
            await _folderWatcher.StartAsync(stoppingToken);

            // Start processing queue in background
            _processingTask = Task.Run(async () =>
            {
                try
                {
                    await _processingQueue.ProcessQueueAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected during shutdown
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Fatal error in processing queue");
                    _lifetime.StopApplication();
                }
            }, stoppingToken);

            // Start statistics reporting
            _statisticsTimer = new Timer(
                ReportStatistics,
                null,
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(5));

            _logger.LogInformation("CamBridge Service started successfully");
            _logger.LogInformation("Monitoring {FolderCount} folders for JPEG files",
                _settings.WatchFolders.Count(f => f.Enabled));

            // Keep service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);

                // Check if processing task is still running
                if (_processingTask?.IsCompleted == true)
                {
                    _logger.LogError("Processing task terminated unexpectedly");
                    _lifetime.StopApplication();
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Fatal error in CamBridge Service");
            throw;
        }
        finally
        {
            _logger.LogInformation("CamBridge Service shutting down");

            // Stop statistics timer
            _statisticsTimer?.Dispose();

            // Stop folder watcher
            await _folderWatcher.StopAsync(CancellationToken.None);

            // Wait for processing to complete
            if (_processingTask != null)
            {
                try
                {
                    await _processingTask.WaitAsync(TimeSpan.FromSeconds(30));
                }
                catch (TimeoutException)
                {
                    _logger.LogWarning("Processing queue did not complete within timeout");
                }
            }

            // Final statistics
            ReportStatistics(null);

            _logger.LogInformation("CamBridge Service stopped");
        }
    }

    private void ReportStatistics(object? state)
    {
        try
        {
            var stats = _processingQueue.GetStatistics();

            _logger.LogInformation(
                "Processing Statistics - Queue: {QueueLength}, Active: {ActiveProcessing}, " +
                "Total: {TotalProcessed}, Success: {TotalSuccessful} ({SuccessRate:F1}%), " +
                "Failed: {TotalFailed}, Rate: {ProcessingRate:F1} files/min",
                stats.QueueLength,
                stats.ActiveProcessing,
                stats.TotalProcessed,
                stats.TotalSuccessful,
                stats.SuccessRate,
                stats.TotalFailed,
                stats.ProcessingRate);

            // Log active items if any
            var activeItems = _processingQueue.GetActiveItems();
            if (activeItems.Count > 0)
            {
                foreach (var item in activeItems)
                {
                    _logger.LogInformation(
                        "Active: {FilePath} (Attempt: {AttemptCount}, Duration: {Duration:F1}s)",
                        item.FilePath,
                        item.AttemptCount,
                        item.Duration.TotalSeconds);
                }
            }

            // Check system health
            if (stats.TotalProcessed > 100 && stats.SuccessRate < 50)
            {
                _logger.LogWarning("Low success rate detected: {SuccessRate:F1}%", stats.SuccessRate);
            }

            if (stats.QueueLength > 1000)
            {
                _logger.LogWarning("Large queue backlog: {QueueLength} files pending", stats.QueueLength);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting statistics");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("CamBridge Service stop requested");
        await base.StopAsync(cancellationToken);
    }
}
