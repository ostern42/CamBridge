// src/CamBridge.Config/ViewModels/DashboardViewModel.cs
// Version: 0.6.7
// Description: Dashboard ViewModel with multi-pipeline support

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Config.Models;
using CamBridge.Config.Services;
using CamBridge.Core;

namespace CamBridge.Config.ViewModels
{
    public partial class DashboardViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private readonly IConfigurationService _configurationService;
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

        public ObservableCollection<PipelineStatusViewModel> PipelineStatuses { get; } = new();
        public ObservableCollection<RecentActivityViewModel> RecentActivities { get; } = new();

        public DashboardViewModel(IApiService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));

            // ConfigurationService direkt erstellen für jetzt
            _configurationService = new ConfigurationService();

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

                // Load pipeline configurations
                var settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettingsV2>();
                if (settings == null)
                {
                    ConnectionStatus = "Configuration Error";
                    System.Diagnostics.Debug.WriteLine("ERROR: settings is null!");

                    // Create demo pipelines for testing
                    CreateDemoPipelines();
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Loaded settings - Pipeline count: {settings.Pipelines?.Count ?? 0}");

                // Check if we have any pipelines configured
                if (settings.Pipelines == null || settings.Pipelines.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("No pipelines configured - creating demo data");
                    CreateDemoPipelines();
                    ConnectionStatus = "Connected (Demo Mode)";
                    return;
                }

                // Get overall status
                var status = await _apiService.GetStatusAsync();
                if (status != null)
                {
                    UpdateOverallStatistics(status);
                    ConnectionStatus = "Connected";
                }

                // Update pipeline-specific statistics
                await UpdatePipelineStatistics(settings);

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

        private void UpdateOverallStatistics(ServiceStatusModel status)
        {
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

        private async Task UpdatePipelineStatistics(CamBridgeSettingsV2 settings)
        {
            System.Diagnostics.Debug.WriteLine($"=== UpdatePipelineStatistics called ===");
            System.Diagnostics.Debug.WriteLine($"Settings: {settings}");
            System.Diagnostics.Debug.WriteLine($"Pipeline count in settings: {settings?.Pipelines?.Count ?? 0}");

            // For now, we'll simulate pipeline-specific stats since the API doesn't support it yet
            // In a real implementation, we'd call pipeline-specific endpoints

            // Clear existing if pipeline count changed
            if (PipelineStatuses.Count != settings.Pipelines.Count)
            {
                PipelineStatuses.Clear();
            }

            foreach (var pipeline in settings.Pipelines)
            {
                System.Diagnostics.Debug.WriteLine($"Processing pipeline: {pipeline.Name} (ID: {pipeline.Id})");

                var existingStatus = PipelineStatuses.FirstOrDefault(ps => ps.PipelineId == pipeline.Id);

                if (existingStatus == null)
                {
                    // Create new pipeline status
                    var pipelineStatus = new PipelineStatusViewModel
                    {
                        PipelineId = pipeline.Id,
                        PipelineName = pipeline.Name,
                        IsEnabled = pipeline.Enabled,
                        WatchFolder = pipeline.WatchSettings.Path
                    };

                    // Simulate some stats (in real implementation, get from API)
                    if (pipeline.Enabled && TotalSuccessCount > 0)
                    {
                        // Distribute stats proportionally among enabled pipelines
                        var enabledCount = settings.Pipelines.Count(p => p.Enabled);
                        pipelineStatus.ProcessedToday = TotalSuccessCount / enabledCount;
                        pipelineStatus.ErrorsToday = TotalErrorCount / enabledCount;
                        pipelineStatus.QueueLength = TotalQueueLength / enabledCount;
                        pipelineStatus.SuccessRate = OverallSuccessRate; // Same rate for now
                        pipelineStatus.Status = "Active";
                        pipelineStatus.LastProcessed = DateTime.Now.AddMinutes(-Random.Shared.Next(1, 60));
                    }
                    else
                    {
                        pipelineStatus.Status = pipeline.Enabled ? "Idle" : "Disabled";
                    }

                    PipelineStatuses.Add(pipelineStatus);
                    System.Diagnostics.Debug.WriteLine($"Added pipeline status: {pipelineStatus.PipelineName}");
                }
                else
                {
                    // Update existing
                    existingStatus.PipelineName = pipeline.Name;
                    existingStatus.IsEnabled = pipeline.Enabled;
                    existingStatus.WatchFolder = pipeline.WatchSettings.Path;

                    if (pipeline.Enabled && TotalSuccessCount > 0)
                    {
                        var enabledCount = settings.Pipelines.Count(p => p.Enabled);
                        existingStatus.ProcessedToday = TotalSuccessCount / enabledCount;
                        existingStatus.ErrorsToday = TotalErrorCount / enabledCount;
                        existingStatus.QueueLength = TotalQueueLength / enabledCount;
                        existingStatus.SuccessRate = OverallSuccessRate;
                        existingStatus.Status = TotalActiveProcessing > 0 ? "Processing" : "Active";
                    }
                    else
                    {
                        existingStatus.Status = pipeline.Enabled ? "Idle" : "Disabled";
                        existingStatus.ProcessedToday = 0;
                        existingStatus.ErrorsToday = 0;
                        existingStatus.QueueLength = 0;
                    }
                }
            }

            // Remove pipelines that no longer exist
            var pipelineIds = settings.Pipelines.Select(p => p.Id).ToHashSet();
            var toRemove = PipelineStatuses.Where(ps => !pipelineIds.Contains(ps.PipelineId)).ToList();
            foreach (var item in toRemove)
            {
                PipelineStatuses.Remove(item);
            }

            System.Diagnostics.Debug.WriteLine($"Final PipelineStatuses count: {PipelineStatuses.Count}");
        }

        private void CreateDemoPipelines()
        {
            System.Diagnostics.Debug.WriteLine("Creating demo pipelines for testing...");

            PipelineStatuses.Clear();

            // Demo Pipeline 1
            PipelineStatuses.Add(new PipelineStatusViewModel
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
            });

            // Demo Pipeline 2
            PipelineStatuses.Add(new PipelineStatusViewModel
            {
                PipelineId = Guid.NewGuid(),
                PipelineName = "Emergency Pipeline (Demo)",
                IsEnabled = true,
                Status = "Processing",
                WatchFolder = @"C:\CamBridge\Watch\Emergency",
                ProcessedToday = 18,
                ErrorsToday = 0,
                QueueLength = 2,
                SuccessRate = 100,
                LastProcessed = DateTime.Now.AddMinutes(-2)
            });

            // Demo Pipeline 3
            PipelineStatuses.Add(new PipelineStatusViewModel
            {
                PipelineId = Guid.NewGuid(),
                PipelineName = "Archive Pipeline (Demo)",
                IsEnabled = false,
                Status = "Disabled",
                WatchFolder = @"C:\CamBridge\Watch\Archive",
                ProcessedToday = 0,
                ErrorsToday = 0,
                QueueLength = 0,
                SuccessRate = 0,
                LastProcessed = null
            });

            System.Diagnostics.Debug.WriteLine($"Created {PipelineStatuses.Count} demo pipelines");
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
        }
    }

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

    public class RecentActivityViewModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string PipelineName { get; set; } = string.Empty;
    }
}
