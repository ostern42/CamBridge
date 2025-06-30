// src/CamBridge.Service/Controllers/StatusController.cs
// Version: 0.8.7
// Description: REST API endpoints for service status and control
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Linq;
using System.Threading.Tasks;
using CamBridge.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CamBridge.Service.Controllers
{
    /// <summary>
    /// Handles HTTP requests for service status and control
    /// </summary>
    public static class StatusController
    {
        /// <summary>
        /// Gets the current service status
        /// </summary>
        public static async Task GetStatus(HttpContext context)
        {
            var pipelineManager = context.RequestServices.GetRequiredService<PipelineManager>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            try
            {
                // FIXED: Use GetPipelineStatus() instead of GetPipelineStatuses()
                var pipelineInfos = pipelineManager.GetPipelineStatus();

                var status = new
                {
                    service = new
                    {
                        name = ServiceInfo.ServiceName,  // FIXED: ServiceName not Name
                        version = ServiceInfo.Version,
                        status = "running",
                        startTime = Program.ServiceStartTime,
                        uptime = DateTime.UtcNow - Program.ServiceStartTime
                    },
                    // FIXED: Correct property mappings
                    pipelines = pipelineInfos.Select(kvp => new
                    {
                        id = kvp.Key,                              // Key as ID
                        name = kvp.Value.Name,
                        isActive = kvp.Value.IsRunning,            // FIXED: IsRunning not IsActive
                        queueDepth = kvp.Value.QueueLength,        // FIXED: QueueLength not QueueDepth
                        processedCount = kvp.Value.ProcessedCount,
                        errorCount = kvp.Value.ErrorCount,
                        lastProcessed = kvp.Value.LastActivityTime, // Now available!
                        watchPath = kvp.Value.WatchFolder,          // FIXED: WatchFolder not WatchPath
                        outputPath = kvp.Value.OutputFolder         // Now available!
                    }).ToList(),
                    statistics = new
                    {
                        totalPipelines = pipelineInfos.Count,
                        activePipelines = pipelineInfos.Count(p => p.Value.IsRunning),
                        totalProcessed = pipelineInfos.Sum(p => p.Value.ProcessedCount),
                        totalErrors = pipelineInfos.Sum(p => p.Value.ErrorCount),
                        totalQueued = pipelineInfos.Sum(p => p.Value.QueueLength)
                    }
                };

                await context.Response.WriteAsJsonAsync(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting service status");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Gets the service health status
        /// </summary>
        public static async Task GetHealth(HttpContext context)
        {
            var pipelineManager = context.RequestServices.GetRequiredService<PipelineManager>();
            // FIXED: Use GetPipelineStatus()
            var pipelineInfos = pipelineManager.GetPipelineStatus();

            var health = new
            {
                status = "Healthy",
                version = ServiceInfo.Version,
                pipelines = pipelineInfos.Count,
                activePipelines = pipelineInfos.Count(p => p.Value.IsRunning),
                timestamp = DateTime.UtcNow
            };

            context.Response.StatusCode = 200;
            await context.Response.WriteAsJsonAsync(health);
        }

        /// <summary>
        /// Gets the list of configured pipelines
        /// </summary>
        public static async Task GetPipelines(HttpContext context)
        {
            var pipelineManager = context.RequestServices.GetRequiredService<PipelineManager>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                // FIXED: Use GetPipelineStatus()
                var pipelineInfos = pipelineManager.GetPipelineStatus();

                var pipelines = pipelineInfos.Select(kvp => new
                {
                    id = kvp.Key,
                    name = kvp.Value.Name,
                    isActive = kvp.Value.IsRunning,
                    watchPath = kvp.Value.WatchFolder,
                    outputPath = kvp.Value.OutputFolder,
                    queueLength = kvp.Value.QueueLength,
                    processedCount = kvp.Value.ProcessedCount,
                    errorCount = kvp.Value.ErrorCount,
                    lastActivity = kvp.Value.LastActivityTime
                }).ToList();

                await context.Response.WriteAsJsonAsync(pipelines);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting pipelines");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Gets details for a specific pipeline
        /// </summary>
        public static async Task GetPipelineDetails(HttpContext context, string id)
        {
            var pipelineManager = context.RequestServices.GetRequiredService<PipelineManager>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            if (string.IsNullOrWhiteSpace(id))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new { error = "Pipeline ID is required" });
                return;
            }

            try
            {
                // FIXED: Use GetPipelineInfo(id) instead of GetPipelineStatus(id)
                var status = pipelineManager.GetPipelineInfo(id);
                if (status == null)
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsJsonAsync(new { error = $"Pipeline '{id}' not found" });
                    return;
                }

                var details = new
                {
                    id = id,
                    name = status.Name,
                    isActive = status.IsRunning,
                    startTime = status.StartTime,
                    lastActivity = status.LastActivityTime,
                    configuration = new
                    {
                        watchPath = status.WatchFolder,
                        outputPath = status.OutputFolder
                    },
                    statistics = new
                    {
                        processedCount = status.ProcessedCount,
                        errorCount = status.ErrorCount,
                        successCount = status.ProcessedCount - status.ErrorCount,
                        successRate = status.ProcessedCount > 0
                            ? ((double)(status.ProcessedCount - status.ErrorCount) / status.ProcessedCount * 100).ToString("F1") + "%"
                            : "0.0%",
                        queueLength = status.QueueLength
                    }
                };

                await context.Response.WriteAsJsonAsync(details);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting pipeline details for {PipelineId}", id);
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Gets processing statistics
        /// </summary>
        public static async Task GetStatistics(HttpContext context)
        {
            var pipelineManager = context.RequestServices.GetRequiredService<PipelineManager>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                // FIXED: Use GetPipelineStatus()
                var pipelineInfos = pipelineManager.GetPipelineStatus();

                var statistics = new
                {
                    timestamp = DateTime.UtcNow,
                    pipelines = pipelineInfos.Select(kvp => new
                    {
                        id = kvp.Key,
                        name = kvp.Value.Name,
                        processedCount = kvp.Value.ProcessedCount,
                        errorCount = kvp.Value.ErrorCount,
                        lastProcessed = kvp.Value.LastActivityTime,
                        successRate = kvp.Value.ProcessedCount > 0
                            ? ((double)(kvp.Value.ProcessedCount - kvp.Value.ErrorCount) / kvp.Value.ProcessedCount * 100).ToString("F1") + "%"
                            : "0.0%"
                    }).ToList(),
                    summary = new
                    {
                        totalPipelines = pipelineInfos.Count,
                        activePipelines = pipelineInfos.Count(p => p.Value.IsRunning),
                        totalProcessed = pipelineInfos.Sum(p => p.Value.ProcessedCount),
                        totalErrors = pipelineInfos.Sum(p => p.Value.ErrorCount),
                        overallSuccessRate = pipelineInfos.Sum(p => p.Value.ProcessedCount) > 0
                            ? ((double)(pipelineInfos.Sum(p => p.Value.ProcessedCount - p.Value.ErrorCount)) /
                               pipelineInfos.Sum(p => p.Value.ProcessedCount) * 100).ToString("F1") + "%"
                            : "0.0%"
                    }
                };

                await context.Response.WriteAsJsonAsync(statistics);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting statistics");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Gets the service version information
        /// </summary>
        public static async Task GetVersion(HttpContext context)
        {
            var version = new
            {
                version = ServiceInfo.Version,
                serviceName = ServiceInfo.ServiceName,
                displayName = ServiceInfo.DisplayName,
                description = ServiceInfo.Description,
                company = ServiceInfo.Company,
                copyright = ServiceInfo.Copyright,
                buildConfiguration = ServiceInfo.BuildConfiguration,
                fullVersion = ServiceInfo.GetFullVersionString()
            };

            await context.Response.WriteAsJsonAsync(version);
        }

        /// <summary>
        /// Triggers a graceful shutdown of the service
        /// </summary>
        public static async Task Shutdown(HttpContext context)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            var lifetime = context.RequestServices.GetRequiredService<Microsoft.Extensions.Hosting.IHostApplicationLifetime>();

            try
            {
                logger.LogInformation("Shutdown requested via API");

                // Trigger shutdown after a small delay to allow response
                _ = Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    lifetime.StopApplication();
                });

                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Shutdown initiated",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during shutdown request");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { error = "Shutdown failed" });
            }
        }
    }
}
