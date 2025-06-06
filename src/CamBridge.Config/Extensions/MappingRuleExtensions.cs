// File: src/CamBridge.Config/Extensions/MappingRuleExtensions.cs
// Version: 0.6.2
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions
// Modified: 2025-06-07
// Status: Development/Local

using System;
using CamBridge.Core;

namespace CamBridge.Config.Extensions
{
    /// <summary>
    /// Extension methods for MappingRule to support UI-specific functionality
    /// </summary>
    public static class MappingRuleExtensions
    {
        // HINWEIS: Die ApplyTransform Methode ist jetzt direkt in MappingRule.cs implementiert!
        // Diese Klasse könnte gelöscht werden, oder wir behalten sie für andere UI-spezifische Extensions

        /// <summary>
        /// Gets a display-friendly description of the transform
        /// </summary>
        public static string GetTransformDescription(this MappingRule rule)
        {
            return rule.TransformEnum switch
            {
                ValueTransform.None => "No transformation",
                ValueTransform.DateToDicom => "Convert date to DICOM format (YYYYMMDD)",
                ValueTransform.TimeToDicom => "Convert time to DICOM format (HHMMSS)",
                ValueTransform.DateTimeToDicom => "Convert datetime to DICOM format",
                ValueTransform.MapGender => "Map gender to DICOM values (M/F/O)",
                ValueTransform.RemovePrefix => "Remove prefix from value",
                ValueTransform.ExtractDate => "Extract date from datetime",
                ValueTransform.ExtractTime => "Extract time from datetime",
                ValueTransform.ToUpperCase => "Convert to uppercase",
                ValueTransform.ToLowerCase => "Convert to lowercase",
                ValueTransform.Trim => "Remove leading/trailing spaces",
                _ => "Unknown transformation"
            };
        }

        /// <summary>
        /// Validates if the rule is properly configured
        /// </summary>
        public static bool IsValid(this MappingRule rule)
        {
            if (string.IsNullOrWhiteSpace(rule.SourceField))
                return false;

            if (string.IsNullOrWhiteSpace(rule.DicomTag))
                return false;

            // Validate DICOM tag format (XXXX,XXXX)
            if (!System.Text.RegularExpressions.Regex.IsMatch(rule.DicomTag, @"^\([0-9A-Fa-f]{4},[0-9A-Fa-f]{4}\)$"))
                return false;

            return true;
        }

        /// <summary>
        /// Gets a UI-friendly display name for the rule
        /// </summary>
        public static string GetDisplayName(this MappingRule rule)
        {
            if (!string.IsNullOrWhiteSpace(rule.Description))
                return rule.Description;

            if (!string.IsNullOrWhiteSpace(rule.Name))
                return rule.Name;

            return $"{rule.SourceType}.{rule.SourceField} → {rule.DicomTag}";
        }
    }
}
