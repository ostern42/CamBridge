using CamBridge.Core;
using CamBridge.Core.Interfaces;
using CamBridge.Infrastructure;
using CamBridge.Infrastructure.Services;
using CamBridge.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

// Configure Serilog
var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "cambridge-.txt");
Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .WriteTo.File(
        logPath,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting CamBridge Service v0.3.1");

    var host = Host.CreateDefaultBuilder(args)
        .UseWindowsService(options =>
        {
            options.ServiceName = "CamBridge Service";
        })
        .UseSerilog()
        .ConfigureServices((hostContext, services) =>
        {
            // Load configuration
            var configuration = hostContext.Configuration;

            // Configure settings
            services.Configure<CamBridgeSettings>(configuration.GetSection("CamBridge"));
            services.Configure<ProcessingOptions>(configuration.GetSection("CamBridge:Processing"));

            // Register infrastructure services
            var mappingConfigPath = configuration["CamBridge:MappingConfigurationFile"] ?? "mappings.json";
            var useRicohReader = configuration.GetValue<bool>("CamBridge:UseRicohExifReader", true);

            services.AddCamBridgeInfrastructure(mappingConfigPath, useRicohReader);

            // Register processing services
            services.AddSingleton<ProcessingQueue>();
            // IFileProcessor is already registered by AddCamBridgeInfrastructure as Scoped

            // Register hosted services
            services.AddSingleton<FolderWatcherService>();
            services.AddHostedService(provider => provider.GetRequiredService<FolderWatcherService>());
            services.AddHostedService<Worker>();

            // Add health checks (optional)
            services.AddHealthChecks()
                .AddCheck<CamBridgeHealthCheck>("cambridge_health");
        })
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                  .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                      optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables();
        })
        .Build();

    // Validate configuration
    var settings = host.Services.GetRequiredService<IOptions<CamBridgeSettings>>().Value;
    ValidateConfiguration(settings);

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;

void ValidateConfiguration(CamBridgeSettings settings)
{
    if (settings.WatchFolders == null || !settings.WatchFolders.Any(f => f.Enabled))
    {
        Log.Warning("No watch folders configured or enabled");
    }

    foreach (var folder in settings.WatchFolders?.Where(f => f.Enabled) ?? Enumerable.Empty<FolderConfiguration>())
    {
        if (!Directory.Exists(folder.Path))
        {
            Log.Warning("Watch folder does not exist: {Path}", folder.Path);
        }
    }

    if (!Directory.Exists(settings.DefaultOutputFolder))
    {
        try
        {
            Directory.CreateDirectory(settings.DefaultOutputFolder);
            Log.Information("Created default output folder: {Path}", settings.DefaultOutputFolder);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to create default output folder: {Path}", settings.DefaultOutputFolder);
        }
    }
}
