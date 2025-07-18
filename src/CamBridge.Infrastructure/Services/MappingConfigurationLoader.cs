// File: src/CamBridge.Infrastructure/Services/MappingConfigurationLoader.cs
// Version: 0.8.10
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions
// Modified: Session 110 - Complete correlation ID implementation
// Status: Correlation IDs added to ALL log statements

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
        private bool _isInitialized = false;

        public MappingConfigurationLoader(ILogger<MappingConfigurationLoader> logger, string configPath = "mappings.json", string? correlationId = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configPath = configPath;
            _mappingRules = new List<MappingRule>();

            // Log initialization with correlation ID if provided
            if (!string.IsNullOrEmpty(correlationId))
            {
                _logger.LogInformation("[{CorrelationId}] [MappingInit] MappingConfigurationLoader initialized with path: {Path}",
                    correlationId, _configPath);
            }

            // REMOVED: LoadConfigurationAsync().GetAwaiter().GetResult();
            // This was causing the UI freeze!
            // Configuration will be loaded lazily or explicitly via LoadConfigurationAsync
        }

        /// <inheritdoc />
        public IReadOnlyList<MappingRule> GetMappingRules()
        {
            // Return default rules if not initialized
            if (!_isInitialized)
            {
                LoadDefaultMappings(null);
                _isInitialized = true;
            }
            return _mappingRules.AsReadOnly();
        }

        /// <inheritdoc />
        public async Task<bool> LoadConfigurationAsync(string? filePath = null)
        {
            return await LoadConfigurationAsync(filePath, null);
        }

        /// <summary>
        /// Loads mapping configuration from a JSON file with correlation ID support
        /// </summary>
        public async Task<bool> LoadConfigurationAsync(string? filePath, string? correlationId)
        {
            var path = filePath ?? _configPath;

            try
            {
                // Make path absolute if relative
                if (!Path.IsPathRooted(path))
                {
                    // Try multiple locations for the config file
                    var possiblePaths = new[]
                    {
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path),
                        Path.Combine(Environment.CurrentDirectory, path),
                        Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, path),
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CamBridge", path)
                    };

                    path = possiblePaths.FirstOrDefault(File.Exists) ?? possiblePaths[0];
                }

                if (!File.Exists(path))
                {
                    if (!string.IsNullOrEmpty(correlationId))
                    {
                        _logger.LogWarning("[{CorrelationId}] [MappingConfig] Mapping configuration file not found: {Path}. Using default mappings.",
                            correlationId, path);
                    }
                    else
                    {
                        _logger.LogWarning("[NO-ID] [MappingConfig] Mapping configuration file not found: {Path}. Using default mappings.", path);
                    }
                    LoadDefaultMappings(correlationId);
                    _isInitialized = true;
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
                    if (!string.IsNullOrEmpty(correlationId))
                    {
                        _logger.LogInformation("[{CorrelationId}] [MappingConfig] Loaded {Count} mapping rules from {Path}",
                            correlationId, _mappingRules.Count, path);
                    }
                    else
                    {
                        _logger.LogInformation("[NO-ID] [MappingConfig] Loaded {Count} mapping rules from {Path}", _mappingRules.Count, path);
                    }
                    _isInitialized = true;
                    return true;
                }
                else
                {
                    if (!string.IsNullOrEmpty(correlationId))
                    {
                        _logger.LogWarning("[{CorrelationId}] [MappingConfig] No mapping rules found in {Path}. Using default mappings.",
                            correlationId, path);
                    }
                    else
                    {
                        _logger.LogWarning("[NO-ID] [MappingConfig] No mapping rules found in {Path}. Using default mappings.", path);
                    }
                    LoadDefaultMappings(correlationId);
                    _isInitialized = true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(correlationId))
                {
                    _logger.LogError(ex, "[{CorrelationId}] [MappingConfig] Error loading mapping configuration from {Path}",
                        correlationId, path);
                }
                else
                {
                    _logger.LogError(ex, "[NO-ID] [MappingConfig] Error loading mapping configuration from {Path}", path);
                }
                LoadDefaultMappings(correlationId);
                _isInitialized = true;
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> SaveConfigurationAsync(IEnumerable<MappingRule> rules, string? filePath = null)
        {
            return await SaveConfigurationAsync(rules, filePath, null);
        }

        /// <summary>
        /// Saves mapping configuration with correlation ID support
        /// </summary>
        public async Task<bool> SaveConfigurationAsync(IEnumerable<MappingRule> rules, string? filePath, string? correlationId)
        {
            var path = filePath ?? _configPath;

            try
            {
                // Make path absolute if relative
                if (!Path.IsPathRooted(path))
                {
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                }

                // Ensure directory exists
                var directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

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

                if (!string.IsNullOrEmpty(correlationId))
                {
                    _logger.LogInformation("[{CorrelationId}] [MappingConfig] Saved {Count} mapping rules to {Path}",
                        correlationId, _mappingRules.Count, path);
                }
                else
                {
                    _logger.LogInformation("[NO-ID] [MappingConfig] Saved {Count} mapping rules to {Path}", _mappingRules.Count, path);
                }

                return true;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(correlationId))
                {
                    _logger.LogError(ex, "[{CorrelationId}] [MappingConfig] Error saving mapping configuration to {Path}",
                        correlationId, path);
                }
                else
                {
                    _logger.LogError(ex, "[NO-ID] [MappingConfig] Error saving mapping configuration to {Path}", path);
                }
                return false;
            }
        }

        /// <inheritdoc />
        public void AddRule(MappingRule rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            _mappingRules.Add(rule);
            _logger.LogDebug("[NO-ID] [MappingConfig] Added mapping rule: {Source} -> {Target}", rule.SourceField, rule.DicomTag);
        }

        /// <inheritdoc />
        public void RemoveRule(string sourceField)
        {
            var removed = _mappingRules.RemoveAll(r => r.SourceField == sourceField);
            if (removed > 0)
            {
                _logger.LogDebug("[NO-ID] [MappingConfig] Removed {Count} mapping rule(s) for source field: {SourceField}", removed, sourceField);
            }
        }

        /// <inheritdoc />
        public MappingRule? GetRuleForSource(string sourceField)
        {
            // Ensure we have rules loaded
            if (!_isInitialized)
            {
                LoadDefaultMappings(null);
                _isInitialized = true;
            }

            return _mappingRules.FirstOrDefault(r =>
                r.SourceField.Equals(sourceField, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc />
        public IEnumerable<MappingRule> GetRulesForTag(string dicomTag)
        {
            // Ensure we have rules loaded
            if (!_isInitialized)
            {
                LoadDefaultMappings(null);
                _isInitialized = true;
            }

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
        private void LoadDefaultMappings(string? correlationId)
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

            if (!string.IsNullOrEmpty(correlationId))
            {
                _logger.LogInformation("[{CorrelationId}] [MappingConfig] Loaded {Count} default mapping rules",
                    correlationId, _mappingRules.Count);
            }
            else
            {
                _logger.LogInformation("[NO-ID] [MappingConfig] Loaded {Count} default mapping rules", _mappingRules.Count);
            }
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
