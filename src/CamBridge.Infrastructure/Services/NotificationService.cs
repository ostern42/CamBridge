using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Service for sending notifications via email and Windows Event Log
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly NotificationSettings _settings;
        private readonly EventLog? _eventLog;
        private readonly SmtpClient? _smtpClient;
        private readonly SemaphoreSlim _emailThrottle;
        private readonly Dictionary<string, DateTime> _lastNotificationTimes = new();
        private readonly object _throttleLock = new();

        public NotificationService(
            ILogger<NotificationService> logger,
            IOptions<NotificationSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

            _emailThrottle = new SemaphoreSlim(_settings.MaxEmailsPerHour, _settings.MaxEmailsPerHour);

            // Initialize Event Log
            if (_settings.EnableEventLog)
            {
                try
                {
                    var sourceName = "CamBridge Service";
                    var logName = "Application";

                    if (!EventLog.SourceExists(sourceName))
                    {
                        EventLog.CreateEventSource(sourceName, logName);
                    }

                    _eventLog = new EventLog(logName)
                    {
                        Source = sourceName
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize Event Log");
                }
            }

            // Initialize SMTP client
            if (_settings.EnableEmail && !string.IsNullOrEmpty(_settings.SmtpHost))
            {
                try
                {
                    _smtpClient = new SmtpClient(_settings.SmtpHost)
                    {
                        Port = _settings.SmtpPort,
                        EnableSsl = _settings.SmtpUseSsl,
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };

                    if (!string.IsNullOrEmpty(_settings.SmtpUsername))
                    {
                        _smtpClient.Credentials = new NetworkCredential(
                            _settings.SmtpUsername,
                            _settings.SmtpPassword);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize SMTP client");
                }
            }
        }

        /// <summary>
        /// Sends a critical error notification
        /// </summary>
        public async Task NotifyCriticalErrorAsync(string title, string message, Exception? exception = null)
        {
            var fullMessage = BuildErrorMessage(message, exception);

            // Log to Event Log
            LogToEventLog(title, fullMessage, EventLogEntryType.Error);

            // Send email if enabled
            if (ShouldSendEmail(NotificationLevel.Critical))
            {
                await SendEmailAsync(
                    $"[CRITICAL] CamBridge: {title}",
                    fullMessage,
                    NotificationLevel.Critical);
            }

            _logger.LogCritical(exception, "{Title}: {Message}", title, message);
        }

        /// <summary>
        /// Sends a warning notification
        /// </summary>
        public async Task NotifyWarningAsync(string title, string message)
        {
            // Log to Event Log
            LogToEventLog(title, message, EventLogEntryType.Warning);

            // Send email if enabled and not throttled
            if (ShouldSendEmail(NotificationLevel.Warning))
            {
                await SendEmailAsync(
                    $"[WARNING] CamBridge: {title}",
                    message,
                    NotificationLevel.Warning);
            }

            _logger.LogWarning("{Title}: {Message}", title, message);
        }

        /// <summary>
        /// Sends an informational notification
        /// </summary>
        public async Task NotifyInfoAsync(string title, string message)
        {
            // Log to Event Log
            LogToEventLog(title, message, EventLogEntryType.Information);

            // Send email only if explicitly enabled for info level
            if (ShouldSendEmail(NotificationLevel.Info))
            {
                await SendEmailAsync(
                    $"[INFO] CamBridge: {title}",
                    message,
                    NotificationLevel.Info);
            }

            _logger.LogInformation("{Title}: {Message}", title, message);
        }

        /// <summary>
        /// Sends a dead letter queue threshold notification
        /// </summary>
        public async Task NotifyDeadLetterThresholdAsync(int count, DeadLetterStatistics statistics)
        {
            var message = BuildDeadLetterMessage(count, statistics);

            await NotifyWarningAsync(
                $"Dead Letter Queue Threshold Exceeded ({count} items)",
                message);
        }

        /// <summary>
        /// Sends a processing summary notification
        /// </summary>
        public async Task NotifyDailySummaryAsync(ProcessingSummary summary)
        {
            var message = BuildSummaryMessage(summary);

            // Always log to Event Log
            LogToEventLog("Daily Processing Summary", message, EventLogEntryType.Information);

            // Send email if configured
            if (_settings.SendDailySummary)
            {
                await SendEmailAsync(
                    $"CamBridge Daily Summary - {DateTime.Today:yyyy-MM-dd}",
                    message,
                    NotificationLevel.Info,
                    isThrottled: false); // Daily summaries bypass throttling
            }
        }

        private void LogToEventLog(string title, string message, EventLogEntryType type)
        {
            if (_eventLog == null) return;

            try
            {
                // Event Log has a 32KB limit
                var truncatedMessage = message.Length > 30000
                    ? message.Substring(0, 30000) + "\n... (truncated)"
                    : message;

                _eventLog.WriteEntry($"{title}\n\n{truncatedMessage}", type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write to Event Log");
            }
        }

        private async Task SendEmailAsync(string subject, string body, NotificationLevel level, bool isThrottled = true)
        {
            if (_smtpClient == null || string.IsNullOrEmpty(_settings.EmailTo))
                return;

            try
            {
                // Check throttling
                if (isThrottled && !await _emailThrottle.WaitAsync(0))
                {
                    _logger.LogWarning("Email throttle limit reached, skipping notification");
                    return;
                }

                using var message = new MailMessage
                {
                    From = new MailAddress(_settings.EmailFrom),
                    Subject = subject,
                    Body = FormatEmailBody(body),
                    IsBodyHtml = true,
                    Priority = level == NotificationLevel.Critical ? MailPriority.High : MailPriority.Normal
                };

                foreach (var recipient in _settings.EmailTo.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    message.To.Add(recipient.Trim());
                }

                await _smtpClient.SendMailAsync(message);

                _logger.LogInformation("Email notification sent: {Subject}", subject);

                // Reset throttle after configured period
                if (isThrottled)
                {
                    _ = Task.Delay(TimeSpan.FromHours(1))
                        .ContinueWith(_ => _emailThrottle.Release());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email notification");
            }
        }

        private bool ShouldSendEmail(NotificationLevel level)
        {
            if (!_settings.EnableEmail || _smtpClient == null)
                return false;

            // Check notification level
            if (level < _settings.MinimumEmailLevel)
                return false;

            // Check throttling for non-critical notifications
            if (level != NotificationLevel.Critical)
            {
                lock (_throttleLock)
                {
                    var key = $"{level}";
                    if (_lastNotificationTimes.TryGetValue(key, out var lastTime))
                    {
                        var timeSinceLastNotification = DateTime.UtcNow - lastTime;
                        if (timeSinceLastNotification < TimeSpan.FromMinutes(_settings.ThrottleMinutes))
                        {
                            return false;
                        }
                    }
                    _lastNotificationTimes[key] = DateTime.UtcNow;
                }
            }

            return true;
        }

        private string BuildErrorMessage(string message, Exception? exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine(message);

            if (exception != null)
            {
                sb.AppendLine();
                sb.AppendLine("Exception Details:");
                sb.AppendLine($"Type: {exception.GetType().Name}");
                sb.AppendLine($"Message: {exception.Message}");
                sb.AppendLine();
                sb.AppendLine("Stack Trace:");
                sb.AppendLine(exception.StackTrace);

                if (exception.InnerException != null)
                {
                    sb.AppendLine();
                    sb.AppendLine("Inner Exception:");
                    sb.AppendLine($"Type: {exception.InnerException.GetType().Name}");
                    sb.AppendLine($"Message: {exception.InnerException.Message}");
                }
            }

            sb.AppendLine();
            sb.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Machine: {Environment.MachineName}");

            return sb.ToString();
        }

        private string BuildDeadLetterMessage(int count, DeadLetterStatistics stats)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"The dead letter queue has reached {count} items.");
            sb.AppendLine();
            sb.AppendLine("Statistics:");
            sb.AppendLine($"- Last 24 hours: {stats.Last24HoursCount} items");
            sb.AppendLine($"- Last 7 days: {stats.LastWeekCount} items");
            sb.AppendLine($"- Total size: {stats.TotalSizeFormatted}");
            sb.AppendLine();

            if (stats.ErrorCategories.Any())
            {
                sb.AppendLine("Error Categories:");
                foreach (var category in stats.ErrorCategories.OrderByDescending(x => x.Value))
                {
                    sb.AppendLine($"- {category.Key}: {category.Value} items");
                }
            }

            if (stats.OldestItem != null)
            {
                sb.AppendLine();
                sb.AppendLine($"Oldest item: {stats.OldestItem.FileName} (added {stats.OldestItem.AddedAt:yyyy-MM-dd HH:mm})");
            }

            return sb.ToString();
        }

        private string BuildSummaryMessage(ProcessingSummary summary)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Processing Summary for {summary.Date:yyyy-MM-dd}");
            sb.AppendLine("=" + new string('=', 40));
            sb.AppendLine();
            sb.AppendLine($"Total Processed: {summary.TotalProcessed}");
            sb.AppendLine($"Successful: {summary.Successful} ({summary.SuccessRate:F1}%)");
            sb.AppendLine($"Failed: {summary.Failed}");
            sb.AppendLine();

            if (summary.ProcessingTimeSeconds > 0)
            {
                sb.AppendLine($"Total Processing Time: {TimeSpan.FromSeconds(summary.ProcessingTimeSeconds):hh\\:mm\\:ss}");
                sb.AppendLine($"Average Time per File: {summary.AverageProcessingTimeSeconds:F2} seconds");
                sb.AppendLine();
            }

            if (summary.TopErrors.Any())
            {
                sb.AppendLine("Top Errors:");
                foreach (var error in summary.TopErrors.Take(5))
                {
                    sb.AppendLine($"- {error.Key}: {error.Value} occurrences");
                }
                sb.AppendLine();
            }

            if (summary.DeadLetterCount > 0)
            {
                sb.AppendLine($"Dead Letter Queue: {summary.DeadLetterCount} items");
            }

            sb.AppendLine();
            sb.AppendLine($"Service Uptime: {summary.Uptime:dd\\.hh\\:mm\\:ss}");

            return sb.ToString();
        }

        private string FormatEmailBody(string body)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
        pre {{ background-color: #f4f4f4; padding: 10px; border-radius: 5px; overflow-x: auto; }}
        .header {{ background-color: #2c3e50; color: white; padding: 20px; }}
        .content {{ padding: 20px; }}
        .footer {{ background-color: #ecf0f1; padding: 10px; text-align: center; font-size: 0.9em; }}
    </style>
</head>
<body>
    <div class='header'>
        <h2>CamBridge Service Notification</h2>
    </div>
    <div class='content'>
        <pre>{System.Web.HttpUtility.HtmlEncode(body)}</pre>
    </div>
    <div class='footer'>
        <p>© 2025 Claude's Improbably Reliable Software Solutions | CamBridge v0.3.2</p>
    </div>
</body>
</html>";
        }

        public void Dispose()
        {
            _emailThrottle?.Dispose();
            _smtpClient?.Dispose();
            _eventLog?.Dispose();
        }
    }

    /// <summary>
    /// Interface for notification service
    /// </summary>
    public interface INotificationService : IDisposable
    {
        Task NotifyCriticalErrorAsync(string title, string message, Exception? exception = null);
        Task NotifyWarningAsync(string title, string message);
        Task NotifyInfoAsync(string title, string message);
        Task NotifyDeadLetterThresholdAsync(int count, DeadLetterStatistics statistics);
        Task NotifyDailySummaryAsync(ProcessingSummary summary);
    }

    /// <summary>
    /// Notification severity levels
    /// </summary>
    public enum NotificationLevel
    {
        Info = 0,
        Warning = 1,
        Critical = 2
    }

    /// <summary>
    /// Daily processing summary
    /// </summary>
    public class ProcessingSummary
    {
        public DateTime Date { get; set; }
        public int TotalProcessed { get; set; }
        public int Successful { get; set; }
        public int Failed { get; set; }
        public double SuccessRate => TotalProcessed > 0 ? (double)Successful / TotalProcessed * 100 : 0;
        public double ProcessingTimeSeconds { get; set; }
        public double AverageProcessingTimeSeconds => TotalProcessed > 0 ? ProcessingTimeSeconds / TotalProcessed : 0;
        public Dictionary<string, int> TopErrors { get; set; } = new();
        public int DeadLetterCount { get; set; }
        public TimeSpan Uptime { get; set; }
    }

    /// <summary>
    /// Notification settings
    /// </summary>
    public class NotificationSettings
    {
        public bool EnableEmail { get; set; } = false;
        public bool EnableEventLog { get; set; } = true;
        public string EmailFrom { get; set; } = "cambridge@example.com";
        public string EmailTo { get; set; } = string.Empty; // Semicolon-separated list
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public bool SmtpUseSsl { get; set; } = true;
        public string? SmtpUsername { get; set; }
        public string? SmtpPassword { get; set; }
        public NotificationLevel MinimumEmailLevel { get; set; } = NotificationLevel.Warning;
        public int MaxEmailsPerHour { get; set; } = 10;
        public int ThrottleMinutes { get; set; } = 15;
        public bool SendDailySummary { get; set; } = true;
    }
}
