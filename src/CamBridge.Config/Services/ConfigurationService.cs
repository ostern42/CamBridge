// src\CamBridge.Config\Services\ConfigurationService.cs
// Version: 0.7.10
// Description: Simplified configuration service - V2 format ONLY!
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
    /// KISS: One config path, one format (V2 with CamBridge wrapper)!
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
                    Debug.WriteLine("Config file not found - will be created by InitializePrimaryConfig");
                    return null;
                }

                var json = await File.ReadAllTextAsync(_configPath);
                Debug.WriteLine($"Read {json.Length} characters");

                // Special handling for CamBridgeSettingsV2 - ALWAYS load from "CamBridge" section
                if (typeof(T) == typeof(CamBridgeSettingsV2))
                {
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    // We REQUIRE a "CamBridge" section - no fallbacks!
                    if (!root.TryGetProperty("CamBridge", out var cambridgeSection))
                    {
                        throw new InvalidOperationException(
                            "Configuration file is missing required 'CamBridge' section! " +
                            "This is not a valid V2 configuration file.");
                    }

                    // Deserialize from the CamBridge section
                    var settings = JsonSerializer.Deserialize<CamBridgeSettingsV2>(
                        cambridgeSection.GetRawText(),
                        _jsonOptions);

                    if (settings == null)
                    {
                        throw new InvalidOperationException(
                            "Failed to deserialize CamBridge section to CamBridgeSettingsV2");
                    }

                    Debug.WriteLine($"✅ Loaded settings from CamBridge section");
                    Debug.WriteLine($"   Version: {settings.Version}");
                    Debug.WriteLine($"   Pipelines: {settings.Pipelines.Count}");
                    Debug.WriteLine($"   MappingSets: {settings.MappingSets.Count}");

                    return settings as T;
                }
                else
                {
                    // Generic deserialization (for other types if needed)
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
                throw;
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
                        var backupPath = ConfigurationPaths.BackupConfig(_configPath);
                        Debug.WriteLine($"Created backup: {backupPath}");
                    }
                    catch (Exception backupEx)
                    {
                        Debug.WriteLine($"Backup failed (continuing): {backupEx.Message}");
                    }
                }

                // For CamBridgeSettingsV2, we ALWAYS wrap it in the CamBridge section
                if (configuration is CamBridgeSettingsV2 v2Settings)
                {
                    // Create wrapper object with proper V2 format
                    var wrapper = new Dictionary<string, object>
                    {
                        ["CamBridge"] = v2Settings,
                        ["Logging"] = new
                        {
                            LogLevel = new
                            {
                                Default = "Information",
                                Microsoft = "Warning",
                                CamBridge = "Information"
                            }
                        }
                    };

                    var json = JsonSerializer.Serialize(wrapper, _jsonOptions);
                    await File.WriteAllTextAsync(_configPath, json);

                    Debug.WriteLine($"✅ Config saved with CamBridge wrapper ({json.Length} characters)");
                    Debug.WriteLine($"   Pipelines saved: {v2Settings.Pipelines.Count}");
                }
                else
                {
                    // Generic save (shouldn't happen in normal use)
                    var json = JsonSerializer.Serialize(configuration, _jsonOptions);
                    await File.WriteAllTextAsync(_configPath, json);
                    Debug.WriteLine($"✅ Config saved successfully ({json.Length} characters)");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR saving config: {ex.Message}");
                throw new InvalidOperationException($"Failed to save configuration to {_configPath}", ex);
            }
        }
    }
}
