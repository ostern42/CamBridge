// src/CamBridge.Core/CustomMappingConfiguration.cs
// Version: 0.8.12
// Modified: 2025-07-03
// Description: Simple in-memory mapping configuration for pipeline-specific rules
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CamBridge.Core.Interfaces;

namespace CamBridge.Core
{
    /// <summary>
    /// Simple in-memory mapping configuration implementation
    /// Used for pipeline-specific mappings loaded from PipelineConfiguration
    /// </summary>
    public class CustomMappingConfiguration : IMappingConfiguration
    {
        private readonly List<MappingRule> _rules;
        private readonly string _sourceName;

        /// <summary>
        /// Creates a new custom mapping configuration with predefined rules
        /// </summary>
        /// <param name="rules">The mapping rules to use</param>
        /// <param name="sourceName">Name for logging (e.g., "Pipeline: Radiology")</param>
        public CustomMappingConfiguration(IEnumerable<MappingRule> rules, string sourceName = "Custom")
        {
            _rules = rules?.ToList() ?? new List<MappingRule>();
            _sourceName = sourceName;
        }

        /// <summary>
        /// Gets all configured mapping rules
        /// </summary>
        public IReadOnlyList<MappingRule> GetMappingRules() => _rules.AsReadOnly();

        /// <summary>
        /// Not supported - rules are provided in constructor
        /// </summary>
        public Task<bool> LoadConfigurationAsync(string? filePath = null)
        {
            // This implementation doesn't load from file
            // Rules are provided via constructor
            return Task.FromResult(true);
        }

        /// <summary>
        /// Not supported - rules are provided in constructor
        /// </summary>
        public Task<bool> LoadConfigurationAsync(string? filePath, string? correlationId)
        {
            // This implementation doesn't load from file
            // Rules are provided via constructor
            return Task.FromResult(true);
        }

        /// <summary>
        /// Not supported - this is a read-only configuration
        /// </summary>
        public Task<bool> SaveConfigurationAsync(IEnumerable<MappingRule> rules, string? filePath = null)
        {
            // This implementation doesn't save to file
            // It's used for in-memory pipeline configurations
            throw new NotSupportedException($"CustomMappingConfiguration for '{_sourceName}' is read-only");
        }

        /// <summary>
        /// Adds a new mapping rule
        /// </summary>
        public void AddRule(MappingRule rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            _rules.Add(rule);
        }

        /// <summary>
        /// Removes mapping rules for a specific source field
        /// </summary>
        public void RemoveRule(string sourceField)
        {
            if (string.IsNullOrWhiteSpace(sourceField))
                return;

            _rules.RemoveAll(r => string.Equals(r.SourceField, sourceField, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the mapping rule for a specific source field
        /// </summary>
        public MappingRule? GetRuleForSource(string sourceField)
        {
            if (string.IsNullOrWhiteSpace(sourceField))
                return null;

            return _rules.FirstOrDefault(r =>
                string.Equals(r.SourceField, sourceField, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets all mapping rules that target a specific DICOM tag
        /// </summary>
        public IEnumerable<MappingRule> GetRulesForTag(string dicomTag)
        {
            if (string.IsNullOrWhiteSpace(dicomTag))
                return Enumerable.Empty<MappingRule>();

            return _rules.Where(r =>
                string.Equals(r.DicomTag, dicomTag, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Validates all mapping rules
        /// </summary>
        public void ValidateRules()
        {
            foreach (var rule in _rules)
            {
                if (string.IsNullOrWhiteSpace(rule.SourceField))
                    throw new InvalidOperationException($"Rule '{rule.Name ?? "unnamed"}' has no source field");

                if (string.IsNullOrWhiteSpace(rule.DicomTag))
                    throw new InvalidOperationException($"Rule '{rule.Name ?? "unnamed"}' has no DICOM tag");

                // Validate DICOM tag format (basic check)
                if (!rule.DicomTag.StartsWith("(") || !rule.DicomTag.Contains(",") || !rule.DicomTag.EndsWith(")"))
                    throw new InvalidOperationException($"Rule '{rule.Name ?? "unnamed"}' has invalid DICOM tag format: {rule.DicomTag}");
            }
        }

        /// <summary>
        /// Creates default Ricoh camera barcode mappings
        /// </summary>
        public static CustomMappingConfiguration CreateRicohDefaults()
        {
            var rules = new List<MappingRule>
            {
                // Patient Information from Barcode
                new MappingRule
                {
                    Name = "PatientName",
                    SourceField = "PatientName",     // Parsed from barcode
                    DicomTag = "(0010,0010)",
                    Description = "Patient's Name",
                    ValueRepresentation = "PN",
                    Required = true,
                    Transform = "None"
                },
                new MappingRule
                {
                    Name = "PatientID",
                    SourceField = "ExamId",          // First field in barcode
                    DicomTag = "(0010,0020)",
                    Description = "Patient ID",
                    ValueRepresentation = "LO",
                    Required = true,
                    Transform = "None"
                },
                new MappingRule
                {
                    Name = "PatientBirthDate",
                    SourceField = "BirthDate",       // Third field in barcode
                    DicomTag = "(0010,0030)",
                    Description = "Patient's Birth Date",
                    ValueRepresentation = "DA",
                    Transform = "DateToDicom",       // Converts to YYYYMMDD
                    Required = false
                },
                new MappingRule
                {
                    Name = "PatientSex",
                    SourceField = "Gender",          // Fourth field in barcode
                    DicomTag = "(0010,0040)",
                    Description = "Patient's Sex",
                    ValueRepresentation = "CS",
                    Transform = "MapGender",         // F->F, M->M, etc.
                    Required = false,
                    DefaultValue = "O"
                },

                // Study Information
                new MappingRule
                {
                    Name = "StudyID",
                    SourceField = "ExamId",
                    DicomTag = "(0020,0010)",
                    Description = "Study ID",
                    ValueRepresentation = "SH",
                    Required = true
                },
                new MappingRule
                {
                    Name = "StudyDescription",
                    SourceField = "Comment",         // Fifth field in barcode
                    DicomTag = "(0008,1030)",
                    Description = "Study Description",
                    ValueRepresentation = "LO",
                    Required = false
                },

                // Date/Time from File
                new MappingRule
                {
                    Name = "SeriesDate",
                    SourceField = "CaptureDateTime", // From ImageMetadata
                    DicomTag = "(0008,0021)",
                    Description = "Series Date",
                    ValueRepresentation = "DA",
                    Transform = "ExtractDate",
                    Required = true
                },
                new MappingRule
                {
                    Name = "SeriesTime",
                    SourceField = "CaptureDateTime",
                    DicomTag = "(0008,0031)",
                    Description = "Series Time",
                    ValueRepresentation = "TM",
                    Transform = "ExtractTime",
                    Required = true
                },
                new MappingRule
                {
                    Name = "StudyDate",
                    SourceField = "CaptureDateTime",
                    DicomTag = "(0008,0020)",
                    Description = "Study Date",
                    ValueRepresentation = "DA",
                    Transform = "ExtractDate",
                    Required = true
                },
                new MappingRule
                {
                    Name = "StudyTime",
                    SourceField = "CaptureDateTime",
                    DicomTag = "(0008,0030)",
                    Description = "Study Time",
                    ValueRepresentation = "TM",
                    Transform = "ExtractTime",
                    Required = true
                }
            };

            return new CustomMappingConfiguration(rules, "Ricoh Defaults");
        }
    }
}
