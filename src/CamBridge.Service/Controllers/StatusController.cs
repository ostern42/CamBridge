// src/CamBridge.Service/Controllers/StatusController.cs
// Version: 0.7.5+tools
// Description: API controller for service status - focused on pipelines
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Core;
using CamBridge.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CamBridge.Service.Controllers
{
    /// <summary>
    /// API controller for service and pipeline status
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly ILogger<StatusController> _logger;
        private readonly PipelineManager _pipelineManager;
        private readonly IOptionsMonitor<CamBridgeSettingsV2> _settingsMonitor;
        private readonly IHostApplicationLifetime _lifetime;

        public StatusController(
            ILogger<StatusController> logger,
            PipelineManager pipelineManager,
            IOptionsMonitor<CamBridgeSettingsV2> settingsMonitor,
            IHostApplicationLifetime lifetime)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pipelineManager = pipelineManager ?? throw new ArgumentNullException(nameof(pipelineManager));
            _settingsMonitor = settingsMonitor ?? throw new ArgumentNullException(nameof(settingsMonitor));
            _lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
        }

        /// <summary>
        /// Gets current service status and statistics
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceStatusDto), 200)]
        public IActionResult GetStatus()
        {
            try
            {
                var pipelineStatuses = _pipelineManager.GetPipelineStatuses();
                var settings = _settingsMonitor.CurrentValue;

                // Calculate aggregate statistics
                var totalQueued = pipelineStatuses.Sum(p => p.Value.QueueLength);
                var totalActive = pipelineStatuses.Sum(p => p.Value.ActiveProcessing);
                var totalProcessed = pipelineStatuses.Sum(p => p.Value.TotalProcessed);
                var totalSuccessful = pipelineStatuses.Sum(p => p.Value.TotalSuccessful);
                var totalFailed = pipelineStatuses.Sum(p => p.Value.TotalFailed);
                var successRate = totalProcessed > 0
                    ? (double)totalSuccessful / totalProcessed * 100
                    : 0;

                var uptime = DateTime.UtcNow - Program.ServiceStartTime;
                var processingRate = uptime.TotalMinutes > 0
                    ? totalProcessed / uptime.TotalMinutes
                    : 0;

                var status = new ServiceStatusDto
                {
                    ServiceStatus = "Running",
                    Version = ServiceInfo.Version,
                    Timestamp = DateTime.UtcNow,
                    Uptime = uptime,
                    ConfigPath = ConfigurationPaths.GetPrimaryConfigPath(),
                    ConfigExists = ConfigurationPaths.PrimaryConfigExists(),

                    // Pipeline summary
                    PipelineCount = pipelineStatuses.Count,
                    ActivePipelines = pipelineStatuses.Count(p => p.Value.IsActive),

                    // Aggregate statistics
                    QueueLength = totalQueued,
                    ActiveProcessing = totalActive,
                    TotalProcessed = totalProcessed,
                    TotalSuccessful = totalSuccessful,
                    TotalFailed = totalFailed,
                    SuccessRate = successRate,
                    ProcessingRate = processingRate,

                    // Per-pipeline details
                    Pipelines = pipelineStatuses.Select(p => new PipelineStatusDto
                    {
                        Id = p.Key,
                        Name = p.Value.Name,
                        IsActive = p.Value.IsActive,
                        QueueLength = p.Value.QueueLength,
                        ActiveProcessing = p.Value.ActiveProcessing,
                        TotalProcessed = p.Value.TotalProcessed,
                        TotalSuccessful = p.Value.TotalSuccessful,
                        TotalFailed = p.Value.TotalFailed,
                        SuccessRate = p.Value.TotalProcessed > 0
                            ? (double)p.Value.TotalSuccessful / p.Value.TotalProcessed * 100
                            : 0,
                        WatchedFolders = p.Value.WatchedFolders
                    }).ToList(),

                    // Configuration info
                    Configuration = new ConfigurationDto
                    {
                        DefaultOutputFolder = settings.DefaultOutputFolder,
                        ExifToolPath = settings.ExifToolPath,
                        ConfigVersion = settings.Version
                    }
                };

                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting service status");
                return StatusCode(500, new { error = "Failed to retrieve status" });
            }
        }

        /// <summary>
        /// Gets pipeline-specific statistics
        /// </summary>
        [HttpGet("pipelines")]
        [ProducesResponseType(typeof(Dictionary<string, PipelineStatusDto>), 200)]
        public IActionResult GetPipelines()
        {
            try
            {
                var statuses = _pipelineManager.GetPipelineStatuses();
                var result = statuses.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new PipelineStatusDto
                    {
                        Id = kvp.Value.Id,
                        Name = kvp.Value.Name,
                        IsActive = kvp.Value.IsActive,
                        QueueLength = kvp.Value.QueueLength,
                        ActiveProcessing = kvp.Value.ActiveProcessing,
                        TotalProcessed = kvp.Value.TotalProcessed,
                        TotalSuccessful = kvp.Value.TotalSuccessful,
                        TotalFailed = kvp.Value.TotalFailed,
                        SuccessRate = kvp.Value.TotalProcessed > 0
                            ? (double)kvp.Value.TotalSuccessful / kvp.Value.TotalProcessed * 100
                            : 0,
                        WatchedFolders = kvp.Value.WatchedFolders
                    });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pipeline statuses");
                return StatusCode(500, new { error = "Failed to retrieve pipeline statuses" });
            }
        }

        /// <summary>
        /// Gets detailed information for a specific pipeline
        /// </summary>
        [HttpGet("pipelines/{id}")]
        [ProducesResponseType(typeof(PipelineDetailedDto), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetPipelineDetails(string id)
        {
            try
            {
                var details = _pipelineManager.GetPipelineDetails(id);
                if (details == null)
                {
                    return NotFound(new { error = $"Pipeline '{id}' not found" });
                }

                var result = new PipelineDetailedDto
                {
                    Id = details.Id,
                    Name = details.Name,
                    Description = details.Description,
                    IsActive = details.IsActive,

                    // Configuration details
                    WatchFolder = details.Configuration.WatchSettings.Path,
                    FilePattern = details.Configuration.WatchSettings.FilePattern,
                    IncludeSubdirectories = details.Configuration.WatchSettings.IncludeSubdirectories,
                    OutputFolder = details.Configuration.OutputSettings.Path,

                    // Processing options
                    MaxConcurrentProcessing = details.Configuration.ProcessingOptions.MaxConcurrentProcessing,
                    ProcessExistingOnStartup = details.Configuration.ProcessingOptions.ProcessExistingOnStartup,
                    SuccessAction = details.Configuration.ProcessingOptions.SuccessAction.ToString(),
                    FailureAction = details.Configuration.ProcessingOptions.FailureAction.ToString(),

                    // Statistics
                    QueueStatistics = details.QueueStatistics != null ? new QueueStatisticsDto
                    {
                        QueueLength = details.QueueStatistics.QueueLength,
                        ActiveProcessing = details.QueueStatistics.ActiveProcessing,
                        TotalProcessed = details.QueueStatistics.TotalProcessed,
                        TotalSuccessful = details.QueueStatistics.TotalSuccessful,
                        TotalFailed = details.QueueStatistics.TotalFailed,
                        SuccessRate = details.QueueStatistics.SuccessRate,
                        ProcessingRate = details.QueueStatistics.ProcessingRate,
                        AverageProcessingTime = details.QueueStatistics.AverageProcessingTime,
                        LastProcessedTime = details.QueueStatistics.LastProcessedTime
                    } : null,

                    // Active items
                    ActiveItems = details.ActiveItems.Select(item => new ActiveItemDto
                    {
                        FilePath = item.FilePath,
                        FileName = Path.GetFileName(item.FilePath),
                        StartTime = item.StartTime,
                        AttemptCount = item.AttemptCount,
                        Duration = item.Duration
                    }).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pipeline details for {PipelineId}", id);
                return StatusCode(500, new { error = "Failed to retrieve pipeline details" });
            }
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        [ProducesResponseType(typeof(HealthStatusDto), 200)]
        public IActionResult GetHealth()
        {
            try
            {
                var pipelineStatuses = _pipelineManager.GetPipelineStatuses();

                // Calculate overall health
                var totalProcessed = pipelineStatuses.Sum(p => p.Value.TotalProcessed);
                var totalSuccessful = pipelineStatuses.Sum(p => p.Value.TotalSuccessful);
                var overallSuccessRate = totalProcessed > 0
                    ? (double)totalSuccessful / totalProcessed * 100
                    : 100; // No failures if nothing processed

                var totalQueued = pipelineStatuses.Sum(p => p.Value.QueueLength);
                var activePipelines = pipelineStatuses.Count(p => p.Value.IsActive);

                // Determine health status
                var isHealthy = overallSuccessRate >= 50 && totalQueued < 1000;
                var status = isHealthy ? "Healthy" : overallSuccessRate < 25 ? "Unhealthy" : "Degraded";

                return Ok(new HealthStatusDto
                {
                    Status = status,
                    Timestamp = DateTime.UtcNow,
                    Version = ServiceInfo.Version,
                    Uptime = DateTime.UtcNow - Program.ServiceStartTime,
                    Checks = new Dictionary<string, string>
                    {
                        ["service"] = "OK",
                        ["pipelines"] = activePipelines > 0 ? "OK" : "Warning",
                        ["queue"] = totalQueued < 1000 ? "OK" : totalQueued < 5000 ? "Warning" : "Error",
                        ["success_rate"] = overallSuccessRate >= 50 ? "OK" : overallSuccessRate >= 25 ? "Warning" : "Error",
                        ["config"] = ConfigurationPaths.PrimaryConfigExists() ? "OK" : "Error"
                    },
                    Details = new
                    {
                        ActivePipelines = activePipelines,
                        TotalPipelines = pipelineStatuses.Count,
                        QueuedItems = totalQueued,
                        SuccessRate = overallSuccessRate,
                        TotalProcessed = totalProcessed
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting health status");
                return Ok(new HealthStatusDto
                {
                    Status = "Error",
                    Timestamp = DateTime.UtcNow,
                    Version = ServiceInfo.Version,
                    Checks = new Dictionary<string, string>
                    {
                        ["service"] = "Error",
                        ["error"] = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Gets service version information
        /// </summary>
        [HttpGet("version")]
        [ProducesResponseType(typeof(VersionInfoDto), 200)]
        public IActionResult GetVersion()
        {
            return Ok(new VersionInfoDto
            {
                Version = ServiceInfo.Version,
                ServiceName = ServiceInfo.ServiceName,
                DisplayName = ServiceInfo.DisplayName,
                Description = ServiceInfo.Description,
                Copyright = ServiceInfo.Copyright,
                AssemblyVersion = ServiceInfo.GetVersion(),
                ApiPort = ServiceInfo.ApiPort,
                BuildDate = new FileInfo(typeof(Program).Assembly.Location).LastWriteTime
            });
        }
    }

    // DTOs for API responses
    public class ServiceStatusDto
    {
        public string ServiceStatus { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public TimeSpan Uptime { get; set; }
        public string ConfigPath { get; set; } = string.Empty;
        public bool ConfigExists { get; set; }

        public int PipelineCount { get; set; }
        public int ActivePipelines { get; set; }

        public int QueueLength { get; set; }
        public int ActiveProcessing { get; set; }
        public int TotalProcessed { get; set; }
        public int TotalSuccessful { get; set; }
        public int TotalFailed { get; set; }
        public double SuccessRate { get; set; }
        public double ProcessingRate { get; set; }

        public List<PipelineStatusDto> Pipelines { get; set; } = new();
        public ConfigurationDto Configuration { get; set; } = new();
    }

    public class PipelineStatusDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int QueueLength { get; set; }
        public int ActiveProcessing { get; set; }
        public int TotalProcessed { get; set; }
        public int TotalSuccessful { get; set; }
        public int TotalFailed { get; set; }
        public double SuccessRate { get; set; }
        public List<string> WatchedFolders { get; set; } = new();
    }

    public class PipelineDetailedDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        // Configuration
        public string WatchFolder { get; set; } = string.Empty;
        public string FilePattern { get; set; } = string.Empty;
        public bool IncludeSubdirectories { get; set; }
        public string OutputFolder { get; set; } = string.Empty;

        // Processing options
        public int MaxConcurrentProcessing { get; set; }
        public bool ProcessExistingOnStartup { get; set; }
        public string SuccessAction { get; set; } = string.Empty;
        public string FailureAction { get; set; } = string.Empty;

        // Statistics
        public QueueStatisticsDto? QueueStatistics { get; set; }
        public List<ActiveItemDto> ActiveItems { get; set; } = new();
    }

    public class QueueStatisticsDto
    {
        public int QueueLength { get; set; }
        public int ActiveProcessing { get; set; }
        public int TotalProcessed { get; set; }
        public int TotalSuccessful { get; set; }
        public int TotalFailed { get; set; }
        public double SuccessRate { get; set; }
        public double ProcessingRate { get; set; }
        public TimeSpan AverageProcessingTime { get; set; }
        public DateTime? LastProcessedTime { get; set; }
    }

    public class ActiveItemDto
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public int AttemptCount { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public class ConfigurationDto
    {
        public string? DefaultOutputFolder { get; set; }
        public string? ExifToolPath { get; set; }
        public int ConfigVersion { get; set; }
    }

    public class HealthStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Version { get; set; } = string.Empty;
        public TimeSpan Uptime { get; set; }
        public Dictionary<string, string> Checks { get; set; } = new();
        public object? Details { get; set; }
    }

    public class VersionInfoDto
    {
        public string Version { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Copyright { get; set; } = string.Empty;
        public string AssemblyVersion { get; set; } = string.Empty;
        public int ApiPort { get; set; }
        public DateTime BuildDate { get; set; }
    }
}
