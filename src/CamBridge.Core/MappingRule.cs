using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CamBridge.Core
{
    /// <summary>
    /// Represents a mapping rule from source field to DICOM tag
    /// </summary>
    public class MappingRule
    {
        /// <summary>
        /// The source field name from EXIF/QRBridge data
        /// </summary>
        [Required]
        public string SourceField { get; set; } = string.Empty;

        /// <summary>
        /// The target DICOM tag (e.g., "(0010,0010)")
        /// </summary>
        [Required]
        public string DicomTag { get; set; } = string.Empty;

        /// <summary>
        /// Human-readable description of the DICOM tag
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// DICOM Value Representation (e.g., "PN", "LO", "DA")
        /// </summary>
        public string? ValueRepresentation { get; set; }

        /// <summary>
        /// Transform function to apply (as string for XAML compatibility)
        /// </summary>
        public string? Transform { get; set; }

        /// <summary>
        /// Transform as enum (for internal processing)
        /// </summary>
        public ValueTransform? TransformEnum
        {
            get => Enum.TryParse<ValueTransform>(Transform, out var result) ? result : null;
            set => Transform = value?.ToString();
        }

        /// <summary>
        /// Default value if source field is missing
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// Whether this mapping is required
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Additional parameters for the transform function
        /// </summary>
        public Dictionary<string, string>? TransformParameters { get; set; }

        // Additional properties that IMappingConfiguration might expect
        /// <summary>
        /// Type of the source field (for compatibility)
        /// </summary>
        public string? SourceType { get; set; }

        /// <summary>
        /// Alternative name property (for compatibility)
        /// </summary>
        public string? Name => Description;

        /// <summary>
        /// Constructor for object initializer syntax
        /// </summary>
        public MappingRule()
        {
        }

        /// <summary>
        /// Constructor with parameters (for compatibility with IMappingConfiguration)
        /// </summary>
        public MappingRule(string sourceField, string dicomTag, string description, string valueRepresentation, string? transform = null)
        {
            SourceField = sourceField;
            DicomTag = dicomTag;
            Description = description;
            ValueRepresentation = valueRepresentation;
            Transform = transform;
        }
    }
}
