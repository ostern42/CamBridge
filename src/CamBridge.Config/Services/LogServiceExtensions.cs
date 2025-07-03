// src/CamBridge.Config/Services/LogServiceExtensions.cs
// Version: 0.8.16
// Description: Dependency injection registration for log services
// Copyright: (C) 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Config.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Extension methods for registering log services
    /// </summary>
    public static class LogServiceExtensions
    {
        /// <summary>
        /// Register all log viewer services
        /// </summary>
        public static IServiceCollection AddLogViewerServices(this IServiceCollection services)
        {
            // Register log services
            services.AddSingleton<ILogParsingService, LogParsingService>();
            services.AddSingleton<ILogFilterService, LogFilterService>();
            services.AddSingleton<ILogTreeBuilder, LogTreeBuilder>();
            services.AddSingleton<ILogFileService, LogFileService>();

            return services;
        }
    }
}
