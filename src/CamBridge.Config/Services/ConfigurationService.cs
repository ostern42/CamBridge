// src\CamBridge.Config\Services\ConfigurationService.cs
// Version: 0.6.0
// Updated to support both v1 and v2 settings with automatic migration

using CamBridge.Core;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Service for loading and saving configuration from JSON files
    /// Now supports both v1 and v2 formats with automatic migration
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private readonly string _configDirectory;
        private readonly string _serviceConfigPath;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _v2ConfigFileName = "appsettings.v2.json";
        private CamBridgeSettingsV2? _v2SettingsCache;

        public ConfigurationService()
        {
            // Use AppData for config storage
            _configDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CamBridge");

            // Also check for service config in standard location
            _serviceConfigPath = Path.Combine(
                Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) ?? "",
                "..", "..", "..", "src", "CamBridge.Service", "appsettings.json");

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            // Ensure directory exists
            Directory.CreateDirectory(_configDirectory);
        }

        public async Task<T?> LoadConfigurationAsync<T>() where T : class
        {
            // Always load v2 internally
            var v2Settings = await LoadOrMigrateToV2Async();

            // Return in requested format
            if (typeof(T) == typeof(CamBridgeSettingsV2))
            {
                return v2Settings as T;
            }
            else if (typeof(T) == typeof(CamBridgeSettings))
            {
                // Convert to v1 format for backward compatibility
                return v2Settings?.ToV1Format() as T;
            }

            throw new NotSupportedException($"Configuration type {typeof(T).Name} is not supported");
        }

        public async Task SaveConfigurationAsync<T>(T configuration) where T : class
        {
            CamBridgeSettingsV2 v2Settings;

            if (configuration is CamBridgeSettingsV2 v2)
            {
                v2Settings = v2;
            }
            else if (configuration is CamBridgeSettings v1)
            {
                // Check if we already have v2 settings cached
                if (_v2SettingsCache != null)
                {
                    // Update existing v2 with changes from v1
                    v2Settings = UpdateV2FromV1(_v2SettingsCache, v1);
                }
                else
                {
                    // Full migration
                    v2Settings = CamBridgeSettingsV2.MigrateFromV1(v1);
                }
            }
            else
            {
                throw new NotSupportedException($"Configuration type {typeof(T).Name} is not supported");
            }

            // Save v2 format
            await SaveV2SettingsAsync(v2Settings);

            // Also save v1 format for backward compatibility
            await SaveV1CompatibilityAsync(v2Settings.ToV1Format());

            // Update cache
            _v2SettingsCache = v2Settings;
        }

        private async Task<CamBridgeSettingsV2?> LoadOrMigrateToV2Async()
        {
            // Check cache first
            if (_v2SettingsCache != null)
                return _v2SettingsCache;

            // Try to load v2 settings
            var v2ConfigPath = Path.Combine(_configDirectory, _v2ConfigFileName);
            if (File.Exists(v2ConfigPath))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(v2ConfigPath);
                    var v2Settings = JsonSerializer.Deserialize<CamBridgeSettingsV2>(json, _jsonOptions);

                    if (v2Settings != null)
                    {
                        _v2SettingsCache = v2Settings;
                        return v2Settings;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading v2 config: {ex.Message}");
                }
            }

            // Try to load and migrate v1 settings
            var v1Settings = await LoadV1SettingsAsync();
            if (v1Settings != null)
            {
                System.Diagnostics.Debug.WriteLine("Migrating v1 settings to v2...");
                var v2Settings = CamBridgeSettingsV2.MigrateFromV1(v1Settings);

                // Save the migrated settings
                await SaveV2SettingsAsync(v2Settings);
                _v2SettingsCache = v2Settings;

                return v2Settings;
            }

            // Return default v2 settings
            var defaultSettings = GetDefaultV2Settings();
            _v2SettingsCache = defaultSettings;
            return defaultSettings;
        }

        private async Task<CamBridgeSettings?> LoadV1SettingsAsync()
        {
            // Try multiple locations in order of preference
            var configPaths = new[]
            {
                Path.Combine(_configDirectory, "appsettings.json"),  // AppData location
                _serviceConfigPath,                                    // Service location
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json") // Local location
            };

            foreach (var configPath in configPaths)
            {
                try
                {
                    if (File.Exists(configPath))
                    {
                        System.Diagnostics.Debug.WriteLine($"Loading config from: {configPath}");
                        var json = await File.ReadAllTextAsync(configPath);

                        // Handle nested CamBridge section
                        var jsonDoc = JsonDocument.Parse(json);
                        if (jsonDoc.RootElement.TryGetProperty("CamBridge", out var camBridgeElement))
                        {
                            // Extract the CamBridge section and deserialize
                            var settings = JsonSerializer.Deserialize<CamBridgeSettings>(
                                camBridgeElement.GetRawText(), _jsonOptions);

                            if (settings != null)
                            {
                                // Map service config format to our settings
                                MapFromServiceConfig(settings, jsonDoc.RootElement);
                                return settings;
                            }
                        }
                        else
                        {
                            // Try direct deserialization (for AppData format)
                            var settings = JsonSerializer.Deserialize<CamBridgeSettings>(json, _jsonOptions);
                            if (settings != null)
                                return settings;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading config from {configPath}: {ex.Message}");
                    continue;
                }
            }

            return null;
        }

        private async Task SaveV2SettingsAsync(CamBridgeSettingsV2 settings)
        {
            var v2ConfigPath = Path.Combine(_configDirectory, _v2ConfigFileName);

            try
            {
                var json = JsonSerializer.Serialize(settings, _jsonOptions);
                await File.WriteAllTextAsync(v2ConfigPath, json);
                System.Diagnostics.Debug.WriteLine($"V2 config saved to: {v2ConfigPath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving v2 configuration: {ex.Message}");
                throw new InvalidOperationException("Failed to save v2 configuration", ex);
            }
        }

        private async Task SaveV1CompatibilityAsync(CamBridgeSettings settings)
        {
            // Save to AppData location
            var appDataConfig = Path.Combine(_configDirectory, "appsettings.json");

            try
            {
                var json = JsonSerializer.Serialize(settings, _jsonOptions);
                await File.WriteAllTextAsync(appDataConfig, json);
                System.Diagnostics.Debug.WriteLine($"Config saved to: {appDataConfig}");

                // Also update service config if it exists
                if (File.Exists(_serviceConfigPath))
                {
                    await UpdateServiceConfigAsync(settings);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving configuration: {ex.Message}");
                // Don't throw - v1 compatibility is optional for v2
            }
        }

        private async Task UpdateServiceConfigAsync(CamBridgeSettings settings)
        {
            try
            {
                // Read existing service config
                var json = await File.ReadAllTextAsync(_serviceConfigPath);
                var jsonDoc = JsonDocument.Parse(json);

                using var stream = new MemoryStream();
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
                {
                    writer.WriteStartObject();

                    // Copy all root properties except CamBridge
                    foreach (var property in jsonDoc.RootElement.EnumerateObject())
                    {
                        if (property.Name != "CamBridge")
                        {
                            property.WriteTo(writer);
                        }
                    }

                    // Write updated CamBridge section
                    writer.WritePropertyName("CamBridge");
                    JsonSerializer.Serialize(writer, settings, _jsonOptions);

                    writer.WriteEndObject();
                }

                // Write back to file
                var updatedJson = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                await File.WriteAllTextAsync(_serviceConfigPath, updatedJson);
                System.Diagnostics.Debug.WriteLine($"Service config updated: {_serviceConfigPath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating service config: {ex.Message}");
                // Don't throw - service config update is optional
            }
        }

        private void MapFromServiceConfig(CamBridgeSettings settings, JsonElement root)
        {
            // The service config has a different structure, map it appropriately
            if (root.TryGetProperty("CamBridge", out var camBridge))
            {
                // ProcessingOptions might be nested differently
                if (camBridge.TryGetProperty("ProcessingOptions", out var procOpts))
                {
                    // Map processing options from service format
                    if (procOpts.TryGetProperty("OutputFolder", out var outputFolder))
                    {
                        settings.DefaultOutputFolder = outputFolder.GetString() ?? settings.DefaultOutputFolder;
                    }
                }

                // DicomSettings mapping
                if (camBridge.TryGetProperty("DicomSettings", out var dicomSettings))
                {
                    if (dicomSettings.TryGetProperty("ImplementationClassUID", out var uid))
                    {
                        settings.Dicom.ImplementationClassUid = uid.GetString() ?? settings.Dicom.ImplementationClassUid;
                    }
                }
            }
        }

        private CamBridgeSettingsV2 UpdateV2FromV1(CamBridgeSettingsV2 existing, CamBridgeSettings v1Update)
        {
            // Update global settings
            existing.GlobalDicomSettings = v1Update.Dicom;
            existing.DefaultProcessingOptions = v1Update.Processing;
            existing.Logging = v1Update.Logging;
            existing.Service = v1Update.Service;
            existing.Notifications = v1Update.Notifications;

            // Update ALL pipelines with the global processing options from v1
            // (Since v1 doesn't have per-pipeline processing options)
            foreach (var pipeline in existing.Pipelines)
            {
                pipeline.ProcessingOptions = CloneProcessingOptions(v1Update.Processing);
            }

            // Update pipeline watch folders from v1 watch folders
            // Match by index for now (simple approach)
            for (int i = 0; i < v1Update.WatchFolders.Count && i < existing.Pipelines.Count; i++)
            {
                var v1Folder = v1Update.WatchFolders[i];
                var pipeline = existing.Pipelines[i];

                pipeline.WatchSettings.Path = v1Folder.Path;
                pipeline.WatchSettings.FilePattern = v1Folder.FilePattern;
                pipeline.WatchSettings.IncludeSubdirectories = v1Folder.IncludeSubdirectories;
                pipeline.WatchSettings.OutputPath = v1Folder.OutputPath;
                pipeline.Enabled = v1Folder.Enabled;

                // If v1 folder has custom output path, update pipeline's archive folder
                if (!string.IsNullOrEmpty(v1Folder.OutputPath))
                {
                    pipeline.ProcessingOptions.ArchiveFolder = v1Folder.OutputPath;
                }
            }

            // Handle new watch folders (add as new pipelines)
            if (v1Update.WatchFolders.Count > existing.Pipelines.Count)
            {
                for (int i = existing.Pipelines.Count; i < v1Update.WatchFolders.Count; i++)
                {
                    var v1Folder = v1Update.WatchFolders[i];
                    var newPipeline = new PipelineConfiguration
                    {
                        Name = $"Pipeline {i + 1}",
                        Description = $"Added from UI",
                        Enabled = v1Folder.Enabled,
                        WatchSettings = new PipelineWatchSettings
                        {
                            Path = v1Folder.Path,
                            FilePattern = v1Folder.FilePattern,
                            IncludeSubdirectories = v1Folder.IncludeSubdirectories,
                            OutputPath = v1Folder.OutputPath
                        },
                        ProcessingOptions = CloneProcessingOptions(v1Update.Processing),
                        MappingSetId = existing.MappingSets.FirstOrDefault()?.Id
                    };

                    // If v1 folder has custom output path, update pipeline's archive folder
                    if (!string.IsNullOrEmpty(v1Folder.OutputPath))
                    {
                        newPipeline.ProcessingOptions.ArchiveFolder = v1Folder.OutputPath;
                    }

                    existing.Pipelines.Add(newPipeline);
                }
            }

            // Handle removed watch folders (remove pipelines)
            while (existing.Pipelines.Count > v1Update.WatchFolders.Count)
            {
                existing.Pipelines.RemoveAt(existing.Pipelines.Count - 1);
            }

            return existing;
        }

        private CamBridgeSettingsV2 GetDefaultV2Settings()
        {
            // First create default v1 settings
            var v1Settings = GetDefaultSettings();

            // Migrate to v2
            return CamBridgeSettingsV2.MigrateFromV1(v1Settings);
        }

        private CamBridgeSettings GetDefaultSettings()
        {
            return new CamBridgeSettings
            {
                DefaultOutputFolder = @"C:\CamBridge\Output",
                MappingConfigurationFile = "mappings.json",
                UseRicohExifReader = true,
                WatchFolders =
                {
                    new FolderConfiguration
                    {
                        Path = @"C:\CamBridge\Input",
                        Enabled = true,
                        FilePattern = "*.jpg;*.jpeg"
                    }
                },
                Processing = new ProcessingOptions
                {
                    SuccessAction = PostProcessingAction.Archive,
                    FailureAction = PostProcessingAction.MoveToError,
                    ArchiveFolder = @"C:\CamBridge\Archive",
                    ErrorFolder = @"C:\CamBridge\Errors",
                    BackupFolder = @"C:\CamBridge\Backup",
                    CreateBackup = true,
                    MaxConcurrentProcessing = 2,
                    RetryOnFailure = true,
                    MaxRetryAttempts = 3,
                    OutputOrganization = OutputOrganization.ByPatientAndDate
                },
                Dicom = new DicomSettings
                {
                    ImplementationClassUid = "1.2.276.0.7230010.3.0.3.6.4",
                    ImplementationVersionName = "CAMBRIDGE_001",
                    StationName = Environment.MachineName,
                    ValidateAfterCreation = true
                },
                Logging = new LoggingSettings
                {
                    LogLevel = "Information",
                    LogFolder = @"C:\CamBridge\Logs",
                    EnableFileLogging = true,
                    EnableEventLog = true,
                    MaxLogFileSizeMB = 10,
                    MaxLogFiles = 10
                },
                Service = new ServiceSettings
                {
                    ServiceName = "CamBridgeService",
                    DisplayName = "CamBridge JPEG to DICOM Converter",
                    Description = "Monitors folders for JPEG files from Ricoh cameras and converts them to DICOM format",
                    StartupDelaySeconds = 5,
                    FileProcessingDelayMs = 500
                },
                Notifications = new NotificationSettings
                {
                    EnableEventLog = true,
                    EnableEmail = false,
                    Email = new EmailSettings
                    {
                        SmtpPort = 587,
                        UseSsl = true
                    },
                    MinimumEmailLevel = NotificationLevel.Warning,
                    SendDailySummary = false,
                    DailySummaryHour = 8
                }
            };
        }

        /// <summary>
        /// Helper to clone processing options
        /// </summary>
        private static ProcessingOptions CloneProcessingOptions(ProcessingOptions source)
        {
            return new ProcessingOptions
            {
                SuccessAction = source.SuccessAction,
                FailureAction = source.FailureAction,
                ArchiveFolder = source.ArchiveFolder,
                ErrorFolder = source.ErrorFolder,
                BackupFolder = source.BackupFolder,
                CreateBackup = source.CreateBackup,
                MaxConcurrentProcessing = source.MaxConcurrentProcessing,
                RetryOnFailure = source.RetryOnFailure,
                MaxRetryAttempts = source.MaxRetryAttempts,
                OutputOrganization = source.OutputOrganization,
                ProcessExistingOnStartup = source.ProcessExistingOnStartup,
                MaxFileAge = source.MaxFileAge,
                MinimumFileSizeBytes = source.MinimumFileSizeBytes,
                MaximumFileSizeBytes = source.MaximumFileSizeBytes,
                OutputFilePattern = source.OutputFilePattern,
                RetryDelaySeconds = source.RetryDelaySeconds
            };
        }
    }
}
