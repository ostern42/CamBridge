// src\CamBridge.Config\ViewModels\DashboardViewModel.cs
// Version: 0.6.11
// Description: Dashboard ViewModel with FIXED threading and simplified initialization
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CamBridge.Config.Models;
using CamBridge.Config.Services;
using CamBridge.Core;

namespace CamBridge.Config.ViewModels
{
    public partial class DashboardViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private readonly IConfigurationService _configurationService;
        private DispatcherTimer? _refreshTimer;
        private bool _isInitialized = false;

        [ObservableProperty]
        private string serviceStatus = "Checking...";

        [ObservableProperty]
        private int totalQueueLength;

        [ObservableProperty]
        private int totalActiveProcessing;

        [ObservableProperty]
        private int totalSuccessCount;

        [ObservableProperty]
        private int totalErrorCount;

        [ObservableProperty]
        private double overallSuccessRate;

        [ObservableProperty]
        private TimeSpan uptime;

        [ObservableProperty]
        private DateTime lastUpdate = DateTime.Now;

        [ObservableProperty]
        private bool isConnected;

        [ObservableProperty]
        private string connectionStatus = "Connecting...";

        [ObservableProperty]
        private string statusMessage = "";

        // Collections - created on UI thread
        public ObservableCollection<PipelineStatusViewModel> PipelineStatuses { get; }
        public ObservableCollection<RecentActivityViewModel> RecentActivities { get; }

        // Commands
        public IAsyncRelayCommand RefreshDataCommand { get; }

        public DashboardViewModel(
            IApiService apiService,
            IConfigurationService? configurationService = null)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _configurationService = configurationService ?? new ConfigurationService();

            // Initialize collections on UI thread (MUST be in constructor!)
            PipelineStatuses = new ObservableCollection<PipelineStatusViewModel>();
            RecentActivities = new ObservableCollection<RecentActivityViewModel>();

            // Initialize command
            RefreshDataCommand = new AsyncRelayCommand(RefreshDataAsync);

            Debug.WriteLine("DashboardViewModel created - waiting for initialization");

            // NO automatic initialization here! Wait for InitializeAsync()
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            Debug.WriteLine("=== DashboardViewModel.InitializeAsync START ===");

            // Initial data load
            await RefreshDataAsync();

            // Set up refresh timer on UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                _refreshTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(5)
                };
                _refreshTimer.Tick += async (s, e) => await RefreshDataAsync();
                _refreshTimer.Start();
            });

            _isInitialized = true;
            Debug.WriteLine("=== DashboardViewModel.InitializeAsync END ===");
        }

        private async Task RefreshDataAsync()
        {
            try
            {
                Debug.WriteLine($"\n=== DASHBOARD REFRESH START (Thread: {Thread.CurrentThread.ManagedThreadId}) ===");

                IsLoading = true;
                StatusMessage = "Refreshing...";

                // Check service availability
                var isAvailable = await _apiService.IsServiceAvailableAsync();
                IsConnected = isAvailable;

                if (!isAvailable)
                {
                    ConnectionStatus = "Service Offline";
                    ServiceStatus = "Offline";
                }

                // Load configuration
                var settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettingsV2>();

                if (settings?.Pipelines == null || settings.Pipelines.Count == 0)
                {
                    Debug.WriteLine("No pipelines configured - creating demo data");
                    await CreateDemoPipelinesAsync();
                    ConnectionStatus = isAvailable ? "Connected (Demo Mode)" : "Service Offline (Demo Mode)";
                    StatusMessage = "Showing demo pipelines";
                }
                else
                {
                    Debug.WriteLine($"Loading {settings.Pipelines.Count} configured pipelines");
                    await UpdatePipelineStatusesAsync(settings);
                    ConnectionStatus = isAvailable ? "Connected" : "Service Offline (Configuration Loaded)";
                    StatusMessage = $"{settings.Pipelines.Count} pipelines configured";
                }

                // Get service status if online
                if (isAvailable)
                {
                    try
                    {
                        var status = await _apiService.GetStatusAsync();
                        if (status != null)
                        {
                            UpdateOverallStatistics(status);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to get service status: {ex.Message}");
                    }
                }

                LastUpdate = DateTime.Now;
                Debug.WriteLine($"=== DASHBOARD REFRESH END - Pipelines: {PipelineStatuses.Count} ===\n");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in RefreshDataAsync: {ex.Message}");
                ConnectionStatus = "Error";
                StatusMessage = $"Error: {ex.Message}";
                IsConnected = false;

                // Show demo pipelines on error
                if (PipelineStatuses.Count == 0)
                {
                    await CreateDemoPipelinesAsync();
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CreateDemoPipelinesAsync()
        {
            Debug.WriteLine("=== Creating DEMO pipelines (async) ===");

            // ALL UI updates must be on UI thread!
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                PipelineStatuses.Clear();

                // Demo Pipeline 1 - Radiology
                PipelineStatuses.Add(new PipelineStatusViewModel
                {
                    PipelineId = Guid.NewGuid(),
                    PipelineName = "Radiology Pipeline",
                    IsEnabled = true,
                    Status = "Active",
                    WatchFolder = @"C:\CamBridge\Watch\Radiology",
                    ProcessedToday = 42,
                    ErrorsToday = 2,
                    QueueLength = 5,
                    SuccessRate = 95.2,
                    LastProcessed = DateTime.Now.AddMinutes(-15)
                });

                // Demo Pipeline 2 - Cardiology
                PipelineStatuses.Add(new PipelineStatusViewModel
                {
                    PipelineId = Guid.NewGuid(),
                    PipelineName = "Cardiology Pipeline",
                    IsEnabled = true,
                    Status = "Processing",
                    WatchFolder = @"C:\CamBridge\Watch\Cardiology",
                    ProcessedToday = 18,
                    ErrorsToday = 0,
                    QueueLength = 2,
                    SuccessRate = 100,
                    LastProcessed = DateTime.Now.AddMinutes(-2)
                });

                // Demo Pipeline 3 - Emergency (Disabled)
                PipelineStatuses.Add(new PipelineStatusViewModel
                {
                    PipelineId = Guid.NewGuid(),
                    PipelineName = "Emergency Pipeline",
                    IsEnabled = false,
                    Status = "Disabled",
                    WatchFolder = @"C:\CamBridge\Watch\Emergency",
                    ProcessedToday = 0,
                    ErrorsToday = 0,
                    QueueLength = 0,
                    SuccessRate = 0,
                    LastProcessed = null
                });

                // Also create some demo activities
                CreateDemoActivities();
            });

            Debug.WriteLine($"=== Created {PipelineStatuses.Count} DEMO pipelines ===");
        }

        private async Task UpdatePipelineStatusesAsync(CamBridgeSettingsV2 settings)
        {
            Debug.WriteLine("=== Updating pipeline statuses ===");

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                PipelineStatuses.Clear();

                foreach (var pipeline in settings.Pipelines)
                {
                    var pipelineStatus = new PipelineStatusViewModel
                    {
                        PipelineId = pipeline.Id,
                        PipelineName = pipeline.Name,
                        IsEnabled = pipeline.Enabled,
                        WatchFolder = pipeline.WatchSettings?.Path ?? "",
                        Status = pipeline.Enabled ? "Active" : "Disabled"
                    };

                    // Simulate some stats for now
                    if (pipeline.Enabled)
                    {
                        var random = new Random();
                        pipelineStatus.ProcessedToday = random.Next(10, 100);
                        pipelineStatus.ErrorsToday = random.Next(0, 5);
                        pipelineStatus.QueueLength = random.Next(0, 10);
                        pipelineStatus.SuccessRate = pipelineStatus.ProcessedToday > 0
                            ? ((double)(pipelineStatus.ProcessedToday - pipelineStatus.ErrorsToday) / pipelineStatus.ProcessedToday) * 100
                            : 100;
                        pipelineStatus.LastProcessed = DateTime.Now.AddMinutes(-random.Next(1, 60));
                    }

                    PipelineStatuses.Add(pipelineStatus);
                }
            });
        }

        private void UpdateOverallStatistics(ServiceStatusModel status)
        {
            ServiceStatus = status.ServiceStatus;
            TotalQueueLength = status.QueueLength;
            TotalActiveProcessing = status.ActiveProcessing;
            TotalSuccessCount = status.TotalSuccessful;
            TotalErrorCount = status.TotalFailed;
            OverallSuccessRate = Math.Round(status.SuccessRate, 1);
            Uptime = status.Uptime;
        }

        private void CreateDemoActivities()
        {
            RecentActivities.Clear();

            var activities = new[]
            {
                ("IMG_001.jpg → PAT123_20250609_0001.dcm", true, "Radiology Pipeline"),
                ("IMG_002.jpg → PAT124_20250609_0001.dcm", true, "Cardiology Pipeline"),
                ("IMG_003.jpg - Missing patient data", false, "Radiology Pipeline"),
                ("IMG_004.jpg → PAT125_20250609_0001.dcm", true, "Emergency Pipeline"),
                ("IMG_005.jpg → PAT126_20250609_0001.dcm", true, "Radiology Pipeline")
            };

            var random = new Random();
            foreach (var (message, success, pipeline) in activities)
            {
                RecentActivities.Add(new RecentActivityViewModel
                {
                    IsSuccess = success,
                    Message = message,
                    Timestamp = DateTime.Now.AddMinutes(-random.Next(1, 30)),
                    PipelineName = pipeline
                });
            }
        }

        public void Cleanup()
        {
            _refreshTimer?.Stop();
            _refreshTimer = null;
            Debug.WriteLine("Dashboard cleanup completed");
        }
    }
}
