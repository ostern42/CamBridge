using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CamBridge.Core
{
    /// <summary>
    /// Main configuration settings for CamBridge
    /// </summary>
    public class CamBridgeSettings
    {
        /// <summary>
        /// Folders to monitor for new JPEG files
        /// </summary>
        public List<FolderConfiguration> WatchFolders { get; set; } = new();

        /// <summary>
        /// Default output folder for DICOM files
        /// </summary>
        public string DefaultOutputFolder { get; set; } = @"C:\CamBridge\Output";

        /// <summary>
        /// Path to mapping configuration file
        /// </summary>
        public string MappingConfigurationFile { get; set; } = "mappings.json";

        /// <summary>
        /// Whether to use Ricoh-specific EXIF reader
        /// </summary>
        public bool UseRicohExifReader { get; set; } = true;

        /// <summary>
        /// Processing options
        /// </summary>
        public ProcessingOptions Processing { get; set; } = new();

        /// <summary>
        /// DICOM specific settings
        /// </summary>
        public DicomSettings Dicom { get; set; } = new();

        /// <summary>
        /// Logging configuration
        /// </summary>
        public LoggingSettings Logging { get; set; } = new();

        /// <summary>
        /// Service-specific settings
        /// </summary>
        public ServiceSettings Service { get; set; } = new();

        /// <summary>
        /// Notification settings
        /// </summary>
        public NotificationSettings Notifications { get; set; } = new();
    }

    public class FolderConfiguration
    {
        public string Path { get; set; } = string.Empty;
        public string? OutputPath { get; set; }
        public bool Enabled { get; set; } = true;
        public bool IncludeSubdirectories { get; set; } = false;
        public string FilePattern { get; set; } = "*.jpg;*.jpeg";

        [JsonIgnore]
        public bool IsValid => !string.IsNullOrWhiteSpace(Path) &&
                              System.IO.Directory.Exists(Path);
    }

    public class DicomSettings
    {
        /// <summary>
        /// Implementation class UID prefix for this institution
        /// </summary>
        public string ImplementationClassUid { get; set; } = "1.2.276.0.7230010.3.0.3.6.4";

        /// <summary>
        /// Implementation version name
        /// </summary>
        public string ImplementationVersionName { get; set; } = "CAMBRIDGE_001";

        /// <summary>
        /// Default institution name
        /// </summary>
        public string InstitutionName { get; set; } = string.Empty;

        /// <summary>
        /// Station name
        /// </summary>
        public string StationName { get; set; } = Environment.MachineName;

        /// <summary>
        /// Whether to validate DICOM files after creation
        /// </summary>
        public bool ValidateAfterCreation { get; set; } = true;
    }

    public class LoggingSettings
    {
        public string LogLevel { get; set; } = "Information";
        public string LogFolder { get; set; } = @"C:\CamBridge\Logs";
        public bool EnableFileLogging { get; set; } = true;
        public bool EnableEventLog { get; set; } = true;
        public int MaxLogFileSizeMB { get; set; } = 10;
        public int MaxLogFiles { get; set; } = 10;

        /// <summary>
        /// Whether to include patient data in debug logs (CAUTION!)
        /// </summary>
        public bool IncludePatientDataInDebugLogs { get; set; } = false;
    }

    public class ServiceSettings
    {
        public string ServiceName { get; set; } = "CamBridgeService";
        public string DisplayName { get; set; } = "CamBridge JPEG to DICOM Converter";
        public string Description { get; set; } = "Monitors folders for JPEG files from Ricoh cameras and converts them to DICOM format";
        public int StartupDelaySeconds { get; set; } = 5;
        public int FileProcessingDelayMs { get; set; } = 500;
    }
}
