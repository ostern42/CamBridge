// CamBridge.ServiceDebug/Program.cs
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;

namespace CamBridge.ServiceDebug
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== CamBridge Service Installation Debug Tool v0.5.9 ===");
            Console.WriteLine($"© 2025 Claude's Improbably Reliable Software Solutions\n");

            // Check admin rights
            var isAdmin = IsRunningAsAdministrator();
            Console.WriteLine($"Running as Administrator: {(isAdmin ? "✓ YES" : "✗ NO")}");
            if (!isAdmin)
            {
                Console.WriteLine("WARNING: Service installation requires administrator privileges!\n");
            }

            // Find service executable
            Console.WriteLine("\n=== Searching for CamBridge.Service.exe ===");
            var serviceExePath = FindServiceExecutable();
            
            if (string.IsNullOrEmpty(serviceExePath))
            {
                Console.WriteLine("\n✗ ERROR: Could not find CamBridge.Service.exe!");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\n✓ Found service at: {serviceExePath}");
            Console.WriteLine($"  File size: {new FileInfo(serviceExePath).Length:N0} bytes");
            Console.WriteLine($"  Created: {File.GetCreationTime(serviceExePath):yyyy-MM-dd HH:mm:ss}");

            // Check if service is already installed
            Console.WriteLine("\n=== Checking Service Status ===");
            var isInstalled = IsServiceInstalled();
            Console.WriteLine($"Service installed: {(isInstalled ? "✓ YES" : "✗ NO")}");

            if (isInstalled)
            {
                var status = GetServiceStatus();
                Console.WriteLine($"Service status: {status}");
                
                Console.WriteLine("\nService is already installed. Do you want to uninstall it? (y/n)");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine("\n\nUninstalling service...");
                    UninstallService();
                }
                else
                {
                    Console.WriteLine("\n\nPress any key to exit...");
                    Console.ReadKey();
                    return;
                }
            }

            // Try to install service
            Console.WriteLine("\n=== Installing Service ===");
            Console.WriteLine("Do you want to install the service? (y/n)");
            if (Console.ReadKey().Key != ConsoleKey.Y)
            {
                Console.WriteLine("\n\nCancelled. Press any key to exit...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\n");
            InstallService(serviceExePath);

            Console.WriteLine("\n\nPress any key to exit...");
            Console.ReadKey();
        }

        static bool IsRunningAsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static bool IsServiceInstalled()
        {
            try
            {
                using var controller = ServiceController.GetServices()
                    .FirstOrDefault(s => s.ServiceName == "CamBridgeService");
                return controller != null;
            }
            catch
            {
                return false;
            }
        }

        static string GetServiceStatus()
        {
            try
            {
                using var controller = new ServiceController("CamBridgeService");
                return controller.Status.ToString();
            }
            catch
            {
                return "Unknown";
            }
        }

        static string? FindServiceExecutable()
        {
            var searchPaths = new[]
            {
                // Relative to debug tool location
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "src", "CamBridge.Service", "bin", "Debug", "net8.0", "win-x64", "CamBridge.Service.exe"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "src", "CamBridge.Service", "bin", "x64", "Debug", "net8.0", "win-x64", "CamBridge.Service.exe"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "src", "CamBridge.Service", "bin", "x64", "Debug", "net8.0-windows", "win-x64", "CamBridge.Service.exe"),
                
                // Other common locations
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CamBridge.Service.exe"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "CamBridge.Service", "CamBridge.Service.exe"),
            };

            Console.WriteLine("Searching in the following locations:");
            foreach (var path in searchPaths)
            {
                try
                {
                    var normalizedPath = Path.GetFullPath(path);
                    var exists = File.Exists(normalizedPath);
                    Console.WriteLine($"  {(exists ? "✓" : "✗")} {normalizedPath}");
                    
                    if (exists)
                    {
                        return normalizedPath;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ✗ Error checking path: {ex.Message}");
                }
            }

            // Try to find solution directory
            var solutionDir = FindSolutionDirectory();
            if (!string.IsNullOrEmpty(solutionDir))
            {
                Console.WriteLine($"\nFound solution directory: {solutionDir}");
                var additionalPaths = new[]
                {
                    Path.Combine(solutionDir, "src", "CamBridge.Service", "bin", "Debug", "net8.0", "win-x64", "CamBridge.Service.exe"),
                    Path.Combine(solutionDir, "src", "CamBridge.Service", "bin", "x64", "Debug", "net8.0", "win-x64", "CamBridge.Service.exe"),
                    Path.Combine(solutionDir, "src", "CamBridge.Service", "bin", "x64", "Debug", "net8.0-windows", "win-x64", "CamBridge.Service.exe"),
                };

                foreach (var path in additionalPaths)
                {
                    var exists = File.Exists(path);
                    Console.WriteLine($"  {(exists ? "✓" : "✗")} {path}");
                    
                    if (exists)
                    {
                        return path;
                    }
                }
            }

            return null;
        }

        static string? FindSolutionDirectory()
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

        static void InstallService(string serviceExePath)
        {
            try
            {
                var args = $"create CamBridgeService binPath= \"{serviceExePath}\" DisplayName= \"CamBridge Image Processing Service\" start= auto";
                Console.WriteLine($"Executing: sc.exe {args}");

                var processInfo = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processInfo);
                if (process == null)
                {
                    Console.WriteLine("✗ ERROR: Failed to start sc.exe");
                    return;
                }

                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                Console.WriteLine($"\nExit code: {process.ExitCode}");
                
                if (!string.IsNullOrEmpty(output))
                {
                    Console.WriteLine($"Output: {output}");
                }
                
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Error: {error}");
                }

                if (process.ExitCode == 0)
                {
                    Console.WriteLine("\n✓ Service installed successfully!");
                    
                    // Set description
                    var descArgs = "description CamBridgeService \"Monitors folders for JPEG images and converts them to DICOM format.\"";
                    var descProcess = Process.Start("sc.exe", descArgs);
                    descProcess?.WaitForExit();
                }
                else
                {
                    Console.WriteLine("\n✗ Service installation failed!");
                    
                    // Common error codes
                    switch (process.ExitCode)
                    {
                        case 5:
                            Console.WriteLine("ERROR: Access denied. Run as administrator!");
                            break;
                        case 1073:
                            Console.WriteLine("ERROR: Service already exists!");
                            break;
                        default:
                            Console.WriteLine($"ERROR: Unknown error code {process.ExitCode}");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ ERROR: {ex.Message}");
            }
        }

        static void UninstallService()
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = "delete CamBridgeService",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processInfo);
                if (process == null)
                {
                    Console.WriteLine("✗ ERROR: Failed to start sc.exe");
                    return;
                }

                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine("✓ Service uninstalled successfully!");
                }
                else
                {
                    Console.WriteLine($"✗ Service uninstallation failed! Exit code: {process.ExitCode}");
                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine($"Error: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ ERROR: {ex.Message}");
            }
        }
    }
}