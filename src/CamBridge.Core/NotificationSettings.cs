// src\CamBridge.Core\NotificationSettings.cs
// Version: 0.6.5
// Description: Notification settings with proper null safety

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CamBridge.Core
{
    /// <summary>
    /// Settings for notifications
    /// </summary>
    public class NotificationSettings : INotifyPropertyChanged
    {
        private bool _enableEventLog = true;
        private bool _enableEmail = false;
        private EmailSettings _email = new();
        private NotificationLevel _minimumEmailLevel = NotificationLevel.Warning;
        private bool _sendDailySummary = false;
        private int _dailySummaryHour = 8;

        /// <summary>
        /// Enable Windows Event Log notifications
        /// </summary>
        public bool EnableEventLog
        {
            get => _enableEventLog;
            set { _enableEventLog = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Enable email notifications
        /// </summary>
        public bool EnableEmail
        {
            get => _enableEmail;
            set { _enableEmail = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Email settings (never null)
        /// </summary>
        public EmailSettings Email
        {
            get => _email;
            set { _email = value ?? new EmailSettings(); OnPropertyChanged(); }
        }

        /// <summary>
        /// Minimum level for email notifications
        /// </summary>
        public NotificationLevel MinimumEmailLevel
        {
            get => _minimumEmailLevel;
            set { _minimumEmailLevel = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Send daily summary email
        /// </summary>
        public bool SendDailySummary
        {
            get => _sendDailySummary;
            set { _sendDailySummary = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Hour to send daily summary (0-23)
        /// </summary>
        public int DailySummaryHour
        {
            get => _dailySummaryHour;
            set { _dailySummaryHour = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Dead letter threshold for notifications
        /// </summary>
        public int DeadLetterThreshold { get; set; } = 100;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Email configuration settings
    /// </summary>
    public class EmailSettings : INotifyPropertyChanged
    {
        private string? _from;
        private string? _to;
        private string? _smtpHost;
        private int _smtpPort = 587;
        private bool _useSsl = true;
        private string? _username;
        private string? _password;

        /// <summary>
        /// From email address
        /// </summary>
        public string? From
        {
            get => _from;
            set { _from = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// To email addresses (semicolon separated)
        /// </summary>
        public string? To
        {
            get => _to;
            set { _to = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// SMTP host
        /// </summary>
        public string? SmtpHost
        {
            get => _smtpHost;
            set { _smtpHost = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// SMTP port
        /// </summary>
        public int SmtpPort
        {
            get => _smtpPort;
            set { _smtpPort = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Use SSL/TLS
        /// </summary>
        public bool UseSsl
        {
            get => _useSsl;
            set { _useSsl = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// SMTP username
        /// </summary>
        public string? Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// SMTP password
        /// </summary>
        public string? Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Notification severity levels
    /// </summary>
    public enum NotificationLevel
    {
        /// <summary>
        /// Informational messages
        /// </summary>
        Information,

        /// <summary>
        /// Warning messages
        /// </summary>
        Warning,

        /// <summary>
        /// Error messages
        /// </summary>
        Error,

        /// <summary>
        /// Critical errors
        /// </summary>
        Critical
    }
}
