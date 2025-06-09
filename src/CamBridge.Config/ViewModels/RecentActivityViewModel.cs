// src/CamBridge.Config/ViewModels/RecentActivityViewModel.cs
// Version: 0.6.8
// Description: ViewModel for recent activity items

using System;

namespace CamBridge.Config.ViewModels
{
    public class RecentActivityViewModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string PipelineName { get; set; } = string.Empty;
    }
}
