using CamBridge.Core;
using CamBridge.Core.Interfaces;
using CamBridge.Core.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Service for loading and saving mapping configurations from/to JSON files
    /// </summary>
    public class MappingConfigurationLoader
    {
        private readonly ILogger<MappingConfigurationLoader> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public MappingConfigurationLoader(ILogger<MappingConfigurationLoader> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                    new DicomTagJsonConverter()
                }
            };
        }

        /// <summary>
        /// Loads mapping configuration from a JSON file
        /// </summary>
        public async Task<IMappingConfiguration> LoadFromFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            try
            {
                _logger.LogInformation("Loading mapping configuration from {FilePath}", filePath);

                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("Configuration file not found at {FilePath}, using default configuration", filePath);
                    return IMappingConfiguration.GetDefault();
                }

                var json = await File.ReadAllTextAsync(filePath);
                var config = JsonSerializer.Deserialize<MappingConfigurationDto>(json, _jsonOptions);

                if (config?.Mappings == null || config.Mappings.Count == 0)
                {
                    _logger.LogWarning("No mappings found in configuration file, using default configuration");
                    return IMappingConfiguration.GetDefault();
                }

                var mappingConfig = new CustomMappingConfiguration();
                foreach (var mapping in config.Mappings)
                {
                    var rule = ConvertToMappingRule(mapping);
                    mappingConfig.AddRule(rule);
                }

                _logger.LogInformation("Successfully loaded {Count} mapping rules from {FilePath}",
                    config.Mappings.Count, filePath);

                return mappingConfig;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Invalid JSON in configuration file {FilePath}", filePath);
                throw new InvalidOperationException($"Failed to parse mapping configuration from {filePath}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading mapping configuration from {FilePath}", filePath);
                throw;
            }
        }

        /// <summary>
        /// Saves mapping configuration to a JSON file
        /// </summary>
        public async Task SaveToFileAsync(IMappingConfiguration configuration, string filePath)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            try
            {
                _logger.LogInformation("Saving mapping configuration to {FilePath}", filePath);

                var rules = configuration.GetMappingRules();
                var dto = new MappingConfigurationDto
                {
                    Version = "1.0",
                    Description = "CamBridge EXIF to DICOM mapping configuration",
                    Mappings = rules.Select(ConvertToDto).ToList()
                };

                var json = JsonSerializer.Serialize(dto, _jsonOptions);

                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(filePath, json);

                _logger.LogInformation("Successfully saved {Count} mapping rules to {FilePath}",
                    rules.Count, filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving mapping configuration to {FilePath}", filePath);
                throw;
            }
        }

        /// <summary>
        /// Creates a sample configuration file with common mappings
        /// </summary>
        public async Task CreateSampleConfigurationAsync(string filePath)
        {
            _logger.LogInformation("Creating sample mapping configuration at {FilePath}", filePath);

            var sampleConfig = new MappingConfigurationDto
            {
                Version = "1.0",
                Description = "Sample CamBridge mapping configuration for Ricoh cameras with QRBridge",
                Mappings = new List<MappingRuleDto>
                {
                    // Patient mappings
                    new() { Name = "PatientName", SourceType = "QRBridge", SourceField = "name",
                           TargetTag = "(0010,0010)", Transform = "None", Required = true },
                    new() { Name = "PatientID", SourceType = "QRBridge", SourceField = "patientid",
                           TargetTag = "(0010,0020)", Transform = "None", Required = true },
                    new() { Name = "PatientBirthDate", SourceType = "QRBridge", SourceField = "birthdate",
                           TargetTag = "(0010,0030)", Transform = "DateToDicom" },
                    new() { Name = "PatientSex", SourceType = "QRBridge", SourceField = "gender",
                           TargetTag = "(0010,0040)", Transform = "GenderToDicom" },

                    // Study mappings
                    new() { Name = "StudyDescription", SourceType = "QRBridge", SourceField = "comment",
                           TargetTag = "(0008,1030)", Transform = "None" },
                    new() { Name = "StudyID", SourceType = "QRBridge", SourceField = "examid",
                           TargetTag = "(0020,0010)", Transform = "TruncateTo16" },

                    // Equipment mappings from EXIF
                    new() { Name = "Manufacturer", SourceType = "EXIF", SourceField = "Make",
                           TargetTag = "(0008,0070)", Transform = "None" },
                    new() { Name = "Model", SourceType = "EXIF", SourceField = "Model",
                           TargetTag = "(0008,1090)", Transform = "None" },
                    new() { Name = "Software", SourceType = "EXIF", SourceField = "Software",
                           TargetTag = "(0018,1020)", Transform = "None" },

                    // Additional patient identifiers (German specific)
                    new() { Name = "PatientInsuranceNumber", SourceType = "QRBridge", SourceField = "versichertennummer",
                           TargetTag = "(0010,1000)", Transform = "None" },
                    new() { Name = "PatientCase", SourceType = "QRBridge", SourceField = "fallnummer",
                           TargetTag = "(0010,1001)", Transform = "None" }
                }
            };

            var json = JsonSerializer.Serialize(sampleConfig, _jsonOptions);

            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(filePath, json);
            _logger.LogInformation("Sample configuration created successfully");
        }

        private MappingRule ConvertToMappingRule(MappingRuleDto dto)
        {
            var targetTag = DicomTag.Parse(dto.TargetTag);
            var transform = Enum.Parse<ValueTransform>(dto.Transform ?? "None", ignoreCase: true);

            return new MappingRule(
                dto.Name,
                dto.SourceType,
                dto.SourceField,
                targetTag,
                transform,
                dto.Required ?? false,
                dto.DefaultValue
            );
        }

        private MappingRuleDto ConvertToDto(MappingRule rule)
        {
            return new MappingRuleDto
            {
                Name = rule.Name,
                SourceType = rule.SourceType,
                SourceField = rule.SourceField,
                TargetTag = rule.TargetTag.ToString(),
                Transform = rule.Transform.ToString(),
                Required = rule.IsRequired,
                DefaultValue = rule.DefaultValue
            };
        }

        // DTOs for JSON serialization
        private class MappingConfigurationDto
        {
            public string Version { get; set; } = "1.0";
            public string? Description { get; set; }
            public List<MappingRuleDto> Mappings { get; set; } = new();
        }

        private class MappingRuleDto
        {
            public string Name { get; set; } = string.Empty;
            public string SourceType { get; set; } = string.Empty;
            public string SourceField { get; set; } = string.Empty;
            public string TargetTag { get; set; } = string.Empty;
            public string? Transform { get; set; }
            public bool? Required { get; set; }
            public string? DefaultValue { get; set; }
        }

        /// <summary>
        /// Custom JSON converter for DicomTag serialization
        /// </summary>
        private class DicomTagJsonConverter : JsonConverter<DicomTag>
        {
            public override DicomTag Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var tagString = reader.GetString();
                return DicomTag.Parse(tagString!);
            }

            public override void Write(Utf8JsonWriter writer, DicomTag value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }

    /// <summary>
    /// Custom implementation of IMappingConfiguration that can be modified at runtime
    /// </summary>
    public class CustomMappingConfiguration : IMappingConfiguration
    {
        private readonly List<MappingRule> _rules = new();

        public IReadOnlyList<MappingRule> GetMappingRules() => _rules.AsReadOnly();

        public IReadOnlyList<MappingRule> GetRulesForSource(string sourceType)
            => _rules.Where(r => r.SourceType.Equals(sourceType, StringComparison.OrdinalIgnoreCase))
                     .ToList()
                     .AsReadOnly();

        public void AddRule(MappingRule rule)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            _rules.Add(rule);
        }

        public bool RemoveRule(string ruleName)
            => _rules.RemoveAll(r => r.Name.Equals(ruleName, StringComparison.OrdinalIgnoreCase)) > 0;

        /// <summary>
        /// Validates the configuration for consistency and completeness
        /// </summary>
        public ValidationResult Validate()
        {
            var errors = new List<string>();

            // Check for duplicate rule names
            var duplicateNames = _rules
                .GroupBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (var name in duplicateNames)
            {
                errors.Add($"Duplicate rule name: {name}");
            }

            // Check for duplicate target tags from same source
            var duplicateTags = _rules
                .GroupBy(r => new { r.SourceType, r.SourceField, r.TargetTag })
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (var dup in duplicateTags)
            {
                errors.Add($"Duplicate mapping: {dup.SourceType}.{dup.SourceField} -> {dup.TargetTag}");
            }

            // Check required patient identifiers
            var hasPatientName = _rules.Any(r => r.TargetTag.Equals(DicomTag.PatientModule.PatientName));
            var hasPatientId = _rules.Any(r => r.TargetTag.Equals(DicomTag.PatientModule.PatientID));

            if (!hasPatientName)
                errors.Add("Missing required mapping for Patient Name (0010,0010)");
            if (!hasPatientId)
                errors.Add("Missing required mapping for Patient ID (0010,0020)");

            return errors.Count == 0
                ? new ValidationResult { IsValid = true }
                : new ValidationResult { IsValid = false, Errors = errors };
        }

        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public List<string> Errors { get; set; } = new();
        }
    }
}
