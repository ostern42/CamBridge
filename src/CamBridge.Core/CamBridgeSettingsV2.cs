// src\CamBridge.Core\CamBridgeSettingsV2.cs
// Version: 0.7.20
// Description: Version 2 settings with pipeline architecture - CLEAN!
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace CamBridge.Core
{
    /// <summary>
    /// Version 2 configuration with pipeline-based architecture
    /// CLEAN: No more legacy workarounds!
    /// </summary>
    public class CamBridgeSettingsV2
    {
        /// <summary>
        /// Settings version for future compatibility
        /// </summary>
        public string Version { get; set; } = "2.0";

        /// <summary>
        /// List of configured pipelines
        /// </summary>
        public List<PipelineConfiguration> Pipelines { get; set; } = new();

        /// <summary>
        /// Reusable mapping sets
        /// </summary>
        public List<MappingSet> MappingSets { get; set; } = new();

        /// <summary>
        /// Global DICOM settings (can be overridden per pipeline)
        /// </summary>
        public DicomSettings GlobalDicomSettings { get; set; } = new();

        /// <summary>
        /// Global defaults for new pipelines
        /// </summary>
        public ProcessingOptions DefaultProcessingOptions { get; set; } = new();

        /// <summary>
        /// Logging configuration (remains global)
        /// </summary>
        public LoggingSettings Logging { get; set; } = new();

        /// <summary>
        /// Service configuration (remains global)
        /// </summary>
        public ServiceSettings Service { get; set; } = new();

        /// <summary>
        /// Notification settings (remains global)
        /// </summary>
        public NotificationSettings Notifications { get; set; } = new();

        /// <summary>
        /// ExifTool executable path (global setting)
        /// </summary>
        public string ExifToolPath { get; set; } = "Tools\\exiftool.exe";

        /// <summary>
        /// Validate the configuration
        /// </summary>
        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                // At least one pipeline must be configured
                if (Pipelines.Count == 0)
                    return false;

                // All enabled pipelines must be valid
                if (Pipelines.Any(p => p.Enabled && !p.IsValid))
                    return false;

                // All pipelines must have valid mapping sets
                var mappingSetIds = MappingSets.Select(m => m.Id).ToHashSet();
                if (Pipelines.Any(p => p.MappingSetId.HasValue && !mappingSetIds.Contains(p.MappingSetId.Value)))
                    return false;

                return true;
            }
        }
    }

    // All other classes remain the same (DicomSettings, LoggingSettings, etc.)
    // Except FolderConfiguration is REMOVED!

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

        /// <summary>
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

        /// <summary>
        /// API port for web interface
        /// </summary>
        public int ApiPort { get; set; } = 5111;
    }

    /// <summary>
    /// Configuration for the notification system
    /// Part of SystemSettings - Compatible with legacy NotificationService
    /// </summary>
    public class NotificationSettings
    {
        /// <summary>
        /// Enable or disable the notification system
        /// </summary>
        public bool Enabled { get; set; } = true;

        // === LEGACY PROPERTIES FOR COMPATIBILITY ===

        /// <summary>
        /// Enable Windows Event Log notifications
        /// </summary>
        public bool EnableEventLog { get; set; } = true;

        /// <summary>
        /// Enable email notifications
        /// </summary>
        public bool EnableEmail { get; set; } = false;

        /// <summary>
        /// Minimum log level for email notifications (0=Trace, 1=Debug, 2=Info, 3=Warning, 4=Error, 5=Critical)
        /// </summary>
        public int MinimumEmailLevel { get; set; } = 4; // Error level

        /// <summary>
        /// Dead letter queue threshold for notifications
        /// </summary>
        public int DeadLetterThreshold { get; set; } = 100;

        /// <summary>
        /// Send daily summary emails
        /// </summary>
        public bool SendDailySummary { get; set; } = true;

        /// <summary>
        /// Hour to send daily summary (0-23)
        /// </summary>
        public int DailySummaryHour { get; set; } = 8;

        // === NEW STRUCTURED PROPERTIES ===

        /// <summary>
        /// Email notification settings
        /// </summary>
        public EmailSettings Email { get; set; } = new();

        /// <summary>
        /// Windows Event Log settings
        /// </summary>
        public EventLogSettings EventLog { get; set; } = new();

        /// <summary>
        /// Webhook notification settings
        /// </summary>
        public WebhookSettings Webhook { get; set; } = new();

        /// <summary>
        /// Notification rules and filters
        /// </summary>
        public NotificationRules Rules { get; set; } = new();
    }

    // Rest of the notification classes remain unchanged...

    public class EmailSettings
    {
        public bool Enabled { get; set; } = false;

        // Legacy properties for compatibility
        public string SmtpHost { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;

        // New structured properties
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public bool UseSsl { get; set; } = true;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Should be encrypted in production
        public string FromAddress { get; set; } = string.Empty;
        public string FromDisplayName { get; set; } = "CamBridge Service";
        public List<string> ToAddresses { get; set; } = new();
        public List<string> CcAddresses { get; set; } = new();
        public string SubjectPrefix { get; set; } = "[CamBridge]";
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Legacy compatibility - returns first address from ToAddresses
        /// </summary>
        public string GetPrimaryToAddress() => ToAddresses?.FirstOrDefault() ?? To;

        /// <summary>
        /// Legacy compatibility - sets both To and ToAddresses
        /// </summary>
        public void SetToAddress(string address)
        {
            To = address;
            if (!string.IsNullOrEmpty(address) && !ToAddresses.Contains(address))
            {
                ToAddresses.Add(address);
            }
        }
    }

    public class EventLogSettings
    {
        public bool Enabled { get; set; } = true;
        public string LogName { get; set; } = "Application";
        public string SourceName { get; set; } = "CamBridge";
        public bool CreateSourceIfMissing { get; set; } = true;
    }

    public class WebhookSettings
    {
        public bool Enabled { get; set; } = false;
        public string Url { get; set; } = string.Empty;
        public string Method { get; set; } = "POST";
        public Dictionary<string, string> Headers { get; set; } = new();
        public string ContentType { get; set; } = "application/json";
        public int TimeoutSeconds { get; set; } = 30;
        public int RetryCount { get; set; } = 3;
        public int RetryDelaySeconds { get; set; } = 5;
    }

    public class NotificationRules
    {
        /// <summary>
        /// Minimum time between notifications of the same type (anti-spam)
        /// </summary>
        public int MinimumIntervalMinutes { get; set; } = 5;

        /// <summary>
        /// Events that trigger notifications
        /// </summary>
        public NotificationTriggers Triggers { get; set; } = new();

        /// <summary>
        /// Daily summary settings
        /// </summary>
        public DailySummarySettings DailySummary { get; set; } = new();

        /// <summary>
        /// Batch notification settings
        /// </summary>
        public BatchNotificationSettings Batching { get; set; } = new();
    }

    public class NotificationTriggers
    {
        public bool OnServiceStart { get; set; } = true;
        public bool OnServiceStop { get; set; } = true;
        public bool OnError { get; set; } = true;
        public bool OnWarning { get; set; } = false;
        public bool OnSuccess { get; set; } = false;
        public bool OnPipelineComplete { get; set; } = true;
        public bool OnFolderWatchError { get; set; } = true;
        public bool OnConfigurationChange { get; set; } = true;
        public bool OnHealthCheckFailure { get; set; } = true;

        /// <summary>
        /// Error count threshold before notification
        /// </summary>
        public int ErrorThreshold { get; set; } = 5;

        /// <summary>
        /// Time window for error threshold (minutes)
        /// </summary>
        public int ErrorThresholdWindowMinutes { get; set; } = 60;
    }

    public class DailySummarySettings
    {
        public bool Enabled { get; set; } = true;
        public TimeSpan SendTime { get; set; } = new TimeSpan(8, 0, 0); // 8:00 AM
        public bool IncludeStatistics { get; set; } = true;
        public bool IncludeErrors { get; set; } = true;
        public bool IncludeWarnings { get; set; } = false;
        public bool OnlyIfActivity { get; set; } = true;
    }

    public class BatchNotificationSettings
    {
        public bool Enabled { get; set; } = false;
        public int BatchSize { get; set; } = 10;
        public int BatchWindowMinutes { get; set; } = 15;
        public bool GroupBySeverity { get; set; } = true;
    }

    public enum NotificationSeverity
    {
        Information,
        Success,
        Warning,
        Error,
        Critical
    }

    public enum NotificationLevel
    {
        None = 0,
        Critical = 1,
        Error = 2,
        Warning = 3,
        Information = 4,
        Debug = 5,
        All = 6
    }

    public static class NotificationLevelExtensions
    {
        public static int ToInt(this NotificationLevel level)
        {
            return (int)level;
        }

        public static NotificationLevel ToNotificationLevel(this int value)
        {
            if (Enum.IsDefined(typeof(NotificationLevel), value))
                return (NotificationLevel)value;
            return NotificationLevel.Error; // Default
        }
    }

    public enum NotificationEventType
    {
        ServiceStarted,
        ServiceStopped,
        ServiceError,
        ProcessingStarted,
        ProcessingCompleted,
        ProcessingFailed,
        ConfigurationChanged,
        HealthCheckPassed,
        HealthCheckFailed,
        FolderWatchStarted,
        FolderWatchStopped,
        FolderWatchError,
        DailySummary,
        Custom
    }
}
