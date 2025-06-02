namespace CamBridge.Core
{
    /// <summary>
    /// Configuration settings for notifications
    /// </summary>
    public class NotificationSettings
    {
        /// <summary>
        /// Enable desktop notifications
        /// </summary>
        public bool EnableDesktopNotifications { get; set; } = true;

        /// <summary>
        /// Enable Windows Event Log notifications
        /// </summary>
        public bool EnableEventLog { get; set; } = true;

        /// <summary>
        /// Enable email notifications
        /// </summary>
        public bool EnableEmail { get; set; } = false;

        /// <summary>
        /// Email configuration settings
        /// </summary>
        public EmailSettings Email { get; set; } = new EmailSettings();

        /// <summary>
        /// SMTP server hostname or IP address
        /// </summary>
        public string SmtpServer { get; set; } = string.Empty;

        /// <summary>
        /// SMTP server port
        /// </summary>
        public int SmtpPort { get; set; } = 587;

        /// <summary>
        /// Use SSL/TLS for SMTP connection
        /// </summary>
        public bool SmtpUseSsl { get; set; } = true;

        /// <summary>
        /// SMTP authentication username
        /// </summary>
        public string SmtpUsername { get; set; } = string.Empty;

        /// <summary>
        /// SMTP authentication password
        /// </summary>
        public string SmtpPassword { get; set; } = string.Empty;

        /// <summary>
        /// Email sender address
        /// </summary>
        public string SmtpFrom { get; set; } = string.Empty;

        /// <summary>
        /// Email recipient addresses (comma-separated)
        /// </summary>
        public string SmtpTo { get; set; } = string.Empty;

        /// <summary>
        /// Include detailed error information in notifications
        /// </summary>
        public bool IncludeDetailedErrors { get; set; } = true;

        /// <summary>
        /// Notification levels to include
        /// </summary>
        public NotificationLevel MinimumLevel { get; set; } = NotificationLevel.Warning;

        /// <summary>
        /// Minimum level for email notifications
        /// </summary>
        public NotificationLevel MinimumEmailLevel { get; set; } = NotificationLevel.Error;

        /// <summary>
        /// Maximum notifications per hour (rate limiting)
        /// </summary>
        public int MaxNotificationsPerHour { get; set; } = 100;

        /// <summary>
        /// Send daily summary email
        /// </summary>
        public bool EnableDailySummary { get; set; } = false;

        /// <summary>
        /// Send daily summary email (legacy property)
        /// </summary>
        public bool SendDailySummary
        {
            get => EnableDailySummary;
            set => EnableDailySummary = value;
        }

        /// <summary>
        /// Time to send daily summary (24-hour format)
        /// </summary>
        public string DailySummaryTime { get; set; } = "08:00";

        /// <summary>
        /// Hour to send daily summary (0-23)
        /// </summary>
        public int DailySummaryHour
        {
            get
            {
                if (TimeSpan.TryParse(DailySummaryTime, out var time))
                    return time.Hours;
                return 8; // Default to 8 AM
            }
            set => DailySummaryTime = $"{value:D2}:00";
        }

        /// <summary>
        /// Threshold for dead letter queue alerts
        /// </summary>
        public int DeadLetterThreshold { get; set; } = 10;

        // Legacy properties for backward compatibility
        [Obsolete("Use SmtpServer instead")]
        public string SmtpHost
        {
            get => SmtpServer;
            set => SmtpServer = value;
        }

        [Obsolete("Use SmtpTo instead")]
        public string EmailTo
        {
            get => SmtpTo;
            set => SmtpTo = value;
        }
    }

    /// <summary>
    /// Email-specific configuration settings
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// Enable email notifications
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// SMTP server hostname
        /// </summary>
        public string SmtpServer { get; set; } = string.Empty;

        /// <summary>
        /// SMTP server hostname (alias for SmtpServer)
        /// </summary>
        public string SmtpHost
        {
            get => SmtpServer;
            set => SmtpServer = value;
        }

        /// <summary>
        /// SMTP server port
        /// </summary>
        public int SmtpPort { get; set; } = 587;

        /// <summary>
        /// Use SSL/TLS
        /// </summary>
        public bool UseSsl { get; set; } = true;

        /// <summary>
        /// SMTP username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// SMTP password
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Sender email address
        /// </summary>
        public string From { get; set; } = string.Empty;

        /// <summary>
        /// Recipient email addresses (comma-separated)
        /// </summary>
        public string To { get; set; } = string.Empty;
    }

    /// <summary>
    /// Notification severity levels
    /// </summary>
    public enum NotificationLevel
    {
        /// <summary>
        /// Informational messages
        /// </summary>
        Information = 0,

        /// <summary>
        /// Warning messages
        /// </summary>
        Warning = 1,

        /// <summary>
        /// Error messages
        /// </summary>
        Error = 2,

        /// <summary>
        /// Critical error messages
        /// </summary>
        Critical = 3
    }
}
