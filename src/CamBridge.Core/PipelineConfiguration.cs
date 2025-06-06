// src\CamBridge.Core\PipelineConfiguration.cs
// Version: 0.6.0
// Description: Pipeline configuration model for multi-pipeline architecture

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CamBridge.Core
{
    /// <summary>
    /// Represents a complete processing pipeline from input to output
    /// </summary>
    public class PipelineConfiguration
    {
        /// <summary>
        /// Unique identifier for this pipeline
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Display name for this pipeline
        /// </summary>
        public string Name { get; set; } = "New Pipeline";

        /// <summary>
        /// Optional description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Whether this pipeline is active
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Watch folder configuration for this pipeline
        /// </summary>
        public PipelineWatchSettings WatchSettings { get; set; } = new();

        /// <summary>
        /// Processing options specific to this pipeline
        /// </summary>
        public ProcessingOptions ProcessingOptions { get; set; } = new();

        /// <summary>
        /// DICOM overrides for this pipeline (optional)
        /// </summary>
        public DicomOverrides? DicomOverrides { get; set; }

        /// <summary>
        /// ID of the mapping set to use
        /// </summary>
        public Guid? MappingSetId { get; set; }

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

        [JsonIgnore]
        public bool IsValid => WatchSettings?.IsValid ?? false;
    }

    /// <summary>
    /// Watch folder settings for a pipeline
    /// </summary>
    public class PipelineWatchSettings
    {
        /// <summary>
        /// Folder path to watch
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// File pattern to match (e.g., "*.jpg;*.jpeg")
        /// </summary>
        public string FilePattern { get; set; } = "*.jpg;*.jpeg";

        /// <summary>
        /// Include subdirectories in watch
        /// </summary>
        public bool IncludeSubdirectories { get; set; } = false;

        /// <summary>
        /// Custom output path (overrides processing options)
        /// </summary>
        public string? OutputPath { get; set; }

        /// <summary>
        /// Minimum file age before processing (to ensure write completion)
        /// </summary>
        public int MinimumFileAgeSeconds { get; set; } = 2;

        [JsonIgnore]
        public bool IsValid => !string.IsNullOrWhiteSpace(Path) &&
                              System.IO.Directory.Exists(Path);
    }

    /// <summary>
    /// DICOM settings that can be overridden per pipeline
    /// </summary>
    public class DicomOverrides
    {
        /// <summary>
        /// Override institution name for this pipeline
        /// </summary>
        public string? InstitutionName { get; set; }

        /// <summary>
        /// Override institution department for this pipeline
        /// </summary>
        public string? InstitutionDepartment { get; set; }

        /// <summary>
        /// Override station name for this pipeline
        /// </summary>
        public string? StationName { get; set; }

        /// <summary>
        /// Additional DICOM tags to set/override
        /// </summary>
        public Dictionary<string, string> AdditionalTags { get; set; } = new();
    }

    /// <summary>
    /// Represents a reusable mapping configuration set
    /// </summary>
    public class MappingSet
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Display name
        /// </summary>
        public string Name { get; set; } = "New Mapping Set";

        /// <summary>
        /// Optional description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The actual mapping rules
        /// </summary>
        public List<MappingRule> Rules { get; set; } = new();

        /// <summary>
        /// Whether this is a system default (read-only)
        /// </summary>
        public bool IsSystemDefault { get; set; } = false;

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
    }
}
