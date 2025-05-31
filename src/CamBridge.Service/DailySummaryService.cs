using System;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CamBridge.Service
{
    /// <summary>
    /// Background service that sends daily processing summaries
    /// </summary>
    public class DailySummaryService : BackgroundService
    {
        private readonly ILogger<DailySummaryService> _logger;
        private readonly ProcessingQueue _processingQueue;
        private readonly INotificationService? _notificationService;
        private readonly NotificationSettings _notificationSettings;
        private Timer? _dailyTimer;

        public DailySummaryService(
            ILogger<DailySummaryService> logger,
            ProcessingQueue processingQueue,
            IOptions<NotificationSettings> notificationSettings,
            INotificationService? notificationService = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _processingQueue = processingQueue ?? throw new ArgumentNullException(nameof(processingQueue));
            _notificationSettings = notificationSettings?.Value ?? throw new ArgumentNullException(nameof(notificationSettings));
            _notificationService = notificationService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_notificationSettings.SendDailySummary || _notificationService == null)
            {
                _logger.LogInformation("Daily summary notifications are disabled");
                return Task.CompletedTask;
            }

            _logger.LogInformation("Daily summary service started");

            // Calculate time until next summary (default: 7 AM)
            var now = DateTime.Now;
            var nextSummaryTime = now.Date.AddHours(7); // 7 AM
            if (now >= nextSummaryTime)
            {
                nextSummaryTime = nextSummaryTime.AddDays(1);
            }

            var initialDelay = nextSummaryTime - now;
            _logger.LogInformation("Next daily summary scheduled for {Time} (in {Delay})",
                nextSummaryTime, initialDelay);

            // Schedule daily timer
            _dailyTimer = new Timer(
                SendDailySummary,
                null,
                initialDelay,
                TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private async void SendDailySummary(object? state)
        {
            try
            {
                _logger.LogInformation("Generating daily summary");

                var summary = _processingQueue.GetDailySummary();

                // Only send if there was activity
                if (summary.TotalProcessed > 0 || summary.DeadLetterCount > 0)
                {
                    await _notificationService!.NotifyDailySummaryAsync(summary);
                    _logger.LogInformation("Daily summary sent successfully");
                }
                else
                {
                    _logger.LogInformation("No processing activity to report");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending daily summary");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Daily summary service stopping");

            _dailyTimer?.Change(Timeout.Infinite, 0);
            _dailyTimer?.Dispose();

            // Send final summary if service is stopping
            if (_notificationService != null && _processingQueue.TotalProcessed > 0)
            {
                try
                {
                    _logger.LogInformation("Sending final summary before shutdown");
                    var summary = _processingQueue.GetDailySummary();
                    await _notificationService.NotifyDailySummaryAsync(summary);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending final summary");
                }
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
