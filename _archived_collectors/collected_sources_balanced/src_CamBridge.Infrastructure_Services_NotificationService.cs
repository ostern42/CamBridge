// src/CamBridge.Infrastructure/Services/NotificationService.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Implementation of notification service for email and event log
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly NotificationSettings _settings;
        private readonly Dictionary<string, DateTime> _emailThrottle = new();
        private readonly object _throttleLock = new();
        private int _emailsSentInCurrentHour = 0;
        private DateTime _currentHourStart = DateTime.UtcNow;

        public NotificationService(
            ILogger<NotificationService> logger,
            IOptions<NotificationSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        public Task NotifyInfoAsync(string subject, string message)
        {
            _logger.LogInformation("Notification: {Subject} - {Message}", subject, message);

            if (_settings.EnableEventLog)
            {
                WriteEventLog(subject, message, EventLogEntryType.Information);
            }

            // Info messages typically don't trigger emails unless configured
            return Task.CompletedTask;
        }

        public async Task NotifyWarningAsync(string subject, string message)
        {
            _logger.LogWarning("Notification: {Subject} - {Message}", subject, message);

            if (_settings.EnableEventLog)
            {
                WriteEventLog(subject, message, EventLogEntryType.Warning);
            }

            if (ShouldSendEmail("Warning"))
            {
                await SendEmailAsync(subject, message, "Warning");
            }
        }

        public async Task NotifyErrorAsync(string subject, string message, Exception? exception = null)
        {
            if (exception != null)
            {
                _logger.LogError(exception, "Notification: {Subject} - {Message}", subject, message);
            }
            else
            {
                _logger.LogError("Notification: {Subject} - {Message}", subject, message);
            }

            if (_settings.EnableEventLog)
            {
                var fullMessage = exception != null
                    ? $"{message}\n\nException:\n{exception}"
                    : message;
                WriteEventLog(subject, fullMessage, EventLogEntryType.Error);
            }

            if (ShouldSendEmail("Error"))
            {
                var emailMessage = exception != null
                    ? $"{message}\n\nException Details:\n{exception}"
                    : message;
                await SendEmailAsync(subject, emailMessage, "Error");
            }
        }

        public async Task NotifyCriticalErrorAsync(string subject, string message, Exception? exception = null)
        {
            if (exception != null)
            {
                _logger.LogCritical(exception, "Critical Notification: {Subject} - {Message}", subject, message);
            }
            else
            {
                _logger.LogCritical("Critical Notification: {Subject} - {Message}", subject, message);
            }

            if (_settings.EnableEventLog)
            {
                var fullMessage = exception != null
                    ? $"CRITICAL: {message}\n\nException:\n{exception}"
                    : $"CRITICAL: {message}";
                WriteEventLog(subject, fullMessage, EventLogEntryType.Error);
            }

            // Critical errors always attempt to send email if enabled
            if (_settings.EnableEmail && IsEmailConfigured())
            {
                var emailMessage = exception != null
                    ? $"CRITICAL ERROR:\n\n{message}\n\nException Details:\n{exception}"
                    : $"CRITICAL ERROR:\n\n{message}";
                await SendEmailAsync(subject, emailMessage, "Critical", bypassThrottle: true);
            }
        }

        public async Task NotifyDeadLetterThresholdAsync(int count, DeadLetterStatistics statistics)
        {
            var subject = $"CamBridge: Dead Letter Threshold Exceeded ({count} items)";
            var message = $"The dead letter queue has {count} items, which exceeds the configured threshold of {_settings.DeadLetterThreshold}.\n\n" +
                         $"Please investigate and reprocess or remove failed items.\n\n" +
                         $"Statistics:\n{statistics}";

            await NotifyWarningAsync(subject, message);
        }

        public async Task SendDailySummaryAsync(ProcessingSummary summary)
        {
            var subject = $"CamBridge Daily Summary - {summary.Date:yyyy-MM-dd}";
            var message = FormatDailySummary(summary);

            _logger.LogInformation("Sending daily summary for {Date}", summary.Date);

            if (_settings.EnableEventLog)
            {
                WriteEventLog(subject, message, EventLogEntryType.Information);
            }

            if (_settings.EnableEmail && _settings.SendDailySummary)
            {
                await SendEmailAsync(subject, message, "DailySummary", bypassThrottle: true);
            }
        }

        private bool ShouldSendEmail(string level)
        {
            if (!_settings.EnableEmail || !IsEmailConfigured())
                return false;

            var configuredLevel = Enum.Parse<LogLevel>(_settings.MinimumEmailLevel, true);
            var currentLevel = level switch
            {
                "Warning" => LogLevel.Warning,
                "Error" => LogLevel.Error,
                "Critical" => LogLevel.Critical,
                _ => LogLevel.Information
            };

            return currentLevel >= configuredLevel;
        }

        private bool IsEmailConfigured()
        {
            return !string.IsNullOrWhiteSpace(_settings.SmtpHost) &&
                   !string.IsNullOrWhiteSpace(_settings.EmailFrom) &&
                   !string.IsNullOrWhiteSpace(_settings.EmailTo);
        }

        private async Task SendEmailAsync(string subject, string body, string category, bool bypassThrottle = false)
        {
            try
            {
                if (!bypassThrottle && !CanSendEmail(category))
                {
                    _logger.LogWarning("Email throttled for category {Category}", category);
                    return;
                }

                using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
                {
                    EnableSsl = _settings.SmtpUseSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                if (!string.IsNullOrWhiteSpace(_settings.SmtpUsername))
                {
                    client.Credentials = new NetworkCredential(_settings.SmtpUsername, _settings.SmtpPassword);
                }

                // Fix: Check for null before creating MailAddress
                if (string.IsNullOrWhiteSpace(_settings.EmailFrom))
                {
                    _logger.LogError("Cannot send email: EmailFrom is not configured");
                    return;
                }

                var message = new MailMessage
                {
                    From = new MailAddress(_settings.EmailFrom),
                    Subject = $"[CamBridge] {subject}",
                    Body = body,
                    IsBodyHtml = false
                };

                // Add recipients - Fix: Check for null EmailTo
                if (!string.IsNullOrWhiteSpace(_settings.EmailTo))
                {
                    foreach (var recipient in _settings.EmailTo.Split(';', StringSplitOptions.RemoveEmptyEntries))
                    {
                        message.To.Add(recipient.Trim());
                    }
                }
                else
                {
                    _logger.LogError("Cannot send email: EmailTo is not configured");
                    return;
                }

                await client.SendMailAsync(message);

                RecordEmailSent(category);
                _logger.LogInformation("Email sent: {Subject} to {Recipients}", subject, _settings.EmailTo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email: {Subject}", subject);
            }
        }

        [SupportedOSPlatform("windows")]
        private void WriteEventLog(string source, string message, EventLogEntryType type)
        {
            try
            {
                const string logName = "Application";
                const string sourceName = "CamBridge Service";

                if (!EventLog.SourceExists(sourceName))
                {
                    EventLog.CreateEventSource(sourceName, logName);
                }

                EventLog.WriteEntry(sourceName, $"{source}\n\n{message}", type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write to event log");
            }
        }

        private bool CanSendEmail(string category)
        {
            lock (_throttleLock)
            {
                // Check hourly limit
                var now = DateTime.UtcNow;
                if (now.Subtract(_currentHourStart).TotalHours >= 1)
                {
                    _currentHourStart = now;
                    _emailsSentInCurrentHour = 0;
                }

                if (_emailsSentInCurrentHour >= _settings.MaxEmailsPerHour)
                {
                    return false;
                }

                // Check category throttle
                var throttleKey = $"{category}_{now:yyyyMMddHH}";
                if (_emailThrottle.TryGetValue(throttleKey, out var lastSent))
                {
                    if (now.Subtract(lastSent).TotalMinutes < _settings.ThrottleMinutes)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private void RecordEmailSent(string category)
        {
            lock (_throttleLock)
            {
                _emailsSentInCurrentHour++;
                var now = DateTime.UtcNow;
                var throttleKey = $"{category}_{now:yyyyMMddHH}";
                _emailThrottle[throttleKey] = now;

                // Clean up old entries
                var cutoff = now.AddHours(-2);
                var keysToRemove = _emailThrottle
                    .Where(kvp => kvp.Value < cutoff)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var key in keysToRemove)
                {
                    _emailThrottle.Remove(key);
                }
            }
        }

        private string FormatDailySummary(ProcessingSummary summary)
        {
            var message = $@"Daily Processing Summary for {summary.Date:yyyy-MM-dd}

Total Files Processed: {summary.TotalProcessed:N0}
Successful: {summary.Successful:N0} ({summary.SuccessRate:F1}%)
Failed: {summary.Failed:N0}
Dead Letters: {summary.DeadLetterCount:N0}

Processing Time: {summary.ProcessingTimeSeconds:F1} seconds
Average Time per File: {summary.AverageProcessingTime:F2} seconds
Service Uptime: {summary.Uptime:d\.hh\:mm\:ss}";

            if (summary.TopErrors.Any())
            {
                message += "\n\nTop Errors:\n";
                foreach (var error in summary.TopErrors.OrderByDescending(e => e.Value).Take(5))
                {
                    message += $"  - {error.Key}: {error.Value} occurrences\n";
                }
            }

            message += $"\n\nGenerated at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            message += "\n\n© 2025 Claude's Improbably Reliable Software Solutions";

            return message;
        }
    }
}
