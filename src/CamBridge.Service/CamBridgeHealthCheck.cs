// src/CamBridge.Service/CamBridgeHealthCheck.cs
// Version: 0.7.0
// Description: Health check implementation using PipelineManager instead of ProcessingQueue
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Core.Interfaces;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace CamBridge.Service
{
    /// <summary>
    /// Health check for CamBridge service
    /// KISS UPDATE: Now uses PipelineManager to aggregate health from all pipelines
    /// </summary>
    public class CamBridgeHealthCheck : IHealthCheck
    {
        private readonly ILogger<CamBridgeHealthCheck> _logger;
        private readonly PipelineManager _pipelineManager;
        private readonly INotificationService _notificationService;

        public CamBridgeHealthCheck(
            ILogger<CamBridgeHealthCheck> logger,
            PipelineManager pipelineManager,
            INotificationService notificationService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pipelineManager = pipelineManager ?? throw new ArgumentNullException(nameof(pipelineManager));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Get status from all pipelines
                var pipelineStatuses = _pipelineManager.GetPipelineStatuses();

                if (!pipelineStatuses.Any())
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy("No pipelines configured"));
                }

                // Aggregate statistics from all pipelines
                var totalQueued = pipelineStatuses.Sum(p => p.Value.QueueLength);
                var totalActive = pipelineStatuses.Sum(p => p.Value.ActiveProcessing);
                var totalProcessed = pipelineStatuses.Sum(p => p.Value.TotalProcessed);
                var totalFailed = pipelineStatuses.Sum(p => p.Value.TotalFailed);
                var activePipelines = pipelineStatuses.Count(p => p.Value.IsActive);

                var data = new Dictionary<string, object>
                {
                    ["TotalPipelines"] = pipelineStatuses.Count,
                    ["ActivePipelines"] = activePipelines,
                    ["QueueLength"] = totalQueued,
                    ["ActiveProcessing"] = totalActive,
                    ["TotalProcessed"] = totalProcessed,
                    ["TotalFailed"] = totalFailed,
                    ["ServiceUptime"] = DateTime.UtcNow - Program.ServiceStartTime
                };

                // Determine health based on queue size and failure rate
                if (totalQueued > 1000)
                {
                    return Task.FromResult(HealthCheckResult.Degraded(
                        $"Large queue backlog: {totalQueued} items",
                        data: data));
                }

                if (totalProcessed > 0 && totalFailed > totalProcessed * 0.5)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy(
                        $"High failure rate: {totalFailed}/{totalProcessed}",
                        data: data));
                }

                if (activePipelines == 0)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy(
                        "No active pipelines",
                        data: data));
                }

                return Task.FromResult(HealthCheckResult.Healthy(
                    $"Service healthy - {activePipelines}/{pipelineStatuses.Count} pipelines active",
                    data: data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return Task.FromResult(HealthCheckResult.Unhealthy("Health check error", ex));
            }
        }
    }
}
