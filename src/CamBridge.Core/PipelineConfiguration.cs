// src\CamBridge.Core\PipelineConfiguration.cs
// Version: 0.8.0
// Description: Pipeline configuration model with PACS support
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace CamBridge.Core
{
    /// <summary>
    /// Represents a complete processing pipeline from input to output
    /// </summary>
    public class PipelineConfiguration : INotifyPropertyChanged
    {
        private Guid _id = Guid.NewGuid();
        private string _name = "New Pipeline";
        private string? _description;
        private bool _enabled = true;
        private PipelineWatchSettings _watchSettings = new();
        private ProcessingOptions _processingOptions = new();
        private DicomOverrides? _dicomOverrides;
        private Guid? _mappingSetId;
        private PacsConfiguration? _pacsConfiguration;

        /// <summary>
        /// Unique identifier for this pipeline
        /// </summary>
        public Guid Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Display name for this pipeline
        /// </summary>
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Optional description
        /// </summary>
        public string? Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Whether this pipeline is active
        /// </summary>
        public bool Enabled
        {
            get => _enabled;
            set { _enabled = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Watch folder configuration for this pipeline
        /// </summary>
        public PipelineWatchSettings WatchSettings
        {
            get => _watchSettings;
            set { _watchSettings = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Processing options specific to this pipeline
        /// </summary>
        public ProcessingOptions ProcessingOptions
        {
            get => _processingOptions;
            set { _processingOptions = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// DICOM overrides for this pipeline (optional)
        /// </summary>
        public DicomOverrides? DicomOverrides
        {
            get => _dicomOverrides;
            set { _dicomOverrides = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// ID of the mapping set to use
        /// </summary>
        public Guid? MappingSetId
        {
            get => _mappingSetId;
            set { _mappingSetId = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// PACS upload configuration for this pipeline (optional)
        /// </summary>
        public PacsConfiguration? PacsConfiguration
        {
            get => _pacsConfiguration;
            set { _pacsConfiguration = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Pipeline-specific metadata
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = new();

        /// <summary>
        /// Creation timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Last modification timestamp
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Calculated property for UI display
        /// </summary>
        [JsonIgnore]
        public string ProcessedToday => "0 files"; // TODO: Implement actual counting

        [JsonIgnore]
        public bool IsValid => WatchSettings?.IsValid ?? false;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Watch folder settings for a pipeline
    /// </summary>
    public class PipelineWatchSettings : INotifyPropertyChanged
    {
        private string _path = string.Empty;
        private string _filePattern = "*.jpg;*.jpeg";
        private bool _includeSubdirectories = false;
        private string? _outputPath;
        private int _minimumFileAgeSeconds = 2;

        /// <summary>
        /// Folder path to watch
        /// </summary>
        public string Path
        {
            get => _path;
            set { _path = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsValid)); }
        }

        /// <summary>
        /// File pattern to match (e.g., "*.jpg;*.jpeg")
        /// </summary>
        public string FilePattern
        {
            get => _filePattern;
            set { _filePattern = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Include subdirectories in watch
        /// </summary>
        public bool IncludeSubdirectories
        {
            get => _includeSubdirectories;
            set { _includeSubdirectories = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Custom output path (overrides processing options)
        /// </summary>
        public string? OutputPath
        {
            get => _outputPath;
            set { _outputPath = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Minimum file age before processing (to ensure write completion)
        /// </summary>
        public int MinimumFileAgeSeconds
        {
            get => _minimumFileAgeSeconds;
            set { _minimumFileAgeSeconds = value; OnPropertyChanged(); }
        }

        [JsonIgnore]
        public bool IsValid => !string.IsNullOrWhiteSpace(Path) &&
                              System.IO.Directory.Exists(Path);

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// DICOM settings that can be overridden per pipeline
    /// </summary>
    public class DicomOverrides : INotifyPropertyChanged
    {
        private string? _institutionName;
        private string? _institutionDepartment;
        private string? _stationName;

        /// <summary>
        /// Override institution name for this pipeline
        /// </summary>
        public string? InstitutionName
        {
            get => _institutionName;
            set { _institutionName = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Override institution department for this pipeline
        /// </summary>
        public string? InstitutionDepartment
        {
            get => _institutionDepartment;
            set { _institutionDepartment = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Override station name for this pipeline
        /// </summary>
        public string? StationName
        {
            get => _stationName;
            set { _stationName = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Additional DICOM tags to set/override
        /// </summary>
        public Dictionary<string, string> AdditionalTags { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Represents a reusable mapping configuration set
    /// </summary>
    public class MappingSet : INotifyPropertyChanged
    {
        private Guid _id = Guid.NewGuid();
        private string _name = "New Mapping Set";
        private string? _description;
        private bool _isSystemDefault = false;

        /// <summary>
        /// Unique identifier
        /// </summary>
        public Guid Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Display name
        /// </summary>
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Optional description
        /// </summary>
        public string? Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// The actual mapping rules
        /// </summary>
        public List<MappingRule> Rules { get; set; } = new();

        /// <summary>
        /// Whether this is a system default (read-only)
        /// </summary>
        public bool IsSystemDefault
        {
            get => _isSystemDefault;
            set { _isSystemDefault = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Creation timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Last modification timestamp
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Tags for categorization
        /// </summary>
        public List<string> Tags { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// PACS (Picture Archiving and Communication System) configuration for automatic upload
    /// </summary>
    public class PacsConfiguration : INotifyPropertyChanged
    {
        private bool _enabled = false;
        private string _host = string.Empty;
        private int _port = 104;
        private string _calledAeTitle = string.Empty;
        private string _callingAeTitle = "CAMBRIDGE";
        private int _timeoutSeconds = 30;
        private int _maxConcurrentUploads = 1;
        private bool _retryOnFailure = true;
        private int _maxRetryAttempts = 3;
        private int _retryDelaySeconds = 5;

        /// <summary>
        /// Enable automatic PACS upload after DICOM creation
        /// </summary>
        public bool Enabled
        {
            get => _enabled;
            set { _enabled = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// PACS server hostname or IP address
        /// </summary>
        public string Host
        {
            get => _host;
            set { _host = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// PACS server port (default: 104)
        /// </summary>
        public int Port
        {
            get => _port;
            set { _port = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Called AE Title (PACS server's AE Title)
        /// </summary>
        public string CalledAeTitle
        {
            get => _calledAeTitle;
            set { _calledAeTitle = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Calling AE Title (our AE Title)
        /// </summary>
        public string CallingAeTitle
        {
            get => _callingAeTitle;
            set { _callingAeTitle = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Connection timeout in seconds
        /// </summary>
        public int TimeoutSeconds
        {
            get => _timeoutSeconds;
            set { _timeoutSeconds = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Maximum concurrent uploads per pipeline
        /// </summary>
        public int MaxConcurrentUploads
        {
            get => _maxConcurrentUploads;
            set { _maxConcurrentUploads = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Enable retry on upload failure
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
        /// Delay between retry attempts in seconds
        /// </summary>
        public int RetryDelaySeconds
        {
            get => _retryDelaySeconds;
            set { _retryDelaySeconds = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Validate configuration
        /// </summary>
        public bool IsValid() =>
            !string.IsNullOrWhiteSpace(Host) &&
            Port > 0 &&
            Port < 65536 &&
            !string.IsNullOrWhiteSpace(CalledAeTitle) &&
            !string.IsNullOrWhiteSpace(CallingAeTitle) &&
            CalledAeTitle.Length <= 16 &&
            CallingAeTitle.Length <= 16;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
