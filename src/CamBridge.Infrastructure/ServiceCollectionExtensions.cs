// src/CamBridge.Infrastructure/ServiceCollectionExtensions.cs
// Version: 0.7.17
// Description: Extension methods for configuring infrastructure services - WITH ENUM VALIDATION!
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
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
    /// KISS UPDATE: Removed DeadLetterQueue completely!
    /// v0.7.17: Added enum validation!
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all infrastructure services to the dependency injection container
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Core Processing Services (shared across all pipelines)
            services.AddSingleton<FileProcessor>(); // KISS: No interface needed!
            services.AddSingleton<DicomConverter>(); // KISS: No interface needed!
            services.AddSingleton<IMappingConfiguration, MappingConfigurationLoader>();

            // ExifTool Reader - The ONLY solution! No interfaces, no fallbacks!
            services.AddSingleton<ExifToolReader>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<ExifToolReader>>();
                var timeoutMs = configuration.GetValue<int>("CamBridge:ExifToolTimeoutMs", 5000);

                // ExifToolReader handles discovery internally
                return new ExifToolReader(logger, timeoutMs);
            });

            // Pipeline Manager - The new orchestrator!
            services.AddSingleton<PipelineManager>();

            // Mapping Services
            services.AddSingleton<IDicomTagMapper, DicomTagMapper>();

            // Notification Services (ultra-minimal implementation - just logging!)
            services.AddSingleton<INotificationService, NotificationService>();

            // Note: ProcessingQueue is now created per-pipeline by PipelineManager
            // Note: DeadLetterQueue is REMOVED - Simple error folder approach!
            // Note: FolderWatcherService is replaced by per-pipeline watchers

            // Configure Options
            services.Configure<CamBridgeSettings>(configuration.GetSection("CamBridge")); // For backwards compatibility
            services.Configure<CamBridgeSettingsV2>(configuration.GetSection("CamBridge")); // New V2 settings
            services.Configure<ProcessingOptions>(configuration.GetSection("CamBridge:ProcessingOptions"));
            // Note: NotificationSettings removed - we just log now (KISS!)

            // NEW in v0.7.17: Add validation for the settings!
            services.AddSingleton<IPostConfigureOptions<CamBridgeSettingsV2>, CamBridgeSettingsV2Validator>();

            // Settings migration helper
            services.AddSingleton<IPostConfigureOptions<CamBridgeSettingsV2>, CamBridgeSettingsV2PostConfigure>();

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

            // Add ExifTool reader for config tool
            services.AddSingleton<ExifToolReader>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<ExifToolReader>>();
                return new ExifToolReader(logger);
            });

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
                    typeof(ExifToolReader),  // Direct type, no interface!
                    typeof(FileProcessor),   // KISS: Direct type!
                    typeof(DicomConverter),  // KISS: Direct type!
                    typeof(PipelineManager)  // New orchestrator
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

                // Validate ExifTool reader
                var exifToolReader = provider.GetRequiredService<ExifToolReader>();
                logger.LogInformation("ExifTool reader validated - the ONLY EXIF solution");

                // Validate DicomConverter (KISS: Direct reference!)
                var dicomConverter = provider.GetRequiredService<DicomConverter>();
                logger.LogInformation("DicomConverter validated - KISS approach working!");

                // Validate FileProcessor (KISS: Direct reference!)
                var fileProcessor = provider.GetRequiredService<FileProcessor>();
                logger.LogInformation("FileProcessor validated - Simple error handling active!");

                // Validate Pipeline Manager
                var pipelineManager = provider.GetRequiredService<PipelineManager>();
                logger.LogInformation("Pipeline Manager validated - ready for multi-pipeline processing");

                // Validate settings
                var settingsV2 = provider.GetRequiredService<IOptions<CamBridgeSettingsV2>>();
                if (settingsV2.Value.Pipelines.Count == 0)
                {
                    logger.LogWarning("No pipelines configured - using default pipeline from V1 settings");
                }
                else
                {
                    logger.LogInformation("Found {Count} configured pipelines", settingsV2.Value.Pipelines.Count);
                }

                logger.LogInformation("Infrastructure validation completed - KISS approach: DeadLetterQueue eliminated! -650 LOC removed!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Infrastructure validation failed");
                throw;
            }

            return provider;
        }

        /// <summary>
        /// NEW in v0.7.17: Validates enum values in settings
        /// </summary>
        private class CamBridgeSettingsV2Validator : IPostConfigureOptions<CamBridgeSettingsV2>
        {
            private readonly ILogger<CamBridgeSettingsV2Validator> _logger;

            public CamBridgeSettingsV2Validator(ILogger<CamBridgeSettingsV2Validator> logger)
            {
                _logger = logger;
            }

            public void PostConfigure(string? name, CamBridgeSettingsV2 options)
            {
                _logger.LogInformation("Validating CamBridge settings...");

                // Validate each pipeline
                foreach (var pipeline in options.Pipelines)
                {
                    if (pipeline.ProcessingOptions != null)
                    {
                        // Validate OutputOrganization
                        if (!Enum.IsDefined(typeof(OutputOrganization), pipeline.ProcessingOptions.OutputOrganization))
                        {
                            var validValues = string.Join(", ", Enum.GetNames(typeof(OutputOrganization)));
                            var error = $"Pipeline '{pipeline.Name}' has invalid OutputOrganization value. " +
                                       $"Valid values are: {validValues}. " +
                                       $"Check your configuration file!";

                            _logger.LogError(error);
                            throw new InvalidOperationException(error);
                        }

                        // Validate PostProcessingActions
                        if (!Enum.IsDefined(typeof(PostProcessingAction), pipeline.ProcessingOptions.SuccessAction))
                        {
                            var validValues = string.Join(", ", Enum.GetNames(typeof(PostProcessingAction)));
                            var error = $"Pipeline '{pipeline.Name}' has invalid SuccessAction value. " +
                                       $"Valid values are: {validValues}";

                            _logger.LogError(error);
                            throw new InvalidOperationException(error);
                        }

                        if (!Enum.IsDefined(typeof(PostProcessingAction), pipeline.ProcessingOptions.FailureAction))
                        {
                            var validValues = string.Join(", ", Enum.GetNames(typeof(PostProcessingAction)));
                            var error = $"Pipeline '{pipeline.Name}' has invalid FailureAction value. " +
                                       $"Valid values are: {validValues}";

                            _logger.LogError(error);
                            throw new InvalidOperationException(error);
                        }

                        _logger.LogInformation("Pipeline '{Pipeline}' configuration validated successfully", pipeline.Name);
                    }
                }

                _logger.LogInformation("All pipeline configurations validated successfully");
            }
        }

        /// <summary>
        /// Post-configure options to handle V1 to V2 migration
        /// </summary>
        private class CamBridgeSettingsV2PostConfigure : IPostConfigureOptions<CamBridgeSettingsV2>
        {
            private readonly IOptions<CamBridgeSettings> _v1Settings;
            private readonly ILogger<CamBridgeSettingsV2PostConfigure> _logger;

            public CamBridgeSettingsV2PostConfigure(
                IOptions<CamBridgeSettings> v1Settings,
                ILogger<CamBridgeSettingsV2PostConfigure> logger)
            {
                _v1Settings = v1Settings;
                _logger = logger;
            }

            public void PostConfigure(string? name, CamBridgeSettingsV2 options)
            {
                // If no pipelines configured, migrate from V1
                if (options.Pipelines.Count == 0 && _v1Settings.Value != null)
                {
                    _logger.LogInformation("No pipelines configured, migrating from V1 settings");

                    // Use the built-in migration method
                    var migrated = CamBridgeSettingsV2.MigrateFromV1(_v1Settings.Value);

                    // Copy all migrated values to options
                    options.Version = migrated.Version;
                    options.Pipelines = migrated.Pipelines;
                    options.MappingSets = migrated.MappingSets;
                    options.GlobalDicomSettings = migrated.GlobalDicomSettings;
                    options.DefaultProcessingOptions = migrated.DefaultProcessingOptions;
                    options.Logging = migrated.Logging;
                    options.Service = migrated.Service;
                    options.Notifications = migrated.Notifications;
                    options.MigratedFrom = migrated.MigratedFrom;
                }
            }
        }
    }
}
