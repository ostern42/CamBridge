// File: src/CamBridge.Config/Extensions/MappingConfigurationExtensions.cs
// Version: 0.5.24
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions
// Modified: 2025-06-04
// Status: Development/Local

using System.Threading.Tasks;
using CamBridge.Core.Interfaces;
using CamBridge.Infrastructure.Services;

namespace CamBridge.Config.Extensions
{
    /// <summary>
    /// Extension methods for MappingConfigurationLoader to provide UI-expected methods
    /// </summary>
    public static class MappingConfigurationExtensions
    {
        /// <summary>
        /// Load configuration from file (UI-expected method name)
        /// </summary>
        public static async Task<IMappingConfiguration> LoadFromFileAsync(
            this MappingConfigurationLoader loader,
            string filePath)
        {
            await loader.LoadConfigurationAsync(filePath);
            return loader;
        }

        /// <summary>
        /// Save configuration to file (UI-expected method name)
        /// </summary>
        public static async Task SaveToFileAsync(
            this MappingConfigurationLoader loader,
            IMappingConfiguration config,
            string filePath)
        {
            // The loader itself implements IMappingConfiguration
            // So we just save its current rules
            await loader.SaveConfigurationAsync(loader.GetMappingRules(), filePath);
        }
    }
}
