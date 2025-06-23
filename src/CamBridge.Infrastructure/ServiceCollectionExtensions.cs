// src/CamBridge.Infrastructure/ServiceCollectionExtensions.cs
// Version: 0.7.31
// Description: DI container configuration - Fixed ExifToolReader registration
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
// using Microsoft.Extensions.Diagnostics.HealthChecks; // Might need this package

namespace CamBridge.Infrastructure
{
    /// <summary>
    /// Extension methods for service registration
    /// PIPELINE UPDATE: FileProcessor no longer registered as singleton!
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds infrastructure services to the DI container
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add configuration
            services.Configure<CamBridgeSettingsV2>(configuration.GetSection("CamBridge"));
            services.Configure<ProcessingOptions>(configuration.GetSection("CamBridge:DefaultProcessingOptions"));

            // Add notification settings (global)
            services.Configure<NotificationSettings>(configuration.GetSection("CamBridge:Notifications"));

            // Register shared services (used by all pipelines)
            // FIX: ExifToolReader needs the path from configuration!
            services.AddSingleton<ExifToolReader>(sp =>
            {
                var settings = sp.GetRequiredService<IOptionsMonitor<CamBridgeSettingsV2>>().CurrentValue;
                return new ExifToolReader(
                    sp.GetRequiredService<ILogger<ExifToolReader>>(),
                    settings.ExifToolPath ?? "Tools\\exiftool.exe"
                );
            });

            services.AddSingleton<DicomConverter>();

            // FileProcessor is NO LONGER registered here!
            // It's created per-pipeline in PipelineManager!
            // services.AddSingleton<FileProcessor>(); // REMOVED!

            // Register pipeline manager
            services.AddSingleton<PipelineManager>();

            // Register notification service (v0.7.18: Direct class, no interface!)
            services.AddSingleton<NotificationService>();

            // Register remaining interfaces (only 2 left!)
            services.AddSingleton<IMappingConfiguration, MappingConfigurationLoader>();
            services.AddSingleton<IDicomTagMapper, DicomTagMapper>();

            return services;
        }
    }
}
