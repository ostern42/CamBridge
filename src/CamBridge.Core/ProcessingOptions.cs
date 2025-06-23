// src\CamBridge.Core\ProcessingOptions.cs
// Version: 0.6.5
// Description: Processing options with INotifyPropertyChanged support

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace CamBridge.Core
{
    /// <summary>
    /// Options for processing JPEG files
    /// </summary>
    public class ProcessingOptions : INotifyPropertyChanged
    {
        private PostProcessingAction _successAction = PostProcessingAction.Archive;
        private PostProcessingAction _failureAction = PostProcessingAction.MoveToError;
        private string _archiveFolder = @"C:\CamBridge\Archive";
        private string _errorFolder = @"C:\CamBridge\Errors";
        private string _backupFolder = @"C:\CamBridge\Backup";
        private string? _deadLetterFolder = @"C:\CamBridge\DeadLetters";
        private bool _createBackup = true;
        private int _maxConcurrentProcessing = 2;
        private bool _retryOnFailure = true;
        private int _maxRetryAttempts = 3;
        private OutputOrganization _outputOrganization = OutputOrganization.ByPatientAndDate;
        private bool _processExistingOnStartup = true;
        private TimeSpan? _maxFileAge;
        private long? _minimumFileSizeBytes;
        private long? _maximumFileSizeBytes;
        private string? _outputFilePattern;
        private int _retryDelaySeconds = 5;

        /// <summary>
        /// Action to take on successful processing
        /// </summary>
        public PostProcessingAction SuccessAction
        {
            get => _successAction;
            set { _successAction = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Action to take on failed processing
        /// </summary>
        public PostProcessingAction FailureAction
        {
            get => _failureAction;
            set { _failureAction = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Archive folder for successfully processed files
        /// </summary>
        public string ArchiveFolder
        {
            get => _archiveFolder;
            set { _archiveFolder = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Error folder for failed files
        /// </summary>
        public string ErrorFolder
        {
            get => _errorFolder;
            set { _errorFolder = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Backup folder for original files
        /// </summary>
        public string BackupFolder
        {
            get => _backupFolder;
            set { _backupFolder = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Dead letter folder for unprocessable files
        /// </summary>
        public string? DeadLetterFolder
        {
            get => _deadLetterFolder;
            set { _deadLetterFolder = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Whether to create backup before processing
        /// </summary>
        public bool CreateBackup
        {
            get => _createBackup;
            set { _createBackup = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Maximum concurrent file processing
        /// </summary>
        public int MaxConcurrentProcessing
        {
            get => _maxConcurrentProcessing;
            set { _maxConcurrentProcessing = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Whether to retry on failure
        /// </summary>
        public bool RetryOnFailure
        {
            get => _retryOnFailure;
            set { _retryOnFailure = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Maximum retry attempts
        /// </summary>
        public int MaxRetryAttempts
        {
            get => _maxRetryAttempts;
            set { _maxRetryAttempts = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Output folder organization strategy
        /// </summary>
        public OutputOrganization OutputOrganization
        {
            get => _outputOrganization;
            set { _outputOrganization = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Whether to process existing files on startup
        /// </summary>
        public bool ProcessExistingOnStartup
        {
            get => _processExistingOnStartup;
            set { _processExistingOnStartup = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Maximum age of files to process
        /// </summary>
        public TimeSpan? MaxFileAge
        {
            get => _maxFileAge;
            set { _maxFileAge = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Minimum file size in bytes
        /// </summary>
        public long? MinimumFileSizeBytes
        {
            get => _minimumFileSizeBytes;
            set { _minimumFileSizeBytes = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Maximum file size in bytes
        /// </summary>
        public long? MaximumFileSizeBytes
        {
            get => _maximumFileSizeBytes;
            set { _maximumFileSizeBytes = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Pattern for output file naming
        /// </summary>
        public string? OutputFilePattern
        {
            get => _outputFilePattern;
            set { _outputFilePattern = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Delay between retry attempts in seconds
        /// </summary>
        public int RetryDelaySeconds
        {
            get => _retryDelaySeconds;
            set { _retryDelaySeconds = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Actions to take after processing
    /// </summary>
    public enum PostProcessingAction
    {
        /// <summary>
        /// Leave the file in place
        /// </summary>
        Leave,

        /// <summary>
        /// Move to archive folder
        /// </summary>
        Archive,

        /// <summary>
        /// Delete the file
        /// </summary>
        Delete,

        /// <summary>
        /// Move to error folder
        /// </summary>
        MoveToError
    }

    /// <summary>
    /// File action types
    /// </summary>
    public enum FileActionType
    {
        Delete,
        Move,
        Keep
    }

    /// <summary>
    /// Output organization types
    /// </summary>
    public enum OutputOrganizationType
    {
        Flat,           // All files in one folder
        YearMonth,      // Year/Month subfolders
        PatientStudy,   // Patient/Study subfolders
        DatePatient     // Date/Patient subfolders
    }


    /// <summary>
    /// Output folder organization strategies
    /// </summary>
    public enum OutputOrganization
    {
        /// <summary>
        /// No organization
        /// </summary>
        None,

        /// <summary>
        /// Organize by patient name
        /// </summary>
        ByPatient,

        /// <summary>
        /// Organize by date
        /// </summary>
        ByDate,

        /// <summary>
        /// Organize by patient and date
        /// </summary>
        ByPatientAndDate

        
    }
}
