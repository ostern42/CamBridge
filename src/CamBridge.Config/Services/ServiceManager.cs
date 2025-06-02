// src/CamBridge.Config/Services/ServiceManager.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Implementation of service management functionality
    /// </summary>
    public class ServiceManager : IServiceManager
    {
        private const string ServiceName = "CamBridgeService";
        private const string ServiceDisplayName = "CamBridge Image Processing Service";
        private const string ServiceDescription = "Monitors folders for JPEG images and converts them to DICOM format.";

        public bool IsRunningAsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public async Task<bool> IsServiceInstalledAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var controller = ServiceController.GetServices()
                        .FirstOrDefault(s => s.ServiceName == ServiceName);
                    return controller != null;
                }
                catch
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
                    using var controller = new ServiceController(ServiceName);
                    controller.Refresh();

                    return controller.Status switch
                    {
                        ServiceControllerStatus.Running => ServiceStatus.Running,
                        ServiceControllerStatus.Stopped => ServiceStatus.Stopped,
                        ServiceControllerStatus.StartPending => ServiceStatus.Starting,
                        ServiceControllerStatus.StopPending => ServiceStatus.Stopping,
                        _ => ServiceStatus.Unknown
                    };
                }
                catch
                {
                    return ServiceStatus.Unknown;
                }
            });
        }

        public async Task<DateTime?> GetServiceStartTimeAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Try to get start time from Windows Management
                    using var process = Process.GetProcessesByName("CamBridge.Service").FirstOrDefault();
                    return process?.StartTime;
                }
                catch
                {
                    return null;
                }
            });
        }

        public async Task<bool> InstallServiceAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Find the service executable
                    var serviceExePath = FindServiceExecutable();
                    if (string.IsNullOrEmpty(serviceExePath))
                    {
                        // Log all searched paths for debugging
                        var searchPaths = GetSearchPaths();
                        var pathList = string.Join("\n", searchPaths);

                        // Create a detailed error message
                        var errorMsg = new StringBuilder();
                        errorMsg.AppendLine("Could not find CamBridge.Service.exe");
                        errorMsg.AppendLine("\nSearched in:");
                        foreach (var path in searchPaths)
                        {
                            var exists = File.Exists(path);
                            errorMsg.AppendLine($"  {(exists ? "✓" : "✗")} {path}");
                        }

                        throw new FileNotFoundException(errorMsg.ToString());
                    }

                    // Log the found path
                    Debug.WriteLine($"Found service executable at: {serviceExePath}");

                    // Use sc.exe to install the service
                    var processInfo = new ProcessStartInfo
                    {
                        FileName = "sc.exe",
                        Arguments = $"create {ServiceName} binPath= \"{serviceExePath}\" DisplayName= \"{ServiceDisplayName}\" start= auto",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        Verb = "runas"
                    };

                    using var process = Process.Start(processInfo);
                    if (process == null)
                    {
                        throw new InvalidOperationException("Failed to start sc.exe process");
                    }

                    var output = process.StandardOutput.ReadToEnd();
                    var error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    Debug.WriteLine($"sc.exe output: {output}");
                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.WriteLine($"sc.exe error: {error}");
                    }

                    if (process.ExitCode == 0)
                    {
                        // Set service description
                        var descProcessInfo = new ProcessStartInfo
                        {
                            FileName = "sc.exe",
                            Arguments = $"description {ServiceName} \"{ServiceDescription}\"",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            Verb = "runas"
                        };

                        using var descProcess = Process.Start(descProcessInfo);
                        descProcess?.WaitForExit();

                        // Configure recovery options
                        ConfigureServiceRecovery();

                        return true;
                    }
                    else
                    {
                        var errorMessage = $"sc.exe failed with exit code {process.ExitCode}";
                        if (!string.IsNullOrEmpty(error))
                        {
                            errorMessage += $"\nError: {error}";
                        }
                        if (!string.IsNullOrEmpty(output) && output.Contains("error", StringComparison.OrdinalIgnoreCase))
                        {
                            errorMessage += $"\nOutput: {output}";
                        }

                        throw new InvalidOperationException(errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Service installation failed: {ex}");
                    throw;
                }
            });
        }

        public async Task<bool> UninstallServiceAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Use sc.exe to delete the service
                    var processInfo = new ProcessStartInfo
                    {
                        FileName = "sc.exe",
                        Arguments = $"delete {ServiceName}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        Verb = "runas"
                    };

                    using var process = Process.Start(processInfo);
                    process?.WaitForExit();

                    return process?.ExitCode == 0;
                }
                catch
                {
                    return false;
                }
            });
        }

        public async Task<bool> StartServiceAsync()
        {
            try
            {
                using var controller = new ServiceController(ServiceName);

                if (controller.Status == ServiceControllerStatus.Running)
                    return true;

                controller.Start();
                await WaitForServiceStatusAsync(controller, ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));

                return controller.Status == ServiceControllerStatus.Running;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> StopServiceAsync()
        {
            try
            {
                using var controller = new ServiceController(ServiceName);

                if (controller.Status == ServiceControllerStatus.Stopped)
                    return true;

                controller.Stop();
                await WaitForServiceStatusAsync(controller, ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));

                return controller.Status == ServiceControllerStatus.Stopped;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RestartServiceAsync()
        {
            try
            {
                // Stop service
                var stopResult = await StopServiceAsync();
                if (!stopResult)
                    return false;

                // Wait a bit before starting
                await Task.Delay(1000);

                // Start service
                return await StartServiceAsync();
            }
            catch
            {
                return false;
            }
        }

        private async Task WaitForServiceStatusAsync(ServiceController controller, ServiceControllerStatus desiredStatus, TimeSpan timeout)
        {
            await Task.Run(() =>
            {
                try
                {
                    controller.WaitForStatus(desiredStatus, timeout);
                }
                catch (System.ServiceProcess.TimeoutException)
                {
                    // Status change timed out
                }
            });
        }

        private string? FindServiceExecutable()
        {
            var possiblePaths = GetSearchPaths();

            foreach (var path in possiblePaths)
            {
                try
                {
                    var normalizedPath = Path.GetFullPath(path);
                    Debug.WriteLine($"Checking: {normalizedPath}");
                    if (File.Exists(normalizedPath))
                    {
                        Debug.WriteLine($"Found service at: {normalizedPath}");
                        return normalizedPath;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error checking path {path}: {ex.Message}");
                }
            }

            return null;
        }

        private string[] GetSearchPaths()
        {
            var paths = new List<string>
            {
                // Same directory as config app
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "CamBridge.Service.exe"),
                // Parent directory (if config is in subfolder)
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "..", "CamBridge.Service.exe"),
                // Debug/Release output paths
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CamBridge.Service.exe"),
            };

            // Find solution directory first for more accurate paths
            var solutionDir = FindSolutionDirectory();
            if (!string.IsNullOrEmpty(solutionDir))
            {
                // Add all known locations from PROJECT_WISDOM
                paths.AddRange(new[]
                {
                    // Standard debug/release paths
                    Path.Combine(solutionDir, "src", "CamBridge.Service", "bin", "Debug", "net8.0", "CamBridge.Service.exe"),
                    Path.Combine(solutionDir, "src", "CamBridge.Service", "bin", "Release", "net8.0", "CamBridge.Service.exe"),
                    
                    // win-x64 specific paths (from PROJECT_WISDOM)
                    Path.Combine(solutionDir, "src", "CamBridge.Service", "bin", "Debug", "net8.0", "win-x64", "CamBridge.Service.exe"),
                    Path.Combine(solutionDir, "src", "CamBridge.Service", "bin", "Release", "net8.0", "win-x64", "CamBridge.Service.exe"),
                    
                    // x64 configuration paths
                    Path.Combine(solutionDir, "src", "CamBridge.Service", "bin", "x64", "Debug", "net8.0", "CamBridge.Service.exe"),
                    Path.Combine(solutionDir, "src", "CamBridge.Service", "bin", "x64", "Release", "net8.0", "CamBridge.Service.exe"),
                    Path.Combine(solutionDir, "src", "CamBridge.Service", "bin", "x64", "Debug", "net8.0", "win-x64", "CamBridge.Service.exe"),
                    Path.Combine(solutionDir, "src", "CamBridge.Service", "bin", "x64", "Release", "net8.0", "win-x64", "CamBridge.Service.exe"),
                    
                    // net8.0-windows paths (this was missing!)
                    Path.Combine(solutionDir, "src", "CamBridge.Service", "bin", "Debug", "net8.0-windows", "CamBridge.Service.exe"),
                    Path.Combine(solutionDir, "src", "CamBridge.Service", "bin", "Release", "net8.0-windows", "CamBridge.Service.exe"),
                    Path.Combine(solutionDir, "src", "CamBridge.Service", "bin", "Debug", "net8.0-windows", "win-x64", "CamBridge.Service.exe"),
                    Path.Combine(solutionDir, "src", "CamBridge.Service", "bin", "Release", "net8.0-windows", "win-x64", "CamBridge.Service.exe"),
                    Path.Combine(solutionDir, "src", "CamBridge.Service", "bin", "x64", "Debug", "net8.0-windows", "win-x64", "CamBridge.Service.exe"),
                    Path.Combine(solutionDir, "src", "CamBridge.Service", "bin", "x64", "Release", "net8.0-windows", "win-x64", "CamBridge.Service.exe"),
                    
                    // Published output
                    Path.Combine(solutionDir, "publish", "CamBridge.Service.exe")
                });
            }
            else
            {
                // Fallback paths relative to current directory
                paths.AddRange(new[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "CamBridge.Service", "bin", "Debug", "net8.0", "CamBridge.Service.exe"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "CamBridge.Service", "bin", "Release", "net8.0", "CamBridge.Service.exe"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "CamBridge.Service", "bin", "Debug", "net8.0", "win-x64", "CamBridge.Service.exe"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "CamBridge.Service", "bin", "x64", "Debug", "net8.0", "win-x64", "CamBridge.Service.exe"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "CamBridge.Service", "bin", "x64", "Debug", "net8.0-windows", "win-x64", "CamBridge.Service.exe"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "publish", "CamBridge.Service.exe")
                });
            }

            return paths.Distinct().ToArray();
        }

        private string? FindSolutionDirectory()
        {
            var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            while (directory != null && directory.Parent != null)
            {
                if (directory.GetFiles("CamBridge.sln").Any())
                {
                    return directory.FullName;
                }
                directory = directory.Parent;
            }

            return null;
        }

        private void ConfigureServiceRecovery()
        {
            try
            {
                // Configure service to restart on failure
                var processInfo = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = $"failure {ServiceName} reset= 86400 actions= restart/60000/restart/60000/restart/60000",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using var process = Process.Start(processInfo);
                process?.WaitForExit();
            }
            catch
            {
                // Recovery configuration is optional, so we don't fail the installation
            }
        }
    }
}
