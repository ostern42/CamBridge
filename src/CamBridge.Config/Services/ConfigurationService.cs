// src\CamBridge.Config\Services\ConfigurationService.cs
// Version: 0.7.1
// Description: Simplified configuration service with centralized config path
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
    /// <summary>
    /// Configuration service using centralized config management
    /// KISS: One config path for all!
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _configPath;

        public ConfigurationService()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };

            // SINGLE SOURCE OF TRUTH!
            _configPath = ConfigurationPaths.GetPrimaryConfigPath();

            Debug.WriteLine("=== ConfigurationService INIT ===");
            Debug.WriteLine($"Config Path: {_configPath}");
            Debug.WriteLine($"Config Exists: {File.Exists(_configPath)}");
            Debug.WriteLine("=================================");
        }

        public async Task<T?> LoadConfigurationAsync<T>() where T : class
        {
            Debug.WriteLine($"\n=== LOADING {typeof(T).Name} ===");
            Debug.WriteLine($"From: {_configPath}");

            try
            {
                if (!File.Exists(_configPath))
                {
                    Debug.WriteLine("Config file not found!");

                    // For CamBridgeSettingsV2, create minimal default (NO DEMO!)
                    if (typeof(T) == typeof(CamBridgeSettingsV2))
                    {
                        var defaultConfig = CreateMinimalDefaultSettings();
                        await SaveConfigurationAsync(defaultConfig);
                        Debug.WriteLine("Created minimal default config");
                        return defaultConfig as T;
                    }

                    return null;
                }

                var json = await File.ReadAllTextAsync(_configPath);
                Debug.WriteLine($"Read {json.Length} characters");

                // Special handling for CamBridgeSettingsV2
                if (typeof(T) == typeof(CamBridgeSettingsV2))
                {
                    // Try direct deserialization first
                    try
                    {
                        var settings = JsonSerializer.Deserialize<T>(json, _jsonOptions);
                        if (settings != null)
                        {
                            Debug.WriteLine("✅ Loaded settings successfully");
                            return settings;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Direct deserialization failed: {ex.Message}");

                        // Try parsing service format
                        var serviceSettings = ParseServiceFormat(json);
                        if (serviceSettings != null)
                        {
                            Debug.WriteLine("✅ Parsed service format successfully");
                            return serviceSettings as T;
                        }
                    }
                }
                else
                {
                    // Generic deserialization
                    var config = JsonSerializer.Deserialize<T>(json, _jsonOptions);
                    if (config != null)
                    {
                        Debug.WriteLine($"✅ Loaded {typeof(T).Name} successfully");
                        return config;
                    }
                }

                Debug.WriteLine("❌ Failed to deserialize config");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR loading config: {ex.Message}");
                Debug.WriteLine($"Stack: {ex.StackTrace}");

                // Don't hide errors - let them bubble up!
                throw new InvalidOperationException($"Failed to load configuration from {_configPath}", ex);
            }
        }

        public async Task SaveConfigurationAsync<T>(T configuration) where T : class
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            try
            {
                Debug.WriteLine($"\n=== SAVING {typeof(T).Name} ===");
                Debug.WriteLine($"To: {_configPath}");

                // Create backup before saving
                if (File.Exists(_configPath))
                {
                    try
                    {
                        var backupPath = ConfigurationPaths.BackupConfig();
                        Debug.WriteLine($"Created backup: {backupPath}");
                    }
                    catch (Exception backupEx)
                    {
                        Debug.WriteLine($"Backup failed (continuing): {backupEx.Message}");
                    }
                }

                var json = JsonSerializer.Serialize(configuration, _jsonOptions);
                await File.WriteAllTextAsync(_configPath, json);

                Debug.WriteLine($"✅ Config saved successfully ({json.Length} characters)");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR saving config: {ex.Message}");
                throw new InvalidOperationException($"Failed to save configuration to {_configPath}", ex);
            }
        }

        /// <summary>
        /// Parse service format (for compatibility)
        /// </summary>
        private CamBridgeSettingsV2? ParseServiceFormat(string json)
        {
            try
            {
                Debug.WriteLine("Attempting to parse service format...");

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                var settings = new CamBridgeSettingsV2();

                // Parse pipelines from service format
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

                        // Parse WatchSettings
                        if (pipelineElement.TryGetProperty("WatchSettings", out var watch))
                        {
                            pipeline.WatchSettings.Path = watch.TryGetProperty("Path", out var path)
                                ? path.GetString() ?? "" : "";
                            pipeline.WatchSettings.FilePattern = watch.TryGetProperty("Filter", out var filter)
                                ? filter.GetString() ?? "*.jpg" : "*.jpg";
                            pipeline.WatchSettings.IncludeSubdirectories = watch.TryGetProperty("IncludeSubdirectories", out var includeSubDirs)
                                && includeSubDirs.GetBoolean();
                        }

                        // Parse OutputSettings
                        if (pipelineElement.TryGetProperty("OutputSettings", out var output))
                        {
                            if (output.TryGetProperty("Path", out var outputPath))
                            {
                                pipeline.ProcessingOptions.ArchiveFolder = outputPath.GetString() ?? "";
                            }
                        }

                        settings.Pipelines.Add(pipeline);
                        Debug.WriteLine($"Parsed pipeline: {pipeline.Name}");
                    }
                }

                // Initialize other required properties
                settings.GlobalDicomSettings = new DicomSettings();
                settings.DefaultProcessingOptions = new ProcessingOptions();
                settings.Logging = new LoggingSettings();
                settings.Service = new ServiceSettings();
                settings.Notifications = new NotificationSettings();

                Debug.WriteLine($"✅ Parsed {settings.Pipelines.Count} pipelines from service format");
                return settings;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to parse service format: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Create minimal default settings (NO DEMO PIPELINES!)
        /// </summary>
        private CamBridgeSettingsV2 CreateMinimalDefaultSettings()
        {
            Debug.WriteLine("Creating minimal default settings (no demo pipelines)");

            var settings = new CamBridgeSettingsV2
            {
                Version = "2.0",
                Pipelines = new List<PipelineConfiguration>(), // EMPTY!
                GlobalDicomSettings = new DicomSettings(),
                DefaultProcessingOptions = new ProcessingOptions
                {
                    ArchiveFolder = @"C:\CamBridge\Output",
                    ErrorFolder = @"C:\CamBridge\Error",
                    BackupFolder = @"C:\CamBridge\Archive"
                },
                Logging = new LoggingSettings
                {
                    LogFolder = ConfigurationPaths.GetLogsDirectory()
                },
                Service = new ServiceSettings(),
                Notifications = new NotificationSettings()
            };

            return settings;
        }

    }
}
