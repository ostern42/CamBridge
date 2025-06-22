// src\CamBridge.Config\Models\ServiceStatusModel.cs
// Version: 0.7.28
// Description: Service status model matching the actual API response
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CamBridge.Config.Models
{
    /// <summary>
    /// Root response from /api/status endpoint
    /// </summary>
    public class ServiceStatusModel
    {
        // Nested service info
        [JsonPropertyName("service")]
        public ServiceInfo Service { get; set; } = new();

        // Environment info
        [JsonPropertyName("environment")]
        public EnvironmentInfo? Environment { get; set; }

        // Pipeline list
        [JsonPropertyName("pipelines")]
        public List<PipelineStatusData> Pipelines { get; set; } = new();

        // Statistics
        [JsonPropertyName("statistics")]
        public ServiceStatistics? Statistics { get; set; }

        // Configuration
        [JsonPropertyName("configuration")]
        public ServiceConfigurationInfo? Configuration { get; set; }

        // Helper properties for backward compatibility
        [JsonIgnore]
        public string ServiceStatus => Service?.Status ?? "Unknown";

        [JsonIgnore]
        public string Version => Service?.Version ?? "Unknown";

        [JsonIgnore]
        public TimeSpan Uptime => ParseUptime(Service?.Uptime);

        [JsonIgnore]
        public int PipelineCount => Statistics?.TotalPipelines ?? 0;

        [JsonIgnore]
        public int ActivePipelines => Statistics?.ActivePipelines ?? 0;

        [JsonIgnore]
        public int TotalSuccessful => Statistics?.TotalProcessed ?? 0;

        [JsonIgnore]
        public int TotalFailed => Statistics?.TotalErrors ?? 0;

        private static TimeSpan ParseUptime(string? uptimeStr)
        {
            if (string.IsNullOrEmpty(uptimeStr))
                return TimeSpan.Zero;

            // Parse formats like "2m", "1h 5m", "2d 3h 5m"
            var parts = uptimeStr.Split(' ');
            var totalMinutes = 0;

            foreach (var part in parts)
            {
                if (part.EndsWith("d"))
                {
                    if (int.TryParse(part.TrimEnd('d'), out var days))
                        totalMinutes += days * 24 * 60;
                }
                else if (part.EndsWith("h"))
                {
                    if (int.TryParse(part.TrimEnd('h'), out var hours))
                        totalMinutes += hours * 60;
                }
                else if (part.EndsWith("m"))
                {
                    if (int.TryParse(part.TrimEnd('m'), out var minutes))
                        totalMinutes += minutes;
                }
            }

            return TimeSpan.FromMinutes(totalMinutes);
        }
    }

    /// <summary>
    /// Service information
    /// </summary>
    public class ServiceInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("version")]
        public string Version { get; set; } = "Unknown";

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Unknown";

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("uptime")]
        public string Uptime { get; set; } = "";

        [JsonPropertyName("processId")]
        public int ProcessId { get; set; }
    }

    /// <summary>
    /// Environment information
    /// </summary>
    public class EnvironmentInfo
    {
        [JsonPropertyName("machineName")]
        public string MachineName { get; set; } = string.Empty;

        [JsonPropertyName("osVersion")]
        public string OsVersion { get; set; } = string.Empty;

        [JsonPropertyName("processorCount")]
        public int ProcessorCount { get; set; }

        [JsonPropertyName("workingSet")]
        public long WorkingSet { get; set; }

        [JsonPropertyName("dotNetVersion")]
        public string DotNetVersion { get; set; } = string.Empty;
    }

    /// <summary>
    /// Service statistics
    /// </summary>
    public class ServiceStatistics
    {
        [JsonPropertyName("totalPipelines")]
        public int TotalPipelines { get; set; }

        [JsonPropertyName("activePipelines")]
        public int ActivePipelines { get; set; }

        [JsonPropertyName("totalProcessed")]
        public int TotalProcessed { get; set; }

        [JsonPropertyName("totalErrors")]
        public int TotalErrors { get; set; }

        [JsonPropertyName("totalQueued")]
        public int TotalQueued { get; set; }
    }

    /// <summary>
    /// Individual pipeline status
    /// </summary>
    public class PipelineStatusData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("queueDepth")]
        public int QueueDepth { get; set; }

        [JsonPropertyName("processedCount")]
        public int ProcessedCount { get; set; }

        [JsonPropertyName("errorCount")]
        public int ErrorCount { get; set; }

        [JsonPropertyName("lastProcessed")]
        public DateTime LastProcessed { get; set; }

        [JsonPropertyName("watchPath")]
        public string WatchPath { get; set; } = string.Empty;

        [JsonPropertyName("outputPath")]
        public string OutputPath { get; set; } = string.Empty;

        // Backward compatibility
        [JsonIgnore]
        public int QueueLength => QueueDepth;

        [JsonIgnore]
        public int TotalProcessed => ProcessedCount;

        [JsonIgnore]
        public int TotalFailed => ErrorCount;

        [JsonIgnore]
        public int TotalSuccessful => ProcessedCount - ErrorCount;

        [JsonIgnore]
        public List<string> WatchedFolders => new() { WatchPath };

        [JsonIgnore]
        public int ActiveProcessing => 0; // Not in new API
    }

    /// <summary>
    /// Service configuration information
    /// </summary>
    public class ServiceConfigurationInfo
    {
        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("logsDirectory")]
        public string? LogsDirectory { get; set; }

        // Legacy properties
        public string? DefaultOutputFolder { get; set; }
        public string? ExifToolPath { get; set; }
        public string? Version { get; set; }
    }
}
