// src\CamBridge.Config\ViewModels\SettingsViewModel.cs
// Version: 0.5.36
// Description: Fixed Reset button and added folder validation

using CamBridge.Config.Services;
using CamBridge.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CamBridge.Config.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly IConfigurationService _configurationService;
        private CamBridgeSettings _originalSettings = new();
        private bool _isInitializing = false;

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

        public ObservableCollection<NotificationLevel> NotificationLevels { get; } = new()
        {
            NotificationLevel.Information,
            NotificationLevel.Warning,
            NotificationLevel.Error,
            NotificationLevel.Critical
        };

        // Watch Folders
        [ObservableProperty] private ObservableCollection<FolderConfigurationViewModel> _watchFolders = new();

        private FolderConfigurationViewModel? _selectedWatchFolder;
        public FolderConfigurationViewModel? SelectedWatchFolder
        {
            get => _selectedWatchFolder;
            set
            {
                if (SetProperty(ref _selectedWatchFolder, value))
                {
                    // Notify RemoveWatchFolderCommand when selection changes
                    RemoveWatchFolderCommand.NotifyCanExecuteChanged();
                }
            }
        }

        // Processing Options
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Default output folder is required")]
        [CustomValidation(typeof(SettingsViewModel), nameof(ValidateFolderPath))]
        private string _defaultOutputFolder = @"C:\CamBridge\Output";

        [ObservableProperty] private PostProcessingAction _successAction = PostProcessingAction.Archive;
        [ObservableProperty] private PostProcessingAction _failureAction = PostProcessingAction.MoveToError;

        [ObservableProperty]
        [Required(ErrorMessage = "Archive folder is required")]
        [CustomValidation(typeof(SettingsViewModel), nameof(ValidateFolderPath))]
        private string _archiveFolder = @"C:\CamBridge\Archive";

        [ObservableProperty]
        [Required(ErrorMessage = "Error folder is required")]
        [CustomValidation(typeof(SettingsViewModel), nameof(ValidateFolderPath))]
        private string _errorFolder = @"C:\CamBridge\Errors";

        [ObservableProperty] private bool _createBackup = true;

        [ObservableProperty]
        [Required(ErrorMessage = "Backup folder is required")]
        [CustomValidation(typeof(SettingsViewModel), nameof(ValidateFolderPath))]
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
        [ObservableProperty] private NotificationLevel _minimumEmailLevel = NotificationLevel.Warning;
        [ObservableProperty] private bool _sendDailySummary = true;

        [ObservableProperty]
        [Range(0, 23, ErrorMessage = "Daily summary hour must be between 0 and 23")]
        private int _dailySummaryHour = 8;

        // Logging Settings
        [ObservableProperty] private string _logLevel = "Information";

        [ObservableProperty]
        [Required(ErrorMessage = "Log folder is required")]
        [CustomValidation(typeof(SettingsViewModel), nameof(ValidateFolderPath))]
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

        // Debug helper property
        public string DebugInfo => $"HasChanges: {HasChanges}, IsLoading: {IsLoading}, IsSaving: {IsSaving}, CanSave: {CanSave()}";

        public SettingsViewModel(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        // Custom validation for folder paths
        public static ValidationResult? ValidateFolderPath(string folderPath, ValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                return new ValidationResult("Folder path cannot be empty");

            try
            {
                // Check if path is valid
                var fullPath = Path.GetFullPath(folderPath);

                // Check for invalid characters
                if (folderPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                    return new ValidationResult("Folder path contains invalid characters");

                // Don't check if folder exists - it will be created
                // Just ensure the path is valid
                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return new ValidationResult($"Invalid folder path: {ex.Message}");
            }
        }

        // Override OnPropertyChanged to handle change tracking
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            // Track changes for all properties except status properties
            if (e.PropertyName != nameof(HasChanges) &&
                e.PropertyName != nameof(StatusMessage) &&
                e.PropertyName != nameof(IsError) &&
                e.PropertyName != nameof(IsLoading) &&
                e.PropertyName != nameof(IsSaving) &&
                e.PropertyName != nameof(SelectedWatchFolder) &&
                e.PropertyName != nameof(DebugInfo))
            {
                if (!IsLoading && !_isInitializing) // Don't mark as changed during load or init
                {
                    HasChanges = true;
                }
            }
        }

        public async Task InitializeAsync()
        {
            try
            {
                _isInitializing = true;

                // Subscribe to collection changes
                WatchFolders.CollectionChanged += (s, e) =>
                {
                    if (!IsLoading && !_isInitializing)
                    {
                        HasChanges = true;
                    }
                };

                await LoadSettingsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing SettingsViewModel: {ex.Message}");
                StatusMessage = "Failed to load settings";
                IsError = true;
            }
            finally
            {
                _isInitializing = false;
            }
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
                    _originalSettings = CloneSettings(settings);
                    MapFromSettings(settings);
                    HasChanges = false;
                    StatusMessage = "Settings loaded successfully";
                }
                else
                {
                    // Load defaults if no settings exist
                    _originalSettings = new CamBridgeSettings();
                    MapFromSettings(_originalSettings);
                    HasChanges = false;
                    StatusMessage = "Loaded default settings";
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

                _originalSettings = CloneSettings(settings);
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

        private bool CanSave() => HasChanges && !IsLoading && !IsSaving && !HasErrors;

        [RelayCommand(CanExecute = nameof(CanReset))]
        private void ResetSettings()
        {
            try
            {
                _isInitializing = true;
                MapFromSettings(_originalSettings);
                HasChanges = false;
                StatusMessage = "Settings reset to last saved state";
                IsError = false;
            }
            finally
            {
                _isInitializing = false;
            }
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

            // Subscribe to changes
            newFolder.PropertyChanged += (s, e) =>
            {
                if (!IsLoading && !_isInitializing)
                {
                    HasChanges = true;
                }
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

        [RelayCommand]
        private void TestChangeDetection()
        {
            // Simple test to verify change detection works
            DefaultOutputFolder = DefaultOutputFolder + "_test";
            System.Diagnostics.Debug.WriteLine($"Test change - HasChanges: {HasChanges}, CanSave: {CanSave()}");
        }

        private void MapFromSettings(CamBridgeSettings settings)
        {
            // Watch Folders
            WatchFolders.Clear();
            foreach (var folder in settings.WatchFolders)
            {
                var folderVm = new FolderConfigurationViewModel
                {
                    Path = folder.Path,
                    OutputPath = folder.OutputPath,
                    Enabled = folder.Enabled,
                    IncludeSubdirectories = folder.IncludeSubdirectories,
                    FilePattern = folder.FilePattern
                };

                // Subscribe to changes
                folderVm.PropertyChanged += (s, e) =>
                {
                    if (!IsLoading && !_isInitializing)
                    {
                        HasChanges = true;
                    }
                };
                WatchFolders.Add(folderVm);
            }

            // Processing Options
            DefaultOutputFolder = settings.DefaultOutputFolder;
            SuccessAction = settings.Processing.SuccessAction;
            FailureAction = settings.Processing.FailureAction;
            ArchiveFolder = settings.Processing.ArchiveFolder;
            ErrorFolder = settings.Processing.ErrorFolder;
            CreateBackup = settings.Processing.CreateBackup;
            BackupFolder = settings.Processing.BackupFolder;
            MaxConcurrentProcessing = settings.Processing.MaxConcurrentProcessing;
            RetryOnFailure = settings.Processing.RetryOnFailure;
            MaxRetryAttempts = settings.Processing.MaxRetryAttempts;
            OutputOrganization = settings.Processing.OutputOrganization;
            ProcessExistingOnStartup = settings.Processing.ProcessExistingOnStartup;

            if (settings.Processing.MaxFileAge.HasValue)
            {
                MaxFileAgeDays = (int)settings.Processing.MaxFileAge.Value.TotalDays;
            }

            // DICOM Settings
            ImplementationClassUid = settings.Dicom.ImplementationClassUid;
            ImplementationVersionName = settings.Dicom.ImplementationVersionName;
            InstitutionName = settings.Dicom.InstitutionName;
            StationName = settings.Dicom.StationName;
            ValidateAfterCreation = settings.Dicom.ValidateAfterCreation;

            // Notification Settings - safe navigation
            EnableEmail = settings.Notifications?.EnableEmail ?? false;
            EnableEventLog = settings.Notifications?.EnableEventLog ?? true;

            if (settings.Notifications?.Email != null)
            {
                EmailFrom = settings.Notifications.Email.From;
                EmailTo = settings.Notifications.Email.To;
                SmtpHost = settings.Notifications.Email.SmtpHost;
                SmtpPort = settings.Notifications.Email.SmtpPort;
                SmtpUseSsl = settings.Notifications.Email.UseSsl;
                SmtpUsername = settings.Notifications.Email.Username;
                SmtpPassword = settings.Notifications.Email.Password;
            }

            MinimumEmailLevel = settings.Notifications?.MinimumEmailLevel ?? NotificationLevel.Warning;
            SendDailySummary = settings.Notifications?.SendDailySummary ?? false;
            DailySummaryHour = settings.Notifications?.DailySummaryHour ?? 8;

            // Logging Settings
            LogLevel = settings.Logging.LogLevel;
            LogFolder = settings.Logging.LogFolder;
            EnableFileLogging = settings.Logging.EnableFileLogging;
            EnableServiceEventLog = settings.Logging.EnableEventLog;
            MaxLogFileSizeMB = settings.Logging.MaxLogFileSizeMB;
            MaxLogFiles = settings.Logging.MaxLogFiles;

            // Service Settings
            StartupDelaySeconds = settings.Service.StartupDelaySeconds;
            FileProcessingDelayMs = settings.Service.FileProcessingDelayMs;
        }

        private CamBridgeSettings MapToSettings()
        {
            var settings = new CamBridgeSettings
            {
                DefaultOutputFolder = DefaultOutputFolder,
                Processing = new ProcessingOptions
                {
                    SuccessAction = SuccessAction,
                    FailureAction = FailureAction,
                    ArchiveFolder = ArchiveFolder,
                    ErrorFolder = ErrorFolder,
                    CreateBackup = CreateBackup,
                    BackupFolder = BackupFolder,
                    MaxConcurrentProcessing = MaxConcurrentProcessing,
                    RetryOnFailure = RetryOnFailure,
                    MaxRetryAttempts = MaxRetryAttempts,
                    OutputOrganization = OutputOrganization,
                    ProcessExistingOnStartup = ProcessExistingOnStartup,
                    MaxFileAge = TimeSpan.FromDays(MaxFileAgeDays)
                },
                Dicom = new DicomSettings
                {
                    ImplementationClassUid = ImplementationClassUid,
                    ImplementationVersionName = ImplementationVersionName,
                    InstitutionName = InstitutionName,
                    StationName = StationName,
                    ValidateAfterCreation = ValidateAfterCreation
                },
                Notifications = new NotificationSettings
                {
                    EnableEmail = EnableEmail,
                    EnableEventLog = EnableEventLog,
                    Email = new EmailSettings
                    {
                        From = EmailFrom,
                        To = EmailTo,
                        SmtpHost = SmtpHost,
                        SmtpPort = SmtpPort,
                        UseSsl = SmtpUseSsl,
                        Username = SmtpUsername,
                        Password = SmtpPassword
                    },
                    MinimumEmailLevel = MinimumEmailLevel,
                    SendDailySummary = SendDailySummary,
                    DailySummaryHour = DailySummaryHour
                },
                Logging = new LoggingSettings
                {
                    LogLevel = LogLevel,
                    LogFolder = LogFolder,
                    EnableFileLogging = EnableFileLogging,
                    EnableEventLog = EnableServiceEventLog,
                    MaxLogFileSizeMB = MaxLogFileSizeMB,
                    MaxLogFiles = MaxLogFiles
                },
                Service = new ServiceSettings
                {
                    StartupDelaySeconds = StartupDelaySeconds,
                    FileProcessingDelayMs = FileProcessingDelayMs
                }
            };

            // Map Watch Folders
            settings.WatchFolders.Clear();
            foreach (var folder in WatchFolders)
            {
                settings.WatchFolders.Add(new FolderConfiguration
                {
                    Path = folder.Path,
                    OutputPath = folder.OutputPath,
                    Enabled = folder.Enabled,
                    IncludeSubdirectories = folder.IncludeSubdirectories,
                    FilePattern = folder.FilePattern
                });
            }

            return settings;
        }

        // Deep clone settings to keep original state
        private CamBridgeSettings CloneSettings(CamBridgeSettings settings)
        {
            // Use JSON serialization for deep clone
            var json = System.Text.Json.JsonSerializer.Serialize(settings);
            return System.Text.Json.JsonSerializer.Deserialize<CamBridgeSettings>(json) ?? new CamBridgeSettings();
        }

        partial void OnHasChangesChanged(bool value)
        {
            // When HasChanges changes, notify commands
            SaveSettingsCommand.NotifyCanExecuteChanged();
            ResetSettingsCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(DebugInfo));
        }
    }

    public partial class FolderConfigurationViewModel : ObservableValidator
    {
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Path is required")]
        [CustomValidation(typeof(SettingsViewModel), nameof(SettingsViewModel.ValidateFolderPath))]
        private string _path = string.Empty;

        [ObservableProperty]
        [CustomValidation(typeof(SettingsViewModel), nameof(SettingsViewModel.ValidateFolderPath))]
        private string? _outputPath;

        [ObservableProperty] private bool _enabled = true;
        [ObservableProperty] private bool _includeSubdirectories;

        [ObservableProperty]
        [Required(ErrorMessage = "File pattern is required")]
        private string _filePattern = "*.jpg;*.jpeg";
    }
}
