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

            // Register DICOM converter service
            services.AddScoped<IDicomConverter, DicomConverter>();

            // TODO: Register additional services in future phases
            // services.AddScoped<IFileProcessor, FileProcessor>();
            // services.AddScoped<IMappingConfiguration, DefaultMappingConfiguration>();

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

            // Register DICOM converter service
            services.AddScoped<IDicomConverter, DicomConverter>();

            return services;
        }
    }
}