// src/CamBridge.Service/Program.cs
// Version: 0.5.27
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions
// Modified: 2025-06-05
// Status: Production Ready with Windows Service

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
// using Microsoft.OpenApi.Models; // SCHRITT 4: Swagger später

// ========================================
// SCHRITT 1: Basis Pipeline (AKTIV) ✓
// SCHRITT 2: Health Checks (AKTIV) ✓
// SCHRITT 3: Web API (AKTIV) ✓
// SCHRITT 4: Swagger (auskommentiert)
// SCHRITT 5: Windows Service (AKTIV) ✓
// ========================================

// Set service start time
Program.ServiceStartTime = DateTime.UtcNow;

// Determine if running as service or console
// Check if we're running as a Windows Service by looking for console input
var isService = !Environment.UserInteractive ||
                (!args.Contains("--console") && !args.Contains("-c"));

// Configure Serilog early for startup logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting CamBridge Service v0.5.27...");
    Log.Information("Running as: {Mode}", isService ? "Windows Service" : "Console Application");
    Log.Information("Command line args: {Args}", string.Join(" ", args));

    // ========== SCHRITT 1: BASIC PIPELINE (AKTIV) ==========
    Log.Information("SCHRITT 1: Basic Pipeline - AKTIV ✓");

    var builder = Host.CreateDefaultBuilder(args);

    // Configure for Windows Service or Console
    if (isService)
    {
        // SCHRITT 5: Windows Service aktivieren ✓
        Log.Information("SCHRITT 5: Windows Service - AKTIV ✓");
        builder.UseWindowsService(options =>
        {
            options.ServiceName = "CamBridgeService";
        });
    }
    else
    {
        // Console mode - ensure we load Development settings
        builder.UseEnvironment("Development");
    }

    builder.ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment;

        config.SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables("CAMBRIDGE_")
              .AddCommandLine(args);

        Log.Information("Configuration loaded for environment: {Environment}", env.EnvironmentName);
    });

    // Configure Serilog from configuration
    builder.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("ApplicationName", "CamBridge")
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
            path: context.Configuration["Logging:File:Path"] ?? "logs/cambridge-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}"));

    builder.ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // ========== SCHRITT 1: Core Infrastructure (AKTIV) ==========
        services.AddInfrastructure(configuration);

        // Zusätzliche Services die AddInfrastructure nicht abdeckt
        services.AddSingleton<FolderWatcherService>();
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<FolderWatcherService>());

        // Basic Worker - immer aktiv
        services.AddHostedService<Worker>();

        // ========== SCHRITT 2: Health Checks (JETZT AKTIV!) ==========
        Log.Information("SCHRITT 2: Health Checks - AKTIV ✓");
        services.AddHealthChecks()
            .AddCheck<CamBridgeHealthCheck>("cambridge");

        // Daily Summary Service
        services.AddHostedService<DailySummaryService>();

        // ========== SCHRITT 3: Web API (JETZT AKTIV!) ==========
        Log.Information("SCHRITT 3: Web API - AKTIV ✓");

        // Web API is needed for both Service and Console mode
        services.AddControllers();

        // Add CORS for Config UI
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

        // ========== SCHRITT 4: Swagger (AUSKOMMENTIERT) ==========
        // Log.Information("SCHRITT 4: Swagger - INAKTIV ❌");
        // if (context.HostingEnvironment.IsDevelopment())
        // {
        //     services.AddEndpointsApiExplorer();
        //     services.AddSwaggerGen(c =>
        //     {
        //         c.SwaggerDoc("v1", new OpenApiInfo { Title = "CamBridge API", Version = "v1" });
        //     });
        // }
    });

    // ========== SCHRITT 3: Web Host konfigurieren (AKTIV für beide Modi) ==========
    builder.ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
        webBuilder.UseUrls("http://localhost:5050");
    });

    var host = builder.Build();

    // Validate infrastructure
    using (var scope = host.Services.CreateScope())
    {
        try
        {
            scope.ServiceProvider.ValidateInfrastructure();
            Log.Information("Infrastructure validation completed successfully");

            // Zeige aktive Features
            Log.Information("=========================================");
            Log.Information("AKTIVE FEATURES:");
            Log.Information("✓ SCHRITT 1: Basic Pipeline (ExifTool → DICOM)");
            Log.Information("✓ SCHRITT 2: Health Checks");
            Log.Information("✓ SCHRITT 3: Web API");
            Log.Information("✗ SCHRITT 4: Swagger");
            Log.Information(isService ? "✓ SCHRITT 5: Windows Service" : "✗ SCHRITT 5: Windows Service (Console Mode)");
            Log.Information("=========================================");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Infrastructure validation failed");
            throw;
        }
    }

    // Run the host
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");

    // Log to Windows Event Log if running as service
    if (isService)
    {
        try
        {
            using var eventLog = new System.Diagnostics.EventLog("Application");
            eventLog.Source = "CamBridgeService";
            eventLog.WriteEntry($"Service failed to start: {ex.Message}",
                System.Diagnostics.EventLogEntryType.Error, 1001);
        }
        catch { }
    }

    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;

// ========== SCHRITT 3/4: Startup Klasse für API (JETZT AKTIV!) ==========
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

            // SCHRITT 4: Swagger aktivieren
            // app.UseSwagger();
            // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CamBridge API v1"));
        }

        app.UseRouting();

        // Enable CORS
        app.UseCors("ConfigUI");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            // SCHRITT 2: Health Checks aktivieren
            endpoints.MapHealthChecks("/health");

            // Simple status endpoint for Config UI
            endpoints.MapGet("/api/status", async context =>
            {
                var processingQueue = context.RequestServices.GetService<ProcessingQueue>();
                var deadLetterQueue = context.RequestServices.GetService<DeadLetterQueue>();
                var settings = context.RequestServices.GetService<IOptions<CamBridge.Core.CamBridgeSettings>>();

                // Get statistics safely
                var queueStats = processingQueue?.GetStatistics();
                var deadLetterStats = deadLetterQueue?.GetStatistics();

                var totalProcessed = (queueStats?.TotalSuccessful ?? 0) + (queueStats?.TotalFailed ?? 0);
                var successRate = totalProcessed > 0
                    ? (double)(queueStats?.TotalSuccessful ?? 0) / totalProcessed * 100
                    : 0;

                var status = new
                {
                    ServiceStatus = "Running",
                    Version = "0.5.27",
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

            // Development diagnostic endpoints
            if (env.IsDevelopment())
            {
                endpoints.MapGet("/", async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(@"
                        <html>
                        <head><title>CamBridge Service v0.5.27</title></head>
                        <body>
                            <h1>CamBridge Service - Development Mode</h1>
                            <p>API Features sind jetzt aktiviert!</p>
                            <ul>
                                <li>✓ Pipeline läuft</li>
                                <li>✓ Health Check: <a href='/health'>/health</a></li>
                                <li>✓ Service Status: <a href='/api/status'>/api/status</a></li>
                                <li>✓ Windows Service Support</li>
                                <li>✗ API Documentation (Swagger noch deaktiviert)</li>
                            </ul>
                            <hr>
                            <p>Config UI kann sich jetzt über Port 5050 verbinden!</p>
                            <p>Console Mode: Start mit --console oder -c Parameter</p>
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
