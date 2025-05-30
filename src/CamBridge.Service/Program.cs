using CamBridge.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "cambridge-.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting CamBridge Service v0.0.1");

    var host = Host.CreateDefaultBuilder(args)
        .UseWindowsService(options =>
        {
            options.ServiceName = "CamBridge Service";
        })
        .UseSerilog()
        .ConfigureServices((hostContext, services) =>
        {
            // Register services here
            services.AddHostedService<Worker>();

            // TODO: Register Core and Infrastructure services in later phases
        })
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}