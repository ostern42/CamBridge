// src\CamBridge.Core\CamBridgeSettings.cs
// Version: 0.6.2
// Description: Main configuration model for CamBridge application

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
        /// Default output folder for converted DICOM files
        /// </summary>
        public string DefaultOutputFolder { get; set; } = @"C:\CamBridge\Output";

        /// <summary>
        /// Path to mapping configuration file
        /// </summary>
        public string MappingConfigurationFile { get; set; } = "mappings.json";

        /// <summary>
        /// Use Ricoh-specific EXIF reader optimizations
        /// </summary>
        public bool UseRicohExifReader { get; set; } = true;

        /// <summary>
        /// Path to ExifTool executable
        /// </summary>
        public string ExifToolPath { get; set; } = "Tools\\exiftool.exe";

        /// <summary>
        /// List of folders to watch for incoming images
        /// </summary>
        public List<FolderConfiguration> WatchFolders { get; set; } = new();

        /// <summary>
        /// Processing options
        /// </summary>
        public ProcessingOptions Processing { get; set; } = new();

        /// <summary>
        /// DICOM-specific settings
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

    /// <summary>
    /// Configuration for a watched folder
    /// </summary>
    public class FolderConfiguration
    {
        /// <summary>
        /// Folder path to watch
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Custom output path for this folder (overrides default)
        /// </summary>
        public string? OutputPath { get; set; }

        /// <summary>
        /// Whether this folder is actively watched
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Include subdirectories in watch
        /// </summary>
        public bool IncludeSubdirectories { get; set; } = false;

        /// <summary>
        /// File pattern to match (e.g., "*.jpg;*.jpeg")
        /// </summary>
        public string FilePattern { get; set; } = "*.jpg;*.jpeg";

        /// <summary>
        /// Checks if this folder configuration is valid
        /// </summary>
        [JsonIgnore]
        public bool IsValid => !string.IsNullOrWhiteSpace(Path) && System.IO.Directory.Exists(Path);
    }

    /// <summary>
    /// DICOM-specific settings
    /// </summary>
    public class DicomSettings
    {
        /// <summary>
        /// Implementation Class UID for this application
        /// </summary>
        public string ImplementationClassUid { get; set; } = "1.2.276.0.7230010.3.0.3.6.4";

        /// <summary>
        /// Implementation Version Name
        /// </summary>
        public string ImplementationVersionName { get; set; } = "CAMBRIDGE_001";

        /// Source Application Entity Title
        /// </summary>
        public string SourceApplicationEntityTitle { get; set; } = "CAMBRIDGE";

        /// <summary>
        /// Institution Name
        /// </summary>
        public string? InstitutionName { get; set; }

        /// <summary>
        /// Institution Department
        /// </summary>
        public string? InstitutionDepartment { get; set; }

        /// <summary>
        /// Station Name
        /// </summary>
        public string? StationName { get; set; }

        /// <summary>
        /// Modality for created images
        /// </summary>
        public string Modality { get; set; } = "OT"; // Other

        /// <summary>
        /// Validate DICOM files after creation
        /// </summary>
        public bool ValidateAfterCreation { get; set; } = true;
    }

    /// <summary>
    /// Logging configuration
    /// </summary>
    public class LoggingSettings
    {
        /// <summary>
        /// Minimum log level (Trace, Debug, Information, Warning, Error, Critical)
        /// </summary>
        public string LogLevel { get; set; } = "Information";

        /// <summary>
        /// Log folder path
        /// </summary>
        public string LogFolder { get; set; } = @"C:\CamBridge\Logs";

        /// <summary>
        /// Enable file logging
        /// </summary>
        public bool EnableFileLogging { get; set; } = true;

        /// <summary>
        /// Enable Windows Event Log
        /// </summary>
        public bool EnableEventLog { get; set; } = true;

        /// <summary>
        /// Maximum log file size in MB
        /// </summary>
        public int MaxLogFileSizeMB { get; set; } = 10;

        /// <summary>
        /// Maximum number of log files to retain
        /// </summary>
        public int MaxLogFiles { get; set; } = 10;
    }

    /// <summary>
    /// Windows Service specific settings
    /// </summary>
    public class ServiceSettings
    {
        /// <summary>
        /// Service name (for sc.exe)
        /// </summary>
        public string ServiceName { get; set; } = "CamBridgeService";

        /// <summary>
        /// Service display name
        /// </summary>
        public string DisplayName { get; set; } = "CamBridge JPEG to DICOM Converter";

        /// <summary>
        /// Service description
        /// </summary>
        public string Description { get; set; } = "Monitors folders for JPEG files from Ricoh cameras and converts them to DICOM format";

        /// <summary>
        /// Startup delay in seconds
        /// </summary>
        public int StartupDelaySeconds { get; set; } = 5;

        /// <summary>
        /// File processing delay in milliseconds
        /// </summary>
        public int FileProcessingDelayMs { get; set; } = 500;
    }
}
