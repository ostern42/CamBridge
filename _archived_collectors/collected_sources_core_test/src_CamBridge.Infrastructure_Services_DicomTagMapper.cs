using System;
using System.Collections.Generic;
using System.Linq;
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using Microsoft.Extensions.Logging;
using FellowOakDicom;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Service that applies mapping rules to convert source data to DICOM tags
    /// </summary>
    public class DicomTagMapper : IDicomTagMapper
    {
        private readonly ILogger<DicomTagMapper> _logger;
        private readonly IMappingConfiguration _mappingConfiguration;

        public DicomTagMapper(
            ILogger<DicomTagMapper> logger,
            IMappingConfiguration mappingConfiguration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mappingConfiguration = mappingConfiguration ?? throw new ArgumentNullException(nameof(mappingConfiguration));
        }

        /// <summary>
        /// Maps source data to DICOM dataset using configured mapping rules
        /// </summary>
        /// <param name="sourceData">Dictionary containing source data grouped by source type</param>
        /// <param name="dataset">Target DICOM dataset</param>
        /// <returns>Mapping result with statistics and errors</returns>
        public MappingResult MapToDataset(Dictionary<string, Dictionary<string, string>> sourceData, DicomDataset dataset)
        {
            if (sourceData == null) throw new ArgumentNullException(nameof(sourceData));
            if (dataset == null) throw new ArgumentNullException(nameof(dataset));

            var result = new MappingResult();
            var rules = _mappingConfiguration.GetMappingRules();

            _logger.LogInformation("Starting mapping with {RuleCount} rules", rules.Count);

            foreach (var rule in rules)
            {
                try
                {
                    ApplyMappingRule(rule, sourceData, dataset, result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error applying mapping rule {RuleName}", rule.Name);
                    result.AddError($"Rule '{rule.Name}': {ex.Message}");
                }
            }

            // Check for required fields
            ValidateRequiredFields(rules, result);

            _logger.LogInformation("Mapping completed: {SuccessCount} successful, {SkippedCount} skipped, {ErrorCount} errors",
                result.SuccessfulMappings, result.SkippedMappings, result.Errors.Count);

            return result;
        }

        /// <summary>
        /// Maps specific source types to DICOM dataset
        /// </summary>
        public MappingResult MapQRBridgeData(Dictionary<string, string> qrBridgeData, DicomDataset dataset)
        {
            var sourceData = new Dictionary<string, Dictionary<string, string>>
            {
                ["QRBridge"] = qrBridgeData
            };

            return MapToDataset(sourceData, dataset);
        }

        /// <summary>
        /// Maps EXIF data to DICOM dataset
        /// </summary>
        public MappingResult MapExifData(Dictionary<string, string> exifData, DicomDataset dataset)
        {
            var sourceData = new Dictionary<string, Dictionary<string, string>>
            {
                ["EXIF"] = exifData
            };

            return MapToDataset(sourceData, dataset);
        }

        private void ApplyMappingRule(
            MappingRule rule,
            Dictionary<string, Dictionary<string, string>> sourceData,
            DicomDataset dataset,
            MappingResult result)
        {
            // Find source data
            if (!sourceData.TryGetValue(rule.SourceType, out var typeData))
            {
                _logger.LogDebug("Source type {SourceType} not found in input data", rule.SourceType);
                result.SkippedMappings++;
                return;
            }

            // Get source value
            string? sourceValue = null;
            if (typeData.TryGetValue(rule.SourceField, out var value))
            {
                sourceValue = value;
            }

            // Use default value if no source value and not required
            if (string.IsNullOrWhiteSpace(sourceValue))
            {
                if (!string.IsNullOrWhiteSpace(rule.DefaultValue))
                {
                    sourceValue = rule.DefaultValue;
                    _logger.LogDebug("Using default value for {RuleName}: {DefaultValue}",
                        rule.Name, rule.DefaultValue);
                }
                else if (rule.IsRequired)
                {
                    result.AddError($"Required field '{rule.Name}' not found in source data");
                    return;
                }
                else
                {
                    _logger.LogDebug("Skipping optional field {RuleName} - no value found", rule.Name);
                    result.SkippedMappings++;
                    return;
                }
            }

            // Apply transformation
            var transformedValue = rule.ApplyTransform(sourceValue);
            if (transformedValue == null)
            {
                _logger.LogWarning("Transformation resulted in null value for {RuleName}", rule.Name);
                result.SkippedMappings++;
                return;
            }

            // Convert to fo-dicom tag
            var dicomTag = ConvertToFoDicomTag(rule.TargetTag);

            // Add to dataset
            try
            {
                dataset.AddOrUpdate(dicomTag, transformedValue);
                result.SuccessfulMappings++;
                result.AddAppliedRule(rule.Name, sourceValue, transformedValue, rule.TargetTag.ToString());

                _logger.LogDebug("Applied mapping {RuleName}: {SourceValue} -> {TransformedValue} to {Tag}",
                    rule.Name, sourceValue, transformedValue, rule.TargetTag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add tag {Tag} with value {Value}",
                    rule.TargetTag, transformedValue);
                result.AddError($"Failed to add tag {rule.TargetTag}: {ex.Message}");
            }
        }

        private void ValidateRequiredFields(IReadOnlyList<MappingRule> rules, MappingResult result)
        {
            var requiredRules = rules.Where(r => r.IsRequired).ToList();
            foreach (var rule in requiredRules)
            {
                if (!result.AppliedRules.Any(ar => ar.RuleName == rule.Name))
                {
                    result.AddError($"Required mapping '{rule.Name}' was not applied");
                }
            }
        }

        private FellowOakDicom.DicomTag ConvertToFoDicomTag(Core.ValueObjects.DicomTag tag)
        {
            return new FellowOakDicom.DicomTag(tag.Group, tag.Element);
        }
    }

    /// <summary>
    /// Interface for DICOM tag mapping service
    /// </summary>
    public interface IDicomTagMapper
    {
        /// <summary>
        /// Maps source data to DICOM dataset using configured mapping rules
        /// </summary>
        MappingResult MapToDataset(Dictionary<string, Dictionary<string, string>> sourceData, DicomDataset dataset);

        /// <summary>
        /// Maps QRBridge data to DICOM dataset
        /// </summary>
        MappingResult MapQRBridgeData(Dictionary<string, string> qrBridgeData, DicomDataset dataset);

        /// <summary>
        /// Maps EXIF data to DICOM dataset
        /// </summary>
        MappingResult MapExifData(Dictionary<string, string> exifData, DicomDataset dataset);
    }

    /// <summary>
    /// Result of a mapping operation
    /// </summary>
    public class MappingResult
    {
        public int SuccessfulMappings { get; set; }
        public int SkippedMappings { get; set; }
        public List<string> Errors { get; } = new();
        public List<AppliedRule> AppliedRules { get; } = new();

        public bool IsSuccess => Errors.Count == 0;

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public void AddAppliedRule(string ruleName, string sourceValue, string mappedValue, string targetTag)
        {
            AppliedRules.Add(new AppliedRule
            {
                RuleName = ruleName,
                SourceValue = sourceValue,
                MappedValue = mappedValue,
                TargetTag = targetTag
            });
        }

        public class AppliedRule
        {
            public string RuleName { get; set; } = string.Empty;
            public string SourceValue { get; set; } = string.Empty;
            public string MappedValue { get; set; } = string.Empty;
            public string TargetTag { get; set; } = string.Empty;
        }

        public override string ToString()
        {
            if (IsSuccess)
            {
                return $"Mapping successful: {SuccessfulMappings} mapped, {SkippedMappings} skipped";
            }
            else
            {
                return $"Mapping completed with errors: {SuccessfulMappings} mapped, {SkippedMappings} skipped, {Errors.Count} errors";
            }
        }
    }
}
