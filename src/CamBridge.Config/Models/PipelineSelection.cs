// src/CamBridge.Config/Models/PipelineSelection.cs
// Version: 0.8.16
// Description: Represents a selectable pipeline in the log viewer
// Copyright: (C) 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Config.ViewModels;

namespace CamBridge.Config.Models
{
    /// <summary>
    /// Represents a selectable pipeline in the multi-select dropdown
    /// </summary>
    public class PipelineSelection : ViewModelBase
    {
        private bool _isSelected;

        public string Name { get; set; } = string.Empty;
        public string SanitizedName { get; set; } = string.Empty;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }

    /// <summary>
    /// Tracks file position for incremental reading
    /// </summary>
    public class FilePositionInfo
    {
        public long Position { get; set; }
        public DateTime LastRead { get; set; }
    }
}
