using CamBridge.Core;
using CamBridge.Core.Interfaces;
using CamBridge.Infrastructure;
using CamBridge.Infrastructure.Services;
using CamBridge.Service;
using Microsoft.AspNetCore.Builder;
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
    Log.Information("Starting CamBridge Service v0.3.2");

    var builder = WebApplication.CreateBuilder(args);

    // Configure as Windows Service with Web API support
    builder.Host.UseWindowsService(options =>
    {
        options.ServiceName = "CamBridge Service";
    });

    builder.Host.UseSerilog();

    // Configure services
    builder.Services.Configure<CamBridgeSettings>(builder.Configuration.GetSection("CamBridge"));
    builder.Services.Configure<ProcessingOptions>(builder.Configuration.GetSection("CamBridge:Processing"));
    builder.Services.Configure<NotificationSettings>(builder.Configuration.GetSection("CamBridge:Notifications"));

    // Register infrastructure services
    var mappingConfigPath = builder.Configuration["CamBridge:MappingConfigurationFile"] ?? "mappings.json";
    var useRicohReader = builder.Configuration.GetValue<bool>("CamBridge:UseRicohExifReader", true);
    builder.Services.AddCamBridgeInfrastructure(mappingConfigPath, useRicohReader);

    // Register core services
    builder.Services.AddSingleton<DeadLetterQueue>();
    builder.Services.AddSingleton<INotificationService, NotificationService>();

    // Register ProcessingQueue with dependencies
    builder.Services.AddSingleton<ProcessingQueue>(provider =>
    {
        var logger = provider.GetRequiredService<ILogger<ProcessingQueue>>();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
        var options = provider.GetRequiredService<IOptions<ProcessingOptions>>();
        var deadLetterQueue = provider.GetRequiredService<DeadLetterQueue>();
        var notificationService = provider.GetService<INotificationService>();

        return new ProcessingQueue(logger, scopeFactory, options, deadLetterQueue, notificationService);
    });

    // Register hosted services
    builder.Services.AddSingleton<FolderWatcherService>();
    builder.Services.AddHostedService(provider => provider.GetRequiredService<FolderWatcherService>());
    builder.Services.AddHostedService<Worker>();

    // Add daily summary service
    builder.Services.AddHostedService<DailySummaryService>();

    // Add health checks
    builder.Services.AddHealthChecks()
        .AddCheck<CamBridgeHealthCheck>("cambridge_health");

    // Add controllers for Web API
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "CamBridge API",
            Version = "v1",
            Description = "API for monitoring CamBridge JPEG to DICOM conversion service"
        });
    });

    // Configure Kestrel to listen on a specific port
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenLocalhost(5050); // API port
    });

    var app = builder.Build();

    // Validate configuration
    var settings = app.Services.GetRequiredService<IOptions<CamBridgeSettings>>().Value;
    ValidateConfiguration(settings);

    // Configure HTTP pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "CamBridge API v1");
            c.RoutePrefix = string.Empty; // Swagger UI at root
        });
    }

    app.UseRouting();
    app.MapControllers();
    app.MapHealthChecks("/health");

    // Startup notification
    var notificationService = app.Services.GetService<INotificationService>();
    if (notificationService != null)
    {
        await notificationService.NotifyInfoAsync(
            "CamBridge Service Started",
            $"Service version 0.3.2 started successfully on {Environment.MachineName}");
    }

    await app.RunAsync();
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

    // Validate notification settings
    var notificationSettings = settings.Notifications;
    if (notificationSettings?.EnableEmail == true)
    {
        if (string.IsNullOrEmpty(notificationSettings.SmtpHost))
        {
            Log.Warning("Email notifications enabled but SMTP host not configured");
        }
        if (string.IsNullOrEmpty(notificationSettings.EmailTo))
        {
            Log.Warning("Email notifications enabled but recipient email not configured");
        }
    }
}
