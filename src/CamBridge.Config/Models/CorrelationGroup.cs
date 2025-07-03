// src/CamBridge.Config/Models/CorrelationGroup.cs
// Version: 0.8.16
// Description: Groups log entries by correlation ID for tree view
// Copyright: (C) 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Core.Enums;
using CamBridge.Config.Models;
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
        private bool _isExpanded = true; // Default expanded for compact view

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
            Stages.CollectionChanged += (s, e) => OnPropertyChanged(nameof(AllEntries));
            UngroupedEntries.CollectionChanged += (s, e) => OnPropertyChanged(nameof(AllEntries));
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public TimeSpan Duration => EndTime - StartTime;
        public string DurationText => Duration.TotalMilliseconds >= 1000
            ? $"{Duration.TotalSeconds:0.0}s"
            : $"{Duration.TotalMilliseconds:0}ms";

        public int TotalEntries => Stages.Sum(s => s.Entries.Count) + UngroupedEntries.Count;

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

        // UI Helper properties
        public string StatusIcon => Status switch
        {
            ProcessingStatus.Completed => "[OK]",
            ProcessingStatus.Failed => "[FAIL]",
            ProcessingStatus.InProgress => "[...]",
            _ => "[?]"
        };

        public string StatusColor => Status switch
        {
            ProcessingStatus.Completed => "#4CAF50",  // Green
            ProcessingStatus.Failed => "#F44336",      // Red
            ProcessingStatus.InProgress => "#2196F3",  // Blue
            _ => "#9E9E9E"                            // Gray
        };
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
