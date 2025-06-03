using System;
using System.IO;
using System.Linq;
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CamBridge.Infrastructure
{
    /// <summary>
    /// Extension methods for configuring infrastructure services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all infrastructure services to the dependency injection container
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Core Processing Services
            services.AddSingleton<IFileProcessor, FileProcessor>();
            services.AddSingleton<IDicomConverter, DicomConverter>();
            services.AddSingleton<IMappingConfiguration, MappingConfigurationLoader>();

            // EXIF Reader Registration with Fallback Chain
            // Register individual readers
            services.AddSingleton<ExifToolReader>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<ExifToolReader>>();

                var exifToolPath = configuration["CamBridge:ExifToolPath"];
                if (string.IsNullOrEmpty(exifToolPath))
                {
                    // Try environment variable
                    exifToolPath = Environment.GetEnvironmentVariable("CAMBRIDGE_EXIFTOOL_PATH");
                }

                var timeoutMs = configuration.GetValue<int>("CamBridge:ExifToolTimeoutMs", 5000);

                return new ExifToolReader(logger, exifToolPath, timeoutMs);
            });

            services.AddSingleton<ExifToolReader>(); // The ONLY EXIF solution!

            // Queue and Processing Services
            services.AddSingleton<ProcessingQueue>();
            services.AddSingleton<DeadLetterQueue>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<DeadLetterQueue>>();
                var deadLetterPath = configuration["CamBridge:ProcessingOptions:DeadLetterFolder"]
                    ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DeadLetter");

                return new DeadLetterQueue(logger, deadLetterPath);
            });

            // Mapping Services
            services.AddSingleton<IDicomTagMapper, DicomTagMapper>();

            // Notification Services
            services.AddSingleton<INotificationService, NotificationService>();

            // Folder Watcher Service
            services.AddSingleton<FolderWatcherService>();

            // Note: Health checks should be added in the Service project
            // where Microsoft.Extensions.Diagnostics.HealthChecks is referenced

            // Configure Options
            services.Configure<CamBridgeSettings>(configuration.GetSection("CamBridge"));
            services.Configure<ProcessingOptions>(configuration.GetSection("CamBridge:ProcessingOptions"));
            services.Configure<NotificationSettings>(configuration.GetSection("CamBridge:NotificationSettings"));

            // Logging is configured in the host builder
            // No need to configure it here

            return services;
        }

        /// <summary>
        /// Adds infrastructure services for the configuration tool
        /// </summary>
        public static IServiceCollection AddInfrastructureForConfig(this IServiceCollection services)
        {
            // Only add services needed by the configuration tool
            services.AddSingleton<IMappingConfiguration, MappingConfigurationLoader>();
            services.AddSingleton<DicomTagMapper>();

            // Add minimal EXIF reader for testing/preview
            services.AddSingleton<ExifReader>();
            services.AddSingleton<IExifReader>(provider => provider.GetRequiredService<ExifReader>());

            return services;
        }

        /// <summary>
        /// Validates that all required services are properly configured
        /// </summary>
        public static IServiceProvider ValidateInfrastructure(this IServiceProvider provider)
        {
            // Create a logger for validation
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("Infrastructure.Validation");

            try
            {
                // Validate critical services
                var criticalServices = new[]
                {
                    typeof(IExifReader),
                    typeof(IFileProcessor),
                    typeof(IDicomConverter),
                    typeof(ProcessingQueue),
                    typeof(FolderWatcherService)
                };

                foreach (var serviceType in criticalServices)
                {
                    var service = provider.GetService(serviceType);
                    if (service == null)
                    {
                        logger.LogError("Critical service {ServiceType} is not registered", serviceType.Name);
                        throw new InvalidOperationException($"Critical service {serviceType.Name} is not registered");
                    }
                }

                // Check EXIF reader status
                var exifReader = provider.GetRequiredService<IExifReader>();
                if (exifReader is CompositeExifReader composite)
                {
                    var status = composite.GetReaderStatus();
                    logger.LogInformation("EXIF Reader Status: {Status}",
                        string.Join(", ", status.Select(kvp => $"{kvp.Key}={kvp.Value}")));

                    if (!status.Any(kvp => kvp.Value))
                    {
                        logger.LogError("No EXIF readers are available!");
                    }
                }

                logger.LogInformation("Infrastructure validation completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Infrastructure validation failed");
                throw;
            }

            return provider;
        }
    }
}
