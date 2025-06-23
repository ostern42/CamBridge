/**************************************************************************
*  RecentActivityViewModel.cs                                             *
*  PATH: src\CamBridge.Config\ViewModels\RecentActivityViewModel.cs      *
*  VERSION: 0.7.11 | SIZE: ~1.3KB | MODIFIED: 2025-06-13                 *
*                                                                         *
*  DESCRIPTION: ViewModel for recent activity display                     *
*  Copyright (c) 2025 Claude's Improbably Reliable Software Solutions     *
**************************************************************************/

using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace CamBridge.Config.ViewModels
{
    public partial class RecentActivityViewModel : ObservableObject
    {
        private bool _isSuccess;
        private string _message = string.Empty;
        private DateTime _timestamp;
        private string _pipelineName = string.Empty;

        public bool IsSuccess
        {
            get => _isSuccess;
            set => SetProperty(ref _isSuccess, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public DateTime Timestamp
        {
            get => _timestamp;
            set => SetProperty(ref _timestamp, value);
        }

        public string PipelineName
        {
            get => _pipelineName;
            set => SetProperty(ref _pipelineName, value);
        }

        public string StatusIcon => IsSuccess ? "âœ“" : "âœ—";
        public string StatusColor => IsSuccess ? "#4CAF50" : "#F44336";
    }
}
