// src/CamBridge.Config/Models/LogEntry.cs
// Version: 0.8.16
// Description: Core log entry model with correlation support
// Copyright: (C) 2025 Claude's Improbably Reliable Software Solutions

using System;
using CamBridge.Core.Enums;
using Microsoft.Extensions.Logging;

namespace CamBridge.Config.Models
{
    /// <summary>
    /// Enhanced log entry with correlation support
    /// </summary>
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? RawLine { get; set; }
        public string Source { get; set; } = string.Empty; // Which log file this came from

        // Enhanced properties for correlation
        public string? CorrelationId { get; set; }
        public ProcessingStage? Stage { get; set; }
        public string? Pipeline { get; set; }
        public int? DurationMs { get; set; }

        // UI Helper Properties
        public string LevelText => Level switch
        {
            LogLevel.Debug => "DBG",
            LogLevel.Information => "INF",
            LogLevel.Warning => "WRN",
            LogLevel.Error => "ERR",
            LogLevel.Critical => "CRT",
            _ => "???"
        };

        public string LevelColor => Level switch
        {
            LogLevel.Debug => "#808080",      // Gray
            LogLevel.Information => "#0078D4", // Blue
            LogLevel.Warning => "#FFA500",     // Orange
            LogLevel.Error => "#FF0000",       // Red
            LogLevel.Critical => "#8B0000",    // Dark Red
            _ => "#000000"
        };

        // Stage icons using simple ASCII representations
        public string StageIcon => Stage switch
        {
            ProcessingStage.ServiceStartup => "[START]",
            ProcessingStage.ConfigurationLoading => "[CONFIG]",
            ProcessingStage.PipelineInitialization => "[INIT]",
            ProcessingStage.ServiceShutdown => "[STOP]",
            ProcessingStage.FileDetected => "[FILE]",
            ProcessingStage.ExifExtraction => "[EXIF]",
            ProcessingStage.TagMapping => "[MAP]",
            ProcessingStage.DicomConversion => "[DICOM]",
            ProcessingStage.PostProcessing => "[POST]",
            ProcessingStage.PacsUpload => "[PACS]",
            ProcessingStage.Complete => "[OK]",
            ProcessingStage.Error => "[ERR]",
            ProcessingStage.PipelineShutdown => "[SHUTDOWN]",
            ProcessingStage.PipelineRecovery => "[RECOVERY]",
            ProcessingStage.WatcherError => "[WATCH]",
            ProcessingStage.HealthCheck => "[HEALTH]",
            _ => "[*]"
        };

        public string FormattedDuration => DurationMs.HasValue ? $"{DurationMs}ms" : "";
    }
}
