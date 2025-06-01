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
        private readonly JsonSerializerOptions _jsonOptions;

        public ConfigurationService()
        {
            // Use AppData for config storage
            _configDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CamBridge");

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

            var configFile = Path.Combine(_configDirectory, "appsettings.json");

            try
            {
                if (File.Exists(configFile))
                {
                    var json = await File.ReadAllTextAsync(configFile);
                    var settings = JsonSerializer.Deserialize<CamBridgeSettings>(json, _jsonOptions);
                    return settings as T;
                }
                else
                {
                    // Return default settings if file doesn't exist
                    return GetDefaultSettings() as T;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading configuration: {ex.Message}");
                // Return default settings on error
                return GetDefaultSettings() as T;
            }
        }

        public async Task SaveConfigurationAsync<T>(T configuration) where T : class
        {
            if (configuration is not CamBridgeSettings settings)
            {
                throw new NotSupportedException($"Configuration type {typeof(T).Name} is not supported");
            }

            var configFile = Path.Combine(_configDirectory, "appsettings.json");

            try
            {
                var json = JsonSerializer.Serialize(settings, _jsonOptions);
                await File.WriteAllTextAsync(configFile, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving configuration: {ex.Message}");
                throw new InvalidOperationException("Failed to save configuration", ex);
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
                    MinimumEmailLevel = "Warning",
                    SendDailySummary = false,
                    DailySummaryHour = 8
                }
            };
        }
    }
}
