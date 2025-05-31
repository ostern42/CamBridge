using CamBridge.Core.Interfaces;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CamBridge.Infrastructure
{
    /// <summary>
    /// Extension methods for configuring CamBridge infrastructure services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds CamBridge infrastructure services to the dependency injection container
        /// </summary>
        public static IServiceCollection AddCamBridgeInfrastructure(this IServiceCollection services)
        {
            // Register EXIF reader services
            services.AddScoped<IExifReader, ExifReader>();
            services.AddScoped<RicohExifReader>(); // Can be injected directly when needed

            // Register mapping configuration services
            services.AddSingleton<IMappingConfiguration>(provider =>
            {
                // Default to built-in configuration
                // Can be overridden by loading from file in startup
                return IMappingConfiguration.GetDefault();
            });
            services.AddScoped<MappingConfigurationLoader>();
            services.AddScoped<IDicomTagMapper, DicomTagMapper>();

            // Register DICOM converter service with mapper support
            services.AddScoped<IDicomConverter, DicomConverter>();

            // Register file processor service
            services.AddScoped<IFileProcessor, FileProcessor>();

            return services;
        }

        /// <summary>
        /// Adds CamBridge infrastructure services with specific implementations
        /// </summary>
        public static IServiceCollection AddCamBridgeInfrastructure(this IServiceCollection services,
            bool useRicohExifReader)
        {
            // Register EXIF reader based on configuration
            if (useRicohExifReader)
            {
                services.AddScoped<IExifReader, RicohExifReader>();
            }
            else
            {
                services.AddScoped<IExifReader, ExifReader>();
            }

            // Register mapping configuration services
            services.AddSingleton<IMappingConfiguration>(provider =>
            {
                return IMappingConfiguration.GetDefault();
            });
            services.AddScoped<MappingConfigurationLoader>();
            services.AddScoped<IDicomTagMapper, DicomTagMapper>();

            // Register DICOM converter service
            services.AddScoped<IDicomConverter, DicomConverter>();

            // Register file processor service
            services.AddScoped<IFileProcessor, FileProcessor>();

            return services;
        }

        /// <summary>
        /// Adds CamBridge infrastructure with custom mapping configuration from file
        /// </summary>
        public static IServiceCollection AddCamBridgeInfrastructure(this IServiceCollection services,
            string mappingConfigurationPath,
            bool useRicohExifReader = false)
        {
            // Register EXIF reader
            if (useRicohExifReader)
            {
                services.AddScoped<IExifReader, RicohExifReader>();
            }
            else
            {
                services.AddScoped<IExifReader, ExifReader>();
            }

            // Register mapping configuration loader as singleton
            services.AddSingleton<MappingConfigurationLoader>();

            // Register mapping configuration from file
            services.AddSingleton<IMappingConfiguration>(provider =>
            {
                var loader = provider.GetRequiredService<MappingConfigurationLoader>();
                var config = loader.LoadFromFileAsync(mappingConfigurationPath).GetAwaiter().GetResult();

                // Validate configuration
                if (config is CustomMappingConfiguration customConfig)
                {
                    var validation = customConfig.Validate();
                    if (!validation.IsValid)
                    {
                        var logger = provider.GetRequiredService<ILogger<IMappingConfiguration>>();
                        logger.LogWarning("Mapping configuration has validation errors: {Errors}",
                            string.Join("; ", validation.Errors));
                    }
                }

                return config;
            });

            services.AddScoped<IDicomTagMapper, DicomTagMapper>();
            services.AddScoped<IDicomConverter, DicomConverter>();
            services.AddScoped<IFileProcessor, FileProcessor>();

            return services;
        }
    }
}
