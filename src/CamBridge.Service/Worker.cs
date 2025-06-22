// src/CamBridge.Service/Worker.cs
// Version: 0.7.28
// Description: Main worker service with fixed shutdown handling
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Core;
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
                _logger.LogInformation("{ServiceName} starting",
                    ServiceInfo.GetFullVersionString());

                var settings = _settingsMonitor.CurrentValue;

                // Startup delay if configured
                if (settings.Service.StartupDelaySeconds > 0)
                {
                    _logger.LogInformation("Waiting {Delay} seconds before starting...",
                        settings.Service.StartupDelaySeconds);
                    await Task.Delay(TimeSpan.FromSeconds(settings.Service.StartupDelaySeconds), stoppingToken);
                }

                // Start all configured pipelines
                await _pipelineManager.StartAsync(stoppingToken);

                // Get initial status
                var statuses = _pipelineManager.GetPipelineStatuses();
                _logger.LogInformation("CamBridge Service started with {PipelineCount} pipelines",
                    statuses.Count);

                // Log active pipelines - keeping as INFO because it's important startup info
                foreach (var status in statuses.Where(s => s.IsActive))
                {
                    _logger.LogInformation("Pipeline '{PipelineName}' active - watching {WatchPath}",
                        status.Name, status.WatchPath);
                }

                // Setup status reporting timer
                _statusTimer = new Timer(
                    ReportStatus,
                    null,
                    TimeSpan.FromSeconds(30), // Initial delay
                    TimeSpan.FromSeconds(30)  // Report every 30 seconds
                );

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
                _logger.LogInformation("Service shutdown requested");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Fatal error in CamBridge Service");
                throw;
            }
            finally
            {
                _logger.LogInformation("CamBridge Service shutting down");

                // Stop status timer
                _statusTimer?.Dispose();

                // Stop all pipelines
                try
                {
                    await _pipelineManager.StopAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error stopping pipeline manager");
                }

                // Final statistics
                ReportFinalStatistics();

                _logger.LogInformation("CamBridge Service stopped");
            }
        }

        private void ReportStatus(object? state)
        {
            try
            {
                var statuses = _pipelineManager.GetPipelineStatuses();

                // Summary statistics
                var totalQueued = statuses.Sum(s => s.QueueDepth);
                var totalProcessed = statuses.Sum(s => s.ProcessedCount);
                var totalErrors = statuses.Sum(s => s.ErrorCount);
                var totalActive = statuses.Count(s => s.IsActive);

                // Only log if there's activity - keeping as INFO because it's important operational status
                if (totalProcessed > 0 || totalQueued > 0 || totalErrors > 0)
                {
                    _logger.LogInformation(
                        "Service Status - Pipelines: {PipelineCount} ({ActiveCount} active), " +
                        "Queue: {QueueLength}, Total: {TotalProcessed} (Errors: {Errors})",
                        statuses.Count,
                        totalActive,
                        totalQueued,
                        totalProcessed,
                        totalErrors);

                    // Report per-pipeline status if multiple pipelines and there's activity
                    if (statuses.Count > 1)
                    {
                        foreach (var pipeline in statuses.Where(s => s.IsActive))
                        {
                            if (pipeline.ProcessedCount > 0 || pipeline.QueueDepth > 0 || pipeline.ErrorCount > 0)
                            {
                                _logger.LogInformation(
                                    "  Pipeline '{Name}': Queue: {Queue}, Processed: {Processed}, Errors: {Errors}",
                                    pipeline.Name,
                                    pipeline.QueueDepth,
                                    pipeline.ProcessedCount,
                                    pipeline.ErrorCount);
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
                var statuses = _pipelineManager.GetPipelineStatuses();

                foreach (var pipeline in statuses)
                {
                    // Check for high failure rates
                    if (pipeline.ProcessedCount > 100)
                    {
                        var failureRate = (double)pipeline.ErrorCount / pipeline.ProcessedCount;
                        if (failureRate > 0.5) // 50% failure rate
                        {
                            _logger.LogWarning(
                                "Pipeline '{PipelineName}' has high failure rate: {FailureRate:P}",
                                pipeline.Name, failureRate);
                        }
                    }

                    // Check for large queue backlogs
                    if (pipeline.QueueDepth > 1000)
                    {
                        _logger.LogWarning(
                            "Pipeline '{PipelineName}' has large queue backlog: {QueueLength} files",
                            pipeline.Name, pipeline.QueueDepth);
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
                var statuses = _pipelineManager.GetPipelineStatuses();

                _logger.LogInformation("=== Final Pipeline Statistics ===");

                foreach (var pipeline in statuses)
                {
                    if (pipeline.ProcessedCount > 0)
                    {
                        var successRate = pipeline.ProcessedCount > 0
                            ? (double)(pipeline.ProcessedCount - pipeline.ErrorCount) / pipeline.ProcessedCount * 100
                            : 0;

                        _logger.LogInformation(
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
                    _logger.LogInformation(
                        "Overall: Total: {Total}, Success: {Success}, Failed: {Failed}",
                        totalProcessed, totalSuccessful, totalErrors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reporting final statistics");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CamBridge Service stop requested");
            await base.StopAsync(cancellationToken);
        }
    }
}
