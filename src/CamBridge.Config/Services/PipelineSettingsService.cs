// src\CamBridge.Config\Services\PipelineSettingsService.cs
// Version: 0.8.10
// Description: Implementation of pipeline settings service - DeadLetterFolder removed!
// Session: 95 - Business logic extracted from ViewModels
// Session: 107 - DeadLetterFolder cleanup

using CamBridge.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Handles pipeline settings operations
    /// All business logic extracted from PipelineConfigViewModel
    /// </summary>
    public class PipelineSettingsService : IPipelineSettingsService
    {
        private readonly IConfigurationService _configurationService;

        public PipelineSettingsService(IConfigurationService configurationService)
        {
            _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        }

        public async Task<CamBridgeSettingsV2?> LoadSettingsAsync()
        {
            Debug.WriteLine("PipelineSettingsService.LoadSettingsAsync called");

            var settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettingsV2>();

            if (settings != null)
            {
                Debug.WriteLine($"Settings loaded: Version={settings.Version}, Pipelines={settings.Pipelines.Count}");

                // Fix any null PacsConfiguration in existing pipelines
                foreach (var pipeline in settings.Pipelines)
                {
                    if (pipeline.PacsConfiguration == null)
                    {
                        Debug.WriteLine($"Creating PacsConfiguration for pipeline: {pipeline.Name}");
                        pipeline.PacsConfiguration = new PacsConfiguration();
                    }
                }
            }

            return settings;
        }

        public async Task SaveSettingsAsync(CamBridgeSettingsV2 settings)
        {
            await _configurationService.SaveConfigurationAsync(settings);
        }

        public PipelineConfiguration ClonePipeline(PipelineConfiguration source)
        {
            var cloned = new PipelineConfiguration
            {
                Id = source.Id,
                Name = source.Name,
                Description = source.Description,
                Enabled = source.Enabled,
                WatchSettings = new PipelineWatchSettings
                {
                    Path = source.WatchSettings.Path,
                    FilePattern = source.WatchSettings.FilePattern,
                    IncludeSubdirectories = source.WatchSettings.IncludeSubdirectories,
                    OutputPath = source.WatchSettings.OutputPath,
                    MinimumFileAgeSeconds = source.WatchSettings.MinimumFileAgeSeconds
                },
                ProcessingOptions = CloneProcessingOptions(source.ProcessingOptions),
                DicomOverrides = source.DicomOverrides != null ? new DicomOverrides
                {
                    InstitutionName = source.DicomOverrides.InstitutionName,
                    InstitutionDepartment = source.DicomOverrides.InstitutionDepartment,
                    StationName = source.DicomOverrides.StationName
                } : null,
                MappingSetId = source.MappingSetId,
                CreatedAt = source.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            // Clone PACS Configuration
            if (source.PacsConfiguration != null)
            {
                cloned.PacsConfiguration = new PacsConfiguration
                {
                    Enabled = source.PacsConfiguration.Enabled,
                    Host = source.PacsConfiguration.Host,
                    Port = source.PacsConfiguration.Port,
                    CalledAeTitle = source.PacsConfiguration.CalledAeTitle,
                    CallingAeTitle = source.PacsConfiguration.CallingAeTitle,
                    TimeoutSeconds = source.PacsConfiguration.TimeoutSeconds,
                    MaxConcurrentUploads = source.PacsConfiguration.MaxConcurrentUploads,
                    RetryOnFailure = source.PacsConfiguration.RetryOnFailure,
                    MaxRetryAttempts = source.PacsConfiguration.MaxRetryAttempts,
                    RetryDelaySeconds = source.PacsConfiguration.RetryDelaySeconds
                };
            }
            else
            {
                // Always ensure PacsConfiguration exists
                cloned.PacsConfiguration = new PacsConfiguration();
            }

            return cloned;
        }

        public PipelineConfiguration CreateDefaultPipeline(string? name = null)
        {
            var pipelineNumber = DateTime.Now.Ticks % 1000; // Simple unique number

            return new PipelineConfiguration
            {
                Id = Guid.NewGuid(),
                Name = name ?? $"Pipeline {pipelineNumber}",
                Description = "New pipeline configuration",
                Enabled = true,
                WatchSettings = new PipelineWatchSettings
                {
                    Path = @"C:\CamBridge\Watch\New",
                    FilePattern = "*.jpg;*.jpeg",
                    IncludeSubdirectories = false,
                    OutputPath = @"C:\CamBridge\Output\New",
                    MinimumFileAgeSeconds = 5
                },
                ProcessingOptions = new ProcessingOptions
                {
                    ArchiveFolder = @"C:\CamBridge\Archive",
                    ErrorFolder = @"C:\CamBridge\Errors",
                    // DeadLetterFolder removed! Just use ErrorFolder for all failures
                    SuccessAction = PostProcessingAction.Archive,
                    FailureAction = PostProcessingAction.MoveToError,
                    CreateBackup = true,
                    BackupFolder = @"C:\CamBridge\Backup",
                    MaxConcurrentProcessing = 2,
                    RetryOnFailure = true,
                    MaxRetryAttempts = 3,
                    OutputOrganization = OutputOrganization.ByPatientAndDate,
                    ProcessExistingOnStartup = false
                },
                PacsConfiguration = new PacsConfiguration
                {
                    Enabled = false,
                    Host = string.Empty,
                    Port = 104,
                    CalledAeTitle = string.Empty,
                    CallingAeTitle = "CAMBRIDGE",
                    TimeoutSeconds = 30,
                    MaxConcurrentUploads = 1,
                    RetryOnFailure = true,
                    MaxRetryAttempts = 3,
                    RetryDelaySeconds = 5
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public ValidationResult ValidatePipeline(PipelineConfiguration pipeline)
        {
            var errors = new System.Collections.Generic.List<string>();
            var warnings = new System.Collections.Generic.List<string>();

            // Basic validation
            if (string.IsNullOrWhiteSpace(pipeline.Name))
                errors.Add("Pipeline name is required");

            if (string.IsNullOrWhiteSpace(pipeline.WatchSettings.Path))
                errors.Add("Watch folder path is required");
            else if (!Directory.Exists(pipeline.WatchSettings.Path))
                warnings.Add($"Watch folder does not exist: {pipeline.WatchSettings.Path}");

            // PACS validation if enabled
            if (pipeline.PacsConfiguration?.Enabled == true)
            {
                if (string.IsNullOrWhiteSpace(pipeline.PacsConfiguration.Host))
                    errors.Add("PACS host is required when PACS upload is enabled");

                if (string.IsNullOrWhiteSpace(pipeline.PacsConfiguration.CalledAeTitle))
                    errors.Add("Called AE Title is required when PACS upload is enabled");

                if (string.IsNullOrWhiteSpace(pipeline.PacsConfiguration.CallingAeTitle))
                    errors.Add("Calling AE Title is required when PACS upload is enabled");

                if (pipeline.PacsConfiguration.CalledAeTitle?.Length > 16)
                    errors.Add("Called AE Title must be 16 characters or less");

                if (pipeline.PacsConfiguration.CallingAeTitle?.Length > 16)
                    errors.Add("Calling AE Title must be 16 characters or less");

                if (pipeline.PacsConfiguration.Port <= 0 || pipeline.PacsConfiguration.Port > 65535)
                    errors.Add("PACS port must be between 1 and 65535");
            }

            return errors.Any()
                ? ValidationResult.Failure(errors.ToArray())
                : ValidationResult.Success();
        }

        public string GetBackupPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "CamBridge",
                $"appsettings.json.backup_{DateTime.Now:yyyyMMdd_HHmmss}");
        }

        private ProcessingOptions CloneProcessingOptions(ProcessingOptions source)
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
                ProcessExistingOnStartup = source.ProcessExistingOnStartup
                // DeadLetterFolder removed! ErrorFolder handles all failures now
            };
        }
    }
}
