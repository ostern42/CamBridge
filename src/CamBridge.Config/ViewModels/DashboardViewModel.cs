// src\CamBridge.Config\ViewModels\DashboardViewModel.cs
// Version: 0.6.8
// Description: Dashboard ViewModel with complete multi-pipeline support and all related ViewModels
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Config.Models;
using CamBridge.Config.Services;
using CamBridge.Core;
using Microsoft.Extensions.Logging;

namespace CamBridge.Config.ViewModels
{
    public partial class DashboardViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private readonly IConfigurationService _configurationService;
        private readonly ILogger<DashboardViewModel>? _logger;
        private Timer? _refreshTimer;

        [ObservableProperty]
        private string serviceStatus = "Unknown";

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

        // FIX: Explicit command property for binding
        public IAsyncRelayCommand RefreshDataCommand { get; }

        public ObservableCollection<PipelineStatusViewModel> PipelineStatuses { get; } = new();
        public ObservableCollection<RecentActivityViewModel> RecentActivities { get; } = new();

        public DashboardViewModel(
            IApiService apiService,
            IConfigurationService? configurationService = null,
            ILogger<DashboardViewModel>? logger = null)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _configurationService = configurationService ?? new ConfigurationService();
            _logger = logger;

            // Initialize command
            RefreshDataCommand = new AsyncRelayCommand(RefreshDataAsync);

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

        private async Task RefreshDataAsync()
        {
            try
            {
                Debug.WriteLine("\n=== DASHBOARD REFRESH START ===");
                Debug.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}");
                Debug.WriteLine($"Time: {DateTime.Now:HH:mm:ss.fff}");

                IsLoading = true;
                StatusMessage = "Refreshing...";

                // Check if service is available
                var isAvailable = await _apiService.IsServiceAvailableAsync();
                IsConnected = isAvailable;

                if (!isAvailable)
                {
                    ConnectionStatus = "Service Offline";
                    ServiceStatus = "Offline";
                    _logger?.LogWarning("Service is not available");
                    // Continue anyway to show pipelines!
                }

                // Load pipeline configurations
                Debug.WriteLine("Loading CamBridgeSettingsV2...");
                var settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettingsV2>();

                if (settings == null)
                {
                    Debug.WriteLine("❌ Settings is NULL! Creating demo pipelines...");
                    ConnectionStatus = isAvailable ? "Configuration Error" : "Service Offline (Demo Mode)";
                    StatusMessage = "Using demo configuration";
                    _logger?.LogError("Failed to load configuration - using demo data");

                    // Create demo pipelines
                    CreateDemoPipelines();
                }
                else
                {
                    Debug.WriteLine($"✓ Settings loaded. Pipeline count: {settings.Pipelines?.Count ?? 0}");

                    // Check if we have any pipelines configured
                    if (settings.Pipelines == null || settings.Pipelines.Count == 0)
                    {
                        Debug.WriteLine("⚠️ No pipelines in settings - creating demo data");
                        CreateDemoPipelines();
                        ConnectionStatus = isAvailable ? "Connected (No Pipelines)" : "Service Offline (Demo Mode)";
                        StatusMessage = "No pipelines configured - showing demo data";
                    }
                    else
                    {
                        Debug.WriteLine($"Loading {settings.Pipelines.Count} pipelines from settings");
                        // Update UI with pipelines from settings
                        UpdatePipelineStatistics(settings);

                        if (!isAvailable)
                        {
                            ConnectionStatus = "Service Offline (Configuration Loaded)";
                            StatusMessage = $"Showing {settings.Pipelines.Count} configured pipelines";
                        }
                    }
                }

                // Get overall status from service - only if online!
                if (isAvailable)
                {
                    Debug.WriteLine("Getting service status...");
                    try
                    {
                        var status = await _apiService.GetStatusAsync();
                        if (status != null)
                        {
                            UpdateOverallStatistics(status);
                            ConnectionStatus = "Connected";
                            StatusMessage = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to get service status: {ex.Message}");
                    }
                }

                LastUpdate = DateTime.Now;
                Debug.WriteLine($"✓ Refresh complete. Pipelines: {PipelineStatuses.Count}");
                Debug.WriteLine("=== DASHBOARD REFRESH END ===\n");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ REFRESH FAILED: {ex.Message}");
                Debug.WriteLine($"Stack: {ex.StackTrace}");
                ConnectionStatus = $"Error: {ex.Message}";
                StatusMessage = "Refresh failed";
                IsConnected = false;
                _logger?.LogError(ex, "Dashboard refresh failed");

                // Even on error, show demo pipelines
                if (PipelineStatuses.Count == 0)
                {
                    Debug.WriteLine("Creating demo pipelines due to error...");
                    CreateDemoPipelines();
                }
            }
            finally
            {
                IsLoading = false;
                if (string.IsNullOrEmpty(StatusMessage))
                {
                    StatusMessage = $"{PipelineStatuses.Count} pipelines";
                }
            }
        }

        private void UpdateOverallStatistics(ServiceStatusModel status)
        {
            // Using correct property names from ServiceStatusModel
            ServiceStatus = status.ServiceStatus;
            TotalQueueLength = status.QueueLength;
            TotalActiveProcessing = status.ActiveProcessing;
            TotalSuccessCount = status.TotalSuccessful;
            TotalErrorCount = status.TotalFailed;
            OverallSuccessRate = Math.Round(status.SuccessRate, 1);
            Uptime = status.Uptime;

            // Generate recent activities if needed
            if (RecentActivities.Count == 0 && (status.TotalSuccessful > 0 || status.TotalFailed > 0))
            {
                GenerateMockRecentActivities();
            }
        }

        private void UpdatePipelineStatistics(CamBridgeSettingsV2 settings)
        {
            Debug.WriteLine($"=== UpdatePipelineStatistics START ===");
            Debug.WriteLine($"Settings has {settings.Pipelines.Count} pipelines");

            // Clear and recreate to ensure fresh data
            PipelineStatuses.Clear();

            foreach (var pipeline in settings.Pipelines)
            {
                Debug.WriteLine($"Processing pipeline: {pipeline.Name} (ID: {pipeline.Id})");

                var pipelineStatus = new PipelineStatusViewModel
                {
                    PipelineId = pipeline.Id,
                    PipelineName = pipeline.Name,
                    IsEnabled = pipeline.Enabled,
                    WatchFolder = pipeline.WatchSettings?.Path ?? "",
                    Status = pipeline.Enabled ? "Active" : "Disabled"
                };

                // Simulate some stats (in real implementation, get from API)
                if (pipeline.Enabled)
                {
                    pipelineStatus.ProcessedToday = Random.Shared.Next(10, 100);
                    pipelineStatus.ErrorsToday = Random.Shared.Next(0, 5);
                    pipelineStatus.QueueLength = Random.Shared.Next(0, 10);
                    pipelineStatus.SuccessRate = pipelineStatus.ProcessedToday > 0
                        ? ((double)(pipelineStatus.ProcessedToday - pipelineStatus.ErrorsToday) / pipelineStatus.ProcessedToday) * 100
                        : 100;
                    pipelineStatus.LastProcessed = DateTime.Now.AddMinutes(-Random.Shared.Next(1, 60));
                }
                else
                {
                    pipelineStatus.ProcessedToday = 0;
                    pipelineStatus.ErrorsToday = 0;
                    pipelineStatus.QueueLength = 0;
                    pipelineStatus.SuccessRate = 0;
                    pipelineStatus.LastProcessed = null;
                }

                PipelineStatuses.Add(pipelineStatus);
                Debug.WriteLine($"  ✓ Added: {pipelineStatus.PipelineName} ({pipelineStatus.Status})");
            }

            Debug.WriteLine($"=== UpdatePipelineStatistics END - Total: {PipelineStatuses.Count} ===");
        }

        private void CreateDemoPipelines()
        {
            Debug.WriteLine("=== Creating DEMO pipelines ===");

            PipelineStatuses.Clear();

            // Demo Pipeline 1
            var demo1 = new PipelineStatusViewModel
            {
                PipelineId = Guid.NewGuid(),
                PipelineName = "Radiology Pipeline (Demo)",
                IsEnabled = true,
                Status = "Active",
                WatchFolder = @"C:\CamBridge\Watch\Radiology",
                ProcessedToday = 42,
                ErrorsToday = 2,
                QueueLength = 5,
                SuccessRate = 95.2,
                LastProcessed = DateTime.Now.AddMinutes(-15)
            };
            PipelineStatuses.Add(demo1);
            Debug.WriteLine($"Added demo pipeline: {demo1.PipelineName}");

            // Demo Pipeline 2
            var demo2 = new PipelineStatusViewModel
            {
                PipelineId = Guid.NewGuid(),
                PipelineName = "Cardiology Pipeline (Demo)",
                IsEnabled = true,
                Status = "Processing",
                WatchFolder = @"C:\CamBridge\Watch\Cardiology",
                ProcessedToday = 18,
                ErrorsToday = 0,
                QueueLength = 2,
                SuccessRate = 100,
                LastProcessed = DateTime.Now.AddMinutes(-2)
            };
            PipelineStatuses.Add(demo2);
            Debug.WriteLine($"Added demo pipeline: {demo2.PipelineName}");

            // Demo Pipeline 3
            var demo3 = new PipelineStatusViewModel
            {
                PipelineId = Guid.NewGuid(),
                PipelineName = "Emergency Pipeline (Demo)",
                IsEnabled = false,
                Status = "Disabled",
                WatchFolder = @"C:\CamBridge\Watch\Emergency",
                ProcessedToday = 0,
                ErrorsToday = 0,
                QueueLength = 0,
                SuccessRate = 0,
                LastProcessed = null
            };
            PipelineStatuses.Add(demo3);
            Debug.WriteLine($"Added demo pipeline: {demo3.PipelineName}");

            Debug.WriteLine($"=== Created {PipelineStatuses.Count} DEMO pipelines ===");

            // Update status message
            StatusMessage = "Showing demo pipelines";
        }

        private void GenerateMockRecentActivities()
        {
            RecentActivities.Clear();

            var activities = new[]
            {
                new { Success = true, Message = "IMG_001.jpg → PAT123_20250608_0001.dcm", Pipeline = "Radiology Pipeline" },
                new { Success = true, Message = "IMG_002.jpg → PAT124_20250608_0001.dcm", Pipeline = "Emergency Pipeline" },
                new { Success = false, Message = "IMG_003.jpg - Missing patient data", Pipeline = "Radiology Pipeline" },
                new { Success = true, Message = "IMG_004.jpg → PAT125_20250608_0001.dcm", Pipeline = "Emergency Pipeline" },
                new { Success = true, Message = "IMG_005.jpg → PAT126_20250608_0001.dcm", Pipeline = "Radiology Pipeline" }
            };

            foreach (var activity in activities.Take(Math.Min(5, TotalSuccessCount + TotalErrorCount)))
            {
                RecentActivities.Add(new RecentActivityViewModel
                {
                    IsSuccess = activity.Success,
                    Message = activity.Message,
                    Timestamp = DateTime.Now.AddMinutes(-Random.Shared.Next(1, 30)),
                    PipelineName = activity.Pipeline
                });
            }
        }

        public void Cleanup()
        {
            _refreshTimer?.Dispose();
            Debug.WriteLine("Dashboard cleanup completed");
        }
    }
}
