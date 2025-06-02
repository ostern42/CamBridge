using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Entities; // Für ProcessingResult
using CamBridge.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DeadLetterStatistics = CamBridge.Infrastructure.Services.DeadLetterStatistics;


// Explicit using alias to avoid ambiguity
using ProcessingResult = CamBridge.Core.Entities.ProcessingResult;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Service for sending notifications via email and Windows Event Log
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly NotificationSettings _settings;
        private readonly string _eventSource = "CamBridge Service";
        private readonly string _eventLog = "Application";
        private readonly List<ProcessingResult> _dailySummaryBuffer = new();
        private DateTime _lastSummaryDate = DateTime.MinValue;

        public NotificationService(
            ILogger<NotificationService> logger,
            IOptions<NotificationSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;

            // Initialize Windows Event Log source if enabled
            if (_settings.EnableEventLog)
            {
                InitializeEventLog();
            }
        }

        /// <summary>
        /// Send info notification
        /// </summary>
        public async Task NotifyInfoAsync(string subject, string message)
        {
            try
            {
                if (_settings.EnableEventLog)
                {
                    WriteEventLog(message, EventLogEntryType.Information);
                }

                if (ShouldSendEmail(LogLevel.Information))
                {
                    await SendEmailAsync(subject, message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send info notification");
            }
        }

        /// <summary>
        /// Send warning notification
        /// </summary>
        public async Task NotifyWarningAsync(string subject, string message)
        {
            try
            {
                if (_settings.EnableEventLog)
                {
                    WriteEventLog(message, EventLogEntryType.Warning);
                }

                if (ShouldSendEmail(LogLevel.Warning))
                {
                    await SendEmailAsync(subject, message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send warning notification");
            }
        }

        /// <summary>
        /// Send error notification
        /// </summary>
        public async Task NotifyErrorAsync(string subject, string message, Exception? exception = null)
        {
            try
            {
                if (_settings.EnableEventLog)
                {
                    WriteEventLog(message, EventLogEntryType.Error, exception);
                }

                if (ShouldSendEmail(LogLevel.Error))
                {
                    var body = BuildErrorEmailBody(message, exception);
                    await SendEmailAsync(subject, body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send error notification");
            }
        }

        /// <summary>
        /// Send critical error notification
        /// </summary>
        public async Task NotifyCriticalErrorAsync(string subject, string message, Exception? exception = null)
        {
            try
            {
                // Critical errors always go to event log
                if (_settings.EnableEventLog)
                {
                    WriteEventLog($"CRITICAL: {message}", EventLogEntryType.Error, exception);
                }

                // Critical errors always attempt email if configured
                if (_settings.EnableEmail && _settings.Email != null)
                {
                    var body = BuildErrorEmailBody($"CRITICAL ERROR: {message}", exception);
                    await SendEmailAsync($"[CRITICAL] {subject}", body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send critical error notification");
            }
        }

        /// <summary>
        /// Send notification when dead letter threshold is exceeded
        /// </summary>
        public async Task NotifyDeadLetterThresholdAsync(int count, DeadLetterStatistics statistics)
        {
            try
            {
                var message = $"Dead letter queue threshold exceeded. Count: {count}, Threshold: {_settings.DeadLetterThreshold}";

                var details = new StringBuilder();
                details.AppendLine(message);
                details.AppendLine();
                details.AppendLine("Statistics:");
                details.AppendLine($"  Total Items: {statistics.TotalCount}");
                details.AppendLine($"  Oldest Item: {statistics.OldestItem?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"}");
                details.AppendLine($"  Newest Item: {statistics.NewestItem?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"}");
                details.AppendLine($"  Total Size: {statistics.GetFormattedSize()}");
                details.AppendLine($"  Average Attempts: {statistics.AverageAttempts:F1}");

                if (statistics.ErrorCategories.Any())
                {
                    details.AppendLine();
                    details.AppendLine("Error Categories:");
                    foreach (var category in statistics.ErrorCategories.OrderByDescending(x => x.Value).Take(5))
                    {
                        details.AppendLine($"  - {category.Key}: {category.Value} occurrences");
                    }
                }

                await NotifyWarningAsync("Dead Letter Threshold Exceeded", details.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send dead letter threshold notification");
            }
        }

        /// <summary>
        /// Send daily summary report
        /// </summary>
        public async Task SendDailySummaryAsync(ProcessingSummary summary)
        {
            if (!_settings.SendDailySummary || _settings.Email == null)
                return;

            try
            {
                var subject = $"Daily Processing Summary - {summary.Date:yyyy-MM-dd}";
                var body = BuildDailySummaryBody(summary);

                await SendEmailAsync(subject, body);
                _logger.LogInformation("Daily summary sent for {Date}", summary.Date);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send daily summary");
            }
        }

        /// <summary>
        /// Send notification about processing result
        /// </summary>
        public async Task NotifyAsync(ProcessingResult result)
        {
            if (result == null) return;

            try
            {
                // Add to daily summary buffer if enabled
                if (_settings.SendDailySummary)
                {
                    lock (_dailySummaryBuffer)
                    {
                        _dailySummaryBuffer.Add(result);
                    }
                }

                // Check if we should send daily summary
                await CheckAndSendDailySummaryAsync();

                // Determine notification level based on result
                var logLevel = result.Success ? LogLevel.Information : LogLevel.Error;

                // Send notifications based on settings
                if (_settings.EnableEventLog)
                {
                    WriteEventLog(result, logLevel);
                }

                if (ShouldSendEmail(logLevel))
                {
                    await SendEmailNotificationAsync(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification for file: {FilePath}", result.FilePath);
            }
        }

        #region Private Methods

        private void InitializeEventLog()
        {
            try
            {
                if (!EventLog.SourceExists(_eventSource))
                {
                    EventLog.CreateEventSource(_eventSource, _eventLog);
                    _logger.LogInformation("Created Windows Event Log source: {Source}", _eventSource);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create Event Log source. Event logging may not work properly.");
            }
        }

        private bool ShouldSendEmail(LogLevel logLevel)
        {
            return _settings.EnableEmail &&
                   _settings.Email != null &&
                   (int)logLevel >= (int)_settings.MinimumEmailLevel;
        }

        private async Task SendEmailAsync(string subject, string body)
        {
            try
            {
                // Check if email settings are properly configured
                if (_settings.Email == null ||
                    string.IsNullOrWhiteSpace(_settings.Email.SmtpHost) ||
                    string.IsNullOrWhiteSpace(_settings.Email.From) ||
                    string.IsNullOrWhiteSpace(_settings.Email.To))
                {
                    _logger.LogWarning("Email settings not properly configured, skipping email notification");
                    return;
                }

                using var client = new SmtpClient(_settings.Email.SmtpHost, _settings.Email.SmtpPort)
                {
                    EnableSsl = _settings.Email.UseSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Timeout = 30000 // 30 seconds
                };

                // Set credentials if provided
                if (!string.IsNullOrWhiteSpace(_settings.Email.Username))
                {
                    client.Credentials = new NetworkCredential(_settings.Email.Username, _settings.Email.Password);
                }

                using var message = new MailMessage
                {
                    From = new MailAddress(_settings.Email.From),
                    Subject = $"[CamBridge] {subject}",
                    Body = body,
                    IsBodyHtml = false,
                    Priority = MailPriority.Normal
                };

                // Add recipients
                var recipients = _settings.Email.To?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
                foreach (var recipient in recipients)
                {
                    var trimmedRecipient = recipient.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedRecipient))
                    {
                        message.To.Add(new MailAddress(trimmedRecipient));
                    }
                }

                if (message.To.Count == 0)
                {
                    _logger.LogWarning("No valid email recipients configured");
                    return;
                }

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully to {RecipientCount} recipients", message.To.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email notification: {Subject}", subject);
            }
        }

        private async Task SendEmailNotificationAsync(ProcessingResult result)
        {
            var fileName = Path.GetFileName(result.FilePath);
            var subject = result.Success
                ? $"Processing Successful: {fileName}"
                : $"Processing Failed: {fileName}";

            var body = BuildProcessingEmailBody(result);
            await SendEmailAsync(subject, body);
        }

        private void WriteEventLog(ProcessingResult result, LogLevel logLevel)
        {
            var eventType = logLevel switch
            {
                LogLevel.Error => EventLogEntryType.Error,
                LogLevel.Warning => EventLogEntryType.Warning,
                _ => EventLogEntryType.Information
            };

            var fileName = Path.GetFileName(result.FilePath);
            var message = result.Success
                ? $"Successfully processed: {fileName}\nOutput: {result.OutputFile}"
                : $"Failed to process: {fileName}\nError: {result.ErrorMessage}";

            WriteEventLog(message, eventType);
        }

        private void WriteEventLog(string message, EventLogEntryType type, Exception? exception = null)
        {
            try
            {
                if (exception != null)
                {
                    message += $"\n\nException: {exception.GetType().Name}\n{exception.Message}\n\nStack Trace:\n{exception.StackTrace}";
                }

                EventLog.WriteEntry(_eventSource, message, type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write to Windows Event Log");
            }
        }

        private string BuildProcessingEmailBody(ProcessingResult result)
        {
            var sb = new StringBuilder();
            var fileName = Path.GetFileName(result.FilePath);

            sb.AppendLine($"File Processing Notification");
            sb.AppendLine($"==========================");
            sb.AppendLine();
            sb.AppendLine($"Timestamp: {result.ProcessedAt:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"File: {fileName}");
            sb.AppendLine($"Path: {result.FilePath}");
            sb.AppendLine($"Status: {(result.Success ? "SUCCESS" : "FAILED")}");

            if (result.Success)
            {
                sb.AppendLine($"Output: {result.OutputFile}");
                if (result.ProcessingTime.HasValue)
                {
                    sb.AppendLine($"Processing Time: {result.ProcessingTime.Value.TotalMilliseconds:F2} ms");
                }
            }
            else
            {
                sb.AppendLine($"Error: {result.ErrorMessage}");
            }

            // Include patient information if available
            if (result.PatientInfo != null)
            {
                sb.AppendLine();
                sb.AppendLine("Patient Information:");
                sb.AppendLine($"  ID: {result.PatientInfo.Id.Value}");
                sb.AppendLine($"  Name: {result.PatientInfo.Name}");
                if (result.PatientInfo.BirthDate.HasValue)
                {
                    sb.AppendLine($"  Birth Date: {result.PatientInfo.BirthDate.Value:yyyy-MM-dd}");
                }
                sb.AppendLine($"  Gender: {result.PatientInfo.Gender}");
            }

            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine($"CamBridge Service v{typeof(NotificationService).Assembly.GetName().Version}");

            return sb.ToString();
        }

        private string BuildErrorEmailBody(string message, Exception? exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Error Notification");
            sb.AppendLine("==================");
            sb.AppendLine();
            sb.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Message: {message}");

            if (exception != null)
            {
                sb.AppendLine();
                sb.AppendLine("Exception Details:");
                sb.AppendLine($"  Type: {exception.GetType().FullName}");
                sb.AppendLine($"  Message: {exception.Message}");
                sb.AppendLine();
                sb.AppendLine("Stack Trace:");
                sb.AppendLine(exception.StackTrace);

                if (exception.InnerException != null)
                {
                    sb.AppendLine();
                    sb.AppendLine("Inner Exception:");
                    sb.AppendLine($"  Type: {exception.InnerException.GetType().FullName}");
                    sb.AppendLine($"  Message: {exception.InnerException.Message}");
                }
            }

            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine($"CamBridge Service v{typeof(NotificationService).Assembly.GetName().Version}");
            sb.AppendLine($"Machine: {Environment.MachineName}");

            return sb.ToString();
        }

        private string BuildDailySummaryBody(ProcessingSummary summary)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Daily Processing Summary - {summary.Date:yyyy-MM-dd}");
            sb.AppendLine("=====================================");
            sb.AppendLine();

            sb.AppendLine($"Total Files Processed: {summary.TotalProcessed}");
            sb.AppendLine($"Successful: {summary.Successful}");
            sb.AppendLine($"Failed: {summary.Failed}");
            sb.AppendLine($"Success Rate: {summary.SuccessRate:F1}%");
            sb.AppendLine($"Average Processing Time: {summary.AverageProcessingTime:F2} ms");
            sb.AppendLine($"Total Processing Time: {summary.ProcessingTimeSeconds:F1} seconds");
            sb.AppendLine($"Service Uptime: {summary.Uptime:d\\.hh\\:mm\\:ss}");

            if (summary.Failed > 0 && summary.TopErrors.Any())
            {
                sb.AppendLine();
                sb.AppendLine("Top Errors:");
                sb.AppendLine("-----------");
                foreach (var error in summary.TopErrors.Take(5))
                {
                    sb.AppendLine($"  • {error.Key}: {error.Value} occurrences");
                }
            }

            if (summary.DeadLetterCount > 0)
            {
                sb.AppendLine();
                sb.AppendLine($"Dead Letter Queue: {summary.DeadLetterCount} items pending");
            }

            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine($"CamBridge Service v{typeof(NotificationService).Assembly.GetName().Version}");
            sb.AppendLine($"Report generated at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            return sb.ToString();
        }

        private string BuildDailySummaryBody(List<ProcessingResult> results)
        {
            var summary = new ProcessingSummary
            {
                Date = DateTime.Today,
                TotalProcessed = results.Count,
                Successful = results.Count(r => r.Success),
                Failed = results.Count(r => !r.Success),
                ProcessingTimeSeconds = results.Where(r => r.ProcessingTime.HasValue)
                    .Sum(r => r.ProcessingTime!.Value.TotalSeconds),
                TopErrors = results.Where(r => !r.Success && !string.IsNullOrEmpty(r.ErrorMessage))
                    .GroupBy(r => r.ErrorMessage!)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .ToDictionary(g => g.Key, g => g.Count()),
                Uptime = TimeSpan.FromHours(24) // Placeholder
            };

            return BuildDailySummaryBody(summary);
        }

        private async Task CheckAndSendDailySummaryAsync()
        {
            if (!_settings.SendDailySummary)
                return;

            var now = DateTime.Now;

            // Check if we've crossed into a new day and it's the configured hour
            if (now.Date > _lastSummaryDate && now.Hour >= _settings.DailySummaryHour)
            {
                List<ProcessingResult> summaryData;
                lock (_dailySummaryBuffer)
                {
                    if (_dailySummaryBuffer.Count == 0)
                        return;

                    summaryData = new List<ProcessingResult>(_dailySummaryBuffer);
                    _dailySummaryBuffer.Clear();
                }

                await SendEmailAsync($"Daily Processing Summary - {DateTime.Now:yyyy-MM-dd}",
                    BuildDailySummaryBody(summaryData));

                _lastSummaryDate = now.Date;
            }
        }

        private LogLevel ParseLogLevel(string level)
        {
            return level?.ToLower() switch
            {
                "trace" => LogLevel.Trace,
                "debug" => LogLevel.Debug,
                "information" => LogLevel.Information,
                "warning" => LogLevel.Warning,
                "error" => LogLevel.Error,
                "critical" => LogLevel.Critical,
                _ => LogLevel.Warning
            };
        }

        #endregion
    }
}
