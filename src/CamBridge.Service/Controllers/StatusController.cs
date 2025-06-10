// src\CamBridge.Service\Controllers\StatusController.cs
// Version: 0.7.7
// Description: Simple service status API controller (without DeadLetterQueue)

using Microsoft.AspNetCore.Mvc;
using CamBridge.Core;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace CamBridge.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly PipelineManager _pipelineManager;
        private readonly IOptions<CamBridgeSettingsV2> _settings;
        private readonly ILogger<StatusController> _logger;
        private static readonly DateTime _startTime = DateTime.UtcNow;

        public StatusController(
            PipelineManager pipelineManager,
            IOptions<CamBridgeSettingsV2> settings,
            ILogger<StatusController> logger)
        {
            _pipelineManager = pipelineManager;
            _settings = settings;
            _logger = logger;
        }

        /// <summary>
        /// Get service status
        /// </summary>
        [HttpGet]
        public IActionResult GetStatus()
        {
            try
            {
                var pipelineStatuses = _pipelineManager.GetPipelineStatuses();

                var status = new
                {
                    ServiceName = ServiceInfo.ServiceName,
                    Version = ServiceInfo.Version,
                    Status = "Online",
                    Timestamp = DateTime.UtcNow,
                    Uptime = GetUptime(),
                    Environment = new
                    {
                        MachineName = Environment.MachineName,
                        OSVersion = Environment.OSVersion.ToString(),
                        ProcessorCount = Environment.ProcessorCount,
                        WorkingSet = Environment.WorkingSet / (1024 * 1024),
                        DotNetVersion = Environment.Version.ToString()
                    },
                    Pipelines = pipelineStatuses.Select(kvp => new
                    {
                        kvp.Value.Id,
                        kvp.Value.Name,
                        kvp.Value.IsActive,
                        kvp.Value.QueueLength,
                        kvp.Value.ActiveProcessing,
                        kvp.Value.TotalProcessed,
                        kvp.Value.TotalSuccessful,
                        kvp.Value.TotalFailed
                    }).ToList(),
                    TotalStatistics = new
                    {
                        TotalPipelines = pipelineStatuses.Count,
                        ActivePipelines = pipelineStatuses.Count(p => p.Value.IsActive),
                        TotalProcessed = pipelineStatuses.Sum(p => p.Value.TotalProcessed),
                        TotalErrors = pipelineStatuses.Sum(p => p.Value.TotalFailed),
                        TotalQueued = pipelineStatuses.Sum(p => p.Value.QueueLength)
                    },
                    ConfigurationPath = ConfigurationPaths.GetPrimaryConfigPath()
                };

                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting service status");
                return StatusCode(500, new { Error = "Failed to retrieve service status" });
            }
        }

        /// <summary>
        /// Get status for a specific pipeline
        /// </summary>
        [HttpGet("pipeline/{id}")]
        public IActionResult GetPipelineStatus(string id)
        {
            try
            {
                var details = _pipelineManager.GetPipelineDetails(id);
                if (details == null)
                {
                    return NotFound(new { Error = $"Pipeline '{id}' not found" });
                }

                return Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pipeline status for {PipelineId}", id);
                return StatusCode(500, new { Error = "Failed to retrieve pipeline status" });
            }
        }

        /// <summary>
        /// Get health status (minimal endpoint for monitoring)
        /// </summary>
        [HttpGet("health")]
        public IActionResult GetHealth()
        {
            var pipelineStatuses = _pipelineManager.GetPipelineStatuses();

            return Ok(new
            {
                Status = "Healthy",
                Version = ServiceInfo.Version,
                Timestamp = DateTime.UtcNow,
                ActivePipelines = pipelineStatuses.Count(p => p.Value.IsActive)
            });
        }

        /// <summary>
        /// Get version information
        /// </summary>
        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);

            return Ok(new
            {
                Version = ServiceInfo.Version,
                AssemblyVersion = assembly.GetName().Version?.ToString() ?? "Unknown",
                FileVersion = fileVersionInfo.FileVersion ?? "Unknown",
                ProductVersion = fileVersionInfo.ProductVersion ?? "Unknown",
                InformationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? ServiceInfo.Version,
                Copyright = ServiceInfo.Copyright,
                Company = fileVersionInfo.CompanyName ?? "Claude's Improbably Reliable Software Solutions",
                Product = ServiceInfo.DisplayName,
                BuildConfiguration =
#if DEBUG
                    "Debug"
#else
                    "Release"
#endif
            });
        }

        /// <summary>
        /// Get pipeline configurations
        /// </summary>
        [HttpGet("pipelines")]
        public IActionResult GetPipelines()
        {
            try
            {
                var pipelines = _settings.Value.Pipelines.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    IsEnabled = p.Enabled,
                    WatchPath = p.WatchSettings.Path,
                    WatchPattern = p.WatchSettings.FilePattern,
                    IncludeSubdirectories = p.WatchSettings.IncludeSubdirectories,
                    Output = new
                    {
                        Folder = p.WatchSettings.OutputPath ?? p.ProcessingOptions.ArchiveFolder,
                        Organization = p.ProcessingOptions.OutputOrganization.ToString(),
                        FilePattern = p.ProcessingOptions.OutputFilePattern ?? "{PatientID}_{StudyDate}_{InstanceNumber}"
                    },
                    Processing = new
                    {
                        DeleteOriginalAfterProcessing = p.ProcessingOptions.SuccessAction == PostProcessingAction.Delete,
                        MaxConcurrentProcessing = p.ProcessingOptions.MaxConcurrentProcessing,
                        MaxRetryAttempts = p.ProcessingOptions.MaxRetryAttempts,
                        RetryDelaySeconds = p.ProcessingOptions.RetryDelaySeconds
                    }
                }).ToList();

                return Ok(new
                {
                    Count = pipelines.Count,
                    Pipelines = pipelines
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pipeline configurations");
                return StatusCode(500, new { Error = "Failed to retrieve pipeline configurations" });
            }
        }

        /// <summary>
        /// Get pipeline statistics
        /// </summary>
        [HttpGet("statistics")]
        public IActionResult GetStatistics()
        {
            try
            {
                var pipelineStatuses = _pipelineManager.GetPipelineStatuses();

                var statistics = new
                {
                    Timestamp = DateTime.UtcNow,
                    Pipelines = pipelineStatuses.Select(kvp => new
                    {
                        kvp.Value.Id,
                        kvp.Value.Name,
                        Statistics = new
                        {
                            ProcessedCount = kvp.Value.TotalProcessed,
                            SuccessCount = kvp.Value.TotalSuccessful,
                            ErrorCount = kvp.Value.TotalFailed
                        },
                        QueueDepth = kvp.Value.QueueLength,
                        IsProcessing = kvp.Value.ActiveProcessing > 0
                    }).ToList(),
                    Summary = new
                    {
                        TotalPipelines = pipelineStatuses.Count,
                        ActivePipelines = pipelineStatuses.Count(p => p.Value.IsActive),
                        TotalProcessed = pipelineStatuses.Sum(p => p.Value.TotalProcessed),
                        TotalErrors = pipelineStatuses.Sum(p => p.Value.TotalFailed),
                        TotalQueued = pipelineStatuses.Sum(p => p.Value.QueueLength)
                    }
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics");
                return StatusCode(500, new { Error = "Failed to retrieve statistics" });
            }
        }

        /// <summary>
        /// Enable a pipeline
        /// </summary>
        [HttpPost("pipeline/{id}/enable")]
        public async Task<IActionResult> EnablePipeline(string id)
        {
            try
            {
                await _pipelineManager.EnablePipelineAsync(id);
                return Ok(new { Message = $"Pipeline {id} enabled" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling pipeline {PipelineId}", id);
                return StatusCode(500, new { Error = "Failed to enable pipeline" });
            }
        }

        /// <summary>
        /// Disable a pipeline
        /// </summary>
        [HttpPost("pipeline/{id}/disable")]
        public async Task<IActionResult> DisablePipeline(string id)
        {
            try
            {
                await _pipelineManager.DisablePipelineAsync(id);
                return Ok(new { Message = $"Pipeline {id} disabled" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling pipeline {PipelineId}", id);
                return StatusCode(500, new { Error = "Failed to disable pipeline" });
            }
        }

        private string GetUptime()
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
