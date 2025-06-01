// src/CamBridge.Config/Models/ServiceStatusModel.cs
using System;
using System.Collections.Generic;

namespace CamBridge.Config.Models
{
    public class ServiceStatusModel
    {
        public string ServiceStatus { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int QueueLength { get; set; }
        public int ActiveProcessing { get; set; }
        public int TotalProcessed { get; set; }
        public int TotalSuccessful { get; set; }
        public int TotalFailed { get; set; }
        public double SuccessRate { get; set; }
        public double ProcessingRate { get; set; }
        public TimeSpan Uptime { get; set; }
        public int DeadLetterCount { get; set; }
        public List<ActiveItemModel> ActiveItems { get; set; } = new();
    }

    public class ActiveItemModel
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public int AttemptCount { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public class DeadLetterItemModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public DateTime FailedAt { get; set; }
        public int AttemptCount { get; set; }
        public long FileSize { get; set; }
    }

    public class DetailedStatisticsModel
    {
        public Dictionary<string, int> TopErrors { get; set; } = new();
        public Dictionary<string, int> ErrorCategories { get; set; } = new();
    }
}
