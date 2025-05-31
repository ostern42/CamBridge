using System.Threading;
using System.Threading.Tasks;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace CamBridge.Service
{
    /// <summary>
    /// Health check for CamBridge service
    /// </summary>
    public class CamBridgeHealthCheck : IHealthCheck
    {
        private readonly ILogger<CamBridgeHealthCheck> _logger;
        private readonly ProcessingQueue _processingQueue;
        private readonly FolderWatcherService _folderWatcher;

        public CamBridgeHealthCheck(
            ILogger<CamBridgeHealthCheck> logger,
            ProcessingQueue processingQueue,
            FolderWatcherService folderWatcher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _processingQueue = processingQueue ?? throw new ArgumentNullException(nameof(processingQueue));
            _folderWatcher = folderWatcher ?? throw new ArgumentNullException(nameof(folderWatcher));
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var stats = _processingQueue.GetStatistics();

                // Check for unhealthy conditions
                if (stats.QueueLength > 5000)
                {
                    return Task.FromResult(HealthCheckResult.Degraded(
                        $"Queue backlog is very large: {stats.QueueLength} files"));
                }

                if (stats.TotalProcessed > 100 && stats.SuccessRate < 20)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy(
                        $"Very low success rate: {stats.SuccessRate:F1}%"));
                }

                if (stats.TotalProcessed > 50 && stats.SuccessRate < 50)
                {
                    return Task.FromResult(HealthCheckResult.Degraded(
                        $"Low success rate: {stats.SuccessRate:F1}%"));
                }

                // Healthy
                var data = new Dictionary<string, object>
                {
                    ["queue_length"] = stats.QueueLength,
                    ["active_processing"] = stats.ActiveProcessing,
                    ["total_processed"] = stats.TotalProcessed,
                    ["success_rate"] = stats.SuccessRate,
                    ["processing_rate"] = stats.ProcessingRate
                };

                return Task.FromResult(HealthCheckResult.Healthy(
                    $"Service is healthy. Queue: {stats.QueueLength}, Success rate: {stats.SuccessRate:F1}%",
                    data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    "Health check failed",
                    ex));
            }
        }
    }
}
