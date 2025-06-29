// src/CamBridge.Config/ViewModels/ServiceControlViewModel.cs
// Version: 0.8.6
// Modified: Session 96 - Making Logs Great Again!

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CamBridge.Config.Services;
using CamBridge.Core;
using CamBridge.Core.Logging;
using CamBridge.Core.Enums;


namespace CamBridge.Config.ViewModels
{
    public partial class ServiceControlViewModel : ViewModelBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IConfigurationService _configurationService;
        private Timer? _statusTimer;

        [ObservableProperty]
        private ServiceStatus serviceStatus = ServiceStatus.Unknown;

        [ObservableProperty]
        private string statusText = "Checking...";

        [ObservableProperty]
        private string statusColor = "Gray";

        [ObservableProperty]
        private bool canStart = false;

        [ObservableProperty]
        private bool canStop = false;

        [ObservableProperty]
        private bool canRestart = false;

        [ObservableProperty]
        private bool isServiceInstalled = false;

        [ObservableProperty]
        private string? uptime;

        [ObservableProperty]
        private bool requiresElevation = false;

        // Service Settings Properties
        [ObservableProperty]
        private List<string> logVerbosityOptions = new() { "Minimal", "Normal", "Detailed", "Debug" };

        [ObservableProperty]
        private string selectedLogVerbosity = "Detailed"; // Default from sprint plan

        [ObservableProperty]
        private int apiPort = 5111;

        [ObservableProperty]
        private int startupDelaySeconds = 5;

        [ObservableProperty]
        private int fileProcessingDelayMs = 500;

        [ObservableProperty]
        private bool settingsChanged = false;

        [ObservableProperty]
        private string estimatedLogSize = "~1.75 MB/day";

        [ObservableProperty]
        private string filesProcessedToday = "0";

        [ObservableProperty]
        private string estimatedDailyLogSize = "0 KB";

        // Store original values to detect changes
        private string _originalLogVerbosity = "Detailed";
        private int _originalApiPort = 5111;
        private int _originalStartupDelaySeconds = 5;
        private int _originalFileProcessingDelayMs = 500;

        public ServiceControlViewModel(IServiceManager serviceManager, IConfigurationService configurationService)
        {
            _serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
            _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

            // Start monitoring
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            // Check if running as admin
            RequiresElevation = !_serviceManager.IsRunningAsAdministrator();

            // Load service settings
            await LoadServiceSettingsAsync();

            // Initial status check
            await RefreshStatusAsync();

            // Start periodic updates
            _statusTimer = new Timer(
                async _ => await RefreshStatusAsync(),
                null,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2));
        }

        private async Task LoadServiceSettingsAsync()
        {
            try
            {
                // Load the configuration using the correct type
                var settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettingsV2>();

                if (settings?.Service != null)
                {
                    // API Port
                    ApiPort = settings.Service.ApiPort;
                    _originalApiPort = settings.Service.ApiPort;

                    // Startup Delay
                    StartupDelaySeconds = settings.Service.StartupDelaySeconds;
                    _originalStartupDelaySeconds = settings.Service.StartupDelaySeconds;

                    // File Processing Delay
                    FileProcessingDelayMs = settings.Service.FileProcessingDelayMs;
                    _originalFileProcessingDelayMs = settings.Service.FileProcessingDelayMs;

                    // Log Verbosity - convert enum to string
                    SelectedLogVerbosity = settings.Service.LogVerbosity.ToString();
                    _originalLogVerbosity = settings.Service.LogVerbosity.ToString();
                }
                UpdateEstimatedLogSize();
                CheckForChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load service settings: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task SaveServiceSettingsAsync()
        {
            IsLoading = true;

            try
            {
                // Load current settings
                var settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettingsV2>()
                               ?? new CamBridgeSettingsV2();

                // Ensure Service section exists
                settings.Service ??= new ServiceSettings();

                // Update service settings
                settings.Service.ApiPort = ApiPort;
                settings.Service.StartupDelaySeconds = StartupDelaySeconds;
                settings.Service.FileProcessingDelayMs = FileProcessingDelayMs;

                // Convert string back to enum
                if (Enum.TryParse<LogVerbosity>(SelectedLogVerbosity, out var verbosity))
                {
                    settings.Service.LogVerbosity = verbosity;
                }
                else
                {
                    settings.Service.LogVerbosity = LogVerbosity.Detailed; // Default
                }

                // Save back
                await _configurationService.SaveConfigurationAsync(settings);

                // Update original values
                _originalApiPort = ApiPort;
                _originalStartupDelaySeconds = StartupDelaySeconds;
                _originalFileProcessingDelayMs = FileProcessingDelayMs;
                _originalLogVerbosity = SelectedLogVerbosity;

                CheckForChanges();

                MessageBox.Show(
                    "Service settings have been saved successfully.\n\n" +
                    "Please restart the CamBridge Service for the changes to take effect.",
                    "Settings Saved",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // If service is running, suggest restart
                if (ServiceStatus == ServiceStatus.Running)
                {
                    var result = MessageBox.Show(
                        "The service is currently running. Would you like to restart it now to apply the new settings?",
                        "Restart Service?",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        await RestartServiceAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save service settings: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSelectedLogVerbosityChanged(string value)
        {
            UpdateEstimatedLogSize();
            CheckForChanges();
        }

        partial void OnApiPortChanged(int value)
        {
            CheckForChanges();
        }

        partial void OnStartupDelaySecondsChanged(int value)
        {
            CheckForChanges();
        }

        partial void OnFileProcessingDelayMsChanged(int value)
        {
            CheckForChanges();
        }

        private void UpdateEstimatedLogSize()
        {
            EstimatedLogSize = SelectedLogVerbosity switch
            {
                "Minimal" => "~150 KB/day",
                "Normal" => "~750 KB/day",
                "Detailed" => "~1.75 MB/day",
                "Debug" => "~3.5 MB/day",
                _ => "Unknown"
            };
        }

        private void CheckForChanges()
        {
            SettingsChanged =
                SelectedLogVerbosity != _originalLogVerbosity ||
                ApiPort != _originalApiPort ||
                StartupDelaySeconds != _originalStartupDelaySeconds ||
                FileProcessingDelayMs != _originalFileProcessingDelayMs;
        }

        [RelayCommand]
        private async Task RefreshStatusAsync()
        {
            try
            {
                // Check if service is installed
                IsServiceInstalled = await _serviceManager.IsServiceInstalledAsync();

                if (!IsServiceInstalled)
                {
                    ServiceStatus = ServiceStatus.Unknown;
                    StatusText = "Service Not Installed";
                    StatusColor = "Red";
                    UpdateButtons();
                    return;
                }

                // Get current status
                ServiceStatus = await _serviceManager.GetServiceStatusAsync();
                UpdateStatusDisplay();
                UpdateButtons();

                // Get uptime if running
                if (ServiceStatus == ServiceStatus.Running)
                {
                    var startTime = await _serviceManager.GetServiceStartTimeAsync();
                    if (startTime.HasValue)
                    {
                        var uptimeSpan = DateTime.Now - startTime.Value;
                        Uptime = FormatUptime(uptimeSpan);
                    }

                    // Update statistics (mock for now - could query API later)
                    await UpdateStatisticsAsync();
                }
                else
                {
                    Uptime = null;
                    FilesProcessedToday = "0";
                    EstimatedDailyLogSize = "0 KB";
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
                StatusColor = "Red";
            }
        }

        private async Task UpdateStatisticsAsync()
        {
            try
            {
                // TODO: Query the service API for real statistics
                // For now, use mock data
                var random = new Random();
                var filesProcessed = random.Next(100, 500);
                FilesProcessedToday = filesProcessed.ToString();

                // Calculate estimated log size based on files and verbosity
                var bytesPerFile = SelectedLogVerbosity switch
                {
                    "Minimal" => 300,
                    "Normal" => 1500,
                    "Detailed" => 3500,
                    "Debug" => 7000,
                    _ => 3500
                };

                var totalBytes = filesProcessed * bytesPerFile;
                EstimatedDailyLogSize = FormatFileSize(totalBytes);
            }
            catch
            {
                // Ignore statistics errors
            }
        }

        private string FormatFileSize(long bytes)
        {
            if (bytes < 1024)
                return $"{bytes} B";
            else if (bytes < 1024 * 1024)
                return $"{bytes / 1024.0:F1} KB";
            else if (bytes < 1024 * 1024 * 1024)
                return $"{bytes / (1024.0 * 1024.0):F1} MB";
            else
                return $"{bytes / (1024.0 * 1024.0 * 1024.0):F1} GB";
        }

        [RelayCommand]
        private async Task InstallServiceAsync()
        {
            if (RequiresElevation)
            {
                var result = MessageBox.Show(
                    "Installing the service requires administrator privileges. Restart the application as administrator?",
                    "Administrator Required",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    RestartAsAdministrator();
                }
                return;
            }

            IsLoading = true;

            try
            {
                StatusText = "Installing service...";
                StatusColor = "Orange";

                var success = await _serviceManager.InstallServiceAsync();

                if (success)
                {
                    StatusText = "Service installed successfully";
                    StatusColor = "Green";

                    // Wait a moment for the service to be fully registered
                    await Task.Delay(1000);

                    // Refresh status to update UI
                    await RefreshStatusAsync();

                    MessageBox.Show(
                        "CamBridge Service has been installed successfully!\n\n" +
                        "You can now start the service using the Start button.",
                        "Installation Complete",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    StatusText = "Failed to install service";
                    StatusColor = "Red";

                    MessageBox.Show(
                        "Failed to install the CamBridge Service.\n\n" +
                        "Please check the Event Viewer for more details.",
                        "Installation Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                StatusText = "Installation error";
                StatusColor = "Red";

                MessageBox.Show(
                    $"An error occurred while installing the service:\n\n{ex.Message}",
                    "Installation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task UninstallServiceAsync()
        {
            if (RequiresElevation)
            {
                var result = MessageBox.Show(
                    "Uninstalling the service requires administrator privileges. Restart the application as administrator?",
                    "Administrator Required",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    RestartAsAdministrator();
                }
                return;
            }

            var confirmResult = MessageBox.Show(
                "Are you sure you want to uninstall the CamBridge Service?\n\n" +
                "This will permanently remove the service from your system.",
                "Confirm Uninstall",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmResult != MessageBoxResult.Yes)
                return;

            IsLoading = true;

            try
            {
                // Stop service first if running
                if (ServiceStatus == ServiceStatus.Running)
                {
                    StatusText = "Stopping service before uninstall...";
                    StatusColor = "Orange";
                    await _serviceManager.StopServiceAsync();
                    await Task.Delay(2000); // Wait for service to stop
                }

                StatusText = "Uninstalling service...";
                StatusColor = "Orange";

                var success = await _serviceManager.UninstallServiceAsync();

                if (success)
                {
                    StatusText = "Service uninstalled";
                    StatusColor = "Gray";

                    // Wait a moment for the service to be fully removed
                    await Task.Delay(1000);

                    // Refresh status to update UI
                    await RefreshStatusAsync();

                    MessageBox.Show(
                        "CamBridge Service has been uninstalled successfully.",
                        "Uninstall Complete",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    StatusText = "Failed to uninstall service";
                    StatusColor = "Red";

                    MessageBox.Show(
                        "Failed to uninstall the CamBridge Service.\n\n" +
                        "Please check the Event Viewer for more details.",
                        "Uninstall Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                StatusText = "Uninstall error";
                StatusColor = "Red";

                MessageBox.Show(
                    $"An error occurred while uninstalling the service:\n\n{ex.Message}",
                    "Uninstall Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task StartServiceAsync()
        {
            if (RequiresElevation)
            {
                var result = MessageBox.Show(
                    "Starting the service requires administrator privileges. Restart the application as administrator?",
                    "Administrator Required",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    RestartAsAdministrator();
                }
                return;
            }

            IsLoading = true;
            CanStart = false;

            try
            {
                StatusText = "Starting service...";
                StatusColor = "Orange";

                var success = await _serviceManager.StartServiceAsync();

                if (success)
                {
                    StatusText = "Service started successfully";
                    StatusColor = "Green";
                }
                else
                {
                    StatusText = "Failed to start service";
                    StatusColor = "Red";
                }

                await RefreshStatusAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task StopServiceAsync()
        {
            if (RequiresElevation)
            {
                var result = MessageBox.Show(
                    "Stopping the service requires administrator privileges. Restart the application as administrator?",
                    "Administrator Required",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    RestartAsAdministrator();
                }
                return;
            }

            var confirmResult = MessageBox.Show(
                "Are you sure you want to stop the CamBridge Service? This will halt all file processing.",
                "Confirm Stop",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmResult != MessageBoxResult.Yes)
                return;

            IsLoading = true;
            CanStop = false;

            try
            {
                StatusText = "Stopping service...";
                StatusColor = "Orange";

                var success = await _serviceManager.StopServiceAsync();

                if (success)
                {
                    StatusText = "Service stopped";
                    StatusColor = "Gray";
                }
                else
                {
                    StatusText = "Failed to stop service";
                    StatusColor = "Red";
                }

                await RefreshStatusAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task RestartServiceAsync()
        {
            if (RequiresElevation)
            {
                var result = MessageBox.Show(
                    "Restarting the service requires administrator privileges. Restart the application as administrator?",
                    "Administrator Required",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    RestartAsAdministrator();
                }
                return;
            }

            IsLoading = true;
            CanRestart = false;

            try
            {
                StatusText = "Restarting service...";
                StatusColor = "Orange";

                var success = await _serviceManager.RestartServiceAsync();

                if (success)
                {
                    StatusText = "Service restarted successfully";
                    StatusColor = "Green";
                }
                else
                {
                    StatusText = "Failed to restart service";
                    StatusColor = "Red";
                }

                await RefreshStatusAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void OpenServices()
        {
            try
            {
                var processInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "services.msc",
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Services: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void OpenEventViewer()
        {
            try
            {
                var processInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "eventvwr.msc",
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Event Viewer: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStatusDisplay()
        {
            (StatusText, StatusColor) = ServiceStatus switch
            {
                ServiceStatus.Running => ("Running", "Green"),
                ServiceStatus.Stopped => ("Stopped", "Gray"),
                ServiceStatus.Starting => ("Starting...", "Orange"),
                ServiceStatus.Stopping => ("Stopping...", "Orange"),
                _ => ("Unknown", "Red")
            };
        }

        private void UpdateButtons()
        {
            CanStart = IsServiceInstalled && ServiceStatus == ServiceStatus.Stopped && !IsLoading;
            CanStop = IsServiceInstalled && ServiceStatus == ServiceStatus.Running && !IsLoading;
            CanRestart = IsServiceInstalled && ServiceStatus == ServiceStatus.Running && !IsLoading;
        }

        private string FormatUptime(TimeSpan uptime)
        {
            if (uptime.TotalDays >= 1)
                return $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m";
            else if (uptime.TotalHours >= 1)
                return $"{uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
            else
                return $"{uptime.Minutes}m {uptime.Seconds}s";
        }

        private void RestartAsAdministrator()
        {
            try
            {
                var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                var fileName = currentProcess.MainModule?.FileName;

                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Could not determine application path", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = fileName,
                    UseShellExecute = true,
                    Verb = "runas"
                };

                System.Diagnostics.Process.Start(startInfo);
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to restart as administrator: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Cleanup()
        {
            _statusTimer?.Dispose();
        }
    }
}
