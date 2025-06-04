// src\CamBridge.Core\NotificationSettings.cs
// Version: 0.5.26
// Extended notification settings with all required properties

namespace CamBridge.Core
{
    /// <summary>
    /// Notification configuration settings
    /// </summary>
    public class NotificationSettings
    {
        public bool EnableEmail { get; set; } = false;
        public bool EnableEventLog { get; set; } = true;

        public EmailSettings Email { get; set; } = new();

        public NotificationLevel MinimumEmailLevel { get; set; } = NotificationLevel.Warning;
        public bool SendDailySummary { get; set; } = false;
        public int DailySummaryHour { get; set; } = 8;

        // Additional property required by Infrastructure
        public int DeadLetterThreshold { get; set; } = 50;
    }

    /// <summary>
    /// Email configuration settings
    /// </summary>
    public class EmailSettings
    {
        public string? From { get; set; }
        public string? To { get; set; }
        public string? SmtpHost { get; set; }
        public int SmtpPort { get; set; } = 587;
        public bool UseSsl { get; set; } = true;
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    /// <summary>
    /// Notification severity levels
    /// </summary>
    public enum NotificationLevel
    {
        /// <summary>Informational messages</summary>
        Information,

        /// <summary>Warning messages</summary>
        Warning,

        /// <summary>Error messages</summary>
        Error,

        /// <summary>Critical error messages</summary>
        Critical
    }
}
