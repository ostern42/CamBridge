using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace CamBridge.Config.ViewModels
{
    public partial class DashboardViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string serviceStatus = "Unknown";

        [ObservableProperty]
        private int queueLength;

        [ObservableProperty]
        private int successCount;

        [ObservableProperty]
        private int errorCount;

        [ObservableProperty]
        private DateTime lastUpdate = DateTime.Now;

        public DashboardViewModel()
        {
            // Mock data for now
            ServiceStatus = "Running";
            QueueLength = 5;
            SuccessCount = 123;
            ErrorCount = 2;
        }
    }
}