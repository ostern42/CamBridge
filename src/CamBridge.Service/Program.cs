// src/CamBridge.Service/Program.cs
// Version: 0.8.10
// Description: Windows service entry point
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Core;
using CamBridge.Core.Infrastructure;
using CamBridge.Infrastructure;
using CamBridge.Infrastructure.Services;
using CamBridge.Service;
using CamBridge.Service.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// Register encoding provider for Windows-1252 support (needed for Ricoh EXIF data)
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

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

// Configure Serilog with TRUE multi-pipeline support
try
{
    var baseLogPath = ConfigurationPaths.GetLogsDirectory();

    // Ensure logs directory exists
    Directory.CreateDirectory(baseLogPath);

    // Load config to get pipeline names
    var configPath = ConfigurationPaths.GetPrimaryConfigPath();
    var configuration = new ConfigurationBuilder()
        .AddJsonFile(configPath, optional: true)
        .Build();

    var settings = new CamBridgeSettingsV2();
    configuration.GetSection("CamBridge").Bind(settings);

    var logConfig = new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .Enrich.FromLogContext()

        // Global service log - everything EXCEPT pipeline logs
        .WriteTo.Logger(lc => lc
            .Filter.ByExcluding(evt =>
                evt.Properties.ContainsKey("SourceContext") &&
                evt.Properties["SourceContext"].ToString().Contains("Pipeline."))
            .WriteTo.File(
                path: Path.Combine(baseLogPath, "service_.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                shared: true))

        // Event Log for service events
        .WriteTo.EventLog(ServiceInfo.ServiceName, "Application", manageEventSource: false)

        // Console output when not running as service
        .WriteTo.Conditional(
            _ => !isService,
            wt => wt.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"));

    // Create a separate log file for each pipeline
    if (settings?.Pipelines != null)
    {
        foreach (var pipeline in settings.Pipelines)
        {
            var sanitizedName = SanitizeForFileName(pipeline.Name);
            var pipelineContext = $"Pipeline.{sanitizedName}";

            logConfig.WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(evt =>
                    evt.Properties.ContainsKey("SourceContext") &&
                    evt.Properties["SourceContext"].ToString().Contains(pipelineContext))
                .WriteTo.File(
                    path: Path.Combine(baseLogPath, $"pipeline_{sanitizedName}_.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                    shared: true));

            Log.Information("Configured logging for pipeline: {Pipeline} -> {LogFile}",
                pipeline.Name, $"pipeline_{sanitizedName}_{{Date}}.log");
        }
    }

    Log.Logger = logConfig.CreateLogger();

    Log.Information("Serilog configured with multi-pipeline support at {LogPath}", baseLogPath);
    serviceEventLog?.WriteEntry($"Logging initialized with separate pipeline logs at: {baseLogPath}", EventLogEntryType.Information, 1005);
}
catch (Exception ex)
{
    serviceEventLog?.WriteEntry($"Failed to configure Serilog: {ex.Message}", EventLogEntryType.Error, 1006);
    throw;
}

try
{
    Log.Information("=== CamBridge Service Starting ===");
    Log.Information("Version: {Version}", ServiceInfo.GetFullVersionString());
    Log.Information("Running as: {Mode}", isService ? "Windows Service" : "Console Application");

    var builder = Host.CreateDefaultBuilder(args);

    // Use proper configuration for service vs console
    if (isService)
    {
        builder.UseWindowsService(options =>
        {
            options.ServiceName = ServiceInfo.ServiceName;
        });
    }

    builder
        .UseSerilog() // Use Serilog as the logging provider
        .ConfigureAppConfiguration((context, config) =>
        {
            // Clear default sources to have full control
            config.Sources.Clear();

            // Use centralized configuration path
            var configPath = ConfigurationPaths.GetPrimaryConfigPath();
            Log.Information("Loading configuration from: {ConfigPath}", configPath);

            // Initialize config if needed
            if (!File.Exists(configPath))
            {
                ConfigurationPaths.InitializePrimaryConfig();
                Log.Warning("Configuration file not found. Created default at: {ConfigPath}", configPath);
            }

            // Load configuration from our single source
            config.AddJsonFile(configPath, optional: false, reloadOnChange: true);

            // Add environment variables for override capability
            config.AddEnvironmentVariables("CAMBRIDGE_");

            // Add command line args last (highest priority)
            if (args != null)
            {
                config.AddCommandLine(args);
            }
        })
        .ConfigureServices((context, services) =>
        {
            var configuration = context.Configuration;

            // Configure settings with validation
            services.Configure<CamBridgeSettingsV2>(options =>
            {
                var cambridgeSection = configuration.GetSection("CamBridge");
                if (!cambridgeSection.Exists())
                {
                    throw new InvalidOperationException(
                        "Configuration is missing required 'CamBridge' section. " +
                        "Expected format: { \"CamBridge\": { \"Version\": \"2.0\", ... } }");
                }

                cambridgeSection.Bind(options);

                // Validate configuration structure
                ConfigValidator.ValidateSettings(options);

                // Validate configuration file for enum values
                var configPath = ConfigurationPaths.GetPrimaryConfigPath();
                ConfigValidator.ValidateAndWarn(configPath, Log.Logger);
            });

            // CRITICAL FIX: USE THE INFRASTRUCTURE EXTENSION METHOD!
            // This registers DicomStoreService and all other infrastructure services
            services.AddInfrastructure(configuration);

            // Background services
            services.AddHostedService<Worker>();
            //services.AddHostedService<DailySummaryService>(); // Activated!

            // Add health checks
            services.AddHealthChecks()
                .AddCheck<CamBridgeHealthCheck>("cambridge");

            // Add IHttpClientFactory for better HTTP client management
            services.AddHttpClient();
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseKestrel()
                .UseUrls("http://localhost:5111") // CRITICAL: Port 5111!
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseHealthChecks("/health");

                    app.UseEndpoints(endpoints =>
                    {
                        // Service info endpoint
                        endpoints.MapGet("/", async context =>
                        {
                            context.Response.ContentType = "application/json";
                            var info = new
                            {
                                service = ServiceInfo.ServiceName,
                                version = ServiceInfo.Version,
                                status = "running",
                                timestamp = DateTime.UtcNow
                            };
                            await context.Response.WriteAsJsonAsync(info);
                        });

                        // API endpoints
                        endpoints.MapGet("/api/status", StatusController.GetStatus);
                        endpoints.MapGet("/api/status/version", StatusController.GetVersion);
                        endpoints.MapGet("/api/status/health", StatusController.GetHealth);
                        endpoints.MapGet("/api/pipelines", StatusController.GetPipelines);
                        endpoints.MapGet("/api/pipelines/{id}", StatusController.GetPipelineDetails);
                        endpoints.MapGet("/api/statistics", StatusController.GetStatistics);
                    });
                });
        });

    var host = builder.Build();

    // Test configuration loading (non-production diagnostic)
    using (var scope = host.Services.CreateScope())
    {
        var settings = scope.ServiceProvider.GetService<IOptionsSnapshot<CamBridgeSettingsV2>>();
        if (settings?.Value != null)
        {
            Log.Information("Configuration loaded successfully:");
            Log.Information("  Version: {Version}", settings.Value.Version);
            Log.Information("  Pipelines: {Count}", settings.Value.Pipelines.Count);
            Log.Information("  Service Port: {Port}", settings.Value.Service?.ApiPort ?? 5111);
        }
        else
        {
            Log.Warning("Failed to load configuration!");
        }
    }

    await host.RunAsync();
}
catch (Exception ex)
{
    var message = $"Fatal error: {ex.Message}";
    Log.Fatal(ex, "Application terminated unexpectedly");
    serviceEventLog?.WriteEntry(message, EventLogEntryType.Error, 9999);
    return 1;
}
finally
{
    Log.Information("=== CamBridge Service Stopped ===");
    Log.CloseAndFlush();
}

return 0;

// Helper method to sanitize pipeline names for file names
static string SanitizeForFileName(string pipelineName)
{
    var invalid = Path.GetInvalidFileNameChars()
        .Concat(new[] { ' ', '.', ',', '/', '\\', ':', '-' })
        .Distinct()
        .ToArray();

    var sanitized = string.Join("_", pipelineName.Split(invalid, StringSplitOptions.RemoveEmptyEntries));

    if (sanitized.Length > 100)
    {
        sanitized = sanitized.Substring(0, 97) + "...";
    }

    return sanitized;
}

// Static class to hold service start time
public partial class Program
{
    public static DateTime ServiceStartTime { get; set; }
}
