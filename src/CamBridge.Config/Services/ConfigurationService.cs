// src\CamBridge.Config\Services\ConfigurationService.cs
// Version: 0.6.8
// Description: Robust configuration service with correct property mappings
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CamBridge.Core;
using Microsoft.Extensions.Logging;

namespace CamBridge.Config.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly ILogger<ConfigurationService>? _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly List<string> _searchPaths;

        public ConfigurationService(ILogger<ConfigurationService>? logger = null)
        {
            _logger = logger;
            _searchPaths = new List<string>();

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };

            // Initialize search paths - ROBUST for all build configurations!
            InitializeSearchPaths();

            Debug.WriteLine("=== ConfigurationService INIT ===");
            Debug.WriteLine($"Search paths ({_searchPaths.Count}):");
            foreach (var path in _searchPaths)
            {
                Debug.WriteLine($"  - {path}");
            }
        }

        private void InitializeSearchPaths()
        {
            // 1. AppData (Primary location - always works)
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CamBridge");
            _searchPaths.Add(appDataPath);

            // 2. Next to executable (works for all builds)
            var exePath = AppContext.BaseDirectory;
            _searchPaths.Add(exePath);

            // 3. Go up from bin folders (x64/x86 builds)
            var currentDir = new DirectoryInfo(exePath);
            while (currentDir != null && currentDir.Name != "CamBridge.Config")
            {
                if (currentDir.Parent != null)
                {
                    currentDir = currentDir.Parent;
                    _searchPaths.Add(currentDir.FullName);
                }
                else
                {
                    break;
                }
            }

            // 4. Service directory (if running from Config UI)
            var serviceDir = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "..", "..", "src", "CamBridge.Service");
            if (Directory.Exists(serviceDir))
            {
                _searchPaths.Add(Path.GetFullPath(serviceDir));
            }

            // 5. Common local paths
            _searchPaths.Add(@"C:\CamBridge\Config");
            _searchPaths.Add(Directory.GetCurrentDirectory());

            // Remove duplicates
            var uniquePaths = _searchPaths.Distinct().ToList();
            _searchPaths.Clear();
            _searchPaths.AddRange(uniquePaths);

            // Ensure AppData directory exists
            Directory.CreateDirectory(appDataPath);
        }

        public async Task<T?> LoadConfigurationAsync<T>() where T : class
        {
            Debug.WriteLine($"\n=== LOADING CONFIG: {typeof(T).Name} ===");
            Debug.WriteLine($"Searching for: appsettings.json");

            try
            {
                // Try each search path
                foreach (var searchPath in _searchPaths)
                {
                    var configPath = Path.Combine(searchPath, "appsettings.json");
                    Debug.WriteLine($"Checking: {configPath}");

                    if (File.Exists(configPath))
                    {
                        Debug.WriteLine($"✓ FOUND at: {configPath}");

                        try
                        {
                            var json = await File.ReadAllTextAsync(configPath);

                            // For CamBridgeSettingsV2, we need to handle the Service's JSON format
                            if (typeof(T) == typeof(CamBridgeSettingsV2))
                            {
                                var settings = ParseServiceJson(json);
                                if (settings != null)
                                {
                                    Debug.WriteLine($"✓ Loaded {settings.Pipelines.Count} pipelines from service config");
                                    return settings as T;
                                }
                            }
                            else
                            {
                                var result = JsonSerializer.Deserialize<T>(json, _jsonOptions);
                                if (result != null)
                                {
                                    Debug.WriteLine("✓ Config loaded successfully");
                                    return result;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"⚠ Failed to parse {configPath}: {ex.Message}");
                            // Continue to next path
                        }
                    }
                }

                Debug.WriteLine("⚠ No valid config found in any search path");

                // Create and save default configuration
                if (typeof(T) == typeof(CamBridgeSettingsV2))
                {
                    Debug.WriteLine("Creating default CamBridgeSettingsV2...");
                    var defaultConfig = CreateDefaultSettingsV2();

                    // Save to AppData for next time
                    await SaveConfigurationAsync(defaultConfig);

                    return defaultConfig as T;
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ ERROR loading config: {ex.Message}");
                _logger?.LogError(ex, "Failed to load configuration");

                // Always return default config on error
                if (typeof(T) == typeof(CamBridgeSettingsV2))
                {
                    return CreateDefaultSettingsV2() as T;
                }

                return null;
            }
        }

        private CamBridgeSettingsV2? ParseServiceJson(string json)
        {
            try
            {
                // Parse the service's JSON format
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var settings = new CamBridgeSettingsV2();

                // Extract pipelines if they exist
                if (root.TryGetProperty("Pipelines", out var pipelinesElement))
                {
                    foreach (var pipelineElement in pipelinesElement.EnumerateArray())
                    {
                        var pipeline = new PipelineConfiguration();

                        // Basic properties
                        if (pipelineElement.TryGetProperty("Id", out var idElem) &&
                            Guid.TryParse(idElem.GetString(), out var id))
                        {
                            pipeline.Id = id;
                        }

                        if (pipelineElement.TryGetProperty("Name", out var nameElem))
                        {
                            pipeline.Name = nameElem.GetString() ?? "Unknown Pipeline";
                        }

                        if (pipelineElement.TryGetProperty("Enabled", out var enabledElem))
                        {
                            pipeline.Enabled = enabledElem.GetBoolean();
                        }

                        // Watch settings
                        if (pipelineElement.TryGetProperty("WatchSettings", out var watchElem))
                        {
                            if (watchElem.TryGetProperty("Path", out var pathElem))
                            {
                                pipeline.WatchSettings.Path = pathElem.GetString() ?? "";
                            }
                            if (watchElem.TryGetProperty("Filter", out var filterElem))
                            {
                                pipeline.WatchSettings.FilePattern = filterElem.GetString() ?? "*.jpg";
                            }
                            if (watchElem.TryGetProperty("IncludeSubdirectories", out var subDirElem))
                            {
                                pipeline.WatchSettings.IncludeSubdirectories = subDirElem.GetBoolean();
                            }
                        }

                        // Output settings -> ProcessingOptions
                        if (pipelineElement.TryGetProperty("OutputSettings", out var outputElem))
                        {
                            if (outputElem.TryGetProperty("Path", out var outPathElem))
                            {
                                pipeline.ProcessingOptions.ArchiveFolder = outPathElem.GetString() ?? "";
                            }
                        }

                        settings.Pipelines.Add(pipeline);
                    }
                }

                return settings;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to parse service JSON: {ex.Message}");
                return null;
            }
        }

        public async Task SaveConfigurationAsync<T>(T configuration) where T : class
        {
            Debug.WriteLine($"\n=== SAVING CONFIG: {typeof(T).Name} ===");

            try
            {
                // Always save to AppData
                var appDataPath = _searchPaths.First();
                var configPath = Path.Combine(appDataPath, "appsettings.json");

                var json = JsonSerializer.Serialize(configuration, _jsonOptions);
                await File.WriteAllTextAsync(configPath, json);

                Debug.WriteLine($"✓ Config saved to: {configPath}");
                _logger?.LogInformation($"Configuration saved to {configPath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ ERROR saving config: {ex.Message}");
                _logger?.LogError(ex, "Failed to save configuration");
                throw;
            }
        }

        private CamBridgeSettingsV2 CreateDefaultSettingsV2()
        {
            Debug.WriteLine("Creating default pipeline configuration...");

            var settings = new CamBridgeSettingsV2
            {
                Version = "2.0"
            };

            // Initialize global settings with defaults
            settings.GlobalDicomSettings = new DicomSettings();
            settings.DefaultProcessingOptions = new ProcessingOptions();
            settings.Logging = new LoggingSettings();
            settings.Service = new ServiceSettings();
            settings.Notifications = new NotificationSettings();

            // Create default pipelines with CORRECT properties
            var radiologyPipeline = new PipelineConfiguration
            {
                Id = Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3301"),
                Name = "Radiology Pipeline",
                Enabled = true,
                Description = "Main pipeline for radiology department"
            };

            // Set watch settings
            radiologyPipeline.WatchSettings.Path = @"C:\CamBridge\Watch\Radiology";
            radiologyPipeline.WatchSettings.FilePattern = "*.jpg;*.jpeg";
            radiologyPipeline.WatchSettings.IncludeSubdirectories = false;
            radiologyPipeline.WatchSettings.MinimumFileAgeSeconds = 2;

            // Set processing options (using the REAL property!)
            radiologyPipeline.ProcessingOptions = new ProcessingOptions
            {
                ArchiveFolder = @"C:\CamBridge\Output\Radiology",
                ErrorFolder = @"C:\CamBridge\Error\Radiology",
                DeadLetterFolder = @"C:\CamBridge\DeadLetter\Radiology",
                CreateBackup = true,
                BackupFolder = @"C:\CamBridge\Archive\Radiology",
                MaxRetryAttempts = 3,
                RetryDelaySeconds = 30,
                OutputOrganization = OutputOrganization.ByPatientAndDate
            };

            settings.Pipelines.Add(radiologyPipeline);

            // Add cardiology pipeline
            var cardiologyPipeline = new PipelineConfiguration
            {
                Id = Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                Name = "Cardiology Pipeline",
                Enabled = true,
                Description = "Pipeline for cardiology department"
            };

            cardiologyPipeline.WatchSettings.Path = @"C:\CamBridge\Watch\Cardiology";
            cardiologyPipeline.WatchSettings.FilePattern = "*.jpg;*.jpeg";

            cardiologyPipeline.ProcessingOptions = new ProcessingOptions
            {
                ArchiveFolder = @"C:\CamBridge\Output\Cardiology",
                ErrorFolder = @"C:\CamBridge\Error\Cardiology",
                DeadLetterFolder = @"C:\CamBridge\DeadLetter\Cardiology",
                CreateBackup = false,
                MaxRetryAttempts = 3
            };

            settings.Pipelines.Add(cardiologyPipeline);

            // Add emergency pipeline (disabled by default)
            var emergencyPipeline = new PipelineConfiguration
            {
                Id = Guid.Parse("6ba7b814-9dad-11d1-80b4-00c04fd430c8"),
                Name = "Emergency Pipeline",
                Enabled = false,
                Description = "High-priority pipeline for ER (disabled by default)"
            };

            emergencyPipeline.WatchSettings.Path = @"C:\CamBridge\Watch\Emergency";
            emergencyPipeline.WatchSettings.FilePattern = "*.jpg;*.jpeg";
            emergencyPipeline.WatchSettings.MinimumFileAgeSeconds = 1; // Faster for emergency

            emergencyPipeline.ProcessingOptions = new ProcessingOptions
            {
                ArchiveFolder = @"C:\CamBridge\Output\Emergency",
                ErrorFolder = @"C:\CamBridge\Error\Emergency",
                DeadLetterFolder = @"C:\CamBridge\DeadLetter\Emergency",
                CreateBackup = true,
                BackupFolder = @"C:\CamBridge\Archive\Emergency",
                MaxRetryAttempts = 5,
                RetryDelaySeconds = 10,
                OutputOrganization = OutputOrganization.ByPatientAndDate
            };

            settings.Pipelines.Add(emergencyPipeline);

            Debug.WriteLine($"Created {settings.Pipelines.Count} default pipelines");
            return settings;
        }
    }
}
