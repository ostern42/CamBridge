using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using FellowOakDicom;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Service responsible for mapping values to DICOM tags according to mapping rules
    /// </summary>
    public class DicomTagMapper : IDicomTagMapper
    {
        private readonly ILogger<DicomTagMapper> _logger;

        public DicomTagMapper(ILogger<DicomTagMapper> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Applies a mapping rule to transform a value
        /// </summary>
        public string? ApplyTransform(string? value, string? transform)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(transform) || transform == "None")
            {
                return value;
            }

            // Parse transform string to enum
            if (!Enum.TryParse<ValueTransform>(transform, out var transformEnum))
            {
                _logger.LogWarning("Unknown transform: {Transform}", transform);
                return value;
            }

            try
            {
                return transformEnum switch
                {
                    ValueTransform.DateToDicom => ConvertDateToDicom(value),
                    ValueTransform.TimeToDicom => ConvertTimeToDicom(value),
                    ValueTransform.DateTimeToDicom => ConvertDateTimeToDicom(value),
                    ValueTransform.MapGender => MapGenderCode(value),
                    ValueTransform.RemovePrefix => RemovePrefix(value),
                    ValueTransform.ExtractDate => ExtractDate(value),
                    ValueTransform.ExtractTime => ExtractTime(value),
                    ValueTransform.ToUpperCase => value.ToUpperInvariant(),
                    ValueTransform.ToLowerCase => value.ToLowerInvariant(),
                    ValueTransform.Trim => value.Trim(),
                    _ => value
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to apply transform {Transform} to value '{Value}'", transform, value);
                return value;
            }
        }

        private string ConvertDateToDicom(string date)
        {
            // Convert various date formats to DICOM format (YYYYMMDD)
            if (DateTime.TryParse(date, out var dt))
            {
                return dt.ToString("yyyyMMdd");
            }

            // Already in DICOM format?
            if (date.Length == 8 && int.TryParse(date, out _))
            {
                return date;
            }

            _logger.LogWarning("Unable to convert date '{Date}' to DICOM format", date);
            return date;
        }

        private string ConvertTimeToDicom(string time)
        {
            // Convert various time formats to DICOM format (HHMMSS)
            if (DateTime.TryParse(time, out var dt))
            {
                return dt.ToString("HHmmss");
            }

            if (TimeSpan.TryParse(time, out var ts))
            {
                return $"{ts.Hours:D2}{ts.Minutes:D2}{ts.Seconds:D2}";
            }

            _logger.LogWarning("Unable to convert time '{Time}' to DICOM format", time);
            return time;
        }

        private string ConvertDateTimeToDicom(string dateTime)
        {
            if (DateTime.TryParse(dateTime, out var dt))
            {
                return dt.ToString("yyyyMMddHHmmss");
            }

            _logger.LogWarning("Unable to convert datetime '{DateTime}' to DICOM format", dateTime);
            return dateTime;
        }

        /// <summary>
        /// Maps source data to a DICOM dataset using mapping rules
        /// </summary>
        public void MapToDataset(DicomDataset dataset, Dictionary<string, string> sourceData, IEnumerable<MappingRule> mappingRules)
        {
            if (dataset == null) throw new ArgumentNullException(nameof(dataset));
            if (sourceData == null) throw new ArgumentNullException(nameof(sourceData));
            if (mappingRules == null) throw new ArgumentNullException(nameof(mappingRules));

            foreach (var rule in mappingRules)
            {
                try
                {
                    // Get source value
                    if (!sourceData.TryGetValue(rule.SourceField, out var sourceValue))
                    {
                        if (rule.Required)
                        {
                            _logger.LogWarning("Required source field '{Field}' not found in data", rule.SourceField);
                        }

                        // Use default value if available
                        sourceValue = rule.DefaultValue;

                        if (string.IsNullOrEmpty(sourceValue))
                        {
                            continue;
                        }
                    }

                    // Apply transform if specified
                    var transformedValue = ApplyTransform(sourceValue, rule.Transform);

                    if (string.IsNullOrEmpty(transformedValue) && rule.Required)
                    {
                        _logger.LogWarning("Required field '{Field}' resulted in empty value after transform", rule.SourceField);
                    }

                    // Parse DICOM tag
                    if (!TryParseDicomTag(rule.DicomTag, out var group, out var element))
                    {
                        _logger.LogError("Invalid DICOM tag format: {Tag}", rule.DicomTag);
                        continue;
                    }

                    // Add to dataset
                    var tag = new DicomTag(group, element);

                    if (!string.IsNullOrEmpty(transformedValue))
                    {
                        dataset.AddOrUpdate(tag, transformedValue);
                        _logger.LogDebug("Mapped {Source} -> {Tag}: {Value}",
                            rule.SourceField, rule.DicomTag, transformedValue);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error mapping rule {Source} -> {Tag}",
                        rule.SourceField, rule.DicomTag);
                }
            }
        }

        /// <summary>
        /// Parses a DICOM tag string like "(0010,0010)" into group and element
        /// </summary>
        private bool TryParseDicomTag(string tagString, out ushort group, out ushort element)
        {
            group = 0;
            element = 0;

            if (string.IsNullOrEmpty(tagString))
                return false;

            // Remove parentheses and spaces
            var cleaned = tagString.Trim('(', ')', ' ');
            var parts = cleaned.Split(',');

            if (parts.Length != 2)
                return false;

            try
            {
                group = Convert.ToUInt16(parts[0].Trim(), 16);
                element = Convert.ToUInt16(parts[1].Trim(), 16);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string MapGenderCode(string gender)
        {
            return gender?.ToUpperInvariant() switch
            {
                "M" or "MALE" => "M",
                "F" or "FEMALE" => "F",
                "O" or "OTHER" => "O",
                _ => ""
            };
        }

        private string RemovePrefix(string value)
        {
            // Remove common prefixes like "GCM_TAG"
            if (value.StartsWith("GCM_TAG", StringComparison.OrdinalIgnoreCase))
            {
                return value.Substring(7).Trim();
            }

            return value;
        }

        private string ExtractDate(string dateTime)
        {
            if (DateTime.TryParse(dateTime, out var dt))
            {
                return dt.ToString("yyyyMMdd");
            }

            return dateTime;
        }

        private string ExtractTime(string dateTime)
        {
            if (DateTime.TryParse(dateTime, out var dt))
            {
                return dt.ToString("HHmmss");
            }

            return dateTime;
        }
    }
}
