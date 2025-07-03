// src/CamBridge.Config/Models/StageGroup.cs
// Version: 0.8.16
// Description: Groups log entries by processing stage within a correlation
// Copyright: (C) 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.ObjectModel;
using System.Linq;
using CamBridge.Core.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

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
        public ObservableCollection<LogEntry> Entries { get; } = new();

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public TimeSpan Duration => EndTime - StartTime;
        public string DurationText => $"{Duration.TotalMilliseconds:0}ms";

        public string StageIcon => Entries.FirstOrDefault()?.StageIcon ?? "[*]";
    }
}
