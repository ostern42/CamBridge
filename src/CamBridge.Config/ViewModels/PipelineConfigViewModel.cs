// src\CamBridge.Config\ViewModels\PipelineConfigViewModel.cs
// Version: 0.7.21
// Description: Pipeline Configuration ViewModel - FIXED with diagnostics

using CamBridge.Config.Services;
using CamBridge.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace CamBridge.Config.ViewModels
{
    [SupportedOSPlatform("windows")]
    public partial class PipelineConfigViewModel : ViewModelBase
    {
        private readonly IConfigurationService _configurationService;
        private CamBridgeSettingsV2? _originalSettings;

        // Collections
        [ObservableProperty]
        private ObservableCollection<PipelineConfiguration> _pipelines = new();

        [ObservableProperty]
        private ObservableCollection<MappingSet> _mappingSets = new();

        // Selected Pipeline
        private PipelineConfiguration? _selectedPipeline;
        public PipelineConfiguration? SelectedPipeline
        {
            get => _selectedPipeline;
            set
            {
                if (SetProperty(ref _selectedPipeline, value))
                {
                    Debug.WriteLine($"SelectedPipeline changed to: {value?.Name ?? "null"}");
                    OnPropertyChanged(nameof(SelectedPipelineDicomOverrides));
                    OnPropertyChanged(nameof(UseCustomLogging));
                    OnPropertyChanged(nameof(UseCustomNotifications));
                    UpdateCommands();
                }
            }
        }

        // Pipeline-specific properties for binding
        public DicomOverrides? SelectedPipelineDicomOverrides
        {
            get
            {
                if (SelectedPipeline?.DicomOverrides == null && SelectedPipeline != null)
                {
                    SelectedPipeline.DicomOverrides = new DicomOverrides();
                }
                return SelectedPipeline?.DicomOverrides;
            }
        }

        // ComboBox Collections
        public ObservableCollection<string> LogLevels { get; } = new()
        {
            "Trace", "Debug", "Information", "Warning", "Error", "Critical"
        };

        public ObservableCollection<PostProcessingAction> ProcessingActions { get; } = new()
        {
            PostProcessingAction.Leave,
            PostProcessingAction.Archive,
            PostProcessingAction.Delete,
            PostProcessingAction.MoveToError
        };

        public ObservableCollection<OutputOrganization> OutputOrganizations { get; } = new()
        {
            OutputOrganization.None,
            OutputOrganization.ByPatient,
            OutputOrganization.ByDate,
            OutputOrganization.ByPatientAndDate
        };

        public ObservableCollection<NotificationLevel> NotificationLevels { get; } = new()
        {
            NotificationLevel.Information,
            NotificationLevel.Warning,
            NotificationLevel.Error,
            NotificationLevel.Critical
        };

        // Pipeline-specific logging settings
        [ObservableProperty]
        private bool _useCustomLogging;

        [ObservableProperty]
        private string _pipelineLogLevel = "Information";

        [ObservableProperty]
        private string _pipelineLogFolder = @"C:\CamBridge\Logs";

        [ObservableProperty]
        private int _pipelineLogRetentionDays = 30;

        // Pipeline-specific notification settings
        [ObservableProperty]
        private bool _useCustomNotifications;

        [ObservableProperty]
        private string? _pipelineEmailTo;

        [ObservableProperty]
        private NotificationLevel _pipelineAlertLevel = NotificationLevel.Warning;

        [ObservableProperty]
        private bool _pipelineSendDailySummary;

        [ObservableProperty]
        private bool _pipelineAlertOnErrors = true;

        // Status properties
        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isSaving;

        [ObservableProperty]
        private string _statusMessage = "Initializing...";

        [ObservableProperty]
        private bool _isError;

        [ObservableProperty]
        private bool _hasUnsavedChanges;

        [ObservableProperty]
        private int _unsavedChangesCount;

        [ObservableProperty]
        private bool _selectedPipelineHasChanges;

        public PipelineConfigViewModel(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
            Debug.WriteLine("PipelineConfigViewModel constructor called");

            // Subscribe to collection changes
            Pipelines.CollectionChanged += (s, e) =>
            {
                Debug.WriteLine($"Pipelines collection changed: Count = {Pipelines.Count}");
                if (!IsLoading)
                {
                    HasUnsavedChanges = true;
                    UnsavedChangesCount++;
                }
            };
        }

        public async Task InitializeAsync()
        {
            Debug.WriteLine("=== PipelineConfigViewModel.InitializeAsync START ===");
            try
            {
                await LoadSettingsAsync();

                // Ensure we have at least one pipeline
                if (Pipelines.Count == 0)
                {
                    Debug.WriteLine("No pipelines loaded, creating default");
                    CreateDefaultPipeline();
                }

                Debug.WriteLine($"InitializeAsync completed. Pipeline count: {Pipelines.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"InitializeAsync FAILED: {ex.Message}");
                Debug.WriteLine($"Stack: {ex.StackTrace}");
                StatusMessage = $"Initialization failed: {ex.Message}";
                IsError = true;

                // Create default pipeline even on error
                CreateDefaultPipeline();
            }
            finally
            {
                Debug.WriteLine("=== PipelineConfigViewModel.InitializeAsync END ===");
            }
        }

        [RelayCommand]
        private async Task LoadSettingsAsync()
        {
            Debug.WriteLine("LoadSettingsAsync called");
            try
            {
                IsLoading = true;
                IsError = false;
                StatusMessage = "Loading pipeline configuration...";

                Debug.WriteLine("Calling ConfigurationService.LoadConfigurationAsync<CamBridgeSettingsV2>");
                var settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettingsV2>();

                if (settings != null)
                {
                    Debug.WriteLine($"Settings loaded: Version={settings.Version}, Pipelines={settings.Pipelines.Count}");
                    _originalSettings = settings;
                    MapFromSettings(settings);

                    // Create system default mapping sets if needed
                    EnsureSystemDefaults();

                    HasUnsavedChanges = false;
                    UnsavedChangesCount = 0;
                    StatusMessage = $"Loaded {Pipelines.Count} pipelines";
                }
                else
                {
                    Debug.WriteLine("Settings is null - creating default pipeline");
                    // Create default settings
                    CreateDefaultPipeline();
                    StatusMessage = "Created default pipeline configuration";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadSettingsAsync ERROR: {ex.Message}");
                Debug.WriteLine($"Exception type: {ex.GetType().Name}");
                StatusMessage = $"Error loading settings: {ex.Message}";
                IsError = true;

                // Ensure we have at least one pipeline
                if (Pipelines.Count == 0)
                {
                    CreateDefaultPipeline();
                }
            }
            finally
            {
                IsLoading = false;
                Debug.WriteLine($"LoadSettingsAsync completed. Pipelines count: {Pipelines.Count}");
            }
        }

        [RelayCommand(CanExecute = nameof(CanSaveAll))]
        private async Task SaveAllAsync()
        {
            try
            {
                IsSaving = true;
                IsError = false;
                StatusMessage = "Saving all pipelines...";

                var settings = MapToSettings();
                await _configurationService.SaveConfigurationAsync(settings);

                _originalSettings = settings;
                HasUnsavedChanges = false;
                UnsavedChangesCount = 0;
                SelectedPipelineHasChanges = false;
                StatusMessage = "All pipelines saved successfully";

                // Notify success
                MessageBox.Show("Pipeline configuration saved successfully!",
                               "Success",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving: {ex.Message}";
                IsError = true;

                MessageBox.Show($"Failed to save configuration:\n{ex.Message}",
                               "Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
            finally
            {
                IsSaving = false;
            }
        }

        private bool CanSaveAll() => HasUnsavedChanges && !IsLoading && !IsSaving;

        [RelayCommand]
        private void AddPipeline()
        {
            Debug.WriteLine("AddPipeline called");

            var newPipeline = new PipelineConfiguration
            {
                Id = Guid.NewGuid(),
                Name = $"Pipeline {Pipelines.Count + 1}",
                Description = "New pipeline configuration",
                Enabled = true,
                WatchSettings = new PipelineWatchSettings
                {
                    Path = @"C:\CamBridge\NewPipeline\Input",
                    FilePattern = "*.jpg;*.jpeg",
                    IncludeSubdirectories = false,
                    MinimumFileAgeSeconds = 5
                },
                ProcessingOptions = new ProcessingOptions
                {
                    ArchiveFolder = @"C:\CamBridge\NewPipeline\Archive",
                    ErrorFolder = @"C:\CamBridge\NewPipeline\Errors",
                    DeadLetterFolder = @"C:\CamBridge\NewPipeline\DeadLetters",
                    SuccessAction = PostProcessingAction.Archive,
                    FailureAction = PostProcessingAction.MoveToError,
                    CreateBackup = true,
                    BackupFolder = @"C:\CamBridge\NewPipeline\Backup",
                    MaxConcurrentProcessing = 2,
                    RetryOnFailure = true,
                    MaxRetryAttempts = 3,
                    OutputOrganization = OutputOrganization.ByPatientAndDate
                },
                MappingSetId = MappingSets.FirstOrDefault(m => !m.IsSystemDefault)?.Id
            };

            // Subscribe to property changes
            newPipeline.PropertyChanged += Pipeline_PropertyChanged;
            newPipeline.WatchSettings.PropertyChanged += Pipeline_PropertyChanged;
            newPipeline.ProcessingOptions.PropertyChanged += Pipeline_PropertyChanged;

            // Add to collection
            Pipelines.Add(newPipeline);
            SelectedPipeline = newPipeline;

            HasUnsavedChanges = true;
            UnsavedChangesCount++;
            StatusMessage = $"Added new pipeline: {newPipeline.Name}";

            Debug.WriteLine($"Pipeline added. Total count: {Pipelines.Count}");
        }

        [RelayCommand(CanExecute = nameof(CanDeletePipeline))]
        private void DeletePipeline()
        {
            if (SelectedPipeline == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete the pipeline '{SelectedPipeline.Name}'?\n\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var pipelineName = SelectedPipeline.Name;
                Pipelines.Remove(SelectedPipeline);
                SelectedPipeline = Pipelines.FirstOrDefault();

                HasUnsavedChanges = true;
                UnsavedChangesCount++;
                StatusMessage = $"Deleted pipeline: {pipelineName}";
            }
        }

        private bool CanDeletePipeline() => SelectedPipeline != null && Pipelines.Count > 1;

        [RelayCommand(CanExecute = nameof(CanApplyPipeline))]
        private void ApplyPipeline()
        {
            if (SelectedPipeline == null) return;

            // Mark changes as applied
            SelectedPipelineHasChanges = false;
            StatusMessage = $"Applied changes to {SelectedPipeline.Name}";
        }

        private bool CanApplyPipeline() => SelectedPipelineHasChanges && !IsLoading && !IsSaving;

        [RelayCommand(CanExecute = nameof(CanResetPipeline))]
        private void ResetPipeline()
        {
            if (SelectedPipeline == null || _originalSettings == null) return;

            // Find original pipeline by ID
            var originalPipeline = _originalSettings.Pipelines.FirstOrDefault(p => p.Id == SelectedPipeline.Id);
            if (originalPipeline != null)
            {
                // Reset to original values
                var index = Pipelines.IndexOf(SelectedPipeline);
                if (index >= 0)
                {
                    var cloned = ClonePipeline(originalPipeline);
                    cloned.PropertyChanged += Pipeline_PropertyChanged;
                    cloned.WatchSettings.PropertyChanged += Pipeline_PropertyChanged;
                    cloned.ProcessingOptions.PropertyChanged += Pipeline_PropertyChanged;

                    Pipelines[index] = cloned;
                    SelectedPipeline = Pipelines[index];
                }
            }

            SelectedPipelineHasChanges = false;
            StatusMessage = $"Reset {SelectedPipeline.Name} to last saved state";
        }

        private bool CanResetPipeline() => SelectedPipelineHasChanges && !IsLoading && !IsSaving;

        private void MapFromSettings(CamBridgeSettingsV2 settings)
        {
            Debug.WriteLine($"MapFromSettings called. Settings has {settings.Pipelines.Count} pipelines, {settings.MappingSets.Count} mapping sets");

            // Clear existing
            Pipelines.Clear();
            MappingSets.Clear();

            // Map mapping sets
            foreach (var mappingSet in settings.MappingSets)
            {
                MappingSets.Add(mappingSet);
                Debug.WriteLine($"Added mapping set: {mappingSet.Name}");
            }

            // Map pipelines
            foreach (var pipeline in settings.Pipelines)
            {
                var clonedPipeline = ClonePipeline(pipeline);

                // Subscribe to property changes
                clonedPipeline.PropertyChanged += Pipeline_PropertyChanged;
                clonedPipeline.WatchSettings.PropertyChanged += Pipeline_PropertyChanged;
                clonedPipeline.ProcessingOptions.PropertyChanged += Pipeline_PropertyChanged;

                Pipelines.Add(clonedPipeline);
                Debug.WriteLine($"Added pipeline: {clonedPipeline.Name}");
            }

            // Select first pipeline
            SelectedPipeline = Pipelines.FirstOrDefault();
            Debug.WriteLine($"Selected first pipeline: {SelectedPipeline?.Name ?? "none"}");
        }

        private void Pipeline_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!IsLoading)
            {
                SelectedPipelineHasChanges = true;
                HasUnsavedChanges = true;
                Debug.WriteLine($"Pipeline property changed: {e.PropertyName}");
            }
        }

        private CamBridgeSettingsV2 MapToSettings()
        {
            var settings = new CamBridgeSettingsV2
            {
                Version = "2.0",
                GlobalDicomSettings = _originalSettings?.GlobalDicomSettings ?? new DicomSettings(),
                DefaultProcessingOptions = _originalSettings?.DefaultProcessingOptions ?? new ProcessingOptions(),
                Logging = _originalSettings?.Logging ?? new LoggingSettings(),
                Service = _originalSettings?.Service ?? new ServiceSettings(),
                Notifications = _originalSettings?.Notifications ?? new NotificationSettings()
            };

            // Map pipelines
            settings.Pipelines.Clear();
            foreach (var pipeline in Pipelines)
            {
                settings.Pipelines.Add(ClonePipeline(pipeline));
            }

            // Map mapping sets
            settings.MappingSets.Clear();
            foreach (var mappingSet in MappingSets)
            {
                settings.MappingSets.Add(mappingSet);
            }

            return settings;
        }

        private PipelineConfiguration ClonePipeline(PipelineConfiguration source)
        {
            return new PipelineConfiguration
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
                ProcessExistingOnStartup = source.ProcessExistingOnStartup,
                DeadLetterFolder = source.DeadLetterFolder
            };
        }

        private void CreateDefaultPipeline()
        {
            Debug.WriteLine("CreateDefaultPipeline called");

            // Don't clear if we already have pipelines
            if (Pipelines.Count > 0)
            {
                Debug.WriteLine($"Already have {Pipelines.Count} pipelines, not creating default");
                return;
            }

            // Clear mapping sets if needed
            MappingSets.Clear();

            // Create default mapping set
            var defaultMappingSet = new MappingSet
            {
                Id = Guid.NewGuid(),
                Name = "Default Mapping",
                Description = "Default EXIF to DICOM mapping",
                IsSystemDefault = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            MappingSets.Add(defaultMappingSet);

            // Create default pipeline
            var defaultPipeline = new PipelineConfiguration
            {
                Id = Guid.NewGuid(),
                Name = "Default Pipeline",
                Description = "Default processing pipeline",
                Enabled = true,
                WatchSettings = new PipelineWatchSettings
                {
                    Path = @"C:\CamBridge\Watch\Default",
                    FilePattern = "*.jpg;*.jpeg",
                    IncludeSubdirectories = false,
                    OutputPath = @"C:\CamBridge\Output\Default",
                    MinimumFileAgeSeconds = 5
                },
                ProcessingOptions = new ProcessingOptions
                {
                    ArchiveFolder = @"C:\CamBridge\Archive",
                    ErrorFolder = @"C:\CamBridge\Errors",
                    DeadLetterFolder = @"C:\CamBridge\DeadLetters",
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
                MappingSetId = defaultMappingSet.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            defaultPipeline.PropertyChanged += Pipeline_PropertyChanged;
            defaultPipeline.WatchSettings.PropertyChanged += Pipeline_PropertyChanged;
            defaultPipeline.ProcessingOptions.PropertyChanged += Pipeline_PropertyChanged;

            Pipelines.Add(defaultPipeline);
            SelectedPipeline = defaultPipeline;

            Debug.WriteLine($"Default pipeline created. Pipelines count: {Pipelines.Count}");
            StatusMessage = "Created default pipeline";
        }

        private void EnsureSystemDefaults()
        {
            // Check if we have system default mapping sets
            if (!MappingSets.Any(m => m.IsSystemDefault))
            {
                // Add Ricoh Standard as system default
                var ricohStandard = new MappingSet
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Name = "[System] Ricoh Standard",
                    Description = "Built-in mapping for Ricoh cameras",
                    IsSystemDefault = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                MappingSets.Insert(0, ricohStandard);
                Debug.WriteLine("Added system default mapping set: Ricoh Standard");
            }
        }

        private void UpdateCommands()
        {
            DeletePipelineCommand.NotifyCanExecuteChanged();
            ApplyPipelineCommand.NotifyCanExecuteChanged();
            ResetPipelineCommand.NotifyCanExecuteChanged();
        }

        // Handle property changes
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            // Update save command when changes occur
            if (e.PropertyName == nameof(HasUnsavedChanges))
            {
                SaveAllCommand.NotifyCanExecuteChanged();
            }
        }
    }
}
