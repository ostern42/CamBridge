// src/CamBridge.Service/Program.cs
// Version: 0.7.5+tools
// Description: Windows service entry point with centralized config management
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Core;
using CamBridge.Core.Infrastructure;
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


// Set service start time
Program.ServiceStartTime = DateTime.UtcNow;

// Windows Event Log for Debugging
EventLog? serviceEventLog = null;
try
{
    // Try to create event source if not exists
    if (!EventLog.SourceExists(ServiceInfo.ServiceName))
    {
        EventLog.CreateEventSource(ServiceInfo.ServiceName, "Application");
    }

    serviceEventLog = new EventLog("Application");
    serviceEventLog.Source = ServiceInfo.ServiceName;
    serviceEventLog.WriteEntry($"{ServiceInfo.GetFullVersionString()} starting...", EventLogEntryType.Information, 1000);
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

// Configure Serilog with centralized log path
try
{
    var logPath = Path.Combine(
        ConfigurationPaths.GetLogsDirectory(),
        "service-.log"
    );

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.File(
            path: logPath,
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}")
        .WriteTo.EventLog(ServiceInfo.ServiceName, "Application", manageEventSource: false)
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
    Log.Information("Starting {ServiceName} with Centralized Config...", ServiceInfo.GetFullVersionString());
    Log.Information("Running as: {Mode}", isService ? "Windows Service" : "Console Application");
    Log.Information("Command line args: {Args}", string.Join(" ", args));

    // Log configuration paths for diagnostics
    Log.Information("Configuration Paths:\n{DiagnosticInfo}", ConfigurationPaths.GetDiagnosticInfo());

    // CRITICAL: Initialize primary config if needed
    var localConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
    if (ConfigurationPaths.InitializePrimaryConfig())
    {
        Log.Information("Initialized primary config from local template");
        serviceEventLog?.WriteEntry("Copied default config to ProgramData", EventLogEntryType.Information, 1007);
    }

    // CRITICAL FIX: Set working directory to service location (for relative paths like Tools\)
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
            options.ServiceName = ServiceInfo.ServiceName;
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

        // CRITICAL: Use centralized config path!
        var primaryConfigPath = ConfigurationPaths.GetPrimaryConfigPath();

        Log.Information("Loading configuration from: {ConfigPath}", primaryConfigPath);
        serviceEventLog?.WriteEntry($"Loading config from: {primaryConfigPath}", EventLogEntryType.Information, 1012);

        // Clear default sources and add our specific order
        config.Sources.Clear();

        // 1. Primary config (from ProgramData)
        config.AddJsonFile(primaryConfigPath, optional: false, reloadOnChange: true);

        // 2. Local override for development (if exists)
        config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

        // 3. Environment variables
        config.AddEnvironmentVariables("CAMBRIDGE_");

        // 4. Command line (highest priority)
        config.AddCommandLine(args);

        Log.Information("Configuration loaded for environment: {Environment}", env.EnvironmentName);
    });

    // Configure Serilog from configuration
    builder.UseSerilog((context, services, loggerConfiguration) =>
    {
        var logPath = Path.Combine(
            ConfigurationPaths.GetLogsDirectory(),
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
            .WriteTo.EventLog(ServiceInfo.ServiceName, "Application", manageEventSource: false);

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

        // Core Infrastructure with Pipeline support
        services.AddInfrastructure(configuration);

        // Main Worker service
        services.AddHostedService<Worker>();

        // Health Checks
        services.AddHealthChecks()
            .AddCheck<CamBridgeHealthCheck>("cambridge");

        // Daily Summary Service - TEMPORARILY DISABLED until we fix per-pipeline queues
        // services.AddHostedService<DailySummaryService>();

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
        webBuilder.UseUrls($"http://localhost:{ServiceInfo.ApiPort}");

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
            Log.Information("ACTIVE FEATURES v{Version}:", ServiceInfo.Version);
            Log.Information("✓ Centralized Configuration");
            Log.Information("✓ Pipeline Architecture");
            Log.Information("✓ Multi-Pipeline Support");
            Log.Information("✓ Basic Pipeline (ExifTool → DICOM)");
            Log.Information("✓ Health Checks");
            Log.Information("✓ Web API");
            Log.Information(isService ? "✓ Windows Service" : "✓ Console Mode");
            Log.Information("✓ Tab-Complete Testing Tools");
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

// Startup class with pipeline support
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

            // Status endpoint with pipeline support
            endpoints.MapGet("/api/status", async context =>
            {
                var pipelineManager = context.RequestServices.GetService<PipelineManager>();
                var settingsV2 = context.RequestServices.GetService<IOptions<CamBridgeSettingsV2>>();

                var pipelineStatuses = pipelineManager?.GetPipelineStatuses() ?? new Dictionary<string, PipelineStatus>();

                // Calculate totals
                var totalQueued = pipelineStatuses.Sum(p => p.Value.QueueLength);
                var totalActive = pipelineStatuses.Sum(p => p.Value.ActiveProcessing);
                var totalProcessed = pipelineStatuses.Sum(p => p.Value.TotalProcessed);
                var totalSuccessful = pipelineStatuses.Sum(p => p.Value.TotalSuccessful);
                var totalFailed = pipelineStatuses.Sum(p => p.Value.TotalFailed);
                var successRate = totalProcessed > 0
                    ? (double)totalSuccessful / totalProcessed * 100
                    : 0;

                var status = new
                {
                    ServiceStatus = "Running",
                    Version = ServiceInfo.Version,
                    Mode = Environment.UserInteractive ? "Console" : "Service",
                    Uptime = DateTime.UtcNow - Program.ServiceStartTime,
                    ConfigPath = ConfigurationPaths.GetPrimaryConfigPath(),
                    ConfigExists = ConfigurationPaths.PrimaryConfigExists(),
                    PipelineCount = pipelineStatuses.Count,
                    ActivePipelines = pipelineStatuses.Count(p => p.Value.IsActive),
                    QueueLength = totalQueued,
                    ActiveProcessing = totalActive,
                    TotalSuccessful = totalSuccessful,
                    TotalFailed = totalFailed,
                    SuccessRate = successRate,
                    Pipelines = pipelineStatuses.Select(p => new
                    {
                        Id = p.Key,
                        Name = p.Value.Name,
                        IsActive = p.Value.IsActive,
                        QueueLength = p.Value.QueueLength,
                        ActiveProcessing = p.Value.ActiveProcessing,
                        TotalProcessed = p.Value.TotalProcessed,
                        TotalSuccessful = p.Value.TotalSuccessful,
                        TotalFailed = p.Value.TotalFailed,
                        WatchedFolders = p.Value.WatchedFolders
                    }),
                    Configuration = new
                    {
                        DefaultOutputFolder = settingsV2?.Value?.DefaultOutputFolder,
                        ExifToolPath = settingsV2?.Value?.ExifToolPath,
                        Version = settingsV2?.Value?.Version
                    }
                };

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(status);
            });

            // Pipeline-specific endpoints
            endpoints.MapGet("/api/pipelines", async context =>
            {
                var pipelineManager = context.RequestServices.GetService<PipelineManager>();
                var statuses = pipelineManager?.GetPipelineStatuses() ?? new Dictionary<string, PipelineStatus>();

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(statuses);
            });

            endpoints.MapGet("/api/pipelines/{id}", async context =>
            {
                var pipelineId = context.Request.RouteValues["id"]?.ToString();
                if (string.IsNullOrEmpty(pipelineId))
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                var pipelineManager = context.RequestServices.GetService<PipelineManager>();
                var details = pipelineManager?.GetPipelineDetails(pipelineId);

                if (details == null)
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(details);
            });

            // Development endpoints
            if (env.IsDevelopment())
            {
                endpoints.MapGet("/", async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync($@"
                        <html>
                        <head><title>{ServiceInfo.GetFullVersionString()}</title></head>
                        <body>
                            <h1>CamBridge Service - Centralized Configuration</h1>
                            <p>Version: {ServiceInfo.Version}</p>
                            <p>Service läuft mit zentraler Config!</p>
                            <ul>
                                <li>Health: <a href='/health'>/health</a></li>
                                <li>Status: <a href='/api/status'>/api/status</a></li>
                                <li>Pipelines: <a href='/api/pipelines'>/api/pipelines</a></li>
                            </ul>
                            <hr/>
                            <small>{ServiceInfo.Copyright}</small>
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
