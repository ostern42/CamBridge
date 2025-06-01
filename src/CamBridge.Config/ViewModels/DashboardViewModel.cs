// src/CamBridge.Config/ViewModels/DashboardViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Config.Models;
using CamBridge.Config.Services;

namespace CamBridge.Config.ViewModels
{
    public partial class DashboardViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private Timer? _refreshTimer;

        [ObservableProperty]
        private string serviceStatus = "Unknown";

        [ObservableProperty]
        private int queueLength;

        [ObservableProperty]
        private int activeProcessing;

        [ObservableProperty]
        private int successCount;

        [ObservableProperty]
        private int errorCount;

        [ObservableProperty]
        private double successRate;

        [ObservableProperty]
        private TimeSpan uptime;

        [ObservableProperty]
        private DateTime lastUpdate = DateTime.Now;

        [ObservableProperty]
        private bool isConnected;

        [ObservableProperty]
        private string connectionStatus = "Connecting...";

        public ObservableCollection<ActiveItemViewModel> ActiveItems { get; } = new();
        public ObservableCollection<RecentActivityViewModel> RecentActivities { get; } = new();

        public DashboardViewModel(IApiService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));

            // Start loading data immediately
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            // Initial load
            await RefreshDataAsync();

            // Set up auto-refresh timer (5 seconds)
            _refreshTimer = new Timer(
                async _ => await RefreshDataAsync(),
                null,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(5));
        }

        [RelayCommand]
        private async Task RefreshDataAsync()
        {
            try
            {
                IsLoading = true;

                // Check if service is available
                var isAvailable = await _apiService.IsServiceAvailableAsync();
                IsConnected = isAvailable;

                if (!isAvailable)
                {
                    ConnectionStatus = "Service Offline";
                    ServiceStatus = "Offline";
                    return;
                }

                // Get status
                var status = await _apiService.GetStatusAsync();
                if (status != null)
                {
                    UpdateFromStatus(status);
                    ConnectionStatus = "Connected";
                }
                else
                {
                    ConnectionStatus = "Connection Error";
                }

                LastUpdate = DateTime.Now;
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"Error: {ex.Message}";
                IsConnected = false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateFromStatus(ServiceStatusModel status)
        {
            ServiceStatus = status.ServiceStatus;
            QueueLength = status.QueueLength;
            ActiveProcessing = status.ActiveProcessing;
            SuccessCount = status.TotalSuccessful;
            ErrorCount = status.TotalFailed;
            SuccessRate = Math.Round(status.SuccessRate, 1);
            Uptime = status.Uptime;

            // Update active items
            ActiveItems.Clear();
            foreach (var item in status.ActiveItems.Take(5))
            {
                ActiveItems.Add(new ActiveItemViewModel
                {
                    FileName = item.FileName,
                    Duration = FormatDuration(item.Duration),
                    AttemptCount = item.AttemptCount
                });
            }

            // Generate recent activities (mock for now - could be enhanced)
            if (RecentActivities.Count == 0)
            {
                GenerateMockRecentActivities();
            }
        }

        private string FormatDuration(TimeSpan duration)
        {
            if (duration.TotalSeconds < 60)
                return $"{duration.TotalSeconds:F1}s";
            else if (duration.TotalMinutes < 60)
                return $"{duration.TotalMinutes:F1}m";
            else
                return $"{duration.TotalHours:F1}h";
        }

        private void GenerateMockRecentActivities()
        {
            // This could be enhanced to show real recent conversions
            RecentActivities.Clear();

            if (SuccessCount > 0)
            {
                RecentActivities.Add(new RecentActivityViewModel
                {
                    IsSuccess = true,
                    Message = "IMG_001.jpg → PAT123_20250601_0001.dcm"
                });
            }

            if (ErrorCount > 0)
            {
                RecentActivities.Add(new RecentActivityViewModel
                {
                    IsSuccess = false,
                    Message = "IMG_003.jpg - Missing patient data"
                });
            }
        }

        public void Cleanup()
        {
            _refreshTimer?.Dispose();
        }
    }

    public class ActiveItemViewModel
    {
        public string FileName { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int AttemptCount { get; set; }
    }

    public class RecentActivityViewModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
