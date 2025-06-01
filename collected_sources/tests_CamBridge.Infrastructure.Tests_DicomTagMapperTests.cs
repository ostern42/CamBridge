using System;
using System.Collections.Generic;
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using CamBridge.Core.ValueObjects;
using CamBridge.Infrastructure.Services;
using FellowOakDicom;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using DicomTag = CamBridge.Core.ValueObjects.DicomTag;

namespace CamBridge.Infrastructure.Tests
{
    public class DicomTagMapperTests
    {
        private readonly DicomTagMapper _mapper;
        private readonly Mock<ILogger<DicomTagMapper>> _loggerMock;
        private readonly Mock<IMappingConfiguration> _mappingConfigMock;

        public DicomTagMapperTests()
        {
            _loggerMock = new Mock<ILogger<DicomTagMapper>>();
            _mappingConfigMock = new Mock<IMappingConfiguration>();
            _mapper = new DicomTagMapper(_loggerMock.Object, _mappingConfigMock.Object);
        }

        [Fact]
        public void MapToDataset_WithValidData_MapsCorrectly()
        {
            // Arrange
            var rules = new List<MappingRule>
            {
                new MappingRule("PatientName", "QRBridge", "name", DicomTag.PatientModule.PatientName),
                new MappingRule("PatientID", "QRBridge", "patientid", DicomTag.PatientModule.PatientID),
                new MappingRule("Manufacturer", "EXIF", "Make", DicomTag.EquipmentModule.Manufacturer)
            };
            _mappingConfigMock.Setup(m => m.GetMappingRules()).Returns(rules);

            var sourceData = new Dictionary<string, Dictionary<string, string>>
            {
                ["QRBridge"] = new()
                {
                    ["name"] = "Schmidt, Maria",
                    ["patientid"] = "PAT001"
                },
                ["EXIF"] = new()
                {
                    ["Make"] = "RICOH"
                }
            };

            var dataset = new DicomDataset();

            // Act
            var result = _mapper.MapToDataset(sourceData, dataset);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.SuccessfulMappings);
            Assert.Equal(0, result.SkippedMappings);
            Assert.Empty(result.Errors);

            // Verify dataset values
            Assert.Equal("Schmidt, Maria", dataset.GetString(FellowOakDicom.DicomTag.PatientName));
            Assert.Equal("PAT001", dataset.GetString(FellowOakDicom.DicomTag.PatientID));
            Assert.Equal("RICOH", dataset.GetString(FellowOakDicom.DicomTag.Manufacturer));
        }

        [Fact]
        public void MapToDataset_WithMissingSourceType_SkipsMapping()
        {
            // Arrange
            var rules = new List<MappingRule>
            {
                new MappingRule("PatientName", "QRBridge", "name", DicomTag.PatientModule.PatientName),
                new MappingRule("StudyID", "StudyData", "id", DicomTag.StudyModule.StudyID) // StudyData not in source
            };
            _mappingConfigMock.Setup(m => m.GetMappingRules()).Returns(rules);

            var sourceData = new Dictionary<string, Dictionary<string, string>>
            {
                ["QRBridge"] = new()
                {
                    ["name"] = "Test Patient"
                }
            };

            var dataset = new DicomDataset();

            // Act
            var result = _mapper.MapToDataset(sourceData, dataset);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.SuccessfulMappings);
            Assert.Equal(1, result.SkippedMappings);
        }

        [Fact]
        public void MapToDataset_WithMissingRequiredField_AddsError()
        {
            // Arrange
            var rules = new List<MappingRule>
            {
                new MappingRule("PatientName", "QRBridge", "name", DicomTag.PatientModule.PatientName,
                    ValueTransform.None, isRequired: true)
            };
            _mappingConfigMock.Setup(m => m.GetMappingRules()).Returns(rules);

            var sourceData = new Dictionary<string, Dictionary<string, string>>
            {
                ["QRBridge"] = new() // Missing "name" field
            };

            var dataset = new DicomDataset();

            // Act
            var result = _mapper.MapToDataset(sourceData, dataset);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(result.Errors, e => e.Contains("Required field 'PatientName'"));
        }

        [Fact]
        public void MapToDataset_WithDefaultValue_UsesDefaultWhenMissing()
        {
            // Arrange
            var rules = new List<MappingRule>
            {
                new MappingRule("PatientSex", "QRBridge", "gender", DicomTag.PatientModule.PatientSex,
                    ValueTransform.None, isRequired: false, defaultValue: "O")
            };
            _mappingConfigMock.Setup(m => m.GetMappingRules()).Returns(rules);

            var sourceData = new Dictionary<string, Dictionary<string, string>>
            {
                ["QRBridge"] = new() // Missing "gender" field
            };

            var dataset = new DicomDataset();

            // Act
            var result = _mapper.MapToDataset(sourceData, dataset);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.SuccessfulMappings);
            Assert.Equal("O", dataset.GetString(FellowOakDicom.DicomTag.PatientSex));
        }

        [Fact]
        public void MapToDataset_WithTransformation_AppliesTransform()
        {
            // Arrange
            var rules = new List<MappingRule>
            {
                new MappingRule("PatientBirthDate", "QRBridge", "birthdate",
                    DicomTag.PatientModule.PatientBirthDate, ValueTransform.DateToDicom),
                new MappingRule("PatientSex", "QRBridge", "gender",
                    DicomTag.PatientModule.PatientSex, ValueTransform.GenderToDicom)
            };
            _mappingConfigMock.Setup(m => m.GetMappingRules()).Returns(rules);

            var sourceData = new Dictionary<string, Dictionary<string, string>>
            {
                ["QRBridge"] = new()
                {
                    ["birthdate"] = "1985-03-15",
                    ["gender"] = "Female"
                }
            };

            var dataset = new DicomDataset();

            // Act
            var result = _mapper.MapToDataset(sourceData, dataset);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("19850315", dataset.GetString(FellowOakDicom.DicomTag.PatientBirthDate));
            Assert.Equal("F", dataset.GetString(FellowOakDicom.DicomTag.PatientSex));
        }

        [Fact]
        public void MapQRBridgeData_MapsOnlyQRBridgeRules()
        {
            // Arrange
            var rules = new List<MappingRule>
            {
                new MappingRule("PatientName", "QRBridge", "name", DicomTag.PatientModule.PatientName),
                new MappingRule("Manufacturer", "EXIF", "Make", DicomTag.EquipmentModule.Manufacturer)
            };
            _mappingConfigMock.Setup(m => m.GetMappingRules()).Returns(rules);

            var qrBridgeData = new Dictionary<string, string>
            {
                ["name"] = "Test Patient"
            };

            var dataset = new DicomDataset();

            // Act
            var result = _mapper.MapQRBridgeData(qrBridgeData, dataset);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.SuccessfulMappings);
            Assert.Equal(1, result.SkippedMappings); // EXIF rule skipped
            Assert.Equal("Test Patient", dataset.GetString(FellowOakDicom.DicomTag.PatientName));
        }

        [Fact]
        public void MapExifData_MapsOnlyExifRules()
        {
            // Arrange
            var rules = new List<MappingRule>
            {
                new MappingRule("PatientName", "QRBridge", "name", DicomTag.PatientModule.PatientName),
                new MappingRule("Manufacturer", "EXIF", "Make", DicomTag.EquipmentModule.Manufacturer)
            };
            _mappingConfigMock.Setup(m => m.GetMappingRules()).Returns(rules);

            var exifData = new Dictionary<string, string>
            {
                ["Make"] = "RICOH"
            };

            var dataset = new DicomDataset();

            // Act
            var result = _mapper.MapExifData(exifData, dataset);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.SuccessfulMappings);
            Assert.Equal(1, result.SkippedMappings); // QRBridge rule skipped
            Assert.Equal("RICOH", dataset.GetString(FellowOakDicom.DicomTag.Manufacturer));
        }

        [Fact]
        public void MappingResult_ToString_FormatsCorrectly()
        {
            // Arrange
            var result = new MappingResult
            {
                SuccessfulMappings = 5,
                SkippedMappings = 2
            };

            // Act & Assert
            Assert.Equal("Mapping successful: 5 mapped, 2 skipped", result.ToString());

            // Add error
            result.AddError("Test error");
            Assert.Equal("Mapping completed with errors: 5 mapped, 2 skipped, 1 errors", result.ToString());
        }

        [Fact]
        public void MappingResult_AppliedRules_TracksCorrectly()
        {
            // Arrange
            var result = new MappingResult();

            // Act
            result.AddAppliedRule("PatientName", "Schmidt, Maria", "Schmidt, Maria", "(0010,0010)");
            result.AddAppliedRule("PatientBirthDate", "1985-03-15", "19850315", "(0010,0030)");

            // Assert
            Assert.Equal(2, result.AppliedRules.Count);

            var nameRule = result.AppliedRules[0];
            Assert.Equal("PatientName", nameRule.RuleName);
            Assert.Equal("Schmidt, Maria", nameRule.SourceValue);
            Assert.Equal("Schmidt, Maria", nameRule.MappedValue);

            var dateRule = result.AppliedRules[1];
            Assert.Equal("19850315", dateRule.MappedValue); // Transformed value
        }

        [Fact]
        public void MapToDataset_WithNullSourceData_ThrowsException()
        {
            // Arrange
            var dataset = new DicomDataset();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _mapper.MapToDataset(null!, dataset));
        }

        [Fact]
        public void MapToDataset_WithNullDataset_ThrowsException()
        {
            // Arrange
            var sourceData = new Dictionary<string, Dictionary<string, string>>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _mapper.MapToDataset(sourceData, null!));
        }

        [Fact]
        public void MapToDataset_WithExceptionDuringMapping_ContinuesAndReportsError()
        {
            // Arrange
            var rules = new List<MappingRule>
            {
                new MappingRule("BadRule", "QRBridge", "field1", DicomTag.PatientModule.PatientName),
                new MappingRule("GoodRule", "QRBridge", "field2", DicomTag.PatientModule.PatientID)
            };
            _mappingConfigMock.Setup(m => m.GetMappingRules()).Returns(rules);

            var sourceData = new Dictionary<string, Dictionary<string, string>>
            {
                ["QRBridge"] = new()
                {
                    ["field1"] = null!, // This might cause issues
                    ["field2"] = "ID001"
                }
            };

            var dataset = new DicomDataset();

            // Act
            var result = _mapper.MapToDataset(sourceData, dataset);

            // Assert
            // Should continue processing after error
            Assert.Equal("ID001", dataset.GetString(FellowOakDicom.DicomTag.PatientID));
        }
    }
}
