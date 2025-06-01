using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using CamBridge.Core.ValueObjects;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CamBridge.Infrastructure.Tests
{
    public class MappingConfigurationTests : IDisposable
    {
        private readonly MappingConfigurationLoader _loader;
        private readonly Mock<ILogger<MappingConfigurationLoader>> _loggerMock;
        private readonly string _testDirectory;

        public MappingConfigurationTests()
        {
            _loggerMock = new Mock<ILogger<MappingConfigurationLoader>>();
            _loader = new MappingConfigurationLoader(_loggerMock.Object);

            // Create temp directory for test files
            _testDirectory = Path.Combine(Path.GetTempPath(), "CamBridgeTests_" + Guid.NewGuid());
            Directory.CreateDirectory(_testDirectory);
        }

        public void Dispose()
        {
            // Clean up test directory
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Fact]
        public async Task LoadFromFile_WithValidJson_LoadsMappingsCorrectly()
        {
            // Arrange
            var configPath = Path.Combine(_testDirectory, "mappings.json");
            var json = @"{
                ""version"": ""1.0"",
                ""description"": ""Test configuration"",
                ""mappings"": [
                    {
                        ""name"": ""PatientName"",
                        ""sourceType"": ""QRBridge"",
                        ""sourceField"": ""name"",
                        ""targetTag"": ""(0010,0010)"",
                        ""transform"": ""None"",
                        ""required"": true
                    },
                    {
                        ""name"": ""PatientBirthDate"",
                        ""sourceType"": ""QRBridge"",
                        ""sourceField"": ""birthdate"",
                        ""targetTag"": ""(0010,0030)"",
                        ""transform"": ""DateToDicom"",
                        ""defaultValue"": ""19000101""
                    }
                ]
            }";
            await File.WriteAllTextAsync(configPath, json);

            // Act
            var config = await _loader.LoadFromFileAsync(configPath);

            // Assert
            var rules = config.GetMappingRules();
            Assert.Equal(2, rules.Count);

            var nameRule = rules[0];
            Assert.Equal("PatientName", nameRule.Name);
            Assert.Equal("QRBridge", nameRule.SourceType);
            Assert.Equal("name", nameRule.SourceField);
            Assert.Equal(DicomTag.PatientModule.PatientName, nameRule.TargetTag);
            Assert.Equal(ValueTransform.None, nameRule.Transform);
            Assert.True(nameRule.IsRequired);

            var birthDateRule = rules[1];
            Assert.Equal("PatientBirthDate", birthDateRule.Name);
            Assert.Equal(ValueTransform.DateToDicom, birthDateRule.Transform);
            Assert.Equal("19000101", birthDateRule.DefaultValue);
        }

        [Fact]
        public async Task LoadFromFile_WithMissingFile_ReturnsDefaultConfiguration()
        {
            // Arrange
            var configPath = Path.Combine(_testDirectory, "nonexistent.json");

            // Act
            var config = await _loader.LoadFromFileAsync(configPath);

            // Assert
            Assert.NotNull(config);
            var rules = config.GetMappingRules();
            Assert.NotEmpty(rules); // Should have default rules

            // Verify it's the default configuration
            Assert.Contains(rules, r => r.Name == "PatientName");
            Assert.Contains(rules, r => r.Name == "PatientID");
        }

        [Fact]
        public async Task SaveToFile_WithValidConfiguration_SavesCorrectly()
        {
            // Arrange
            var config = new CustomMappingConfiguration();
            config.AddRule(new MappingRule(
                "TestRule",
                "EXIF",
                "Make",
                DicomTag.EquipmentModule.Manufacturer,
                ValueTransform.ToUpper,
                false,
                "Unknown"
            ));

            var configPath = Path.Combine(_testDirectory, "saved-mappings.json");

            // Act
            await _loader.SaveToFileAsync(config, configPath);

            // Assert
            Assert.True(File.Exists(configPath));
            var json = await File.ReadAllTextAsync(configPath);
            Assert.Contains("TestRule", json);
            Assert.Contains("EXIF", json);
            Assert.Contains("(0008,0070)", json); // Manufacturer tag
            Assert.Contains("\"toUpper\"", json); // JSON uses camelCase with quotes
        }

        [Fact]
        public async Task CreateSampleConfiguration_CreatesValidSampleFile()
        {
            // Arrange
            var samplePath = Path.Combine(_testDirectory, "sample-mappings.json");

            // Act
            await _loader.CreateSampleConfigurationAsync(samplePath);

            // Assert
            Assert.True(File.Exists(samplePath));

            // Load and verify
            var config = await _loader.LoadFromFileAsync(samplePath);
            var rules = config.GetMappingRules();

            // Should have all essential mappings
            Assert.Contains(rules, r => r.Name == "PatientName");
            Assert.Contains(rules, r => r.Name == "PatientID");
            Assert.Contains(rules, r => r.Name == "PatientBirthDate");
            Assert.Contains(rules, r => r.Name == "PatientSex");
            Assert.Contains(rules, r => r.Name == "StudyDescription");
            Assert.Contains(rules, r => r.Name == "StudyID");
            Assert.Contains(rules, r => r.Name == "Manufacturer");
        }

        [Fact]
        public void CustomMappingConfiguration_AddRule_AddsRuleCorrectly()
        {
            // Arrange
            var config = new CustomMappingConfiguration();
            var rule = new MappingRule(
                "TestRule",
                "QRBridge",
                "test",
                DicomTag.PatientModule.PatientName,
                ValueTransform.None
            );

            // Act
            config.AddRule(rule);

            // Assert
            var rules = config.GetMappingRules();
            Assert.Single(rules);
            Assert.Equal("TestRule", rules[0].Name);
        }

        [Fact]
        public void CustomMappingConfiguration_RemoveRule_RemovesRuleCorrectly()
        {
            // Arrange
            var config = new CustomMappingConfiguration();
            config.AddRule(new MappingRule("Rule1", "EXIF", "field1", DicomTag.PatientModule.PatientName));
            config.AddRule(new MappingRule("Rule2", "EXIF", "field2", DicomTag.PatientModule.PatientID));

            // Act
            var removed = config.RemoveRule("Rule1");

            // Assert
            Assert.True(removed);
            var rules = config.GetMappingRules();
            Assert.Single(rules);
            Assert.Equal("Rule2", rules[0].Name);
        }

        [Fact]
        public void CustomMappingConfiguration_GetRulesForSource_FiltersCorrectly()
        {
            // Arrange
            var config = new CustomMappingConfiguration();
            config.AddRule(new MappingRule("Rule1", "QRBridge", "field1", DicomTag.PatientModule.PatientName));
            config.AddRule(new MappingRule("Rule2", "EXIF", "field2", DicomTag.PatientModule.PatientID));
            config.AddRule(new MappingRule("Rule3", "QRBridge", "field3", DicomTag.StudyModule.StudyID));

            // Act
            var qrBridgeRules = config.GetRulesForSource("QRBridge");

            // Assert
            Assert.Equal(2, qrBridgeRules.Count);
            Assert.All(qrBridgeRules, r => Assert.Equal("QRBridge", r.SourceType));
        }

        [Fact]
        public void CustomMappingConfiguration_Validate_DetectsDuplicateRuleNames()
        {
            // Arrange
            var config = new CustomMappingConfiguration();
            config.AddRule(new MappingRule("DuplicateName", "EXIF", "field1", DicomTag.PatientModule.PatientName));
            config.AddRule(new MappingRule("DuplicateName", "EXIF", "field2", DicomTag.PatientModule.PatientID));

            // Act
            var result = config.Validate();

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("Duplicate rule name"));
        }

        [Fact]
        public void CustomMappingConfiguration_Validate_DetectsMissingRequiredMappings()
        {
            // Arrange
            var config = new CustomMappingConfiguration();
            // Add some rule but not patient name/ID

            config.AddRule(new MappingRule("SomeRule", "EXIF", "field", DicomTag.StudyModule.StudyID));

            // Act
            var result = config.Validate();

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("Patient Name"));
            Assert.Contains(result.Errors, e => e.Contains("Patient ID"));
        }

        [Fact]
        public void MappingRule_ApplyTransform_DateToDicom_TransformsCorrectly()
        {
            // Arrange
            var rule = new MappingRule(
                "DateRule",
                "QRBridge",
                "date",
                DicomTag.PatientModule.PatientBirthDate,
                ValueTransform.DateToDicom
            );

            // Act
            var result1 = rule.ApplyTransform("1985-03-15");
            var result2 = rule.ApplyTransform("15.03.1985");
            var result3 = rule.ApplyTransform("March 15, 1985");

            // Assert
            Assert.NotNull(result1);
            Assert.Equal("19850315", result1);
            Assert.NotNull(result2);
            Assert.Equal("19850315", result2);
            Assert.NotNull(result3);
            Assert.Equal("19850315", result3);
        }

        [Fact]
        public void MappingRule_ApplyTransform_GenderToDicom_TransformsCorrectly()
        {
            // Arrange
            var rule = new MappingRule(
                "GenderRule",
                "QRBridge",
                "gender",
                DicomTag.PatientModule.PatientSex,
                ValueTransform.GenderToDicom
            );

            // Act & Assert
            Assert.Equal("M", rule.ApplyTransform("M"));
            Assert.Equal("M", rule.ApplyTransform("Male"));
            Assert.Equal("M", rule.ApplyTransform("MALE"));
            Assert.Equal("M", rule.ApplyTransform("männlich"));
            Assert.Equal("F", rule.ApplyTransform("F"));
            Assert.Equal("F", rule.ApplyTransform("Female"));
            Assert.Equal("F", rule.ApplyTransform("weiblich"));
            Assert.Equal("O", rule.ApplyTransform("Other"));
            Assert.Equal("O", rule.ApplyTransform("divers"));
            Assert.Equal("O", rule.ApplyTransform("unknown"));
        }

        [Fact]
        public void MappingRule_ApplyTransform_TruncateTo16_TruncatesLongStrings()
        {
            // Arrange
            var rule = new MappingRule(
                "TruncateRule",
                "QRBridge",
                "field",
                DicomTag.StudyModule.StudyID,
                ValueTransform.TruncateTo16
            );

            // Act
            var shortResult = rule.ApplyTransform("SHORT");
            var longResult = rule.ApplyTransform("THIS_IS_A_VERY_LONG_STUDY_ID_THAT_EXCEEDS_16_CHARS");

            // Assert
            Assert.NotNull(shortResult);
            Assert.Equal("SHORT", shortResult);
            Assert.NotNull(longResult);
            Assert.Equal("THIS_IS_A_VERY_L", longResult);
            Assert.Equal(16, longResult!.Length);
        }

        [Fact]
        public void MappingRule_ApplyTransform_WithDefaultValue_ReturnsDefaultWhenEmpty()
        {
            // Arrange
            var rule = new MappingRule(
                "DefaultRule",
                "QRBridge",
                "field",
                DicomTag.PatientModule.PatientSex,
                ValueTransform.None,
                false,
                "O" // Default value
            );

            // Act
            var result1 = rule.ApplyTransform(null);
            var result2 = rule.ApplyTransform("");
            var result3 = rule.ApplyTransform("M");

            // Assert
            Assert.Equal("O", result1);
            Assert.Equal("O", result2);
            Assert.Equal("M", result3);
        }

        [Fact]
        public async Task LoadFromFile_WithInvalidJson_ThrowsException()
        {
            // Arrange
            var configPath = Path.Combine(_testDirectory, "invalid.json");
            await File.WriteAllTextAsync(configPath, "{ invalid json ]");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _loader.LoadFromFileAsync(configPath)
            );
        }

        [Theory]
        [InlineData("QRBridge")]
        [InlineData("qrbridge")]
        [InlineData("QRBRIDGE")]
        public void GetRulesForSource_IsCaseInsensitive(string sourceType)
        {
            // Arrange
            var config = new CustomMappingConfiguration();
            config.AddRule(new MappingRule("Rule1", "QRBridge", "field", DicomTag.PatientModule.PatientName));

            // Act
            var rules = config.GetRulesForSource(sourceType);

            // Assert
            Assert.Single(rules);
        }
    }
}
