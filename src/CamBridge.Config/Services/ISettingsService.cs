// src\CamBridge.Config\Services\ISettingsService.cs
// Version: 0.7.3
// Description: Multi-layer settings service interface
// Â© 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CamBridge.Core;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Service for managing the 3-layer settings architecture:
    /// - System settings (shared between Service and Config Tool)
    /// - Pipeline configurations (shared, multiple instances)
    /// - User preferences (per-user UI settings)
    /// </summary>
    public interface ISettingsService
    {
        // === SYSTEM-WIDE SETTINGS ===

        /// <summary>
        /// Gets the current system settings
        /// </summary>
        Task<SystemSettings> GetSystemSettingsAsync();

        /// <summary>
        /// Saves system settings with backup
        /// </summary>
        Task SaveSystemSettingsAsync(SystemSettings settings);

        /// <summary>
        /// Reloads system settings from disk
        /// </summary>
        Task ReloadSystemSettingsAsync();

        /// <summary>
        /// Validates system settings
        /// </summary>
        Task<SettingsValidationResult> ValidateSystemSettingsAsync(SystemSettings settings);

        // === PIPELINE CONFIGURATIONS ===

        /// <summary>
        /// Gets all available pipeline configurations
        /// </summary>
        Task<IList<PipelineConfiguration>> GetPipelinesAsync();

        /// <summary>
        /// Gets a specific pipeline configuration
        /// </summary>
        Task<PipelineConfiguration?> GetPipelineAsync(Guid pipelineId);

        /// <summary>
        /// Saves a pipeline configuration
        /// </summary>
        Task SavePipelineAsync(PipelineConfiguration pipeline);

        /// <summary>
        /// Deletes a pipeline configuration
        /// </summary>
        Task DeletePipelineAsync(Guid pipelineId);

        /// <summary>
        /// Imports a pipeline configuration from file
        /// </summary>
        Task<PipelineConfiguration> ImportPipelineAsync(string filePath);

        /// <summary>
        /// Exports a pipeline configuration to file
        /// </summary>
        Task ExportPipelineAsync(Guid pipelineId, string filePath);

        /// <summary>
        /// Creates a copy of an existing pipeline
        /// </summary>
        Task<PipelineConfiguration> ClonePipelineAsync(Guid sourcePipelineId, string newName);

        // === USER PREFERENCES ===

        /// <summary>
        /// Gets the current user preferences
        /// </summary>
        Task<UserPreferences> GetUserPreferencesAsync();

        /// <summary>
        /// Saves user preferences
        /// </summary>
        Task SaveUserPreferencesAsync(UserPreferences preferences);

        /// <summary>
        /// Resets user preferences to defaults
        /// </summary>
        Task ResetUserPreferencesAsync();

        /// <summary>
        /// Exports user preferences for backup
        /// </summary>
        Task ExportUserPreferencesAsync(string filePath);

        /// <summary>
        /// Imports user preferences from backup
        /// </summary>
        Task ImportUserPreferencesAsync(string filePath);

        // === MIGRATION AND MAINTENANCE ===

        /// <summary>
        /// Checks if settings migration is needed and performs it
        /// </summary>
        Task<SettingsMigrationResult> MigrateSettingsIfNeededAsync();

        /// <summary>
        /// Creates backups of all settings
        /// </summary>
        Task<SettingsBackupResult> BackupAllSettingsAsync();

        /// <summary>
        /// Restores settings from a backup
        /// </summary>
        Task RestoreFromBackupAsync(string backupPath);

        /// <summary>
        /// Validates all settings files
        /// </summary>
        Task<SettingsHealthCheckResult> ValidateAllSettingsAsync();

        /// <summary>
        /// Cleans up old backups and temporary files
        /// </summary>
        Task CleanupAsync(int keepBackupCount = 10);

        // === EVENTS ===

        /// <summary>
        /// Raised when system settings change
        /// </summary>
        event EventHandler<SettingsChangedEventArgs>? SystemSettingsChanged;

        /// <summary>
        /// Raised when a pipeline configuration changes
        /// </summary>
        event EventHandler<PipelineChangedEventArgs>? PipelineChanged;

        /// <summary>
        /// Raised when user preferences change
        /// </summary>
        event EventHandler<SettingsChangedEventArgs>? UserPreferencesChanged;
    }

    /// <summary>
    /// Result of settings validation
    /// </summary>
    public class SettingsValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }

    /// <summary>
    /// Result of settings migration
    /// </summary>
    public class SettingsMigrationResult
    {
        public bool MigrationPerformed { get; set; }
        public string FromVersion { get; set; } = string.Empty;
        public string ToVersion { get; set; } = string.Empty;
        public List<string> MigratedFiles { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }

    /// <summary>
    /// Result of backup operation
    /// </summary>
    public class SettingsBackupResult
    {
        public bool Success { get; set; }
        public string BackupPath { get; set; } = string.Empty;
        public List<string> BackedUpFiles { get; set; } = new();
        public long TotalSizeBytes { get; set; }
    }

    /// <summary>
    /// Result of health check
    /// </summary>
    public class SettingsHealthCheckResult
    {
        public bool IsHealthy { get; set; }
        public Dictionary<string, FileHealthStatus> FileStatuses { get; set; } = new();
        public List<string> Issues { get; set; } = new();
    }

    /// <summary>
    /// Health status of a settings file
    /// </summary>
    public class FileHealthStatus
    {
        public bool Exists { get; set; }
        public bool IsReadable { get; set; }
        public bool IsWritable { get; set; }
        public bool IsValidJson { get; set; }
        public long SizeBytes { get; set; }
        public DateTime LastModified { get; set; }
    }

    /// <summary>
    /// Event args for settings changes
    /// </summary>
    public class SettingsChangedEventArgs : EventArgs
    {
        public string SettingsType { get; set; } = string.Empty;
        public string ChangedBy { get; set; } = string.Empty;
        public DateTime ChangeTime { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Event args for pipeline changes
    /// </summary>
    public class PipelineChangedEventArgs : SettingsChangedEventArgs
    {
        public Guid PipelineId { get; set; }
        public string PipelineName { get; set; } = string.Empty;
        public PipelineChangeType ChangeType { get; set; }
    }

    /// <summary>
    /// Type of pipeline change
    /// </summary>
    public enum PipelineChangeType
    {
        Created,
        Updated,
        Deleted,
        Imported,
        Exported
    }
}
