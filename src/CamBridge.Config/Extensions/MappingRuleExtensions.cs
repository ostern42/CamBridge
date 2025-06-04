// File: src/CamBridge.Config/Extensions/MappingRuleExtensions.cs
// Version: 0.5.24
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions
// Modified: 2025-06-04
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
        /// <summary>
        /// Apply transform to a value (UI preview functionality)
        /// </summary>
        public static string ApplyTransform(this MappingRule rule, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return rule.DefaultValue ?? string.Empty;

            var transform = rule.TransformEnum ?? ValueTransform.None;

            return transform switch
            {
                ValueTransform.DateToDicom => TransformDateToDicom(input),
                ValueTransform.TimeToDicom => TransformTimeToDicom(input),
                ValueTransform.DateTimeToDicom => TransformDateTimeToDicom(input),
                ValueTransform.MapGender => TransformGender(input),
                ValueTransform.RemovePrefix => RemovePrefix(input, rule.TransformParameters),
                ValueTransform.ExtractDate => ExtractDate(input),
                ValueTransform.ExtractTime => ExtractTime(input),
                ValueTransform.ToUpperCase => input.ToUpperInvariant(),
                ValueTransform.ToLowerCase => input.ToLowerInvariant(),
                ValueTransform.Trim => input.Trim(),
                _ => input
            };
        }

        private static string TransformDateToDicom(string input)
        {
            if (DateTime.TryParse(input, out var date))
            {
                return date.ToString("yyyyMMdd");
            }
            return input;
        }

        private static string TransformTimeToDicom(string input)
        {
            if (DateTime.TryParse(input, out var time))
            {
                return time.ToString("HHmmss");
            }
            return input;
        }

        private static string TransformDateTimeToDicom(string input)
        {
            if (DateTime.TryParse(input, out var dateTime))
            {
                return dateTime.ToString("yyyyMMddHHmmss");
            }
            return input;
        }

        private static string TransformGender(string input)
        {
            return input.ToUpperInvariant() switch
            {
                "M" or "MALE" => "M",
                "F" or "FEMALE" => "F",
                "O" or "OTHER" => "O",
                _ => input
            };
        }

        private static string RemovePrefix(string input, Dictionary<string, string>? parameters)
        {
            if (parameters?.TryGetValue("prefix", out var prefix) == true && !string.IsNullOrEmpty(prefix))
            {
                return input.StartsWith(prefix) ? input.Substring(prefix.Length) : input;
            }
            return input;
        }

        private static string ExtractDate(string input)
        {
            if (DateTime.TryParse(input, out var dateTime))
            {
                return dateTime.ToString("yyyyMMdd");
            }
            return input;
        }

        private static string ExtractTime(string input)
        {
            if (DateTime.TryParse(input, out var dateTime))
            {
                return dateTime.ToString("HHmmss");
            }
            return input;
        }
    }

    /// <summary>
    /// Extension methods for CustomMappingConfiguration
    /// </summary>
    public static class CustomMappingConfigurationExtensions
    {
        /// <summary>
        /// Add a mapping rule to the configuration
        /// </summary>
        public static void AddRule(this CustomMappingConfiguration config, MappingRule rule)
        {
            // This is a stub implementation
            // In a real implementation, this would add to an internal collection
            // For now, it's just to satisfy the UI compilation
        }
    }
}
