// src/CamBridge.Config/Services/ServiceManager.cs
using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CamBridge.Config.Services
{
    [SupportedOSPlatform("windows")]
    public class ServiceManager : IServiceManager
    {
        private readonly ILogger<ServiceManager> _logger;
        private const string SERVICE_NAME = "CamBridgeService";
        private const int TIMEOUT_SECONDS = 30;

        public ServiceManager(ILogger<ServiceManager> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> IsServiceInstalledAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var controller = new ServiceController(SERVICE_NAME);
                    _ = controller.Status; // Will throw if not found
                    return true;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            });
        }

        public async Task<ServiceStatus> GetServiceStatusAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var controller = new ServiceController(SERVICE_NAME);
                    return controller.Status switch
                    {
                        ServiceControllerStatus.Running => ServiceStatus.Running,
                        ServiceControllerStatus.Stopped => ServiceStatus.Stopped,
                        ServiceControllerStatus.StartPending => ServiceStatus.Starting,
                        ServiceControllerStatus.StopPending => ServiceStatus.Stopping,
                        _ => ServiceStatus.Unknown
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to get service status");
                    return ServiceStatus.Unknown;
                }
            });
        }

        public async Task<bool> StartServiceAsync()
        {
            if (!IsRunningAsAdministrator())
            {
                _logger.LogWarning("Start service requires administrator privileges");
                return await RunElevatedAsync("start");
            }

            return await Task.Run(async () =>
            {
                try
                {
                    using var controller = new ServiceController(SERVICE_NAME);

                    if (controller.Status == ServiceControllerStatus.Running)
                    {
                        _logger.LogInformation("Service is already running");
                        return true;
                    }

                    controller.Start();
                    await WaitForStatusAsync(controller, ServiceControllerStatus.Running);

                    _logger.LogInformation("Service started successfully");
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to start service");
                    return false;
                }
            });
        }

        public async Task<bool> StopServiceAsync()
        {
            if (!IsRunningAsAdministrator())
            {
                _logger.LogWarning("Stop service requires administrator privileges");
                return await RunElevatedAsync("stop");
            }

            return await Task.Run(async () =>
            {
                try
                {
                    using var controller = new ServiceController(SERVICE_NAME);

                    if (controller.Status == ServiceControllerStatus.Stopped)
                    {
                        _logger.LogInformation("Service is already stopped");
                        return true;
                    }

                    controller.Stop();
                    await WaitForStatusAsync(controller, ServiceControllerStatus.Stopped);

                    _logger.LogInformation("Service stopped successfully");
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to stop service");
                    return false;
                }
            });
        }

        public async Task<bool> RestartServiceAsync()
        {
            _logger.LogInformation("Restarting service");

            if (!await StopServiceAsync())
            {
                return false;
            }

            // Wait a moment between stop and start
            await Task.Delay(1000);

            return await StartServiceAsync();
        }

        public async Task<DateTime?> GetServiceStartTimeAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Get process start time via WMI
                    using var process = GetServiceProcess();
                    return process?.StartTime;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to get service start time");
                    return null;
                }
            });
        }

        public bool IsRunningAsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private async Task<bool> RunElevatedAsync(string action)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = $"{action} {SERVICE_NAME}",
                    UseShellExecute = true,
                    Verb = "runas", // Run as administrator
                    CreateNoWindow = true
                };

                var process = Process.Start(startInfo);
                if (process != null)
                {
                    await process.WaitForExitAsync();
                    return process.ExitCode == 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to run elevated command");
                return false;
            }
        }

        private async Task WaitForStatusAsync(ServiceController controller, ServiceControllerStatus desiredStatus)
        {
            var timeout = TimeSpan.FromSeconds(TIMEOUT_SECONDS);
            await Task.Run(() => controller.WaitForStatus(desiredStatus, timeout));
        }

        private Process? GetServiceProcess()
        {
            var processes = Process.GetProcessesByName("CamBridge.Service");
            return processes.Length > 0 ? processes[0] : null;
        }
    }
}
