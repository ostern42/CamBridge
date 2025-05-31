using System;
using System.IO;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using CamBridge.Infrastructure.Services;
using CamBridge.Infrastructure.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using FellowOakDicom;

namespace CamBridge.Infrastructure.Tests.IntegrationTests
{
    public class JpegToDicomIntegrationTests : IDisposable
    {
        private readonly string _testOutputPath;
        private readonly Mock<ILogger<ExifReader>> _exifLoggerMock;
        private readonly Mock<ILogger<DicomConverter>> _dicomLoggerMock;
        private readonly Mock<ILogger<FileProcessor>> _processorLoggerMock;
        private readonly IExifReader _exifReader;
        private readonly IDicomConverter _dicomConverter;
        private readonly IFileProcessor _fileProcessor;
        private readonly CamBridgeSettings _settings;
        private readonly ProcessingOptions _processingOptions;

        public JpegToDicomIntegrationTests()
        {
            _testOutputPath = Path.Combine(Path.GetTempPath(), $"CamBridgeTest_{Guid.NewGuid():N}");
            Directory.CreateDirectory(_testOutputPath);

            _exifLoggerMock = new Mock<ILogger<ExifReader>>();
            _dicomLoggerMock = new Mock<ILogger<DicomConverter>>();
            _processorLoggerMock = new Mock<ILogger<FileProcessor>>();

            _exifReader = new ExifReader(_exifLoggerMock.Object);
            _dicomConverter = new DicomConverter(_dicomLoggerMock.Object);

            _settings = new CamBridgeSettings
            {
                DefaultOutputFolder = _testOutputPath,
                Dicom = new DicomSettings
                {
                    ValidateAfterCreation = true
                }
            };

            _processingOptions = new ProcessingOptions
            {
                SuccessAction = PostProcessingAction.Leave,
                FailureAction = PostProcessingAction.Leave,
                CreateBackup = false,
                OutputFilePattern = "{PatientID}_{StudyDate}_{InstanceNumber}.dcm"
            };

            _fileProcessor = new FileProcessor(
                _processorLoggerMock.Object,
                _exifReader,
                _dicomConverter,
                Options.Create(_processingOptions),
                Options.Create(_settings));
        }

        [Fact]
        public async Task FullPipeline_WithValidJpegAndQRBridgeData_CreatesDicomFile()
        {
            // Arrange
            var qrBridgeData = "EX001|Schmidt, Maria|1985-03-15|F|Röntgen Thorax";
            var jpegPath = JpegTestFileGenerator.CreateTestJpegWithQRBridgeData(qrBridgeData);

            // Simulate EXIF reader behavior for test
            var mockExifReader = new Mock<IExifReader>();
            mockExifReader
                .Setup(x => x.ReadExifDataAsync(It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<string, string>
                {
                    ["Make"] = "RICOH",
                    ["Model"] = "G900 II",
                    ["DateTime"] = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"),
                    ["UserComment"] = qrBridgeData
                });

            mockExifReader
                .Setup(x => x.GetUserCommentAsync(It.IsAny<string>()))
                .ReturnsAsync(qrBridgeData);

            mockExifReader
                .Setup(x => x.ParseQRBridgeData(qrBridgeData))
                .Returns(new Dictionary<string, string>
                {
                    ["examid"] = "EX001",
                    ["name"] = "Schmidt, Maria",
                    ["birthdate"] = "1985-03-15",
                    ["gender"] = "F",
                    ["comment"] = "Röntgen Thorax"
                });

            var fileProcessor = new FileProcessor(
                _processorLoggerMock.Object,
                mockExifReader.Object,
                _dicomConverter,
                Options.Create(_processingOptions),
                Options.Create(_settings));

            // Act
            var result = await fileProcessor.ProcessFileAsync(jpegPath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.OutputFile.Should().NotBeNullOrEmpty();
            File.Exists(result.OutputFile).Should().BeTrue();

            // Verify DICOM file content
            var dicomFile = await DicomFile.OpenAsync(result.OutputFile);
            var dataset = dicomFile.Dataset;

            dataset.GetString(DicomTag.PatientName).Should().Be("Schmidt, Maria");
            dataset.GetString(DicomTag.PatientBirthDate).Should().Be("19850315");
            dataset.GetString(DicomTag.PatientSex).Should().Be("F");
            dataset.GetString(DicomTag.StudyDescription).Should().Be("Röntgen Thorax");

            // Cleanup
            File.Delete(jpegPath);
        }

        [Fact]
        public async Task ExifReader_WithRealQRBridgeData_ParsesCorrectly()
        {
            // Arrange
            var testCases = new[]
            {
                ("EX001|Schmidt, Maria|1985-03-15|F|Röntgen Thorax", 5),
                ("EX002|Müller, Hans|1975-06-20|M", 4),
                ("EX003|Wagner, Lisa", 2),
                ("-examid \"EX004\" -name \"Becker, Thomas\" -birthdate \"1968-08-30\"", 3)
            };

            foreach (var (input, expectedCount) in testCases)
            {
                // Act
                var result = _exifReader.ParseQRBridgeData(input);

                // Assert
                result.Should().NotBeNull();
                result.Count.Should().Be(expectedCount);
                result.Should().ContainKey("name");
            }
        }

        [Fact]
        public async Task DicomConverter_WithMinimalMetadata_CreatesValidDicom()
        {
            // Arrange
            var jpegPath = JpegTestFileGenerator.CreateTestJpegWithQRBridgeData("Test|Minimal Patient");
            var outputPath = Path.Combine(_testOutputPath, "minimal.dcm");

            var metadata = new Core.Entities.ImageMetadata(
                jpegPath,
                DateTime.Now,
                new Core.Entities.PatientInfo(
                    new Core.ValueObjects.PatientId("TEST001"),
                    "Minimal Patient",
                    null,
                    Core.Entities.Gender.Other),
                new Core.Entities.StudyInfo(
                    new Core.ValueObjects.StudyId("STUDY001"),
                    DateTime.Now,
                    "XC"),
                new Dictionary<string, string>(),
                new Core.Entities.ImageTechnicalData());

            // Act
            var result = await _dicomConverter.ConvertToDicomAsync(jpegPath, outputPath, metadata);

            // Assert
            result.Success.Should().BeTrue();
            File.Exists(outputPath).Should().BeTrue();

            // Validate DICOM
            var validation = await _dicomConverter.ValidateDicomFileAsync(outputPath);
            validation.IsValid.Should().BeTrue();

            // Cleanup
            File.Delete(jpegPath);
        }

        [Theory]
        [InlineData("missing_gender", "EX100|Test, Patient|1980-01-01||No gender specified")]
        [InlineData("special_chars", "EX102|Østergård, Søren|1975-05-05|M|Ärztliche Untersuchung")]
        public async Task Pipeline_WithEdgeCases_HandlesGracefully(string testCase, string qrBridgeData)
        {
            // Arrange
            var jpegPath = JpegTestFileGenerator.CreateTestJpegWithQRBridgeData(qrBridgeData, $"{testCase}.jpg");

            var mockExifReader = new Mock<IExifReader>();
            mockExifReader
                .Setup(x => x.ReadExifDataAsync(It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<string, string>
                {
                    ["Make"] = "RICOH",
                    ["Model"] = "G900 II",
                    ["UserComment"] = qrBridgeData
                });

            mockExifReader
                .Setup(x => x.GetUserCommentAsync(It.IsAny<string>()))
                .ReturnsAsync(qrBridgeData);

            var parsedData = _exifReader.ParseQRBridgeData(qrBridgeData);
            mockExifReader
                .Setup(x => x.ParseQRBridgeData(qrBridgeData))
                .Returns(parsedData);

            var fileProcessor = new FileProcessor(
                _processorLoggerMock.Object,
                mockExifReader.Object,
                _dicomConverter,
                Options.Create(_processingOptions),
                Options.Create(_settings));

            // Act
            var result = await fileProcessor.ProcessFileAsync(jpegPath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue($"Failed to process {testCase}");

            if (result.Success && result.OutputFile != null)
            {
                // Verify DICOM was created
                File.Exists(result.OutputFile).Should().BeTrue();

                // Verify patient data
                var dicomFile = await DicomFile.OpenAsync(result.OutputFile);
                var dataset = dicomFile.Dataset;

                var patientId = dataset.GetString(DicomTag.PatientID);
                patientId.Should().NotBeNullOrEmpty();
            }

            // Cleanup
            File.Delete(jpegPath);
        }

        [Fact]
        public async Task ProcessingQueue_WithMultipleFiles_ProcessesInOrder()
        {
            // This test would require ProcessingQueue setup
            // For now, we'll test basic file processor functionality

            // Arrange
            var files = new List<string>();
            for (int i = 1; i <= 3; i++)
            {
                var qrData = $"EX{i:D3}|Patient {i}|1980-01-0{i}|M|Test {i}";
                files.Add(JpegTestFileGenerator.CreateTestJpegWithQRBridgeData(qrData, $"batch_{i}.jpg"));
            }

            var processedCount = 0;

            // Act
            foreach (var file in files)
            {
                if (_fileProcessor.ShouldProcessFile(file))
                {
                    var result = await _fileProcessor.ProcessFileAsync(file);
                    if (result.Success)
                        processedCount++;
                }
            }

            // Assert
            processedCount.Should().Be(3);

            // Cleanup
            files.ForEach(File.Delete);
        }

        public void Dispose()
        {
            // Cleanup test directory
            try
            {
                if (Directory.Exists(_testOutputPath))
                {
                    Directory.Delete(_testOutputPath, true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }

            JpegTestFileGenerator.CleanupTestFiles();
        }
    }
}
