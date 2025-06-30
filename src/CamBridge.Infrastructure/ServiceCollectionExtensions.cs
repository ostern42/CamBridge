// src/CamBridge.Infrastructure/ServiceCollectionExtensions.cs
// Version: 0.8.8
// Description: DI container configuration with FIXED duplicate registration
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

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
            // FIXED: REMOVED duplicate Configure<CamBridgeSettingsV2>!
            // This is already done in Program.cs with validation logic
            // services.Configure<CamBridgeSettingsV2>(configuration.GetSection("CamBridge"));

            // Add other configuration sections (these are fine)
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

            // Register DICOM Store Service for PACS upload
            services.AddSingleton<DicomStoreService>();

            // FileProcessor is NO LONGER registered here!
            // It's created per-pipeline in PipelineManager!
            // services.AddSingleton<FileProcessor>(); // REMOVED!

            // Register pipeline manager
            services.AddSingleton<PipelineManager>();

            // Register notification service (v0.7.18: Direct class, no interface!)
            services.AddSingleton<NotificationService>();

            // FIXED: Register MappingConfigurationLoader as both interface AND concrete type
            services.AddSingleton<MappingConfigurationLoader>();
            services.AddSingleton<IMappingConfiguration>(sp => sp.GetRequiredService<MappingConfigurationLoader>());

            // Register remaining interfaces (only 2 left!)
            services.AddSingleton<IDicomTagMapper, DicomTagMapper>();

            return services;
        }
    }
}
