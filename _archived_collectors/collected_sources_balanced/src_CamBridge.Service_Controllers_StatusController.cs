// src/CamBridge.Service/Controllers/StatusController.cs
using CamBridge.Core;
using CamBridge.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace CamBridge.Service.Controllers
{
    /// <summary>
    /// API controller for service status and statistics
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly ILogger<StatusController> _logger;
        private readonly ProcessingQueue _processingQueue;
        private readonly DeadLetterQueue _deadLetterQueue;
        private readonly IHostApplicationLifetime _lifetime;

        public StatusController(
            ILogger<StatusController> logger,
            ProcessingQueue processingQueue,
            DeadLetterQueue deadLetterQueue,
            IHostApplicationLifetime lifetime)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _processingQueue = processingQueue ?? throw new ArgumentNullException(nameof(processingQueue));
            _deadLetterQueue = deadLetterQueue ?? throw new ArgumentNullException(nameof(deadLetterQueue));
            _lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
        }

        /// <summary>
        /// Gets current service status and statistics
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceStatusDto), 200)]
        public IActionResult GetStatus()
        {
            try
            {
                var stats = _processingQueue.GetStatistics();
                var activeItems = _processingQueue.GetActiveItems();
                var deadLetterStats = _deadLetterQueue.GetStatistics();

                var status = new ServiceStatusDto
                {
                    ServiceStatus = "Running",
                    Timestamp = DateTime.UtcNow,
                    QueueLength = stats.QueueLength,
                    ActiveProcessing = stats.ActiveProcessing,
                    TotalProcessed = stats.TotalProcessed,
                    TotalSuccessful = stats.TotalSuccessful,
                    TotalFailed = stats.TotalFailed,
                    SuccessRate = stats.SuccessRate,
                    ProcessingRate = stats.ProcessingRate,
                    Uptime = stats.UpTime,
                    DeadLetterCount = deadLetterStats.TotalCount,
                    ActiveItems = activeItems.Select(item => new ActiveItemDto
                    {
                        FilePath = item.FilePath,
                        FileName = Path.GetFileName(item.FilePath),
                        StartTime = item.StartTime,
                        AttemptCount = item.AttemptCount,
                        Duration = item.Duration
                    }).ToList()
                };

                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting service status");
                return StatusCode(500, new { error = "Failed to retrieve status" });
            }
        }

        /// <summary>
        /// Gets detailed statistics
        /// </summary>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(DetailedStatisticsDto), 200)]
        public IActionResult GetStatistics()
        {
            try
            {
                var stats = _processingQueue.GetStatistics();
                var deadLetterStats = _deadLetterQueue.GetStatistics();

                var detailed = new DetailedStatisticsDto
                {
                    QueueStatistics = stats,
                    DeadLetterStatistics = deadLetterStats,
                    TopErrors = stats.TopErrors,
                    ErrorCategories = deadLetterStats.ErrorCategories
                };

                return Ok(detailed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting detailed statistics");
                return StatusCode(500, new { error = "Failed to retrieve statistics" });
            }
        }

        /// <summary>
        /// Gets dead letter items
        /// </summary>
        [HttpGet("deadletters")]
        [ProducesResponseType(typeof(List<DeadLetterItemDto>), 200)]
        public IActionResult GetDeadLetters()
        {
            try
            {
                var items = _deadLetterQueue.GetAllItems();
                var dtos = items.Select(item => new DeadLetterItemDto
                {
                    Id = item.Id,
                    FileName = item.FileName,
                    FilePath = item.FilePath,
                    Error = item.Error,
                    FailedAt = item.FailedAt,
                    AttemptCount = item.AttemptCount,
                    FileSize = item.FileSize
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dead letter items");
                return StatusCode(500, new { error = "Failed to retrieve dead letter items" });
            }
        }

        /// <summary>
        /// Reprocesses a dead letter item
        /// </summary>
        [HttpPost("deadletters/{id}/reprocess")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ReprocessDeadLetter(Guid id)
        {
            try
            {
                var item = _deadLetterQueue.GetItem(id);
                if (item == null)
                    return NotFound();

                // Remove from dead letter queue
                await _deadLetterQueue.RemoveAsync(id);

                // Re-enqueue for processing
                if (_processingQueue.TryEnqueue(item.FilePath))
                {
                    _logger.LogInformation("Reprocessing dead letter item {Id}: {FilePath}", id, item.FilePath);
                    return NoContent();
                }
                else
                {
                    // Add back to dead letter if enqueue failed
                    await _deadLetterQueue.AddAsync(item.FilePath, "Failed to re-enqueue", item.AttemptCount);
                    return BadRequest(new { error = "Failed to enqueue item for reprocessing" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reprocessing dead letter item {Id}", id);
                return StatusCode(500, new { error = "Failed to reprocess item" });
            }
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        [ProducesResponseType(typeof(HealthStatusDto), 200)]
        public IActionResult GetHealth()
        {
            var stats = _processingQueue.GetStatistics();
            var isHealthy = stats.SuccessRate >= 50 || stats.TotalProcessed < 10;

            return Ok(new HealthStatusDto
            {
                Status = isHealthy ? "Healthy" : "Degraded",
                Timestamp = DateTime.UtcNow,
                Checks = new Dictionary<string, string>
                {
                    ["queue"] = stats.QueueLength < 1000 ? "OK" : "Warning",
                    ["processing"] = stats.ActiveProcessing < 10 ? "OK" : "Warning",
                    ["success_rate"] = stats.SuccessRate >= 50 ? "OK" : "Error"
                }
            });
        }
    }

    // DTOs for API responses
    public class ServiceStatusDto
    {
        public string ServiceStatus { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int QueueLength { get; set; }
        public int ActiveProcessing { get; set; }
        public int TotalProcessed { get; set; }
        public int TotalSuccessful { get; set; }
        public int TotalFailed { get; set; }
        public double SuccessRate { get; set; }
        public double ProcessingRate { get; set; }
        public TimeSpan Uptime { get; set; }
        public int DeadLetterCount { get; set; }
        public List<ActiveItemDto> ActiveItems { get; set; } = new();
    }

    public class ActiveItemDto
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public int AttemptCount { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public class DeadLetterItemDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public DateTime FailedAt { get; set; }
        public int AttemptCount { get; set; }
        public long FileSize { get; set; }
    }

    public class DetailedStatisticsDto
    {
        public QueueStatistics QueueStatistics { get; set; } = null!;
        public DeadLetterStatistics DeadLetterStatistics { get; set; } = null!;
        public Dictionary<string, int> TopErrors { get; set; } = new();
        public Dictionary<string, int> ErrorCategories { get; set; } = new();
    }

    public class HealthStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public Dictionary<string, string> Checks { get; set; } = new();
    }
}
