// src\CamBridge.Config\Models\DeadLetterModels.cs
// Version: 0.7.1
// Description: Dead letter queue models for API communication
// Copyright: Â© 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;

namespace CamBridge.Config.Models
{
    /// <summary>
    /// Dead letter item from API
    /// </summary>
    public class DeadLetterItemModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string PipelineName { get; set; } = string.Empty;
        public DateTime FailedAt { get; set; }
        public DateTime FirstAttempt { get; set; }
        public DateTime LastAttempt { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public int RetryCount { get; set; }
        public int AttemptCount { get; set; }
        public string OriginalPath { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }

    /// <summary>
    /// Detailed statistics from API
    /// </summary>
    public class DetailedStatisticsModel
    {
        public int TotalProcessed { get; set; }
        public int TotalSuccessful { get; set; }
        public int TotalFailed { get; set; }
        public int TotalQueued { get; set; }
        public double AverageProcessingTimeMs { get; set; }
        public DateTime LastUpdate { get; set; }

        // Per-pipeline statistics
        public Dictionary<string, PipelineStatistics>? PipelineStats { get; set; }
    }

    /// <summary>
    /// Statistics for a single pipeline
    /// </summary>
    public class PipelineStatistics
    {
        public string PipelineId { get; set; } = string.Empty;
        public string PipelineName { get; set; } = string.Empty;
        public int Processed { get; set; }
        public int Successful { get; set; }
        public int Failed { get; set; }
        public int Queued { get; set; }
        public double SuccessRate { get; set; }
    }
}
