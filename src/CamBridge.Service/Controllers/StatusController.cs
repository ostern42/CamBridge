// src\CamBridge.Service\Controllers\StatusController.cs
// Version: 0.7.28
// Description: Service status API controller with static methods for minimal API
// Copyright: Â© 2025 Claude's Improbably Reliable Software Solutions

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CamBridge.Core;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using CamBridge.Core.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CamBridge.Service.Controllers
{
    /// <summary>
    /// Service status controller with static methods for minimal API endpoints
    /// </summary>
    public static class StatusController
    {
        private static DateTime _startTime = DateTime.UtcNow;

        /// <summary>
        /// Get service status
        /// </summary>
        public static async Task GetStatus(HttpContext context)
        {
            var pipelineManager = context.RequestServices.GetRequiredService<PipelineManager>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                var pipelineStatuses = pipelineManager.GetPipelineStatuses();

                var status = new
                {
                    service = new
                    {
                        name = ServiceInfo.ServiceName,
                        displayName = ServiceInfo.DisplayName,
                        version = ServiceInfo.Version,
                        status = "Online",
                        timestamp = DateTime.UtcNow,
                        startTime = _startTime,
                        uptime = GetUptime(),
                        processId = Environment.ProcessId
                    },
                    environment = new
                    {
                        machineName = Environment.MachineName,
                        osVersion = Environment.OSVersion.ToString(),
                        processorCount = Environment.ProcessorCount,
                        workingSet = Environment.WorkingSet / (1024 * 1024),
                        dotNetVersion = Environment.Version.ToString()
                    },
                    pipelines = pipelineStatuses.Select(p => new
                    {
                        id = p.Id,
                        name = p.Name,
                        isActive = p.IsActive,
                        queueDepth = p.QueueDepth,
                        processedCount = p.ProcessedCount,
                        errorCount = p.ErrorCount,
                        lastProcessed = p.LastProcessed,
                        watchPath = p.WatchPath,
                        outputPath = p.OutputPath
                    }).ToList(),
                    statistics = new
                    {
                        totalPipelines = pipelineStatuses.Count,
                        activePipelines = pipelineStatuses.Count(p => p.IsActive),
                        totalProcessed = pipelineStatuses.Sum(p => p.ProcessedCount),
                        totalErrors = pipelineStatuses.Sum(p => p.ErrorCount),
                        totalQueued = pipelineStatuses.Sum(p => p.QueueDepth)
                    },
                    configuration = new
                    {
                        path = ConfigurationPaths.GetPrimaryConfigPath(),
                        logsDirectory = ConfigurationPaths.GetLogsDirectory()
                    }
                };

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting service status");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { error = "Failed to retrieve service status" });
            }
        }

        /// <summary>
        /// Get just the version string
        /// </summary>
        public static async Task GetVersion(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(ServiceInfo.Version);
        }

        /// <summary>
        /// Get health status (minimal endpoint for monitoring)
        /// </summary>
        public static async Task GetHealth(HttpContext context)
        {
            var pipelineManager = context.RequestServices.GetRequiredService<PipelineManager>();

            var pipelineStatuses = pipelineManager.GetPipelineStatuses();

            var health = new
            {
                status = "Healthy",
                version = ServiceInfo.Version,
                timestamp = DateTime.UtcNow,
                activePipelines = pipelineStatuses.Count(p => p.IsActive),
                totalPipelines = pipelineStatuses.Count
            };

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(health);
        }

        /// <summary>
        /// Get pipeline configurations
        /// </summary>
        public static async Task GetPipelines(HttpContext context)
        {
            var settings = context.RequestServices.GetRequiredService<IOptionsSnapshot<CamBridgeSettingsV2>>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                var pipelines = settings.Value.Pipelines.Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    description = p.Description,
                    enabled = p.Enabled,
                    watchPath = p.WatchSettings.Path,
                    watchPattern = p.WatchSettings.FilePattern,
                    includeSubdirectories = p.WatchSettings.IncludeSubdirectories,
                    outputPath = p.WatchSettings.OutputPath,
                    processingOptions = new
                    {
                        outputOrganization = p.ProcessingOptions.OutputOrganization.ToString(),
                        successAction = p.ProcessingOptions.SuccessAction.ToString(),
                        failureAction = p.ProcessingOptions.FailureAction.ToString(),
                        maxConcurrentProcessing = p.ProcessingOptions.MaxConcurrentProcessing,
                        errorFolder = p.ProcessingOptions.ErrorFolder
                    }
                }).ToList();

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    count = pipelines.Count,
                    pipelines = pipelines
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting pipeline configurations");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { error = "Failed to retrieve pipeline configurations" });
            }
        }

        /// <summary>
        /// Get detailed status for a specific pipeline
        /// </summary>
        public static async Task GetPipelineDetails(HttpContext context)
        {
            var pipelineManager = context.RequestServices.GetRequiredService<PipelineManager>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            var id = context.Request.RouteValues["id"]?.ToString();
            if (string.IsNullOrEmpty(id))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new { error = "Pipeline ID is required" });
                return;
            }

            try
            {
                var status = pipelineManager.GetPipelineStatus(id);
                if (status == null)
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsJsonAsync(new { error = $"Pipeline '{id}' not found" });
                    return;
                }

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting pipeline status for {PipelineId}", id);
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { error = "Failed to retrieve pipeline status" });
            }
        }

        /// <summary>
        /// Get statistics (currently returns basic info, expand as needed)
        /// </summary>
        public static async Task GetStatistics(HttpContext context)
        {
            var pipelineManager = context.RequestServices.GetRequiredService<PipelineManager>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                var pipelineStatuses = pipelineManager.GetPipelineStatuses();

                var statistics = new
                {
                    timestamp = DateTime.UtcNow,
                    pipelines = pipelineStatuses.Select(p => new
                    {
                        id = p.Id,
                        name = p.Name,
                        processedCount = p.ProcessedCount,
                        errorCount = p.ErrorCount,
                        successCount = p.ProcessedCount - p.ErrorCount,
                        queueDepth = p.QueueDepth,
                        isActive = p.IsActive,
                        lastProcessed = p.LastProcessed
                    }).ToList(),
                    summary = new
                    {
                        totalPipelines = pipelineStatuses.Count,
                        activePipelines = pipelineStatuses.Count(p => p.IsActive),
                        totalProcessed = pipelineStatuses.Sum(p => p.ProcessedCount),
                        totalErrors = pipelineStatuses.Sum(p => p.ErrorCount),
                        totalQueued = pipelineStatuses.Sum(p => p.QueueDepth)
                    }
                };

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(statistics);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting statistics");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { error = "Failed to retrieve statistics" });
            }
        }

        private static string GetUptime()
        {
            var uptime = DateTime.UtcNow - _startTime;

            if (uptime.TotalDays >= 1)
            {
                return $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m";
            }
            else if (uptime.TotalHours >= 1)
            {
                return $"{(int)uptime.TotalHours}h {uptime.Minutes}m";
            }
            else
            {
                return $"{(int)uptime.TotalMinutes}m";
            }
        }
    }
}
