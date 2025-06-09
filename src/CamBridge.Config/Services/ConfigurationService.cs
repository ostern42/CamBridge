// src\CamBridge.Config\Services\ConfigurationService.cs
// Version: 0.6.12
// Description: Fixed configuration service - WatchSettings → PipelineWatchSettings
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CamBridge.Core;

namespace CamBridge.Config.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _primaryConfigPath;
        private readonly string _fallbackConfigPath;

        public ConfigurationService()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };

            // Simple, clear paths
            _primaryConfigPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CamBridge",
                "appsettings.json");

            _fallbackConfigPath = Path.Combine(
                AppContext.BaseDirectory,
                "appsettings.json");

            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(_primaryConfigPath)!);

            Debug.WriteLine("=== ConfigurationService INIT ===");
            Debug.WriteLine($"Primary: {_primaryConfigPath}");
            Debug.WriteLine($"Fallback: {_fallbackConfigPath}");
        }

        public async Task<T?> LoadConfigurationAsync<T>() where T : class
        {
            Debug.WriteLine($"\n=== LOADING {typeof(T).Name} ===");

            try
            {
                // Try primary location first
                if (File.Exists(_primaryConfigPath))
                {
                    Debug.WriteLine($"Found config at primary location: {_primaryConfigPath}");
                    var json = await File.ReadAllTextAsync(_primaryConfigPath);

                    // For CamBridgeSettingsV2, try direct deserialization first
                    if (typeof(T) == typeof(CamBridgeSettingsV2))
                    {
                        try
                        {
                            var settings = JsonSerializer.Deserialize<T>(json, _jsonOptions);
                            if (settings != null)
                            {
                                Debug.WriteLine("✅ Loaded settings from primary location");
                                return settings;
                            }
                        }
                        catch
                        {
                            // Try parsing as service format
                            var serviceSettings = ParseServiceFormat(json);
                            if (serviceSettings != null)
                            {
                                Debug.WriteLine("✅ Parsed service format from primary location");
                                return serviceSettings as T;
                            }
                        }
                    }
                    else
                    {
                        var config = JsonSerializer.Deserialize<T>(json, _jsonOptions);
                        if (config != null) return config;
                    }
                }

                // Try fallback location
                if (File.Exists(_fallbackConfigPath))
                {
                    Debug.WriteLine($"Found config at fallback location");
                    var json = await File.ReadAllTextAsync(_fallbackConfigPath);
                    var config = DeserializeConfig<T>(json);
                    if (config != null) return config;
                }

                // No config found - create default
                Debug.WriteLine("No config found - creating default");
                if (typeof(T) == typeof(CamBridgeSettingsV2))
                {
                    var defaultConfig = CreateDefaultSettings();
                    await SaveConfigurationAsync(defaultConfig);
                    return defaultConfig as T;
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR loading config: {ex.Message}");

                // Always return default on error
                if (typeof(T) == typeof(CamBridgeSettingsV2))
                {
                    return CreateDefaultSettings() as T;
                }

                return null;
            }
        }

        public async Task SaveConfigurationAsync<T>(T configuration) where T : class
        {
            try
            {
                var json = JsonSerializer.Serialize(configuration, _jsonOptions);
                await File.WriteAllTextAsync(_primaryConfigPath, json);
                Debug.WriteLine($"Config saved to: {_primaryConfigPath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR saving config: {ex.Message}");
                throw;
            }
        }

        private T? DeserializeConfig<T>(string json) where T : class
        {
            try
            {
                // Special handling for CamBridgeSettingsV2 from service format
                if (typeof(T) == typeof(CamBridgeSettingsV2))
                {
                    // Try direct deserialization first
                    try
                    {
                        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
                    }
                    catch
                    {
                        // If that fails, try parsing service format
                        return ParseServiceFormat(json) as T;
                    }
                }

                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Deserialization failed: {ex.Message}");
                return null;
            }
        }

        private CamBridgeSettingsV2? ParseServiceFormat(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                var settings = new CamBridgeSettingsV2();

                if (root.TryGetProperty("Pipelines", out var pipelines))
                {
                    foreach (var pipelineElement in pipelines.EnumerateArray())
                    {
                        var pipeline = new PipelineConfiguration
                        {
                            Id = Guid.TryParse(pipelineElement.GetProperty("Id").GetString(), out var id)
                                ? id : Guid.NewGuid(),
                            Name = pipelineElement.GetProperty("Name").GetString() ?? "Unknown",
                            Enabled = pipelineElement.TryGetProperty("Enabled", out var enabled)
                                && enabled.GetBoolean()
                        };

                        // Parse nested properties if they exist
                        if (pipelineElement.TryGetProperty("WatchSettings", out var watch))
                        {
                            pipeline.WatchSettings.Path = watch.TryGetProperty("Path", out var path)
                                ? path.GetString() ?? "" : "";
                            pipeline.WatchSettings.FilePattern = watch.TryGetProperty("Filter", out var filter)
                                ? filter.GetString() ?? "*.jpg" : "*.jpg";
                        }

                        settings.Pipelines.Add(pipeline);
                    }
                }

                return settings.Pipelines.Any() ? settings : null;
            }
            catch
            {
                return null;
            }
        }

        private CamBridgeSettingsV2 CreateDefaultSettings()
        {
            Debug.WriteLine("Creating default settings with demo pipelines");

            var settings = new CamBridgeSettingsV2
            {
                Version = "2.0",
                Pipelines = new List<PipelineConfiguration>
                {
                    new PipelineConfiguration
                    {
                        Id = Guid.NewGuid(),
                        Name = "Radiology Pipeline",
                        Enabled = true,
                        Description = "Main pipeline for radiology department",
                        WatchSettings = new PipelineWatchSettings  // ✅ FIXED!
                        {
                            Path = @"C:\CamBridge\Watch\Radiology",
                            FilePattern = "*.jpg;*.jpeg",
                            IncludeSubdirectories = false,
                            MinimumFileAgeSeconds = 2
                        },
                        ProcessingOptions = new ProcessingOptions
                        {
                            ArchiveFolder = @"C:\CamBridge\Output\Radiology",
                            ErrorFolder = @"C:\CamBridge\Error\Radiology",
                            CreateBackup = true,
                            BackupFolder = @"C:\CamBridge\Archive\Radiology",
                            OutputOrganization = OutputOrganization.ByPatientAndDate
                        }
                    },
                    new PipelineConfiguration
                    {
                        Id = Guid.NewGuid(),
                        Name = "Cardiology Pipeline",
                        Enabled = true,
                        Description = "Pipeline for cardiology department",
                        WatchSettings = new PipelineWatchSettings  // ✅ FIXED!
                        {
                            Path = @"C:\CamBridge\Watch\Cardiology",
                            FilePattern = "*.jpg;*.jpeg"
                        },
                        ProcessingOptions = new ProcessingOptions
                        {
                            ArchiveFolder = @"C:\CamBridge\Output\Cardiology",
                            ErrorFolder = @"C:\CamBridge\Error\Cardiology"
                        }
                    },
                    new PipelineConfiguration
                    {
                        Id = Guid.NewGuid(),
                        Name = "Emergency Pipeline",
                        Enabled = false,
                        Description = "High-priority pipeline for ER (disabled by default)",
                        WatchSettings = new PipelineWatchSettings  // ✅ FIXED!
                        {
                            Path = @"C:\CamBridge\Watch\Emergency",
                            FilePattern = "*.jpg;*.jpeg",
                            MinimumFileAgeSeconds = 1
                        },
                        ProcessingOptions = new ProcessingOptions
                        {
                            ArchiveFolder = @"C:\CamBridge\Output\Emergency",
                            ErrorFolder = @"C:\CamBridge\Error\Emergency",
                            CreateBackup = true,
                            BackupFolder = @"C:\CamBridge\Archive\Emergency"
                        }
                    }
                }
            };

            // Initialize other settings with defaults - ALL CORRECT NAMES!
            settings.GlobalDicomSettings = new DicomSettings();         // ✅ CORRECT!
            settings.DefaultProcessingOptions = new ProcessingOptions(); // ✅ CORRECT!
            settings.Logging = new LoggingSettings();                   // ✅ CORRECT!
            settings.Service = new ServiceSettings();                   // ✅ CORRECT!
            settings.Notifications = new NotificationSettings();        // ✅ CORRECT!

            return settings;
        }
    }
}
