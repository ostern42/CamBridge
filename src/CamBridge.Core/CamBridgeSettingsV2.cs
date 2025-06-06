// src\CamBridge.Core\CamBridgeSettingsV2.cs
// Version: 0.6.2
// Description: Version 2 settings with pipeline architecture and migration support

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace CamBridge.Core
{
    /// <summary>
    /// Version 2 configuration with pipeline-based architecture
    /// </summary>
    public class CamBridgeSettingsV2
    {
        /// <summary>
        /// Settings version for migration support
        /// </summary>
        public string Version { get; set; } = "2.0";

        /// <summary>
        /// List of configured pipelines
        /// </summary>
        public List<PipelineConfiguration> Pipelines { get; set; } = new();

        /// <summary>
        /// Reusable mapping sets
        /// </summary>
        public List<MappingSet> MappingSets { get; set; } = new();

        /// <summary>
        /// Global DICOM settings (can be overridden per pipeline)
        /// </summary>
        public DicomSettings GlobalDicomSettings { get; set; } = new();

        /// <summary>
        /// Global defaults for new pipelines
        /// </summary>
        public ProcessingOptions DefaultProcessingOptions { get; set; } = new();

        /// <summary>
        /// Logging configuration (remains global)
        /// </summary>
        public LoggingSettings Logging { get; set; } = new();

        /// <summary>
        /// Service configuration (remains global)
        /// </summary>
        public ServiceSettings Service { get; set; } = new();

        /// <summary>
        /// Notification settings (remains global)
        /// </summary>
        public NotificationSettings Notifications { get; set; } = new();

        /// <summary>
        /// Migration timestamp
        /// </summary>
        public DateTime? MigratedFrom { get; set; }

        /// <summary>
        /// ExifTool executable path (global setting)
        /// </summary>
        public string ExifToolPath { get; set; } = "Tools\\exiftool.exe";

        /// <summary>
        /// Convenience property for backward compatibility - returns first pipeline's output folder
        /// </summary>
        [JsonIgnore]
        public string DefaultOutputFolder
        {
            get => Pipelines.FirstOrDefault()?.ProcessingOptions?.ArchiveFolder
                   ?? DefaultProcessingOptions?.ArchiveFolder
                   ?? @"C:\CamBridge\Output";
            set
            {
                if (DefaultProcessingOptions == null)
                    DefaultProcessingOptions = new ProcessingOptions();
                DefaultProcessingOptions.ArchiveFolder = value;
            }
        }

        /// <summary>
        /// Create V2 settings from V1 settings (migration)
        /// </summary>
        public static CamBridgeSettingsV2 MigrateFromV1(CamBridgeSettings v1Settings)
        {
            var v2Settings = new CamBridgeSettingsV2
            {
                MigratedFrom = DateTime.UtcNow,
                GlobalDicomSettings = v1Settings.Dicom,
                DefaultProcessingOptions = v1Settings.Processing,
                Logging = v1Settings.Logging,
                Service = v1Settings.Service,
                Notifications = v1Settings.Notifications,
                ExifToolPath = v1Settings.ExifToolPath ?? "Tools\\exiftool.exe"
            };

            // First, load existing mappings from file if available
            var defaultMappingSet = new MappingSet
            {
                Id = Guid.NewGuid(),
                Name = "Default Mapping Set",
                Description = "Migrated from v1 mappings.json",
                IsSystemDefault = false
            };

            // TODO: Load actual mapping rules from v1Settings.MappingConfigurationFile
            // For now, we'll assume the mapping loader will handle this separately

            v2Settings.MappingSets.Add(defaultMappingSet);

            // Convert each watch folder to a pipeline
            int pipelineIndex = 1;
            foreach (var folder in v1Settings.WatchFolders)
            {
                var pipeline = new PipelineConfiguration
                {
                    Id = Guid.NewGuid(),
                    Name = $"Legacy Pipeline {pipelineIndex}",
                    Description = $"Migrated from watch folder: {folder.Path}",
                    Enabled = folder.Enabled,
                    WatchSettings = new PipelineWatchSettings
                    {
                        Path = folder.Path,
                        FilePattern = folder.FilePattern,
                        IncludeSubdirectories = folder.IncludeSubdirectories,
                        OutputPath = folder.OutputPath
                    },
                    ProcessingOptions = CloneProcessingOptions(v1Settings.Processing),
                    MappingSetId = defaultMappingSet.Id,
                    Metadata = new Dictionary<string, string>
                    {
                        ["MigratedFrom"] = "v1",
                        ["OriginalIndex"] = (pipelineIndex - 1).ToString()
                    }
                };

                // If the folder had a custom output path, update the processing options
                if (!string.IsNullOrEmpty(folder.OutputPath))
                {
                    pipeline.ProcessingOptions.ArchiveFolder = folder.OutputPath;
                }

                v2Settings.Pipelines.Add(pipeline);
                pipelineIndex++;
            }

            // If no watch folders existed, create a default pipeline
            if (v2Settings.Pipelines.Count == 0)
            {
                v2Settings.Pipelines.Add(new PipelineConfiguration
                {
                    Name = "Default Pipeline",
                    Description = "Default pipeline created during migration",
                    MappingSetId = defaultMappingSet.Id,
                    ProcessingOptions = CloneProcessingOptions(v1Settings.Processing)
                });
            }

            return v2Settings;
        }

        /// <summary>
        /// Convert V2 settings back to V1 format (for backward compatibility)
        /// </summary>
        public CamBridgeSettings ToV1Format()
        {
            var v1Settings = new CamBridgeSettings
            {
                Dicom = GlobalDicomSettings,
                Processing = DefaultProcessingOptions,
                Logging = Logging,
                Service = Service,
                Notifications = Notifications,
                DefaultOutputFolder = DefaultOutputFolder,
                UseRicohExifReader = true,
                MappingConfigurationFile = "mappings.json",
                ExifToolPath = ExifToolPath
            };

            // If we have pipelines, use the first enabled pipeline's processing options
            // (since v1 only supports global processing options)
            var firstEnabledPipeline = Pipelines.FirstOrDefault(p => p.Enabled);
            if (firstEnabledPipeline != null)
            {
                v1Settings.Processing = CloneProcessingOptions(firstEnabledPipeline.ProcessingOptions);
            }

            // Convert pipelines back to watch folders
            foreach (var pipeline in Pipelines.Where(p => p.Enabled))
            {
                var folderConfig = new FolderConfiguration
                {
                    Path = pipeline.WatchSettings.Path,
                    OutputPath = pipeline.WatchSettings.OutputPath,
                    Enabled = pipeline.Enabled,
                    IncludeSubdirectories = pipeline.WatchSettings.IncludeSubdirectories,
                    FilePattern = pipeline.WatchSettings.FilePattern
                };

                v1Settings.WatchFolders.Add(folderConfig);
            }

            return v1Settings;
        }

        /// <summary>
        /// Helper to clone processing options
        /// </summary>
        private static ProcessingOptions CloneProcessingOptions(ProcessingOptions source)
        {
            return new ProcessingOptions
            {
                SuccessAction = source.SuccessAction,
                FailureAction = source.FailureAction,
                ArchiveFolder = source.ArchiveFolder,
                ErrorFolder = source.ErrorFolder,
                BackupFolder = source.BackupFolder,
                CreateBackup = source.CreateBackup,
                MaxConcurrentProcessing = source.MaxConcurrentProcessing,
                RetryOnFailure = source.RetryOnFailure,
                MaxRetryAttempts = source.MaxRetryAttempts,
                OutputOrganization = source.OutputOrganization,
                ProcessExistingOnStartup = source.ProcessExistingOnStartup,
                MaxFileAge = source.MaxFileAge,
                MinimumFileSizeBytes = source.MinimumFileSizeBytes,
                MaximumFileSizeBytes = source.MaximumFileSizeBytes,
                OutputFilePattern = source.OutputFilePattern,
                RetryDelaySeconds = source.RetryDelaySeconds,
                DeadLetterFolder = source.DeadLetterFolder
            };
        }

        /// <summary>
        /// Validate the configuration
        /// </summary>
        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                // At least one pipeline must be configured
                if (Pipelines.Count == 0)
                    return false;

                // All enabled pipelines must be valid
                if (Pipelines.Any(p => p.Enabled && !p.IsValid))
                    return false;

                // All pipelines must have valid mapping sets
                var mappingSetIds = MappingSets.Select(m => m.Id).ToHashSet();
                if (Pipelines.Any(p => p.MappingSetId.HasValue && !mappingSetIds.Contains(p.MappingSetId.Value)))
                    return false;

                return true;
            }
        }
    }

    /// <summary>
    /// Extension methods for settings migration
    /// </summary>
    public static class SettingsMigrationExtensions
    {
        /// <summary>
        /// Detect settings version from JSON
        /// </summary>
        public static bool IsV2Settings(string json)
        {
            return json.Contains("\"version\"") && json.Contains("\"pipelines\"");
        }

        /// <summary>
        /// Check if migration is needed
        /// </summary>
        public static bool NeedsMigration(CamBridgeSettings settings)
        {
            // If we have watch folders but no pipeline config, we need migration
            return settings.WatchFolders.Count > 0;
        }
    }
}
