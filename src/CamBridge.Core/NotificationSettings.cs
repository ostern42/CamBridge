// src\CamBridge.Core\NotificationSettings.cs
// Version: 0.7.3
// Description: Notification configuration settings
// © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.Linq;

namespace CamBridge.Core
{
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

    /// <summary>
    /// Email notification configuration
    /// </summary>
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

    /// <summary>
    /// Windows Event Log configuration
    /// </summary>
    public class EventLogSettings
    {
        public bool Enabled { get; set; } = true;
        public string LogName { get; set; } = "Application";
        public string SourceName { get; set; } = "CamBridge";
        public bool CreateSourceIfMissing { get; set; } = true;
    }

    /// <summary>
    /// Webhook notification configuration
    /// </summary>
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

    /// <summary>
    /// Notification rules and filtering
    /// </summary>
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

    /// <summary>
    /// Events that trigger notifications
    /// </summary>
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

    /// <summary>
    /// Daily summary configuration
    /// </summary>
    public class DailySummarySettings
    {
        public bool Enabled { get; set; } = true;
        public TimeSpan SendTime { get; set; } = new TimeSpan(8, 0, 0); // 8:00 AM
        public bool IncludeStatistics { get; set; } = true;
        public bool IncludeErrors { get; set; } = true;
        public bool IncludeWarnings { get; set; } = false;
        public bool OnlyIfActivity { get; set; } = true;
    }

    /// <summary>
    /// Batch notification configuration
    /// </summary>
    public class BatchNotificationSettings
    {
        public bool Enabled { get; set; } = false;
        public int BatchSize { get; set; } = 10;
        public int BatchWindowMinutes { get; set; } = 15;
        public bool GroupBySeverity { get; set; } = true;
    }

    /// <summary>
    /// Notification severity levels
    /// </summary>
    public enum NotificationSeverity
    {
        Information,
        Success,
        Warning,
        Error,
        Critical
    }

    /// <summary>
    /// Notification level (for legacy compatibility)
    /// </summary>
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

    /// <summary>
    /// Extension methods for NotificationLevel conversion
    /// </summary>
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

    /// <summary>
    /// Notification event types
    /// </summary>
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
