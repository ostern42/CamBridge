// src\CamBridge.Config\Models\ServiceStatusModel.cs
// Version: 0.7.1
// Description: Service status model with config path information
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;

namespace CamBridge.Config.Models
{
    /// <summary>
    /// Service status information from API
    /// </summary>
    public class ServiceStatusModel
    {
        public string ServiceStatus { get; set; } = "Unknown";
        public string Version { get; set; } = "Unknown";
        public string Mode { get; set; } = "Unknown";
        public TimeSpan Uptime { get; set; }

        // Config information (new in v0.7.1)
        public string? ConfigPath { get; set; }
        public bool ConfigExists { get; set; }

        // Pipeline statistics
        public int PipelineCount { get; set; }
        public int ActivePipelines { get; set; }
        public int QueueLength { get; set; }
        public int ActiveProcessing { get; set; }
        public int TotalSuccessful { get; set; }
        public int TotalFailed { get; set; }
        public double SuccessRate { get; set; }

        // Individual pipeline data
        public List<PipelineStatusData>? Pipelines { get; set; }

        // Configuration info
        public ServiceConfigurationInfo? Configuration { get; set; }
    }

    /// <summary>
    /// Individual pipeline status from service
    /// </summary>
    public class PipelineStatusData
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int QueueLength { get; set; }
        public int ActiveProcessing { get; set; }
        public int TotalProcessed { get; set; }
        public int TotalSuccessful { get; set; }
        public int TotalFailed { get; set; }
        public List<string>? WatchedFolders { get; set; }
    }

    /// <summary>
    /// Service configuration information
    /// </summary>
    public class ServiceConfigurationInfo
    {
        public string? DefaultOutputFolder { get; set; }
        public string? ExifToolPath { get; set; }
        public string? Version { get; set; }
    }
}
