// src/CamBridge.Service/Worker.cs
// Version: 0.8.8
// Description: Main worker service with hierarchical logging support
// Copyright: (c) 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Enums;
using CamBridge.Core.Logging;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CamBridge.Service
{
    /// <summary>
    /// Main worker service that manages the pipeline infrastructure
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly PipelineManager _pipelineManager;
        private readonly IOptionsMonitor<CamBridgeSettingsV2> _settingsMonitor;
        private readonly IHostApplicationLifetime _lifetime;
        private Timer? _statusTimer;
        private LogContext? _serviceLogContext;

        public Worker(
            ILogger<Worker> logger,
            PipelineManager pipelineManager,
            IOptionsMonitor<CamBridgeSettingsV2> settingsMonitor,
            IHostApplicationLifetime lifetime)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pipelineManager = pipelineManager ?? throw new ArgumentNullException(nameof(pipelineManager));
            _settingsMonitor = settingsMonitor ?? throw new ArgumentNullException(nameof(settingsMonitor));
            _lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var settings = _settingsMonitor.CurrentValue;
                var logVerbosity = settings.Service.LogVerbosity;

                // Create service-level correlation ID
                var serviceCorrelationId = $"S{DateTime.Now:yyyyMMddHHmmss}-Service";
                _serviceLogContext = _logger.CreateContext(serviceCorrelationId, "Service", logVerbosity);

                // Service startup stage
                using (var startupStage = _serviceLogContext.BeginStage(ProcessingStage.ServiceStartup, "CamBridge Service starting"))
                {
                    _serviceLogContext.LogInformation("{ServiceName}", ServiceInfo.GetFullVersionString());
                    _serviceLogContext.LogInformation("Log verbosity: {Verbosity}", logVerbosity);
                    _serviceLogContext.LogInformation("API Port: {Port}", settings.Service.ApiPort);

                    // Startup delay if configured
                    if (settings.Service.StartupDelaySeconds > 0)
                    {
                        _serviceLogContext.LogInformation("Waiting {Delay} seconds before starting...",
                            settings.Service.StartupDelaySeconds);
                        await Task.Delay(TimeSpan.FromSeconds(settings.Service.StartupDelaySeconds), stoppingToken);
                    }
                }

                // Configuration loading stage
                using (var configStage = _serviceLogContext.BeginStage(ProcessingStage.ConfigurationLoading, "Loading configuration"))
                {
                    _serviceLogContext.LogInformation("Pipelines configured: {Count}",
                        settings.Pipelines?.Count ?? 0);
                    _serviceLogContext.LogInformation("Notification settings: Email={Email}, EventLog={EventLog}",
                        settings.Notifications?.Email?.Enabled ?? false,
                        settings.Notifications?.EventLog?.Enabled ?? false);
                }

                // Pipeline initialization stage
                using (var pipelineStage = _serviceLogContext.BeginStage(ProcessingStage.PipelineInitialization, "Starting pipelines"))
                {
                    // Start all configured pipelines
                    await _pipelineManager.StartAsync(stoppingToken);

                    // Get initial status - FIXED: Use GetPipelineStatus()
                    var pipelines = _pipelineManager.GetPipelineStatus();
                    var statuses = pipelines.Values.ToList();

                    _serviceLogContext.LogInformation("Started {PipelineCount} pipelines", statuses.Count);

                    // Log active pipelines - FIXED: Use IsRunning instead of IsActive
                    foreach (var status in statuses.Where(s => s.IsRunning))
                    {
                        _serviceLogContext.LogInformation("Pipeline '{PipelineName}' active - watching {WatchPath}",
                            status.Name, status.WatchFolder);  // FIXED: WatchFolder instead of WatchPath
                    }
                }

                // Setup status reporting timer
                _statusTimer = new Timer(
                    ReportStatus,
                    null,
                    TimeSpan.FromSeconds(30), // Initial delay
                    TimeSpan.FromSeconds(30)  // Report every 30 seconds
                );

                _serviceLogContext.LogInformation("CamBridge Service started successfully");

                // Keep service running
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                    CheckPipelineHealth();
                }
            }
            catch (OperationCanceledException)
            {
                // This is NORMAL during shutdown - don't log as error!
                if (_serviceLogContext != null && _serviceLogContext.ShouldLog(LogVerbosity.Normal))
                {
                    _serviceLogContext.LogInformation("Service shutdown requested");
                }
            }
            catch (Exception ex)
            {
                if (_serviceLogContext != null)
                {
                    _serviceLogContext.LogError(ex, "Fatal error in CamBridge Service");
                }
                else
                {
                    _logger.LogCritical(ex, "Fatal error in CamBridge Service");
                }
                throw;
            }
            finally
            {
                await ShutdownAsync();
            }
        }

        private async Task ShutdownAsync()
        {
            if (_serviceLogContext != null)
            {
                using (var shutdownStage = _serviceLogContext.BeginStage(ProcessingStage.ServiceShutdown, "Shutting down service"))
                {
                    _serviceLogContext.LogInformation("CamBridge Service shutting down");

                    // Stop status timer
                    if (_statusTimer != null)
                    {
                        _statusTimer.Dispose();
                        _serviceLogContext.LogDebug("Status timer stopped");
                    }

                    // Stop all pipelines
                    try
                    {
                        await _pipelineManager.StopAsync();
                        _serviceLogContext.LogInformation("All pipelines stopped");
                    }
                    catch (Exception ex)
                    {
                        _serviceLogContext.LogError(ex, "Error stopping pipeline manager");
                    }

                    // Final statistics
                    ReportFinalStatistics();

                    _serviceLogContext.LogInformation("CamBridge Service stopped");
                }
            }
            else
            {
                _logger.LogInformation("CamBridge Service stopped");
            }
        }

        private void ReportStatus(object? state)
        {
            try
            {
                // FIXED: Use GetPipelineStatus()
                var pipelines = _pipelineManager.GetPipelineStatus();
                var statuses = pipelines.Values.ToList();

                // Summary statistics - FIXED: Use correct property names
                var totalQueued = statuses.Sum(s => s.QueueLength);      // FIXED: QueueLength instead of QueueDepth
                var totalProcessed = statuses.Sum(s => s.ProcessedCount);
                var totalErrors = statuses.Sum(s => s.ErrorCount);
                var totalActive = statuses.Count(s => s.IsRunning);      // FIXED: IsRunning instead of IsActive

                // Only log if there's activity
                if (totalProcessed > 0 || totalQueued > 0 || totalErrors > 0)
                {
                    if (_serviceLogContext != null && _serviceLogContext.ShouldLog(LogVerbosity.Normal))
                    {
                        _serviceLogContext.LogInformation(
                            "Service Status - Pipelines: {PipelineCount} ({ActiveCount} active), " +
                            "Queue: {QueueLength}, Total: {TotalProcessed} (Errors: {Errors})",
                            statuses.Count,
                            totalActive,
                            totalQueued,
                            totalProcessed,
                            totalErrors);

                        // Report per-pipeline status if multiple pipelines and there's activity
                        if (statuses.Count > 1 && _serviceLogContext.ShouldLog(LogVerbosity.Detailed))
                        {
                            foreach (var pipeline in statuses.Where(s => s.IsRunning))  // FIXED: IsRunning
                            {
                                if (pipeline.ProcessedCount > 0 || pipeline.QueueLength > 0 || pipeline.ErrorCount > 0)
                                {
                                    _serviceLogContext.LogInformation(
                                        "  Pipeline '{Name}': Queue: {Queue}, Processed: {Processed}, Errors: {Errors}",
                                        pipeline.Name,
                                        pipeline.QueueLength,     // FIXED: QueueLength instead of QueueDepth
                                        pipeline.ProcessedCount,
                                        pipeline.ErrorCount);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reporting status");
            }
        }

        private void CheckPipelineHealth()
        {
            try
            {
                // FIXED: Use GetPipelineStatus()
                var pipelines = _pipelineManager.GetPipelineStatus();
                var statuses = pipelines.Values.ToList();

                foreach (var pipeline in statuses)
                {
                    // Check for high failure rates
                    if (pipeline.ProcessedCount > 100)
                    {
                        var failureRate = (double)pipeline.ErrorCount / pipeline.ProcessedCount;
                        if (failureRate > 0.5) // 50% failure rate
                        {
                            if (_serviceLogContext != null)
                            {
                                _serviceLogContext.LogWarning(
                                    "Pipeline '{PipelineName}' has high failure rate: {FailureRate:P}",
                                    pipeline.Name, failureRate);
                            }
                        }
                    }

                    // Check for large queue backlogs - FIXED: Use QueueLength
                    if (pipeline.QueueLength > 1000)
                    {
                        if (_serviceLogContext != null)
                        {
                            _serviceLogContext.LogWarning(
                                "Pipeline '{PipelineName}' has large queue backlog: {QueueLength} files",
                                pipeline.Name, pipeline.QueueLength);  // FIXED: QueueLength property
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking pipeline health");
            }
        }

        private void ReportFinalStatistics()
        {
            try
            {
                // FIXED: Use GetPipelineStatus()
                var pipelines = _pipelineManager.GetPipelineStatus();
                var statuses = pipelines.Values.ToList();

                if (_serviceLogContext != null && _serviceLogContext.ShouldLog(LogVerbosity.Normal))
                {
                    _serviceLogContext.LogInformation("=== Final Pipeline Statistics ===");

                    foreach (var pipeline in statuses)
                    {
                        if (pipeline.ProcessedCount > 0)
                        {
                            var successRate = pipeline.ProcessedCount > 0
                                ? (double)(pipeline.ProcessedCount - pipeline.ErrorCount) / pipeline.ProcessedCount * 100
                                : 0;

                            _serviceLogContext.LogInformation(
                                "Pipeline '{Name}': Total: {Total}, Success: {Success} ({SuccessRate:F1}%), Failed: {Failed}",
                                pipeline.Name,
                                pipeline.ProcessedCount,
                                pipeline.ProcessedCount - pipeline.ErrorCount,
                                successRate,
                                pipeline.ErrorCount);
                        }
                    }

                    var totalProcessed = statuses.Sum(s => s.ProcessedCount);
                    var totalErrors = statuses.Sum(s => s.ErrorCount);
                    var totalSuccessful = totalProcessed - totalErrors;

                    if (totalProcessed > 0)
                    {
                        _serviceLogContext.LogInformation(
                            "Overall: Total: {Total}, Success: {Success}, Failed: {Failed}",
                            totalProcessed, totalSuccessful, totalErrors);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reporting final statistics");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_serviceLogContext != null && _serviceLogContext.ShouldLog(LogVerbosity.Normal))
            {
                _serviceLogContext.LogInformation("CamBridge Service stop requested");
            }
            await base.StopAsync(cancellationToken);
        }
    }
}
