// src/CamBridge.Config/ViewModels/PipelineStatusViewModel.cs
// Version: 0.6.8
// Description: ViewModel for individual pipeline status display

using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace CamBridge.Config.ViewModels
{
    public class PipelineStatusViewModel : ObservableObject
    {
        private Guid _pipelineId;
        private string _pipelineName = string.Empty;
        private bool _isEnabled;
        private string _status = "Unknown";
        private int _processedToday;
        private int _errorsToday;
        private int _queueLength;
        private double _successRate;
        private DateTime? _lastProcessed;
        private string _watchFolder = string.Empty;

        public Guid PipelineId
        {
            get => _pipelineId;
            set => SetProperty(ref _pipelineId, value);
        }

        public string PipelineName
        {
            get => _pipelineName;
            set => SetProperty(ref _pipelineName, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public int ProcessedToday
        {
            get => _processedToday;
            set => SetProperty(ref _processedToday, value);
        }

        public int ErrorsToday
        {
            get => _errorsToday;
            set => SetProperty(ref _errorsToday, value);
        }

        public int QueueLength
        {
            get => _queueLength;
            set => SetProperty(ref _queueLength, value);
        }

        public double SuccessRate
        {
            get => _successRate;
            set => SetProperty(ref _successRate, value);
        }

        public DateTime? LastProcessed
        {
            get => _lastProcessed;
            set => SetProperty(ref _lastProcessed, value);
        }

        public string WatchFolder
        {
            get => _watchFolder;
            set => SetProperty(ref _watchFolder, value);
        }

        public string StatusColor => Status switch
        {
            "Processing" => "#4CAF50",
            "Active" => "#2196F3",
            "Idle" => "#FFC107",
            "Disabled" => "#9E9E9E",
            "Error" => "#F44336",
            _ => "#9E9E9E"
        };
    }
}
