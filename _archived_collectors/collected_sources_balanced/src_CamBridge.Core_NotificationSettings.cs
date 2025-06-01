namespace CamBridge.Core
{
    /// <summary>
    /// Settings for various notification mechanisms
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
        /// Send daily summary emails
        /// </summary>
        public bool SendDailySummary { get; set; }

        /// <summary>
        /// Hour of day to send daily summary (0-23)
        /// </summary>
        public int DailySummaryHour { get; set; } = 8;
    }

    /// <summary>
    /// Email-specific settings
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
}
