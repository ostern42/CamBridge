// src/CamBridge.Infrastructure/Services/NotificationService.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Entities;
using CamBridge.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Service for sending notifications via email and Windows Event Log
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly NotificationSettings _settings;
        private readonly string _eventSource = "CamBridge";
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
                _logger.LogError(ex, "Failed to send notification for file: {FileName}", result.FileName);
            }
        }

        /// <summary>
        /// Send notification for critical errors
        /// </summary>
        public async Task NotifyErrorAsync(string message, Exception? exception = null)
        {
            try
            {
                if (_settings.EnableEventLog)
                {
                    WriteEventLog(message, EventLogEntryType.Error, exception);
                }

                if (ShouldSendEmail(LogLevel.Error))
                {
                    var subject = "Critical Error";
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
        /// Send notification for warnings
        /// </summary>
        public async Task NotifyWarningAsync(string message)
        {
            try
            {
                if (_settings.EnableEventLog)
                {
                    WriteEventLog(message, EventLogEntryType.Warning);
                }

                if (ShouldSendEmail(LogLevel.Warning))
                {
                    await SendEmailAsync("Warning", message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send warning notification");
            }
        }

        /// <summary>
        /// Send daily summary report
        /// </summary>
        public async Task SendDailySummaryAsync()
        {
            if (!_settings.SendDailySummary || _settings.Email == null)
                return;

            try
            {
                List<ProcessingResult> summaryData;
                lock (_dailySummaryBuffer)
                {
                    if (_dailySummaryBuffer.Count == 0)
                        return;

                    summaryData = new List<ProcessingResult>(_dailySummaryBuffer);
                    _dailySummaryBuffer.Clear();
                }

                var subject = $"Daily Processing Summary - {DateTime.Now:yyyy-MM-dd}";
                var body = BuildDailySummaryBody(summaryData);

                await SendEmailAsync(subject, body);
                _logger.LogInformation("Daily summary sent with {Count} items", summaryData.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send daily summary");
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
                   logLevel >= ParseLogLevel(_settings.MinimumEmailLevel);
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
            var subject = result.Success
                ? $"Processing Successful: {result.FileName}"
                : $"Processing Failed: {result.FileName}";

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

            var message = result.Success
                ? $"Successfully processed: {result.FileName}\nOutput: {result.OutputFile}"
                : $"Failed to process: {result.FileName}\nError: {result.ErrorMessage}";

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
            sb.AppendLine($"File Processing Notification");
            sb.AppendLine($"==========================");
            sb.AppendLine();
            sb.AppendLine($"Timestamp: {result.ProcessedAt:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"File: {result.FileName}");
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

            if (result.PatientInfo != null)
            {
                sb.AppendLine();
                sb.AppendLine("Patient Information:");
                sb.AppendLine($"  Name: {result.PatientInfo.PatientName}");
                sb.AppendLine($"  ID: {result.PatientInfo.PatientId}");
                sb.AppendLine($"  Study ID: {result.PatientInfo.StudyId}");
            }

            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine($"CamBridge Service v{typeof(NotificationService).Assembly.GetName().Version}");

            return sb.ToString();
        }

        private string BuildErrorEmailBody(string message, Exception? exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Critical Error Notification");
            sb.AppendLine("==========================");
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

        private string BuildDailySummaryBody(List<ProcessingResult> results)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Daily Processing Summary - {DateTime.Now:yyyy-MM-dd}");
            sb.AppendLine("=====================================");
            sb.AppendLine();

            var successful = results.Count(r => r.Success);
            var failed = results.Count(r => !r.Success);

            sb.AppendLine($"Total Files Processed: {results.Count}");
            sb.AppendLine($"Successful: {successful}");
            sb.AppendLine($"Failed: {failed}");
            sb.AppendLine($"Success Rate: {(results.Count > 0 ? (successful * 100.0 / results.Count) : 0):F1}%");

            if (results.Any(r => r.ProcessingTime.HasValue))
            {
                var avgTime = results.Where(r => r.ProcessingTime.HasValue)
                    .Average(r => r.ProcessingTime!.Value.TotalMilliseconds);
                sb.AppendLine($"Average Processing Time: {avgTime:F2} ms");
            }

            if (failed > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Failed Files:");
                sb.AppendLine("--------------");
                foreach (var failure in results.Where(r => !r.Success))
                {
                    sb.AppendLine($"  • {failure.FileName}: {failure.ErrorMessage}");
                }
            }

            sb.AppendLine();
            sb.AppendLine("Processed Files by Hour:");
            sb.AppendLine("------------------------");
            var byHour = results.GroupBy(r => r.ProcessedAt.Hour)
                               .OrderBy(g => g.Key);
            foreach (var hourGroup in byHour)
            {
                sb.AppendLine($"  {hourGroup.Key:00}:00 - {hourGroup.Key:00}:59: {hourGroup.Count()} files");
            }

            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine($"CamBridge Service v{typeof(NotificationService).Assembly.GetName().Version}");
            sb.AppendLine($"Report generated at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            return sb.ToString();
        }

        private async Task CheckAndSendDailySummaryAsync()
        {
            if (!_settings.SendDailySummary)
                return;

            var now = DateTime.Now;

            // Check if we've crossed into a new day and it's the configured hour
            if (now.Date > _lastSummaryDate && now.Hour >= _settings.DailySummaryHour)
            {
                await SendDailySummaryAsync();
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
