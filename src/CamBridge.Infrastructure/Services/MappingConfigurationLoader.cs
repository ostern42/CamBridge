using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Loads and manages DICOM mapping configurations from JSON files
    /// </summary>
    public class MappingConfigurationLoader : IMappingConfiguration
    {
        private readonly ILogger<MappingConfigurationLoader> _logger;
        private readonly string _configPath;
        private List<MappingRule> _mappingRules;

        public MappingConfigurationLoader(ILogger<MappingConfigurationLoader> logger, string configPath = "mappings.json")
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configPath = configPath;
            _mappingRules = new List<MappingRule>();

            // Try to load default configuration
            LoadConfigurationAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public IReadOnlyList<MappingRule> GetMappingRules()
        {
            return _mappingRules.AsReadOnly();
        }

        /// <inheritdoc />
        public async Task<bool> LoadConfigurationAsync(string? filePath = null)
        {
            var path = filePath ?? _configPath;

            try
            {
                if (!File.Exists(path))
                {
                    _logger.LogWarning("Mapping configuration file not found: {Path}. Using default mappings.", path);
                    LoadDefaultMappings();
                    return false;
                }

                var json = await File.ReadAllTextAsync(path);
                var config = JsonSerializer.Deserialize<MappingConfiguration>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (config?.Rules != null && config.Rules.Count > 0)
                {
                    _mappingRules = config.Rules;
                    _logger.LogInformation("Loaded {Count} mapping rules from {Path}", _mappingRules.Count, path);
                    return true;
                }
                else
                {
                    _logger.LogWarning("No mapping rules found in {Path}. Using default mappings.", path);
                    LoadDefaultMappings();
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading mapping configuration from {Path}", path);
                LoadDefaultMappings();
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> SaveConfigurationAsync(IEnumerable<MappingRule> rules, string? filePath = null)
        {
            var path = filePath ?? _configPath;

            try
            {
                var config = new MappingConfiguration
                {
                    Version = "1.0",
                    Rules = rules.ToList()
                };

                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true
                });

                await File.WriteAllTextAsync(path, json);

                _mappingRules = config.Rules;
                _logger.LogInformation("Saved {Count} mapping rules to {Path}", _mappingRules.Count, path);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving mapping configuration to {Path}", path);
                return false;
            }
        }

        /// <inheritdoc />
        public void AddRule(MappingRule rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            _mappingRules.Add(rule);
            _logger.LogDebug("Added mapping rule: {Source} -> {Target}", rule.SourceField, rule.DicomTag);
        }

        /// <inheritdoc />
        public void RemoveRule(string sourceField)
        {
            var removed = _mappingRules.RemoveAll(r => r.SourceField == sourceField);
            if (removed > 0)
            {
                _logger.LogDebug("Removed {Count} mapping rule(s) for source field: {SourceField}", removed, sourceField);
            }
        }

        /// <inheritdoc />
        public MappingRule? GetRuleForSource(string sourceField)
        {
            return _mappingRules.FirstOrDefault(r =>
                r.SourceField.Equals(sourceField, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc />
        public IEnumerable<MappingRule> GetRulesForTag(string dicomTag)
        {
            return _mappingRules.Where(r =>
                r.DicomTag.Equals(dicomTag, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc />
        public void ValidateRules()
        {
            foreach (var rule in _mappingRules)
            {
                if (string.IsNullOrWhiteSpace(rule.SourceField))
                    throw new InvalidOperationException($"Invalid rule: SourceField is empty");

                if (string.IsNullOrWhiteSpace(rule.DicomTag))
                    throw new InvalidOperationException($"Invalid rule: DicomTag is empty for source {rule.SourceField}");

                // Additional validation can be added here
            }
        }

        /// <summary>
        /// Loads default mapping rules for Ricoh G900 II
        /// </summary>
        private void LoadDefaultMappings()
        {
            _mappingRules = new List<MappingRule>
            {
                // Patient Information
                new MappingRule
                {
                    SourceField = "name",
                    DicomTag = "(0010,0010)",
                    Description = "Patient's Name",
                    ValueRepresentation = "PN",
                    Required = true
                },
                new MappingRule
                {
                    SourceField = "examid",
                    DicomTag = "(0010,0020)",
                    Description = "Patient ID",
                    ValueRepresentation = "LO",
                    Required = true
                },
                new MappingRule
                {
                    SourceField = "birthdate",
                    DicomTag = "(0010,0030)",
                    Description = "Patient's Birth Date",
                    ValueRepresentation = "DA",
                    Transform = "DateToDicom",
                    Required = false
                },
                new MappingRule
                {
                    SourceField = "gender",
                    DicomTag = "(0010,0040)",
                    Description = "Patient's Sex",
                    ValueRepresentation = "CS",
                    Transform = "MapGender",
                    Required = false
                },
                
                // Study Information
                new MappingRule
                {
                    SourceField = "examid",
                    DicomTag = "(0020,0010)",
                    Description = "Study ID",
                    ValueRepresentation = "SH",
                    Required = true
                },
                new MappingRule
                {
                    SourceField = "comment",
                    DicomTag = "(0008,1030)",
                    Description = "Study Description",
                    ValueRepresentation = "LO",
                    Required = false
                },
                
                // Series Information
                new MappingRule
                {
                    SourceField = "_datetime",
                    DicomTag = "(0008,0021)",
                    Description = "Series Date",
                    ValueRepresentation = "DA",
                    Transform = "ExtractDate",
                    Required = true
                },
                new MappingRule
                {
                    SourceField = "_datetime",
                    DicomTag = "(0008,0031)",
                    Description = "Series Time",
                    ValueRepresentation = "TM",
                    Transform = "ExtractTime",
                    Required = true
                }
            };

            _logger.LogInformation("Loaded {Count} default mapping rules", _mappingRules.Count);
        }

        /// <summary>
        /// Internal configuration class for JSON serialization
        /// </summary>
        private class MappingConfiguration
        {
            public string Version { get; set; } = "1.0";
            public List<MappingRule> Rules { get; set; } = new();
        }
    }
}
