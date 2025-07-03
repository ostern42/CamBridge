// src/CamBridge.Config/Models/StageGroup.cs
// Version: 0.8.19
// Description: Groups log entries by processing stage - ENHANCED
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using CamBridge.Core.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace CamBridge.Config.Models
{
    /// <summary>
    /// Represents a group of log entries for a specific processing stage
    /// </summary>
    public class StageGroup : ObservableObject
    {
        private bool _isExpanded = true;

        public ProcessingStage Stage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ObservableCollection<LogEntry> Entries { get; }

        public StageGroup()
        {
            Entries = new ObservableCollection<LogEntry>();
            Entries.CollectionChanged += OnEntriesChanged;
        }

        private void OnEntriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(EntryCount));
            OnPropertyChanged(nameof(ErrorCount));
            OnPropertyChanged(nameof(WarningCount));
            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(HasWarnings));
            OnPropertyChanged(nameof(Summary));

            // Update times if entries added
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (LogEntry entry in e.NewItems)
                {
                    if (!StartTime.Equals(default) && entry.Timestamp < StartTime)
                        StartTime = entry.Timestamp;
                    if (entry.Timestamp > EndTime)
                        EndTime = entry.Timestamp;
                }

                OnPropertyChanged(nameof(Duration));
                OnPropertyChanged(nameof(DurationText));
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public TimeSpan Duration => EndTime - StartTime;

        public string DurationText
        {
            get
            {
                if (Duration.TotalSeconds >= 1)
                    return $"{Duration.TotalSeconds:0.0}s";
                else
                    return $"{Duration.TotalMilliseconds:0}ms";
            }
        }

        // Entry counts
        public int EntryCount => Entries.Count;
        public int ErrorCount => Entries.Count(e => e.Level == LogLevel.Error);
        public int WarningCount => Entries.Count(e => e.Level == LogLevel.Warning);

        public bool HasErrors => ErrorCount > 0;
        public bool HasWarnings => WarningCount > 0;

        // UI Helper properties
        public string StageIcon => Stage switch
        {
            ProcessingStage.ServiceStartup => "▶",
            ProcessingStage.ConfigurationLoading => "⚙",
            ProcessingStage.PipelineInitialization => "🔧",
            ProcessingStage.ServiceShutdown => "◼",
            ProcessingStage.FileDetected => "📄",
            ProcessingStage.ExifExtraction => "🔍",
            ProcessingStage.TagMapping => "🏷",
            ProcessingStage.DicomConversion => "🏥",
            ProcessingStage.PacsUpload => "☁",
            ProcessingStage.Complete => "✓",
            ProcessingStage.PostProcessing => "📦",
            ProcessingStage.Error => "✗",
            ProcessingStage.WatcherError => "👁",
            ProcessingStage.PipelineShutdown => "⛔",
            ProcessingStage.PipelineRecovery => "♻",
            ProcessingStage.HealthCheck => "💚",
            _ => "•"
        };

        public string StageColor => Stage switch
        {
            ProcessingStage.ServiceStartup => "#1E88E5",        // Blue
            ProcessingStage.ConfigurationLoading => "#5E35B1",   // Deep Purple
            ProcessingStage.PipelineInitialization => "#3949AB", // Indigo
            ProcessingStage.ServiceShutdown => "#616161",        // Gray

            ProcessingStage.FileDetected => "#00ACC1",           // Cyan
            ProcessingStage.ExifExtraction => "#8E24AA",         // Purple
            ProcessingStage.TagMapping => "#AB47BC",             // Light Purple
            ProcessingStage.DicomConversion => "#FB8C00",        // Orange
            ProcessingStage.PacsUpload => "#FFB300",             // Amber

            ProcessingStage.Complete => "#43A047",               // Green
            ProcessingStage.PostProcessing => "#66BB6A",         // Light Green

            ProcessingStage.Error => "#E53935",                  // Red
            ProcessingStage.WatcherError => "#EF5350",           // Light Red
            ProcessingStage.PipelineShutdown => "#C62828",       // Dark Red
            ProcessingStage.PipelineRecovery => "#FF6E40",       // Deep Orange

            ProcessingStage.HealthCheck => "#00C853",            // Green Accent
            _ => "#757575"                                       // Gray
        };

        public string StageDisplayName => Stage switch
        {
            ProcessingStage.ServiceStartup => "Service Startup",
            ProcessingStage.ConfigurationLoading => "Configuration",
            ProcessingStage.PipelineInitialization => "Pipeline Init",
            ProcessingStage.ServiceShutdown => "Service Shutdown",
            ProcessingStage.FileDetected => "File Detected",
            ProcessingStage.ExifExtraction => "EXIF Extraction",
            ProcessingStage.TagMapping => "Tag Mapping",
            ProcessingStage.DicomConversion => "DICOM Conversion",
            ProcessingStage.PacsUpload => "PACS Upload",
            ProcessingStage.Complete => "Complete",
            ProcessingStage.PostProcessing => "Post Processing",
            ProcessingStage.Error => "Error",
            ProcessingStage.WatcherError => "Watcher Error",
            ProcessingStage.PipelineShutdown => "Pipeline Shutdown",
            ProcessingStage.PipelineRecovery => "Pipeline Recovery",
            ProcessingStage.HealthCheck => "Health Check",
            _ => Stage.ToString()
        };

        // Summary for tooltips
        public string Summary
        {
            get
            {
                var parts = new System.Collections.Generic.List<string>();
                parts.Add($"{EntryCount} entries");

                if (Duration.TotalMilliseconds > 0)
                    parts.Add($"{DurationText}");

                if (ErrorCount > 0)
                    parts.Add($"{ErrorCount} errors");

                if (WarningCount > 0)
                    parts.Add($"{WarningCount} warnings");

                return string.Join(" • ", parts);
            }
        }

        // Get the most severe log level in this stage
        public LogLevel MaxLogLevel
        {
            get
            {
                if (Entries.Any(e => e.Level == LogLevel.Critical))
                    return LogLevel.Critical;
                if (Entries.Any(e => e.Level == LogLevel.Error))
                    return LogLevel.Error;
                if (Entries.Any(e => e.Level == LogLevel.Warning))
                    return LogLevel.Warning;
                if (Entries.Any(e => e.Level == LogLevel.Information))
                    return LogLevel.Information;
                if (Entries.Any(e => e.Level == LogLevel.Debug))
                    return LogLevel.Debug;
                return LogLevel.Trace;
            }
        }

        // Visual hint for severity
        public bool IsErrorStage => Stage == ProcessingStage.Error || HasErrors;
        public bool IsWarningStage => HasWarnings && !IsErrorStage;
        public bool IsSuccessStage => Stage == ProcessingStage.Complete && !HasErrors && !HasWarnings;
    }
}
