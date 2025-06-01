// src/CamBridge.Service/Program.cs
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
using System;
using System.IO;
using System.Runtime.Versioning;

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
    Log.Information("Starting CamBridge Service v0.4.0");

    var builder = WebApplication.CreateBuilder(args);

    // Configure as Windows Service with Web API support
    builder.Host.UseWindowsService(options =>
    {
        options.ServiceName = "CamBridge Service";
    });

    builder.Host.UseSerilog();

    // Configure services
    ConfigureServices(builder.Services, builder.Configuration);

    // Configure Kestrel to listen on a specific port
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenLocalhost(5050); // API port
    });

    var app = builder.Build();

    // Validate configuration
    await ValidateConfigurationAsync(app.Services);

    // Configure HTTP pipeline
    ConfigureHttpPipeline(app);

    // Startup notification
    await SendStartupNotificationAsync(app.Services);

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

[SupportedOSPlatform("windows")]
static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Configure settings
    services.Configure<CamBridgeSettings>(configuration.GetSection("CamBridge"));
    services.Configure<ProcessingOptions>(configuration.GetSection("CamBridge:Processing"));
    services.Configure<NotificationSettings>(configuration.GetSection("CamBridge:Notifications"));

    // Register infrastructure services
    var mappingConfigPath = configuration["CamBridge:MappingConfigurationFile"] ?? "mappings.json";
    var useRicohReader = configuration.GetValue<bool>("CamBridge:UseRicohExifReader", true);
    services.AddCamBridgeInfrastructure(mappingConfigPath, useRicohReader);

    // Register core services
    services.AddSingleton<DeadLetterQueue>();
    services.AddSingleton<INotificationService, NotificationService>();

    // Register ProcessingQueue with dependencies
    services.AddSingleton<ProcessingQueue>(provider =>
    {
        var logger = provider.GetRequiredService<ILogger<ProcessingQueue>>();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
        var options = provider.GetRequiredService<IOptions<ProcessingOptions>>();
        var deadLetterQueue = provider.GetRequiredService<DeadLetterQueue>();
        var notificationService = provider.GetService<INotificationService>();

        return new ProcessingQueue(logger, scopeFactory, options, deadLetterQueue, notificationService);
    });

    // Register hosted services
    services.AddSingleton<FolderWatcherService>();
    services.AddHostedService(provider => provider.GetRequiredService<FolderWatcherService>());
    services.AddHostedService<Worker>();

    // Add daily summary service
    services.AddHostedService<DailySummaryService>();

    // Add health checks
    services.AddHealthChecks()
        .AddCheck<CamBridgeHealthCheck>("cambridge_health");

    // Add controllers for Web API
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "CamBridge API",
            Version = "v1",
            Description = "API for monitoring CamBridge JPEG to DICOM conversion service"
        });

        // Include XML documentation if available
        var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });
}

static void ConfigureHttpPipeline(WebApplication app)
{
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

    // Serve dashboard HTML if exists
    var dashboardPath = Path.Combine(app.Environment.ContentRootPath, "dashboard.html");
    if (File.Exists(dashboardPath))
    {
        app.MapGet("/dashboard", async context =>
        {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(dashboardPath);
        });
    }
}

[SupportedOSPlatform("windows")]
static async Task ValidateConfigurationAsync(IServiceProvider services)
{
    var settings = services.GetRequiredService<IOptions<CamBridgeSettings>>().Value;

    if (settings.WatchFolders == null || !settings.WatchFolders.Any(f => f.Enabled))
    {
        Log.Warning("No watch folders configured or enabled");
    }

    foreach (var folder in settings.WatchFolders?.Where(f => f.Enabled) ?? Enumerable.Empty<FolderConfiguration>())
    {
        if (!Directory.Exists(folder.Path))
        {
            Log.Warning("Watch folder does not exist: {Path}", folder.Path);
            try
            {
                Directory.CreateDirectory(folder.Path);
                Log.Information("Created watch folder: {Path}", folder.Path);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create watch folder: {Path}", folder.Path);
            }
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

    // Validate processing folders
    var processingOptions = services.GetRequiredService<IOptions<ProcessingOptions>>().Value;
    EnsureDirectoryExists(processingOptions.ArchiveFolder, "Archive");
    EnsureDirectoryExists(processingOptions.ErrorFolder, "Error");
    EnsureDirectoryExists(processingOptions.BackupFolder, "Backup");

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

    await Task.CompletedTask;
}

static void EnsureDirectoryExists(string path, string name)
{
    if (!Directory.Exists(path))
    {
        try
        {
            Directory.CreateDirectory(path);
            Log.Information("Created {Name} folder: {Path}", name, path);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to create {Name} folder: {Path}", name, path);
        }
    }
}

[SupportedOSPlatform("windows")]
static async Task SendStartupNotificationAsync(IServiceProvider services)
{
    try
    {
        var notificationService = services.GetService<INotificationService>();
        if (notificationService != null)
        {
            await notificationService.NotifyInfoAsync(
                "CamBridge Service Started",
                $"Service version 0.4.0 started successfully on {Environment.MachineName}\n" +
                $"API endpoint: http://localhost:5050\n" +
                $"Process ID: {Environment.ProcessId}");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Failed to send startup notification");
    }
}
