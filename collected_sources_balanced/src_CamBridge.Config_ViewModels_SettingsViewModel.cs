using CamBridge.Config.Services;
using CamBridge.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CamBridge.Config.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly IConfigurationService _configurationService;
        private CamBridgeSettings _originalSettings = new();

        // Collections for ComboBox bindings
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

        // Watch Folders
        [ObservableProperty] private ObservableCollection<FolderConfigurationViewModel> _watchFolders = new();
        [ObservableProperty] private FolderConfigurationViewModel? _selectedWatchFolder;

        // Processing Options
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Default output folder is required")]
        private string _defaultOutputFolder = @"C:\CamBridge\Output";

        [ObservableProperty] private PostProcessingAction _successAction = PostProcessingAction.Archive;
        [ObservableProperty] private PostProcessingAction _failureAction = PostProcessingAction.MoveToError;

        [ObservableProperty]
        [Required(ErrorMessage = "Archive folder is required")]
        private string _archiveFolder = @"C:\CamBridge\Archive";

        [ObservableProperty]
        [Required(ErrorMessage = "Error folder is required")]
        private string _errorFolder = @"C:\CamBridge\Errors";

        [ObservableProperty] private bool _createBackup = true;

        [ObservableProperty]
        [Required(ErrorMessage = "Backup folder is required")]
        private string _backupFolder = @"C:\CamBridge\Backup";

        [ObservableProperty]
        [Range(1, 10, ErrorMessage = "Max concurrent processing must be between 1 and 10")]
        private int _maxConcurrentProcessing = 2;

        [ObservableProperty] private bool _retryOnFailure = true;

        [ObservableProperty]
        [Range(1, 10, ErrorMessage = "Max retry attempts must be between 1 and 10")]
        private int _maxRetryAttempts = 3;

        [ObservableProperty] private OutputOrganization _outputOrganization = OutputOrganization.ByPatientAndDate;
        [ObservableProperty] private bool _processExistingOnStartup = true;
        [ObservableProperty] private int _maxFileAgeDays = 30;

        // DICOM Settings
        [ObservableProperty]
        [Required(ErrorMessage = "Implementation class UID is required")]
        private string _implementationClassUid = "1.2.276.0.7230010.3.0.3.6.4";

        [ObservableProperty]
        [Required(ErrorMessage = "Implementation version name is required")]
        private string _implementationVersionName = "CAMBRIDGE_001";

        [ObservableProperty] private string _institutionName = string.Empty;
        [ObservableProperty] private string _stationName = Environment.MachineName;
        [ObservableProperty] private bool _validateAfterCreation = true;

        // Notification Settings
        [ObservableProperty] private bool _enableEmail;
        [ObservableProperty] private bool _enableEventLog = true;

        [ObservableProperty]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        private string? _emailFrom;

        [ObservableProperty] private string? _emailTo;
        [ObservableProperty] private string? _smtpHost;

        [ObservableProperty]
        [Range(1, 65535, ErrorMessage = "SMTP port must be between 1 and 65535")]
        private int _smtpPort = 587;

        [ObservableProperty] private bool _smtpUseSsl = true;
        [ObservableProperty] private string? _smtpUsername;
        [ObservableProperty] private string? _smtpPassword;
        [ObservableProperty] private string _minimumEmailLevel = "Warning";
        [ObservableProperty] private bool _sendDailySummary = true;

        [ObservableProperty]
        [Range(0, 23, ErrorMessage = "Daily summary hour must be between 0 and 23")]
        private int _dailySummaryHour = 8;

        // Logging Settings
        [ObservableProperty] private string _logLevel = "Information";

        [ObservableProperty]
        [Required(ErrorMessage = "Log folder is required")]
        private string _logFolder = @"C:\CamBridge\Logs";

        [ObservableProperty] private bool _enableFileLogging = true;
        [ObservableProperty] private bool _enableServiceEventLog = true;

        [ObservableProperty]
        [Range(1, 1000, ErrorMessage = "Max log file size must be between 1 and 1000 MB")]
        private int _maxLogFileSizeMB = 10;

        [ObservableProperty]
        [Range(1, 100, ErrorMessage = "Max log files must be between 1 and 100")]
        private int _maxLogFiles = 10;

        // Service Settings
        [ObservableProperty]
        [Range(0, 300, ErrorMessage = "Startup delay must be between 0 and 300 seconds")]
        private int _startupDelaySeconds = 5;

        [ObservableProperty]
        [Range(100, 10000, ErrorMessage = "File processing delay must be between 100 and 10000 ms")]
        private int _fileProcessingDelayMs = 500;

        // Status properties
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private bool _isSaving;
        [ObservableProperty] private bool _hasChanges;
        [ObservableProperty] private string? _statusMessage;
        [ObservableProperty] private bool _isError;

        public SettingsViewModel(IConfigurationService configurationService)
        {
            _configurationService = configurationService;

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != nameof(HasChanges) &&
                    e.PropertyName != nameof(StatusMessage) &&
                    e.PropertyName != nameof(IsError) &&
                    e.PropertyName != nameof(IsLoading) &&
                    e.PropertyName != nameof(IsSaving))
                {
                    HasChanges = true;
                }
            };
        }

        public async Task InitializeAsync()
        {
            await LoadSettingsAsync();
        }

        [RelayCommand]
        private async Task LoadSettingsAsync()
        {
            try
            {
                IsLoading = true;
                IsError = false;
                StatusMessage = "Loading settings...";

                var settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettings>();
                if (settings != null)
                {
                    _originalSettings = settings;
                    MapFromSettings(settings);
                    HasChanges = false;
                    StatusMessage = "Settings loaded successfully";
                }
                else
                {
                    StatusMessage = "Failed to load settings";
                    IsError = true;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading settings: {ex.Message}";
                IsError = true;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SaveSettingsAsync()
        {
            try
            {
                ValidateAllProperties();
                if (HasErrors)
                {
                    StatusMessage = "Please fix validation errors before saving";
                    IsError = true;
                    return;
                }

                IsSaving = true;
                IsError = false;
                StatusMessage = "Saving settings...";

                var settings = MapToSettings();
                await _configurationService.SaveConfigurationAsync(settings);

                _originalSettings = settings;
                HasChanges = false;
                StatusMessage = "Settings saved successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving settings: {ex.Message}";
                IsError = true;
            }
            finally
            {
                IsSaving = false;
            }
        }

        private bool CanSave() => HasChanges && !IsLoading && !IsSaving;

        [RelayCommand(CanExecute = nameof(CanReset))]
        private void ResetSettings()
        {
            MapFromSettings(_originalSettings);
            HasChanges = false;
            StatusMessage = "Settings reset to last saved state";
            IsError = false;
        }

        private bool CanReset() => HasChanges && !IsLoading && !IsSaving;

        [RelayCommand]
        private void AddWatchFolder()
        {
            var newFolder = new FolderConfigurationViewModel
            {
                Path = @"C:\CamBridge\NewFolder",
                Enabled = true,
                FilePattern = "*.jpg;*.jpeg"
            };

            WatchFolders.Add(newFolder);
            SelectedWatchFolder = newFolder;
            HasChanges = true;
        }

        [RelayCommand(CanExecute = nameof(CanRemoveWatchFolder))]
        private void RemoveWatchFolder()
        {
            if (SelectedWatchFolder != null)
            {
                WatchFolders.Remove(SelectedWatchFolder);
                SelectedWatchFolder = WatchFolders.FirstOrDefault();
                HasChanges = true;
            }
        }

        private bool CanRemoveWatchFolder() => SelectedWatchFolder != null;

        private void MapFromSettings(CamBridgeSettings settings)
        {
            // Map all settings to view model properties
            // Implementation details omitted for brevity
        }

        private CamBridgeSettings MapToSettings()
        {
            // Map view model properties back to settings
            // Implementation details omitted for brevity
            return new CamBridgeSettings();
        }
    }

    public partial class FolderConfigurationViewModel : ObservableValidator
    {
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Path is required")]
        private string _path = string.Empty;

        [ObservableProperty] private string? _outputPath;
        [ObservableProperty] private bool _enabled = true;
        [ObservableProperty] private bool _includeSubdirectories;

        [ObservableProperty]
        [Required(ErrorMessage = "File pattern is required")]
        private string _filePattern = "*.jpg;*.jpeg";
    }
}
