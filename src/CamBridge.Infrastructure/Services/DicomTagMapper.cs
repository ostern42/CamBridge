// src/CamBridge.Infrastructure/Services/DicomTagMapper.cs
// Version: 0.8.24 - CLEANED UP - No more duplicate transform logic!
// Description: Service for mapping values to DICOM tags with correlation ID support
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

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
        /// Maps source data to a DICOM dataset using mapping rules (Interface method)
        /// </summary>
        public void MapToDataset(DicomDataset dataset, Dictionary<string, string> sourceData, IEnumerable<MappingRule> mappingRules)
        {
            // Call the overloaded method with null correlationId
            MapToDataset(dataset, sourceData, mappingRules, null);
        }

        /// <summary>
        /// Maps source data to a DICOM dataset using mapping rules with correlation ID support
        /// </summary>
        public void MapToDataset(DicomDataset dataset, Dictionary<string, string> sourceData, IEnumerable<MappingRule> mappingRules, string? correlationId)
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
                            // FIXED: Add correlation ID to warning
                            if (!string.IsNullOrEmpty(correlationId))
                            {
                                _logger.LogWarning("[{CorrelationId}] [TagMapping] Required source field '{Field}' not found in data",
                                    correlationId, rule.SourceField);
                            }
                            else
                            {
                                _logger.LogWarning("Required source field '{Field}' not found in data", rule.SourceField);
                            }
                        }

                        // Use default value if available
                        sourceValue = rule.DefaultValue;

                        if (string.IsNullOrEmpty(sourceValue))
                        {
                            continue;
                        }
                    }

                    // FIXED: Use the MappingRule's ApplyTransform method instead of duplicating logic!
                    var transformedValue = rule.ApplyTransform(sourceValue);

                    if (string.IsNullOrEmpty(transformedValue) && rule.Required)
                    {
                        // FIXED: Add correlation ID to warning
                        if (!string.IsNullOrEmpty(correlationId))
                        {
                            _logger.LogWarning("[{CorrelationId}] [TagMapping] Required field '{Field}' resulted in empty value after transform",
                                correlationId, rule.SourceField);
                        }
                        else
                        {
                            _logger.LogWarning("Required field '{Field}' resulted in empty value after transform", rule.SourceField);
                        }
                    }

                    // Parse DICOM tag
                    if (!TryParseDicomTag(rule.DicomTag, out var group, out var element))
                    {
                        // FIXED: Add correlation ID to error
                        if (!string.IsNullOrEmpty(correlationId))
                        {
                            _logger.LogError("[{CorrelationId}] [TagMapping] Invalid DICOM tag format: {Tag}",
                                correlationId, rule.DicomTag);
                        }
                        else
                        {
                            _logger.LogError("Invalid DICOM tag format: {Tag}", rule.DicomTag);
                        }
                        continue;
                    }

                    // Add to dataset
                    var tag = new DicomTag(group, element);

                    if (!string.IsNullOrEmpty(transformedValue))
                    {
                        dataset.AddOrUpdate(tag, transformedValue);

                        // FIXED: Add correlation ID to debug log
                        if (!string.IsNullOrEmpty(correlationId))
                        {
                            _logger.LogDebug("[{CorrelationId}] [TagMapping] Mapped {Source} -> {Tag}: {Value}",
                                correlationId, rule.SourceField, rule.DicomTag, transformedValue);
                        }
                        else
                        {
                            _logger.LogDebug("Mapped {Source} -> {Tag}: {Value}",
                                rule.SourceField, rule.DicomTag, transformedValue);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // FIXED: Add correlation ID to error
                    if (!string.IsNullOrEmpty(correlationId))
                    {
                        _logger.LogError(ex, "[{CorrelationId}] [TagMapping] Error mapping rule {Source} -> {Tag}",
                            correlationId, rule.SourceField, rule.DicomTag);
                    }
                    else
                    {
                        _logger.LogError(ex, "Error mapping rule {Source} -> {Tag}",
                            rule.SourceField, rule.DicomTag);
                    }
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
    }
}
