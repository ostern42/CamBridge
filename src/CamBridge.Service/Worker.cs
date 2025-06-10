// src/CamBridge.Service/Worker.cs
// Version: 0.7.5+tools
// Description: Main worker service that orchestrates pipeline processing
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

                // Log active pipelines
                foreach (var status in statuses.Where(s => s.Value.IsActive))
                {
                    _logger.LogInformation("Pipeline '{PipelineName}' active - watching {FolderCount} folders",
                        status.Value.Name, status.Value.WatchedFolders.Count);

                    foreach (var folder in status.Value.WatchedFolders)
                    {
                        _logger.LogInformation("  → Watching: {Path}", folder);
                    }
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

                    // Check if any pipeline has critical errors
                    CheckPipelineHealth();
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
                var totalQueued = statuses.Sum(s => s.Value.QueueLength);
                var totalActive = statuses.Sum(s => s.Value.ActiveProcessing);
                var totalProcessed = statuses.Sum(s => s.Value.TotalProcessed);
                var totalSuccessful = statuses.Sum(s => s.Value.TotalSuccessful);
                var totalFailed = statuses.Sum(s => s.Value.TotalFailed);

                _logger.LogInformation(
                    "Service Status - Pipelines: {PipelineCount} ({ActiveCount} active), " +
                    "Queue: {QueueLength}, Processing: {ActiveProcessing}, " +
                    "Total: {TotalProcessed} (Success: {Success}, Failed: {Failed})",
                    statuses.Count,
                    statuses.Count(s => s.Value.IsActive),
                    totalQueued,
                    totalActive,
                    totalProcessed,
                    totalSuccessful,
                    totalFailed);

                // Report per-pipeline status if multiple pipelines
                if (statuses.Count > 1)
                {
                    foreach (var pipeline in statuses.Where(s => s.Value.IsActive))
                    {
                        if (pipeline.Value.TotalProcessed > 0 || pipeline.Value.QueueLength > 0)
                        {
                            _logger.LogInformation(
                                "  Pipeline '{Name}': Queue: {Queue}, Active: {Active}, " +
                                "Processed: {Processed} (Success: {Success}, Failed: {Failed})",
                                pipeline.Value.Name,
                                pipeline.Value.QueueLength,
                                pipeline.Value.ActiveProcessing,
                                pipeline.Value.TotalProcessed,
                                pipeline.Value.TotalSuccessful,
                                pipeline.Value.TotalFailed);
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
                    if (pipeline.Value.TotalProcessed > 100)
                    {
                        var failureRate = (double)pipeline.Value.TotalFailed / pipeline.Value.TotalProcessed;
                        if (failureRate > 0.5) // 50% failure rate
                        {
                            _logger.LogWarning(
                                "Pipeline '{PipelineName}' has high failure rate: {FailureRate:P}",
                                pipeline.Value.Name, failureRate);
                        }
                    }

                    // Check for large queue backlogs
                    if (pipeline.Value.QueueLength > 1000)
                    {
                        _logger.LogWarning(
                            "Pipeline '{PipelineName}' has large queue backlog: {QueueLength} files",
                            pipeline.Value.Name, pipeline.Value.QueueLength);
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
                    if (pipeline.Value.TotalProcessed > 0)
                    {
                        var successRate = pipeline.Value.TotalProcessed > 0
                            ? (double)pipeline.Value.TotalSuccessful / pipeline.Value.TotalProcessed * 100
                            : 0;

                        _logger.LogInformation(
                            "Pipeline '{Name}': Total: {Total}, Success: {Success} ({SuccessRate:F1}%), Failed: {Failed}",
                            pipeline.Value.Name,
                            pipeline.Value.TotalProcessed,
                            pipeline.Value.TotalSuccessful,
                            successRate,
                            pipeline.Value.TotalFailed);
                    }
                }

                var totalProcessed = statuses.Sum(s => s.Value.TotalProcessed);
                var totalSuccessful = statuses.Sum(s => s.Value.TotalSuccessful);
                var totalFailed = statuses.Sum(s => s.Value.TotalFailed);

                if (totalProcessed > 0)
                {
                    _logger.LogInformation(
                        "Overall: Total: {Total}, Success: {Success}, Failed: {Failed}",
                        totalProcessed, totalSuccessful, totalFailed);
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
