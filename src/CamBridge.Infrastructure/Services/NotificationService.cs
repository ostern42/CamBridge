// src/CamBridge.Infrastructure/Services/NotificationService.cs
// Version: 0.7.18
// Description: Ultra-minimal notification service - KISS approach without interface!
// Copyright: Â© 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Linq;
using System.Threading.Tasks;
using CamBridge.Core;
using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Ultra-minimal notification service - just logs!
    /// KISS: No email implementation, no interface!
    /// v0.7.18: Direct dependency pattern
    /// </summary>
    public class NotificationService // No more interface!
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Send daily summary - KISS: just log!
        /// </summary>
        public async Task SendDailySummaryAsync(ProcessingSummary summary)
        {
            await Task.CompletedTask;

            _logger.LogInformation(
                "Daily Summary: Processed {Total}, Success {Success}, Failed {Failed}, Uptime {Uptime}",
                summary.TotalProcessed,
                summary.Successful,
                summary.Failed,
                summary.Uptime);

            if (summary.TopErrors != null && summary.TopErrors.Count > 0)
            {
                _logger.LogInformation("Top Errors: {Errors}",
                    string.Join(", ", summary.TopErrors.Select(e => $"{e.Key}: {e.Value}")));
            }
        }

        /// <summary>
        /// Notify critical error - KISS: just log!
        /// </summary>
        public async Task NotifyErrorAsync(string message, Exception? exception = null)
        {
            await Task.CompletedTask;

            if (exception != null)
            {
                _logger.LogError(exception, "Critical Error: {Message}", message);
            }
            else
            {
                _logger.LogError("Critical Error: {Message}", message);
            }
        }
    }
}
