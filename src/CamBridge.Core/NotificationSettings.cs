namespace CamBridge.Core
{
    /// <summary>
    /// Configuration for notifications and alerts
    /// </summary>
    public class NotificationSettings
    {
        /// <summary>
        /// Enable email notifications
        /// </summary>
        public bool EnableEmail { get; set; }

        /// <summary>
        /// Enable Windows Event Log notifications
        /// </summary>
        public bool EnableEventLog { get; set; } = true;

        /// <summary>
        /// Email configuration
        /// </summary>
        public EmailSettings Email { get; set; } = new();

        /// <summary>
        /// Minimum log level for email notifications
        /// </summary>
        public string MinimumEmailLevel { get; set; } = "Warning";

        /// <summary>
        /// Maximum emails per hour (throttling)
        /// </summary>
        public int MaxEmailsPerHour { get; set; } = 10;

        /// <summary>
        /// Throttle period in minutes
        /// </summary>
        public int ThrottleMinutes { get; set; } = 15;

        /// <summary>
        /// Send daily summary email
        /// </summary>
        public bool SendDailySummary { get; set; }

        /// <summary>
        /// Hour to send daily summary (0-23)
        /// </summary>
        public int DailySummaryHour { get; set; } = 8;

        /// <summary>
        /// Dead letter threshold for alerts
        /// </summary>
        public int DeadLetterThreshold { get; set; } = 50;
    }

    /// <summary>
    /// Email server configuration
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// From email address
        /// </summary>
        public string? From { get; set; }

        /// <summary>
        /// To email addresses (semicolon separated)
        /// </summary>
        public string? To { get; set; }

        /// <summary>
        /// SMTP server host
        /// </summary>
        public string? SmtpHost { get; set; }

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
        public string? Username { get; set; }

        /// <summary>
        /// SMTP password
        /// </summary>
        public string? Password { get; set; }
    }
}
