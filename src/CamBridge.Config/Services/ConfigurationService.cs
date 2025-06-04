// src\CamBridge.Config\Services\ConfigurationService.cs
// Version: 0.5.26
// Updated to work with both Config UI and Service

using CamBridge.Core;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Service for loading and saving configuration from JSON files
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private readonly string _configDirectory;
        private readonly string _serviceConfigPath;
        private readonly JsonSerializerOptions _jsonOptions;

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
            if (typeof(T) != typeof(CamBridgeSettings))
            {
                throw new NotSupportedException($"Configuration type {typeof(T).Name} is not supported");
            }

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
                                return settings as T;
                            }
                        }
                        else
                        {
                            // Try direct deserialization (for AppData format)
                            var settings = JsonSerializer.Deserialize<CamBridgeSettings>(json, _jsonOptions);
                            if (settings != null)
                                return settings as T;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading config from {configPath}: {ex.Message}");
                    continue;
                }
            }

            // Return default settings if no file found
            return GetDefaultSettings() as T;
        }

        public async Task SaveConfigurationAsync<T>(T configuration) where T : class
        {
            if (configuration is not CamBridgeSettings settings)
            {
                throw new NotSupportedException($"Configuration type {typeof(T).Name} is not supported");
            }

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
                throw new InvalidOperationException("Failed to save configuration", ex);
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
    }
}
