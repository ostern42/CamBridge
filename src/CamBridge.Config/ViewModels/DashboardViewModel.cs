// src\CamBridge.Config\ViewModels\DashboardViewModel.cs
// Version: 0.7.1
// Description: Dashboard ViewModel without demo pipelines - shows real data only!
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

        [ObservableProperty]
        private string configPath = "";

        // Collections - created on UI thread
        public ObservableCollection<PipelineStatusViewModel> PipelineStatuses { get; }
        public ObservableCollection<RecentActivityViewModel> RecentActivities { get; }

        // Commands
        public IAsyncRelayCommand RefreshDataCommand { get; }
        public IRelayCommand ShowConfigPathCommand { get; }

        public DashboardViewModel(
            IApiService apiService,
            IConfigurationService? configurationService = null)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _configurationService = configurationService ?? new ConfigurationService();

            // Initialize collections on UI thread (MUST be in constructor!)
            PipelineStatuses = new ObservableCollection<PipelineStatusViewModel>();
            RecentActivities = new ObservableCollection<RecentActivityViewModel>();

            // Initialize commands
            RefreshDataCommand = new AsyncRelayCommand(RefreshDataAsync);
            ShowConfigPathCommand = new RelayCommand(ShowConfigPath);

            // Set config path for display
            ConfigPath = ConfigurationPaths.GetPrimaryConfigPath();

            Debug.WriteLine("DashboardViewModel created - waiting for initialization");
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
                Debug.WriteLine($"Config Path: {ConfigPath}");

                IsLoading = true;
                StatusMessage = "Refreshing...";

                // Check service availability
                var isAvailable = await _apiService.IsServiceAvailableAsync();
                IsConnected = isAvailable;

                if (!isAvailable)
                {
                    ConnectionStatus = "Service Offline";
                    ServiceStatus = "Offline";
                    StatusMessage = "Service is not running. Please start the service.";
                }

                // Load configuration
                var settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettingsV2>();

                if (settings?.Pipelines == null || settings.Pipelines.Count == 0)
                {
                    Debug.WriteLine("No pipelines configured");
                    await ClearPipelinesAsync();
                    ConnectionStatus = isAvailable ? "Connected (No Pipelines)" : "Service Offline";
                    StatusMessage = "No pipelines configured. Use Pipeline Configuration to add pipelines.";
                }
                else
                {
                    Debug.WriteLine($"Loading {settings.Pipelines.Count} configured pipelines");
                    await UpdatePipelineStatusesAsync(settings, isAvailable);
                    ConnectionStatus = isAvailable ? "Connected" : "Service Offline (Config Loaded)";
                    StatusMessage = $"{settings.Pipelines.Count} pipeline(s) configured";
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

                            // Check if service uses same config
                            if (status.ConfigPath != null && status.ConfigPath != ConfigPath)
                            {
                                StatusMessage += $" ⚠️ Service uses different config!";
                                Debug.WriteLine($"CONFIG MISMATCH! Tool: {ConfigPath}, Service: {status.ConfigPath}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to get service status: {ex.Message}");
                    }
                }

                // Update activities if we have pipelines
                if (PipelineStatuses.Count > 0)
                {
                    UpdateRecentActivities();
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
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ClearPipelinesAsync()
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                PipelineStatuses.Clear();
                RecentActivities.Clear();
            });
        }

        private async Task UpdatePipelineStatusesAsync(CamBridgeSettingsV2 settings, bool serviceAvailable)
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
                        Status = pipeline.Enabled ? (serviceAvailable ? "Active" : "Service Offline") : "Disabled"
                    };

                    // If service is available, real-time stats will be updated from service
                    // Otherwise, show zero stats
                    if (!serviceAvailable)
                    {
                        pipelineStatus.ProcessedToday = 0;
                        pipelineStatus.ErrorsToday = 0;
                        pipelineStatus.QueueLength = 0;
                        pipelineStatus.SuccessRate = 0;
                        pipelineStatus.LastProcessed = null;
                    }

                    PipelineStatuses.Add(pipelineStatus);
                }
            });

            Debug.WriteLine($"Updated {PipelineStatuses.Count} pipeline statuses");
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

            // Update individual pipeline stats if available
            if (status.Pipelines != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var pipelineData in status.Pipelines)
                    {
                        var localPipeline = PipelineStatuses.FirstOrDefault(p =>
                            p.PipelineId.ToString() == pipelineData.Id);

                        if (localPipeline != null)
                        {
                            localPipeline.QueueLength = pipelineData.QueueLength;
                            localPipeline.ProcessedToday = pipelineData.TotalProcessed;
                            localPipeline.ErrorsToday = pipelineData.TotalFailed;
                            localPipeline.SuccessRate = pipelineData.TotalProcessed > 0
                                ? ((double)pipelineData.TotalSuccessful / pipelineData.TotalProcessed) * 100
                                : 0;
                            localPipeline.Status = pipelineData.IsActive ? "Active" : "Inactive";
                        }
                    }
                });
            }
        }

        private void UpdateRecentActivities()
        {
            // Only update if we don't have activities yet
            if (RecentActivities.Count == 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RecentActivities.Add(new RecentActivityViewModel
                    {
                        IsSuccess = true,
                        Message = "Waiting for processing activity...",
                        Timestamp = DateTime.Now,
                        PipelineName = PipelineStatuses.FirstOrDefault()?.PipelineName ?? "Unknown"
                    });
                });
            }
        }

        private void ShowConfigPath()
        {
            var info = $"Config Path: {ConfigPath}\n" +
                      $"Config Exists: {System.IO.File.Exists(ConfigPath)}\n\n" +
                      ConfigurationPaths.GetDiagnosticInfo();

            MessageBox.Show(info, "Configuration Information",
                           MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Cleanup()
        {
            _refreshTimer?.Stop();
            _refreshTimer = null;
            Debug.WriteLine("Dashboard cleanup completed");
        }
    }
}
