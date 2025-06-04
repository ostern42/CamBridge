using CamBridge.Infrastructure;
using CamBridge.Service;
using CamBridge.Infrastructure.Services; // Für FolderWatcherService
using Microsoft.Extensions.Hosting;
using Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
// Später aktivieren:
// using Microsoft.OpenApi.Models;
// using Microsoft.AspNetCore.Http;

// ========================================
// SCHRITT 1: Basis Pipeline (AKTIV)
// SCHRITT 2: Health Checks (auskommentiert)
// SCHRITT 3: Web API (auskommentiert)
// SCHRITT 4: Swagger (auskommentiert)
// SCHRITT 5: Windows Service (auskommentiert)
// ========================================

// Determine if running as service or console
var isService = false; // SCHRITT 5: Später auf true für Windows Service
// var isService = !(Environment.UserInteractive && args.Length > 0 && args[0] == "--console");

// Configure Serilog early for startup logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting CamBridge Service v0.5.22...");
    Log.Information("Running as: {Mode}", isService ? "Windows Service" : "Console Application");

    // ========== SCHRITT 1: BASIC PIPELINE (AKTIV) ==========
    Log.Information("SCHRITT 1: Basic Pipeline - AKTIV ✓");

    var builder = Host.CreateDefaultBuilder(args);

    // Configure for Windows Service or Console
    if (isService)
    {
        // SCHRITT 5: Windows Service aktivieren
        /*
        builder.UseWindowsService(options =>
        {
            options.ServiceName = "CamBridge Image Converter";
        });
        */
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

        // ========== SCHRITT 2: Health Checks (AUSKOMMENTIERT) ==========
        // Log.Information("SCHRITT 2: Health Checks - INAKTIV ❌");
        /*
        services.AddHealthChecks()
            .AddCheck<CamBridgeHealthCheck>("cambridge");
            
        // Daily Summary Service
        services.AddHostedService<DailySummaryService>();
        */

        // ========== SCHRITT 3: Web API (AUSKOMMENTIERT) ==========
        // Log.Information("SCHRITT 3: Web API - INAKTIV ❌");
        /*
        if (!isService)
        {
            services.AddControllers();

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
        }
        */
    });

    // ========== SCHRITT 3: Web Host konfigurieren (AUSKOMMENTIERT) ==========
    /*
    if (!isService)
    {
        builder.ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.UseUrls("http://localhost:5050");
        });
    }
    */

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
            Log.Information("✗ SCHRITT 2: Health Checks");
            Log.Information("✗ SCHRITT 3: Web API");
            Log.Information("✗ SCHRITT 4: Swagger");
            Log.Information("✗ SCHRITT 5: Windows Service");
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
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;

// ========== SCHRITT 3/4: Startup Klasse für API (AUSKOMMENTIERT) ==========
/*
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
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            
            // SCHRITT 2: Health Checks aktivieren
            // endpoints.MapHealthChecks("/health");

            // Development diagnostic endpoints
            if (env.IsDevelopment())
            {
                endpoints.MapGet("/", async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(@"
                        <html>
                        <head><title>CamBridge Service</title></head>
                        <body>
                            <h1>CamBridge Service - Development Mode</h1>
                            <p>API Features sind noch deaktiviert!</p>
                            <ul>
                                <li>✓ Pipeline läuft</li>
                                <li>✗ Health Check</li>
                                <li>✗ Service Status</li>
                                <li>✗ API Documentation</li>
                            </ul>
                        </body>
                        </html>");
                });
            }
        });
    }
}
*/
