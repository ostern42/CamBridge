// src\CamBridge.Config\Services\IPipelineSettingsService.cs
// Version: 0.8.5
// Description: Service interface for pipeline settings operations
// Session: 95 - Extracting business logic from ViewModels

using CamBridge.Core;
using System;
using System.Threading.Tasks;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Service for pipeline settings operations
    /// Extracted from PipelineConfigViewModel to enable testing
    /// </summary>
    public interface IPipelineSettingsService
    {
        /// <summary>
        /// Load settings from configuration
        /// </summary>
        Task<CamBridgeSettingsV2?> LoadSettingsAsync();

        /// <summary>
        /// Save settings to configuration with backup
        /// </summary>
        Task SaveSettingsAsync(CamBridgeSettingsV2 settings);

        /// <summary>
        /// Create a deep clone of a pipeline configuration
        /// </summary>
        PipelineConfiguration ClonePipeline(PipelineConfiguration source);

        /// <summary>
        /// Create a new pipeline with default settings
        /// </summary>
        PipelineConfiguration CreateDefaultPipeline(string? name = null);

        /// <summary>
        /// Validate pipeline configuration
        /// </summary>
        ValidationResult ValidatePipeline(PipelineConfiguration pipeline);

        /// <summary>
        /// Get backup path for current save operation
        /// </summary>
        string GetBackupPath();
    }

    /// <summary>
    /// Result of pipeline validation
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; init; }
        public string[] Errors { get; init; } = Array.Empty<string>();
        public string[] Warnings { get; init; } = Array.Empty<string>();

        public static ValidationResult Success() => new() { IsValid = true };

        public static ValidationResult Failure(params string[] errors) => new()
        {
            IsValid = false,
            Errors = errors
        };
    }
}
