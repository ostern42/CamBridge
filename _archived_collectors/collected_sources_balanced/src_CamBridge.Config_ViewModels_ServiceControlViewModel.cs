// src/CamBridge.Config/ViewModels/ServiceControlViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CamBridge.Config.Services;

namespace CamBridge.Config.ViewModels
{
    public partial class ServiceControlViewModel : ViewModelBase
    {
        private readonly IServiceManager _serviceManager;
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

        public ServiceControlViewModel(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));

            // Start monitoring
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            // Check if running as admin
            RequiresElevation = !_serviceManager.IsRunningAsAdministrator();

            // Initial status check
            await RefreshStatusAsync();

            // Start periodic updates
            _statusTimer = new Timer(
                async _ => await RefreshStatusAsync(),
                null,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2));
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
                }
                else
                {
                    Uptime = null;
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
                StatusColor = "Red";
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
                System.Diagnostics.Process.Start("services.msc");
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
                System.Diagnostics.Process.Start("eventvwr.msc");
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
