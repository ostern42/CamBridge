namespace CamBridge.Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CamBridge Service started at: {time}", DateTimeOffset.Now);

        // TODO: Implement file watching and conversion logic in Phase 6-7
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation("CamBridge Service stopped at: {time}", DateTimeOffset.Now);
    }
}