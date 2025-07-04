// src/CamBridge.Config/Models/CorrelationGroup.cs
// Version: 0.8.22
// Description: Groups log entries by correlation ID for tree view - WITH UPDATE METHOD
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Core.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace CamBridge.Config.Models
{
    /// <summary>
    /// Represents a group of log entries with the same correlation ID
    /// </summary>
    public class CorrelationGroup : ObservableObject
    {
        private bool _isExpanded = false; // Changed to false for default collapsed

        public string CorrelationId { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Pipeline { get; set; } = string.Empty;
        public ProcessingStatus Status { get; set; }
        public ObservableCollection<StageGroup> Stages { get; }
        public ObservableCollection<LogEntry> UngroupedEntries { get; }

        // Constructor
        public CorrelationGroup()
        {
            Stages = new ObservableCollection<StageGroup>();
            UngroupedEntries = new ObservableCollection<LogEntry>();

            // IMPORTANT: Notify AllEntries when collections change
            Stages.CollectionChanged += OnCollectionChanged;
            UngroupedEntries.CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(AllEntries));
            OnPropertyChanged(nameof(TotalEntries));
            OnPropertyChanged(nameof(ErrorCount));
            OnPropertyChanged(nameof(WarningCount));

            // If new stages added, check for status changes
            if (e.Action == NotifyCollectionChangedAction.Add && sender == Stages)
            {
                UpdateStatus();
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

        public int TotalEntries => Stages.Sum(s => s.Entries.Count) + UngroupedEntries.Count;

        public int ErrorCount => AllEntries.Count(e => e.Level == Microsoft.Extensions.Logging.LogLevel.Error);
        public int WarningCount => AllEntries.Count(e => e.Level == Microsoft.Extensions.Logging.LogLevel.Warning);

        /// <summary>
        /// All entries in chronological order for compact display
        /// </summary>
        public IEnumerable<LogEntry> AllEntries
        {
            get
            {
                var allEntries = new List<LogEntry>();

                // Add all stage entries
                foreach (var stage in Stages.OrderBy(s => s.StartTime))
                {
                    allEntries.AddRange(stage.Entries);
                }

                // Add ungrouped entries
                allEntries.AddRange(UngroupedEntries);

                // Return sorted by timestamp
                return allEntries.OrderBy(e => e.Timestamp);
            }
        }

        /// <summary>
        /// Update this group with data from another group (for flicker-free refresh)
        /// </summary>
        public void Update(CorrelationGroup newData)
        {
            // Update basic properties
            StartTime = newData.StartTime;
            EndTime = newData.EndTime;
            Pipeline = newData.Pipeline;
            Status = newData.Status;

            // Update stages while preserving expansion state
            var existingStageStates = Stages.ToDictionary(s => s.Stage, s => s.IsExpanded);

            // Remove stages that no longer exist
            var newStageTypes = new HashSet<ProcessingStage>(newData.Stages.Select(s => s.Stage));
            var toRemove = Stages.Where(s => !newStageTypes.Contains(s.Stage)).ToList();
            foreach (var stage in toRemove)
            {
                Stages.Remove(stage);
            }

            // Add or update stages
            foreach (var newStage in newData.Stages)
            {
                var existingStage = Stages.FirstOrDefault(s => s.Stage == newStage.Stage);
                if (existingStage != null)
                {
                    // Update existing stage - just add new entries
                    var existingTimestamps = new HashSet<DateTime>(existingStage.Entries.Select(e => e.Timestamp));
                    foreach (var newEntry in newStage.Entries)
                    {
                        if (!existingTimestamps.Contains(newEntry.Timestamp))
                        {
                            existingStage.Entries.Add(newEntry);
                        }
                    }
                }
                else
                {
                    // Preserve expansion state if it existed before, otherwise default to collapsed
                    if (existingStageStates.TryGetValue(newStage.Stage, out var wasExpanded))
                    {
                        newStage.IsExpanded = wasExpanded;
                    }
                    else
                    {
                        newStage.IsExpanded = false; // Default collapsed
                    }
                    Stages.Add(newStage);
                }
            }

            // Update ungrouped entries
            var existingUngroupedTimestamps = new HashSet<DateTime>(UngroupedEntries.Select(e => e.Timestamp));
            foreach (var newEntry in newData.UngroupedEntries)
            {
                if (!existingUngroupedTimestamps.Contains(newEntry.Timestamp))
                {
                    UngroupedEntries.Add(newEntry);
                }
            }

            // Trigger property updates
            OnPropertyChanged(nameof(Duration));
            OnPropertyChanged(nameof(DurationText));
            OnPropertyChanged(nameof(FormattedStartTime));
            OnPropertyChanged(nameof(FormattedDate));
            OnPropertyChanged(nameof(FormattedTime));
            OnPropertyChanged(nameof(Summary));
            UpdateStatus();
        }

        // Update status based on stages
        private void UpdateStatus()
        {
            if (Stages.Any(s => s.Stage == ProcessingStage.Error))
            {
                Status = ProcessingStatus.Failed;
            }
            else if (Stages.Any(s => s.Stage == ProcessingStage.Complete))
            {
                Status = ProcessingStatus.Completed;
            }
            else if (Stages.Any(s => s.Stage == ProcessingStage.PipelineShutdown))
            {
                Status = ProcessingStatus.Failed;
            }
            else if (Stages.Count > 0)
            {
                Status = ProcessingStatus.InProgress;
            }
            else
            {
                Status = ProcessingStatus.Unknown;
            }

            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(StatusIcon));
            OnPropertyChanged(nameof(StatusColor));
            OnPropertyChanged(nameof(StatusText));
        }

        // UI Helper properties
        public string StatusIcon => Status switch
        {
            ProcessingStatus.Completed => "✓",
            ProcessingStatus.Failed => "✗",
            ProcessingStatus.InProgress => "⋯",
            _ => "?"
        };

        public string StatusColor => Status switch
        {
            ProcessingStatus.Completed => "#4CAF50",  // Green
            ProcessingStatus.Failed => "#F44336",      // Red
            ProcessingStatus.InProgress => "#2196F3",  // Blue
            _ => "#9E9E9E"                            // Gray
        };

        public string StatusText => Status switch
        {
            ProcessingStatus.Completed => "Complete",
            ProcessingStatus.Failed => "Failed",
            ProcessingStatus.InProgress => "Processing",
            _ => "Unknown"
        };

        // Additional UI properties for beautiful display
        public string FormattedStartTime => StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        public string FormattedDate => StartTime.ToString("yyyy-MM-dd");
        public string FormattedTime => StartTime.ToString("HH:mm:ss.fff");

        public bool HasErrors => ErrorCount > 0;
        public bool HasWarnings => WarningCount > 0;

        // Summary text for tooltips
        public string Summary
        {
            get
            {
                var parts = new List<string>();
                parts.Add($"{TotalEntries} entries");
                parts.Add($"Duration: {DurationText}");

                if (ErrorCount > 0)
                    parts.Add($"{ErrorCount} errors");
                if (WarningCount > 0)
                    parts.Add($"{WarningCount} warnings");

                return string.Join(" • ", parts);
            }
        }
    }

    /// <summary>
    /// Processing status for correlation groups
    /// </summary>
    public enum ProcessingStatus
    {
        Unknown,
        InProgress,
        Completed,
        Failed
    }
}
