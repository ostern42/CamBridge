// src\CamBridge.Core\MappingRule.cs
// Version: 0.6.2
// Description: DICOM mapping rule configuration

using System;
using System.Text.Json.Serialization;

namespace CamBridge.Core
{
    /// <summary>
    /// Defines a mapping rule from source data to DICOM tag
    /// </summary>
    public class MappingRule
    {
        /// <summary>
        /// Unique name for this mapping rule
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Human-readable description of what this rule maps
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Source type: QRBridge, EXIF, Static, etc.
        /// </summary>
        [JsonPropertyName("sourceType")]
        public string SourceType { get; set; } = "QRBridge";

        /// <summary>
        /// Field name in source data
        /// </summary>
        [JsonPropertyName("sourceField")]
        public string SourceField { get; set; } = string.Empty;

        /// <summary>
        /// Target DICOM tag in format (XXXX,XXXX)
        /// </summary>
        [JsonPropertyName("targetTag")]
        public string DicomTag { get; set; } = string.Empty;

        /// <summary>
        /// Alternative property name for compatibility
        /// </summary>
        [JsonPropertyName("dicomTag")]
        public string? TargetTag
        {
            get => DicomTag;
            set => DicomTag = value ?? string.Empty;
        }

        /// <summary>
        /// Value transformation to apply
        /// </summary>
        [JsonPropertyName("transform")]
        public string Transform { get; set; } = "None";

        /// <summary>
        /// Whether this field is required
        /// </summary>
        [JsonPropertyName("required")]
        public bool Required { get; set; }

        /// <summary>
        /// Default value if source is empty
        /// </summary>
        [JsonPropertyName("defaultValue")]
        public string? DefaultValue { get; set; }

        /// <summary>
        /// DICOM Value Representation (e.g., PN, DA, TM, etc.)
        /// </summary>
        [JsonPropertyName("valueRepresentation")]
        public string? ValueRepresentation { get; set; }

        /// <summary>
        /// Transform parameters (JSON serialized)
        /// </summary>
        [JsonPropertyName("transformParameters")]
        public string? TransformParameters { get; set; }

        /// <summary>
        /// Transform as enum (computed property)
        /// </summary>
        [JsonIgnore]
        public ValueTransform TransformEnum
        {
            get
            {
                if (Enum.TryParse<ValueTransform>(Transform, out var result))
                    return result;
                return ValueTransform.None;
            }
            set => Transform = value.ToString();
        }

        /// <summary>
        /// Apply the configured transformation
        /// </summary>
        public string? ApplyTransform(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return DefaultValue;

            // Fix legacy transform names
            var normalizedTransform = Transform switch
            {
                "GenderToDicom" => "MapGender",
                "TruncateTo16" => "None", // Not implemented in current version
                _ => Transform
            };

            if (!Enum.TryParse<ValueTransform>(normalizedTransform, out var transform))
                transform = ValueTransform.None;

            return transform switch
            {
                ValueTransform.None => input,
                ValueTransform.DateToDicom => ConvertDateToDicom(input),
                ValueTransform.TimeToDicom => ConvertTimeToDicom(input),
                ValueTransform.DateTimeToDicom => ConvertDateTimeToDicom(input),
                ValueTransform.MapGender => MapGenderValue(input),
                ValueTransform.RemovePrefix => RemovePrefixTransform(input),
                ValueTransform.ExtractDate => ExtractDateFromDateTime(input),
                ValueTransform.ExtractTime => ExtractTimeFromDateTime(input),
                ValueTransform.ToUpperCase => input.ToUpperInvariant(),
                ValueTransform.ToLowerCase => input.ToLowerInvariant(),
                ValueTransform.Trim => input.Trim(),
                _ => input
            };
        }

        #region Transform Methods

        private string ConvertDateToDicom(string input)
        {
            if (DateTime.TryParse(input, out var date))
            {
                return date.ToString("yyyyMMdd");
            }
            return input;
        }

        private string ConvertTimeToDicom(string input)
        {
            if (DateTime.TryParse(input, out var time))
            {
                return time.ToString("HHmmss");
            }
            return input;
        }

        private string ConvertDateTimeToDicom(string input)
        {
            if (DateTime.TryParse(input, out var dateTime))
            {
                return dateTime.ToString("yyyyMMddHHmmss");
            }
            return input;
        }

        private string MapGenderValue(string input)
        {
            return input?.ToUpperInvariant() switch
            {
                "M" or "MALE" => "M",
                "F" or "FEMALE" => "F",
                "O" or "OTHER" => "O",
                _ => DefaultValue ?? "O"
            };
        }

        private string RemovePrefixTransform(string input)
        {
            if (input.StartsWith("IMG_", StringComparison.OrdinalIgnoreCase))
            {
                return input.Substring(4);
            }
            return input;
        }

        private string ExtractDateFromDateTime(string input)
        {
            if (DateTime.TryParse(input, out var dateTime))
            {
                return dateTime.ToString("yyyyMMdd");
            }
            return input;
        }

        private string ExtractTimeFromDateTime(string input)
        {
            if (DateTime.TryParse(input, out var dateTime))
            {
                return dateTime.ToString("HHmmss");
            }
            return input;
        }

        #endregion
    }
}
