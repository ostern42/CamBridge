using System.Collections.Generic;

namespace CamBridge.Core
{
    /// <summary>
    /// Settings for notification system
    /// </summary>
    public class NotificationSettings
    {
        public bool EnableEmail { get; set; }
        public bool EnableEventLog { get; set; } = true;
        public string? EmailFrom { get; set; }
        public string? EmailTo { get; set; }
        public string? SmtpHost { get; set; }
        public int SmtpPort { get; set; } = 587;
        public bool SmtpUseSsl { get; set; } = true;
        public string? SmtpUsername { get; set; }
        public string? SmtpPassword { get; set; }
        public string MinimumEmailLevel { get; set; } = "Warning";
        public int MaxEmailsPerHour { get; set; } = 10;
        public int ThrottleMinutes { get; set; } = 15;
        public bool SendDailySummary { get; set; } = true;
        public int DailySummaryHour { get; set; } = 8;
        public int DeadLetterThreshold { get; set; } = 50;
    }
}
