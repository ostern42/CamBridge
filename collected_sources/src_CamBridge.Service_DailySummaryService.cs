using System;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CamBridge.Service
{
    /// <summary>
    /// Background service that sends daily summaries
    /// </summary>
    public class DailySummaryService : BackgroundService
    {
        private readonly ILogger<DailySummaryService> _logger;
        private readonly ProcessingQueue _processingQueue;
        private readonly INotificationService _notificationService;
        private readonly NotificationSettings _settings;
        private Timer? _timer;

        public DailySummaryService(
            ILogger<DailySummaryService> logger,
            ProcessingQueue processingQueue,
            INotificationService notificationService,
            IOptions<NotificationSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _processingQueue = processingQueue ?? throw new ArgumentNullException(nameof(processingQueue));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_settings.SendDailySummary)
            {
                _logger.LogInformation("Daily summary notifications are disabled");
                return Task.CompletedTask;
            }

            // Calculate time until next summary
            var now = DateTime.Now;
            var nextRun = now.Date.AddHours(_settings.DailySummaryHour);
            if (nextRun <= now)
            {
                nextRun = nextRun.AddDays(1);
            }

            var delay = nextRun - now;
            _logger.LogInformation("Daily summary scheduled for {NextRun} (in {Delay})",
                nextRun, delay);

            _timer = new Timer(
                SendDailySummary,
                null,
                delay,
                TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private async void SendDailySummary(object? state)
        {
            try
            {
                _logger.LogInformation("Sending daily summary");
                var summary = _processingQueue.GetDailySummary();
                await _notificationService.SendDailySummaryAsync(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending daily summary");
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}
