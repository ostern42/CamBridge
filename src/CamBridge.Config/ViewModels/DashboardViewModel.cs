/**************************************************************************
*  DashboardViewModel.cs                                                  *
*  PATH: src\CamBridge.Config\ViewModels\DashboardViewModel.cs           *
*  VERSION: 0.7.11 | SIZE: ~10KB | MODIFIED: 2025-06-13                  *
*                                                                         *
*  DESCRIPTION: ViewModel for the main dashboard display                  *
*  Copyright (c) 2025 Claude's Improbably Reliable Software Solutions     *
**************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CamBridge.Config.Models;
using CamBridge.Config.Services;
using CamBridge.Core;
using CamBridge.Core.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CamBridge.Config.ViewModels
{
    /// <summary>
    /// ViewModel for the dashboard page showing service and pipeline status
    /// KISS Edition - Works with actual API!
    /// </summary>
    public partial class DashboardViewModel : ObservableObject
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

        [ObservableProperty]
        private bool isLoading;

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

                if (isAvailable)
                {
                    // Get service status
                    var status = await _apiService.GetStatusAsync();
                    if (status != null)
                    {
                        UpdateServiceStatus(status);
                    }

                    ConnectionStatus = "Connected";
                    StatusMessage = "Service is running";
                }
                else
                {
                    ConnectionStatus = "Service Offline";
                    ServiceStatus = "Offline";
                    StatusMessage = "Service is not running. Please start the service.";
                }

                // Load configuration for pipeline info
                var settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettingsV2>();
                if (settings?.Pipelines != null && settings.Pipelines.Count > 0)
                {
                    UpdatePipelinesFromConfig(settings.Pipelines);
                }

                LastUpdate = DateTime.Now;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error refreshing dashboard: {ex.Message}");
                HandleError(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateServiceStatus(ServiceStatusModel status)
        {
            ServiceStatus = status.ServiceStatus == "Running" ? "Running" : "Stopped";
            Uptime = status.Uptime;

            // Update totals from service
            TotalQueueLength = status.QueueLength;
            TotalActiveProcessing = status.ActiveProcessing;
            TotalSuccessCount = status.TotalSuccessful;
            TotalErrorCount = status.TotalFailed;
            OverallSuccessRate = status.SuccessRate;

            // Update pipeline statuses if available
            if (status.Pipelines != null)
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    foreach (var pipelineData in status.Pipelines)
                    {
                        // FIX: Use PipelineName instead of Name
                        var existing = PipelineStatuses.FirstOrDefault(p => p.PipelineName == pipelineData.Name);
                        if (existing != null)
                        {
                            existing.IsEnabled = pipelineData.IsActive;
                            existing.Status = pipelineData.IsActive ? "Active" : "Inactive";
                            existing.QueueLength = pipelineData.QueueLength;

                            // Map to existing properties in PipelineStatusViewModel
                            existing.ProcessedToday = pipelineData.TotalSuccessful + pipelineData.TotalFailed;
                            existing.ErrorsToday = pipelineData.TotalFailed;

                            var total = pipelineData.TotalSuccessful + pipelineData.TotalFailed;
                            existing.SuccessRate = total > 0 ?
                                (double)pipelineData.TotalSuccessful / total * 100 : 0;
                        }
                    }
                });
            }
        }

        private void UpdatePipelinesFromConfig(List<PipelineConfiguration> pipelines)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    // Update existing or add new
                    foreach (var pipeline in pipelines)
                    {
                        // FIX 1: Use PipelineName instead of Name
                        var existing = PipelineStatuses.FirstOrDefault(p => p.PipelineName == pipeline.Name);
                        if (existing == null)
                        {
                            PipelineStatuses.Add(new PipelineStatusViewModel
                            {
                                // FIX 2: Use correct property names
                                PipelineName = pipeline.Name,
                                IsEnabled = pipeline.Enabled,
                                Status = pipeline.Enabled ? "Ready" : "Disabled",  // FIX 3: Complete ternary operator!
                                PipelineId = pipeline.Id,
                                WatchFolder = pipeline.WatchSettings?.Path ?? ""   // FIX 4: Use Path not WatchFolder
                            });
                        }
                        else
                        {
                            existing.IsEnabled = pipeline.Enabled;
                            existing.Status = pipeline.Enabled ? "Ready" : "Disabled";
                            existing.WatchFolder = pipeline.WatchSettings?.Path ?? "";  // FIX 5: Use Path
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error updating pipelines: {ex.Message}");
                }
            });
        }

        private void HandleError(Exception ex)
        {
            IsConnected = false;
            ConnectionStatus = "Error";
            StatusMessage = $"Error: {ex.Message}";
            ServiceStatus = "Error";
        }

        private void ShowConfigPath()
        {
            try
            {
                var dir = Path.GetDirectoryName(ConfigPath);
                if (Directory.Exists(dir))
                {
                    Process.Start("explorer.exe", dir);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening config path: {ex.Message}");
            }
        }

        /// <summary>
        /// Cleanup when view model is disposed
        /// </summary>
        public void Cleanup()
        {
            _refreshTimer?.Stop();
        }
    }
}
