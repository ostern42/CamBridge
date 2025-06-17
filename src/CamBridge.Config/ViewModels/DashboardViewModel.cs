// src\CamBridge.Config\ViewModels\DashboardViewModel.cs
// Version: 0.7.21
// Description: MINIMAL Dashboard - Just show if service is running!
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CamBridge.Config.Models;
using CamBridge.Config.Services;
using CamBridge.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CamBridge.Config.ViewModels
{
    /// <summary>
    /// MINIMAL Dashboard - KISS approach!
    /// </summary>
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IConfigurationService _configurationService;
        private DispatcherTimer? _refreshTimer;
        private readonly HttpClient _httpClient = new();

        [ObservableProperty]
        private string serviceStatus = "Checking...";

        [ObservableProperty]
        private bool isServiceRunning = false;

        [ObservableProperty]
        private string uptimeText = "";

        [ObservableProperty]
        private string versionText = "";

        [ObservableProperty]
        private DateTime lastUpdate = DateTime.Now;

        [ObservableProperty]
        private bool isLoading;

        // Collections
        public ObservableCollection<PipelineStatusViewModel> PipelineStatuses { get; }

        // Commands
        public IAsyncRelayCommand RefreshCommand { get; }
        public IAsyncRelayCommand StartServiceCommand { get; }

        public DashboardViewModel(IApiService? apiService = null, IConfigurationService? configurationService = null)
        {
            // We ignore IApiService - go direct!
            _configurationService = configurationService ?? new ConfigurationService();

            PipelineStatuses = new ObservableCollection<PipelineStatusViewModel>();

            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
            StartServiceCommand = new AsyncRelayCommand(StartServiceAsync);

            // Setup timer
            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _refreshTimer.Tick += async (s, e) => await RefreshAsync();
            _refreshTimer.Start();

            // Initial load
            Task.Run(async () => await RefreshAsync());
        }

        private async Task RefreshAsync()
        {
            try
            {
                IsLoading = true;
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] Dashboard refresh...");

                // MINIMAL: Direct HTTP call!
                var response = await _httpClient.GetAsync("http://localhost:5111/api/status");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var status = JsonSerializer.Deserialize<ServiceStatusModel>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (status != null)
                    {
                        // Update UI on dispatcher
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            IsServiceRunning = true;
                            ServiceStatus = status.ServiceStatus;
                            UptimeText = $"Uptime: {status.Uptime:hh\\:mm\\:ss}";
                            VersionText = $"Version: {status.Version}";

                            Debug.WriteLine($"Service is {status.ServiceStatus}!");

                            // Update pipelines
                            if (status.Pipelines != null)
                            {
                                UpdatePipelines(status.Pipelines);
                            }
                        });
                    }
                }
                else
                {
                    // Service offline
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        IsServiceRunning = false;
                        ServiceStatus = "Offline";
                        UptimeText = "";
                        VersionText = "";
                        Debug.WriteLine("Service is offline!");
                    });
                }

                LastUpdate = DateTime.Now;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Dashboard refresh error: {ex.Message}");

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    IsServiceRunning = false;
                    ServiceStatus = "Offline";
                    UptimeText = "";
                    VersionText = "Cannot connect to service";
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdatePipelines(List<PipelineStatusData> pipelines)
        {
            PipelineStatuses.Clear();

            foreach (var p in pipelines)
            {
                PipelineStatuses.Add(new PipelineStatusViewModel
                {
                    PipelineName = p.Name,
                    Status = p.IsActive ? "Active" : "Inactive",
                    IsEnabled = p.IsActive,
                    QueueLength = p.QueueLength,
                    ProcessedToday = p.TotalProcessed,
                    ErrorsToday = p.TotalFailed,
                    WatchFolder = p.WatchedFolders?.Count > 0 ? p.WatchedFolders[0] : ""
                });
            }
        }

        private async Task StartServiceAsync()
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "net.exe",
                    Arguments = "start CamBridgeService",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                var process = Process.Start(startInfo);
                await process!.WaitForExitAsync();

                // Wait and refresh
                await Task.Delay(2000);
                await RefreshAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to start service: {ex.Message}");
            }
        }

        public void Cleanup()
        {
            _refreshTimer?.Stop();
            _httpClient?.Dispose();
        }
    }
}
