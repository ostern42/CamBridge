using System;
using CamBridge.Core.ValueObjects;

namespace CamBridge.Core
{
    /// <summary>
    /// Defines a mapping rule from source data to DICOM tag
    /// </summary>
    public class MappingRule
    {
        public string Name { get; }
        public string SourceType { get; }
        public string SourceField { get; }
        public DicomTag TargetTag { get; }
        public ValueTransform Transform { get; }
        public bool IsRequired { get; }
        public string? DefaultValue { get; }

        public MappingRule(
            string name,
            string sourceType,
            string sourceField,
            DicomTag targetTag,
            ValueTransform transform = ValueTransform.None,
            bool isRequired = false,
            string? defaultValue = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            SourceField = sourceField ?? throw new ArgumentNullException(nameof(sourceField));
            TargetTag = targetTag ?? throw new ArgumentNullException(nameof(targetTag));
            Transform = transform;
            IsRequired = isRequired;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Applies the transformation to a value
        /// </summary>
        public string? ApplyTransform(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return DefaultValue;

            return Transform switch
            {
                ValueTransform.None => value,
                ValueTransform.ToUpper => value.ToUpper(),
                ValueTransform.ToLower => value.ToLower(),
                ValueTransform.DateToDicom => TransformDateToDicom(value),
                ValueTransform.TimeToDicom => TransformTimeToDicom(value),
                ValueTransform.GenderToDicom => TransformGenderToDicom(value),
                ValueTransform.TruncateTo16 => value.Length > 16 ? value.Substring(0, 16) : value,
                ValueTransform.TruncateTo64 => value.Length > 64 ? value.Substring(0, 64) : value,
                _ => value
            };
        }

        private static string? TransformDateToDicom(string value)
        {
            // Convert various date formats to DICOM format (YYYYMMDD)
            if (DateTime.TryParse(value, out var date))
            {
                return date.ToString("yyyyMMdd");
            }
            return value;
        }

        private static string? TransformTimeToDicom(string value)
        {
            // Convert various time formats to DICOM format (HHMMSS.FFFFFF)
            if (DateTime.TryParse(value, out var time))
            {
                return time.ToString("HHmmss.ffffff");
            }
            return value;
        }

        private static string TransformGenderToDicom(string value)
        {
            // Convert gender to DICOM format (M, F, O)
            return value?.ToUpperInvariant() switch
            {
                "M" or "MALE" or "MANN" or "MÄNNLICH" => "M",
                "F" or "FEMALE" or "FRAU" or "WEIBLICH" => "F",
                "O" or "OTHER" or "ANDERE" or "DIVERS" => "O",
                _ => "O"
            };
        }
    }

    /// <summary>
    /// Value transformation types
    /// </summary>
    public enum ValueTransform
    {
        None,
        ToUpper,
        ToLower,
        DateToDicom,
        TimeToDicom,
        GenderToDicom,
        TruncateTo16,
        TruncateTo64
    }
}