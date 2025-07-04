// src\CamBridge.Config\ViewModels\PipelineConfigViewModel.cs
// Version: 0.8.5
// Description: Pipeline Configuration ViewModel - Refactored with child ViewModels

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
        private readonly IPipelineSettingsService _pipelineSettingsService;
        private readonly PacsConfigViewModel _pacsConfigViewModel;
        private CamBridgeSettingsV2? _originalSettings;

        // Collections
        [ObservableProperty]
        private ObservableCollection<PipelineConfiguration> _pipelines = new();

        [ObservableProperty]
        private ObservableCollection<MappingSet> _mappingSets = new();

        // Child ViewModels
        public PacsConfigViewModel PacsConfigViewModel => _pacsConfigViewModel;

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

                    // Initialize child ViewModels
                    _pacsConfigViewModel.Initialize(value);

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
        private string _statusMessage = "";

        [ObservableProperty]
        private bool _isError;

        [ObservableProperty]
        private bool _hasUnsavedChanges;

        [ObservableProperty]
        private int _unsavedChangesCount;

        [ObservableProperty]
        private bool _selectedPipelineHasChanges;

        public PipelineConfigViewModel(
            IConfigurationService configurationService,
            IPipelineSettingsService pipelineSettingsService,
            PacsConfigViewModel pacsConfigViewModel)
        {
            _configurationService = configurationService;
            _pipelineSettingsService = pipelineSettingsService;
            _pacsConfigViewModel = pacsConfigViewModel;

            Debug.WriteLine("PipelineConfigViewModel constructor called");

            // Wire up configuration changes from child ViewModels
            _pacsConfigViewModel.ConfigurationChanged += (s, e) =>
            {
                if (!IsLoading)
                {
                    HasUnsavedChanges = true;
                    UnsavedChangesCount++;
                    SelectedPipelineHasChanges = true;
                }
            };

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
                StatusMessage = $"Error: {ex.Message}";
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
                StatusMessage = "";

                var settings = await _pipelineSettingsService.LoadSettingsAsync();

                if (settings != null)
                {
                    Debug.WriteLine($"Settings loaded: Version={settings.Version}, Pipelines={settings.Pipelines.Count}");
                    _originalSettings = settings;
                    MapFromSettings(settings);

                    // Create system default mapping sets if needed
                    EnsureSystemDefaults();

                    HasUnsavedChanges = false;
                    UnsavedChangesCount = 0;
                    StatusMessage = "";
                }
                else
                {
                    Debug.WriteLine("Settings is null - creating default pipeline");
                    CreateDefaultPipeline();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadSettingsAsync ERROR: {ex.Message}");
                Debug.WriteLine($"Exception type: {ex.GetType().Name}");
                StatusMessage = $"Error: {ex.Message}";
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
                await _pipelineSettingsService.SaveSettingsAsync(settings);

                _originalSettings = settings;
                HasUnsavedChanges = false;
                UnsavedChangesCount = 0;
                SelectedPipelineHasChanges = false;

                // Show save confirmation briefly
                StatusMessage = $"Saved at {DateTime.Now:HH:mm:ss}";

                // Clear status message after 3 seconds
                _ = Task.Run(async () =>
                {
                    await Task.Delay(3000);
                    StatusMessage = "";
                });

                // Notify success
                var backupPath = _pipelineSettingsService.GetBackupPath();

                MessageBox.Show($"Pipeline configuration saved successfully!\n\nAuto-backup created at:\n{backupPath}",
                               "Success",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
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

            var newPipeline = _pipelineSettingsService.CreateDefaultPipeline($"Pipeline {Pipelines.Count + 1}");

            // Subscribe to property changes
            newPipeline.PropertyChanged += Pipeline_PropertyChanged;
            newPipeline.WatchSettings.PropertyChanged += Pipeline_PropertyChanged;
            newPipeline.ProcessingOptions.PropertyChanged += Pipeline_PropertyChanged;
            if (newPipeline.PacsConfiguration != null)
            {
                newPipeline.PacsConfiguration.PropertyChanged += Pipeline_PropertyChanged;
            }

            // Add to collection
            Pipelines.Add(newPipeline);
            SelectedPipeline = newPipeline;

            HasUnsavedChanges = true;
            UnsavedChangesCount++;

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
            }
        }

        private bool CanDeletePipeline() => SelectedPipeline != null && Pipelines.Count > 1;

        [RelayCommand(CanExecute = nameof(CanApplyPipeline))]
        private void ApplyPipeline()
        {
            if (SelectedPipeline == null) return;

            // Mark changes as applied
            SelectedPipelineHasChanges = false;
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
                    var cloned = _pipelineSettingsService.ClonePipeline(originalPipeline);
                    cloned.PropertyChanged += Pipeline_PropertyChanged;
                    cloned.WatchSettings.PropertyChanged += Pipeline_PropertyChanged;
                    cloned.ProcessingOptions.PropertyChanged += Pipeline_PropertyChanged;
                    if (cloned.PacsConfiguration != null)
                    {
                        cloned.PacsConfiguration.PropertyChanged += Pipeline_PropertyChanged;
                    }

                    Pipelines[index] = cloned;
                    SelectedPipeline = Pipelines[index];
                }
            }

            SelectedPipelineHasChanges = false;
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
                var clonedPipeline = _pipelineSettingsService.ClonePipeline(pipeline);

                // Subscribe to property changes
                clonedPipeline.PropertyChanged += Pipeline_PropertyChanged;
                clonedPipeline.WatchSettings.PropertyChanged += Pipeline_PropertyChanged;
                clonedPipeline.ProcessingOptions.PropertyChanged += Pipeline_PropertyChanged;
                if (clonedPipeline.PacsConfiguration != null)
                {
                    clonedPipeline.PacsConfiguration.PropertyChanged += Pipeline_PropertyChanged;
                }

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
                settings.Pipelines.Add(_pipelineSettingsService.ClonePipeline(pipeline));
            }

            // Map mapping sets
            settings.MappingSets.Clear();
            foreach (var mappingSet in MappingSets)
            {
                settings.MappingSets.Add(mappingSet);
            }

            return settings;
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
            var defaultPipeline = _pipelineSettingsService.CreateDefaultPipeline("Default Pipeline");
            defaultPipeline.MappingSetId = defaultMappingSet.Id;

            defaultPipeline.PropertyChanged += Pipeline_PropertyChanged;
            defaultPipeline.WatchSettings.PropertyChanged += Pipeline_PropertyChanged;
            defaultPipeline.ProcessingOptions.PropertyChanged += Pipeline_PropertyChanged;
            if (defaultPipeline.PacsConfiguration != null)
            {
                defaultPipeline.PacsConfiguration.PropertyChanged += Pipeline_PropertyChanged;
            }

            Pipelines.Add(defaultPipeline);
            SelectedPipeline = defaultPipeline;

            Debug.WriteLine($"Default pipeline created. Pipelines count: {Pipelines.Count}");
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
