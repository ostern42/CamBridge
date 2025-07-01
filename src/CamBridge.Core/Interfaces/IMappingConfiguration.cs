// File: src/CamBridge.Core/Interfaces/IMappingConfiguration.cs
// Version: 0.8.10
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions
// Modified: 2025-06-30
// Status: Added correlation ID overload

using System.Collections.Generic;
using System.Threading.Tasks;

namespace CamBridge.Core.Interfaces
{
    /// <summary>
    /// Interface for managing DICOM mapping configurations
    /// </summary>
    public interface IMappingConfiguration
    {
        /// <summary>
        /// Gets all configured mapping rules
        /// </summary>
        IReadOnlyList<MappingRule> GetMappingRules();

        /// <summary>
        /// Loads mapping configuration from a file
        /// </summary>
        /// <param name="filePath">Path to the configuration file (optional)</param>
        /// <returns>True if loaded successfully, false otherwise</returns>
        Task<bool> LoadConfigurationAsync(string? filePath = null);

        /// <summary>
        /// Loads mapping configuration from a file with correlation ID for logging
        /// </summary>
        /// <param name="filePath">Path to the configuration file (optional)</param>
        /// <param name="correlationId">Correlation ID for log tracking</param>
        /// <returns>True if loaded successfully, false otherwise</returns>
        Task<bool> LoadConfigurationAsync(string? filePath, string? correlationId);

        /// <summary>
        /// Saves mapping configuration to a file
        /// </summary>
        /// <param name="rules">The mapping rules to save</param>
        /// <param name="filePath">Path to save the configuration (optional)</param>
        /// <returns>True if saved successfully, false otherwise</returns>
        Task<bool> SaveConfigurationAsync(IEnumerable<MappingRule> rules, string? filePath = null);

        /// <summary>
        /// Adds a new mapping rule
        /// </summary>
        void AddRule(MappingRule rule);

        /// <summary>
        /// Removes mapping rules for a specific source field
        /// </summary>
        void RemoveRule(string sourceField);

        /// <summary>
        /// Gets the mapping rule for a specific source field
        /// </summary>
        MappingRule? GetRuleForSource(string sourceField);

        /// <summary>
        /// Gets all mapping rules that target a specific DICOM tag
        /// </summary>
        IEnumerable<MappingRule> GetRulesForTag(string dicomTag);

        /// <summary>
        /// Validates all mapping rules
        /// </summary>
        void ValidateRules();
    }
}
