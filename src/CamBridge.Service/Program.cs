// src/CamBridge.Service/Program.cs
// Version: 0.5.29
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions
// Modified: 2025-06-05
// Status: Windows Service Debugging

using CamBridge.Infrastructure;
using CamBridge.Infrastructure.Services;
using CamBridge.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System.Diagnostics;

// Set service start time
Program.ServiceStartTime = DateTime.UtcNow;

// Windows Event Log für Debugging
EventLog? serviceEventLog = null;
try
{
    // Try to create event source if not exists
    if (!EventLog.SourceExists("CamBridgeService"))
    {
        EventLog.CreateEventSource("CamBridgeService", "Application");
    }

    serviceEventLog = new EventLog("Application");
    serviceEventLog.Source = "CamBridgeService";
    serviceEventLog.WriteEntry("CamBridge Service starting...", EventLogEntryType.Information, 1000);
}
catch (Exception ex)
{
    // Ignore if we can't write to event log (permissions)
    Console.WriteLine($"Could not initialize Event Log: {ex.Message}");
}

// Better service detection
var isService = false;
try
{
    // More reliable check using command line args first
    if (args.Contains("--console") || args.Contains("-c"))
    {
        isService = false;
        serviceEventLog?.WriteEntry("Running in console mode (explicit parameter)", EventLogEntryType.Information, 1001);
    }
    else if (args.Contains("--service"))
    {
        isService = true;
        serviceEventLog?.WriteEntry("Running as Windows Service (explicit parameter)", EventLogEntryType.Information, 1002);
    }
    else
    {
        // Fallback to environment check
        isService = !Environment.UserInteractive;
        serviceEventLog?.WriteEntry($"Running as: {(isService ? "Service" : "Console")} (auto-detected)", EventLogEntryType.Information, 1003);
    }
}
catch (Exception ex)
{
    serviceEventLog?.WriteEntry($"Error detecting service mode: {ex.Message}", EventLogEntryType.Error, 1004);
}

// Configure Serilog with better Windows Service support
try
{
    var logPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        "CamBridge",
        "Logs",
        "service-.log"
    );

    // Ensure log directory exists
    Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.File(
            path: logPath,
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}")
        .WriteTo.EventLog("CamBridgeService", "Application", manageEventSource: false)
        .WriteTo.Conditional(
            _ => !isService,
            wt => wt.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"))
        .CreateBootstrapLogger();

    serviceEventLog?.WriteEntry($"Serilog configured. Logging to: {logPath}", EventLogEntryType.Information, 1005);
}
catch (Exception ex)
{
    serviceEventLog?.WriteEntry($"Failed to configure Serilog: {ex.Message}", EventLogEntryType.Error, 1006);
}

try
{
    Log.Information("Starting CamBridge Service v0.5.29...");
    Log.Information("Running as: {Mode}", isService ? "Windows Service" : "Console Application");
    Log.Information("Command line args: {Args}", string.Join(" ", args));

    // CRITICAL FIX: Set working directory to service location
    if (isService)
    {
        var serviceDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        if (!string.IsNullOrEmpty(serviceDirectory))
        {
            Directory.SetCurrentDirectory(serviceDirectory);
            Log.Information("Working directory set to: {Directory}", serviceDirectory);
            serviceEventLog?.WriteEntry($"Working directory: {serviceDirectory}", EventLogEntryType.Information, 1009);
        }
    }

    serviceEventLog?.WriteEntry("Host builder starting...", EventLogEntryType.Information, 1010);

    var builder = Host.CreateDefaultBuilder(args);

    // Configure for Windows Service or Console
    if (isService)
    {
        Log.Information("Configuring for Windows Service mode");
        serviceEventLog?.WriteEntry("Configuring Windows Service support", EventLogEntryType.Information, 1011);

        builder.UseWindowsService(options =>
        {
            options.ServiceName = "CamBridgeService";
        });

        // Don't start web host immediately for service
        builder.ConfigureServices((context, services) =>
        {
            services.Configure<HostOptions>(options =>
            {
                // Give service more time to start
                options.ShutdownTimeout = TimeSpan.FromSeconds(30);
            });
        });
    }
    else
    {
        builder.UseEnvironment("Development");
    }

    builder.ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment;

        // Use service directory as base path
        var basePath = Directory.GetCurrentDirectory();
        if (isService)
        {
            var serviceDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrEmpty(serviceDirectory))
            {
                basePath = serviceDirectory;
            }
        }

        config.SetBasePath(basePath)
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables("CAMBRIDGE_")
              .AddCommandLine(args);

        Log.Information("Configuration loaded from: {BasePath} for environment: {Environment}", basePath, env.EnvironmentName);
        serviceEventLog?.WriteEntry($"Configuration loaded from {basePath}: {env.EnvironmentName}", EventLogEntryType.Information, 1012);
    });

    // Configure Serilog from configuration
    builder.UseSerilog((context, services, loggerConfiguration) =>
    {
        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "CamBridge",
            "Logs",
            "service-.log"
        );

        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", "CamBridge")
            .WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}")
            .WriteTo.EventLog("CamBridgeService", "Application", manageEventSource: false);

        if (!isService)
        {
            loggerConfiguration.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}");
        }
    });

    builder.ConfigureServices((context, services) =>
    {
        serviceEventLog?.WriteEntry("Configuring services...", EventLogEntryType.Information, 1013);

        var configuration = context.Configuration;

        // Core Infrastructure
        services.AddInfrastructure(configuration);
        services.AddSingleton<FolderWatcherService>();
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<FolderWatcherService>());
        services.AddHostedService<Worker>();

        // Health Checks
        services.AddHealthChecks()
            .AddCheck<CamBridgeHealthCheck>("cambridge");

        // Daily Summary Service
        services.AddHostedService<DailySummaryService>();

        // Web API
        services.AddControllers();
        services.AddCors(options =>
        {
            options.AddPolicy("ConfigUI", policy =>
            {
                policy.WithOrigins("http://localhost:*", "https://localhost:*")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        serviceEventLog?.WriteEntry("Services configured successfully", EventLogEntryType.Information, 1014);
    });

    // Configure Web Host
    serviceEventLog?.WriteEntry("Configuring web host...", EventLogEntryType.Information, 1015);

    builder.ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
        webBuilder.UseUrls("http://localhost:5050");

        // For Windows Service, configure Kestrel to not wait for requests
        if (isService)
        {
            webBuilder.ConfigureKestrel(serverOptions =>
            {
                serverOptions.AllowSynchronousIO = true;
            });
        }
    });

    serviceEventLog?.WriteEntry("Building host...", EventLogEntryType.Information, 1016);
    var host = builder.Build();

    // Validate infrastructure
    serviceEventLog?.WriteEntry("Validating infrastructure...", EventLogEntryType.Information, 1017);
    using (var scope = host.Services.CreateScope())
    {
        try
        {
            scope.ServiceProvider.ValidateInfrastructure();
            Log.Information("Infrastructure validation completed successfully");
            serviceEventLog?.WriteEntry("Infrastructure validation successful", EventLogEntryType.Information, 1018);

            // Show active features
            Log.Information("=========================================");
            Log.Information("AKTIVE FEATURES:");
            Log.Information("✓ Basic Pipeline (ExifTool → DICOM)");
            Log.Information("✓ Health Checks");
            Log.Information("✓ Web API");
            Log.Information(isService ? "✓ Windows Service" : "✓ Console Mode");
            Log.Information("=========================================");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Infrastructure validation failed");
            serviceEventLog?.WriteEntry($"Infrastructure validation failed: {ex.Message}", EventLogEntryType.Error, 1019);
            throw;
        }
    }

    // Run the host
    serviceEventLog?.WriteEntry("Starting host...", EventLogEntryType.Information, 1020);
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    serviceEventLog?.WriteEntry($"Service failed: {ex}", EventLogEntryType.Error, 1999);
    return 1;
}
finally
{
    serviceEventLog?.WriteEntry("Service stopping", EventLogEntryType.Information, 1021);
    Log.CloseAndFlush();
    serviceEventLog?.Dispose();
}

return 0;

// Startup class remains the same
public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Services are already configured in Program.cs
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseCors("ConfigUI");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");

            // Status endpoint
            endpoints.MapGet("/api/status", async context =>
            {
                var processingQueue = context.RequestServices.GetService<ProcessingQueue>();
                var deadLetterQueue = context.RequestServices.GetService<DeadLetterQueue>();
                var settings = context.RequestServices.GetService<IOptions<CamBridge.Core.CamBridgeSettings>>();

                var queueStats = processingQueue?.GetStatistics();
                var deadLetterStats = deadLetterQueue?.GetStatistics();

                var totalProcessed = (queueStats?.TotalSuccessful ?? 0) + (queueStats?.TotalFailed ?? 0);
                var successRate = totalProcessed > 0
                    ? (double)(queueStats?.TotalSuccessful ?? 0) / totalProcessed * 100
                    : 0;

                var status = new
                {
                    ServiceStatus = "Running",
                    Version = "0.5.29",
                    Mode = Environment.UserInteractive ? "Console" : "Service",
                    Uptime = DateTime.UtcNow - Program.ServiceStartTime,
                    QueueLength = queueStats?.QueueLength ?? 0,
                    ActiveProcessing = queueStats?.ActiveProcessing ?? 0,
                    TotalSuccessful = queueStats?.TotalSuccessful ?? 0,
                    TotalFailed = queueStats?.TotalFailed ?? 0,
                    SuccessRate = successRate,
                    DeadLetterCount = deadLetterStats?.TotalCount ?? 0,
                    Configuration = new
                    {
                        WatchFolders = settings?.Value?.WatchFolders?.Count ?? 0,
                        OutputFolder = settings?.Value?.DefaultOutputFolder
                    }
                };

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(status);
            });

            // Development endpoints
            if (env.IsDevelopment())
            {
                endpoints.MapGet("/", async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(@"
                        <html>
                        <head><title>CamBridge Service v0.5.29</title></head>
                        <body>
                            <h1>CamBridge Service - Development Mode</h1>
                            <p>Service läuft!</p>
                            <ul>
                                <li>Health: <a href='/health'>/health</a></li>
                                <li>Status: <a href='/api/status'>/api/status</a></li>
                            </ul>
                        </body>
                        </html>");
                });
            }
        });
    }
}

// Service start time storage
public partial class Program
{
    public static DateTime ServiceStartTime { get; private set; }
}
