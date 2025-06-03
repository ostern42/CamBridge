// In CamBridge.Service/Program.cs
// Add these updates to properly load development configuration

using System;
using System.IO;
using System.Threading.Tasks;
using CamBridge.Infrastructure;
using CamBridge.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;

// Determine if running as service or console
var isService = !(Environment.UserInteractive && args.Length > 0 && args[0] == "--console");

// Configure Serilog early for startup logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting CamBridge Service...");
    Log.Information("Running as: {Mode}", isService ? "Windows Service" : "Console Application");

    var builder = Host.CreateDefaultBuilder(args);

    // Configure for Windows Service or Console
    if (isService)
    {
        builder.UseWindowsService(options =>
        {
            options.ServiceName = "CamBridge Image Converter";
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
    builder.UseSerilog((context, services, configuration) => configuration
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

        // Add infrastructure services
        services.AddInfrastructure(configuration);

        // Add service layer services
        services.AddServiceLayerServices();

        // Add the worker service
        services.AddHostedService<Worker>();

        // Add ASP.NET Core for API (if not running as service)
        if (!isService)
        {
            services.AddControllers();
            services.AddHealthChecks();

            // Add Swagger in development
            if (context.HostingEnvironment.IsDevelopment())
            {
                services.AddEndpointsApiExplorer();
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CamBridge API", Version = "v1" });
                });
            }
        }
    });

    if (!isService)
    {
        builder.ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.UseUrls("http://localhost:5050");
        });
    }

    var host = builder.Build();

    // Validate infrastructure
    using (var scope = host.Services.CreateScope())
    {
        try
        {
            scope.ServiceProvider.ValidateInfrastructure();
            Log.Information("Infrastructure validation completed successfully");
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

// Startup class for API configuration (console mode only)
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
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CamBridge API v1"));
        }

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");

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
                            <ul>
                                <li><a href='/health'>Health Check</a></li>
                                <li><a href='/api/status'>Service Status</a></li>
                                <li><a href='/swagger'>API Documentation</a></li>
                            </ul>
                        </body>
                        </html>");
                });
            }
        });
    }
}
