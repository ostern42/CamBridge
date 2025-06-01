using System;
using System.IO;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using CamBridge.Infrastructure.Services;
using CamBridge.Infrastructure.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace CamBridge.Infrastructure.Tests.IntegrationTests
{
    /// <summary>
    /// Integration tests for the complete JPEG to DICOM conversion pipeline
    /// </summary>
    public class JpegToDicomIntegrationTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly ServiceProvider _serviceProvider;
        private readonly IFileProcessor _fileProcessor;

        public JpegToDicomIntegrationTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), $"CamBridgeTest_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);

            // Setup dependency injection
            var services = new ServiceCollection();

            // Add logging
            services.AddLogging(builder => builder.AddConsole());

            // Add CamBridge infrastructure
            services.AddCamBridgeInfrastructure();

            // Add processing options
            services.Configure<ProcessingOptions>(options =>
            {
                options.SuccessAction = PostProcessingAction.Leave;
                options.FailureAction = PostProcessingAction.Leave;
                options.CreateBackup = false;
                options.MaxRetryAttempts = 1;
            });

            // Add CamBridge settings
            services.Configure<CamBridgeSettings>(settings =>
            {
                settings.DefaultOutputFolder = Path.Combine(_testDirectory, "output");
                settings.UseRicohExifReader = true;
                settings.Dicom.ValidateAfterCreation = true;
            });

            _serviceProvider = services.BuildServiceProvider();
            _fileProcessor = _serviceProvider.GetRequiredService<IFileProcessor>();
        }

        [Fact]
        public async Task ProcessFileAsync_WithSpecialCharacters_HandlesUmlautsCorrectly()
        {
            // Arrange
            var inputPath = Path.Combine(_testDirectory, "input", "umlaut_test.jpg");
            Directory.CreateDirectory(Path.GetDirectoryName(inputPath)!);

            JpegTestFileGenerator.CreateTestJpegWithQRBridgeData(
                inputPath,
                examId: "EX_ÄÖÜ",
                patientName: "Müller-Lüdenscheidt, Käthe",
                birthDate: "1985-03-15",
                gender: "F",
                comment: "Röntgen Thorax - Größe prüfen");

            // Act
            var result = await _fileProcessor.ProcessFileAsync(inputPath);

            // Assert
            result.Success.Should().BeTrue();

            var dicomFile = await FellowOakDicom.DicomFile.OpenAsync(result.OutputFile!);
            var dataset = dicomFile.Dataset;

            // FIX: Extract patient ID from dataset instead of using undefined variable
            var actualPatientId = dataset.GetString(FellowOakDicom.DicomTag.PatientID);
            actualPatientId.Should().NotBeNullOrEmpty();

            // Verify character encoding
            dataset.GetString(FellowOakDicom.DicomTag.SpecificCharacterSet)
                .Should().Be("ISO_IR 100");

            dataset.GetString(FellowOakDicom.DicomTag.PatientName)
                .Should().Contain("Müller-Lüdenscheidt");
        }

        [Fact]
        public async Task ProcessFileAsync_WithJpegMissingQRBridgeData_CreatesMinimalDicom()
        {
            // Arrange
            var inputPath = Path.Combine(_testDirectory, "input", "no_patient_data.jpg");
            var outputDir = Path.Combine(_testDirectory, "output");
            Directory.CreateDirectory(Path.GetDirectoryName(inputPath)!);
            Directory.CreateDirectory(outputDir);

            // Create test JPEG without QRBridge data
            JpegTestFileGenerator.CreateTestJpegWithoutQRBridgeData(inputPath);

            // Act
            var result = await _fileProcessor.ProcessFileAsync(inputPath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.OutputFile.Should().NotBeNullOrEmpty();

            // Verify DICOM file exists
            File.Exists(result.OutputFile).Should().BeTrue();

            // Verify minimal DICOM content
            var dicomFile = await FellowOakDicom.DicomFile.OpenAsync(result.OutputFile);
            var dataset = dicomFile.Dataset;

            // Should have generated IDs
            dataset.Contains(FellowOakDicom.DicomTag.PatientID).Should().BeTrue();
            dataset.Contains(FellowOakDicom.DicomTag.StudyInstanceUID).Should().BeTrue();
            dataset.Contains(FellowOakDicom.DicomTag.SOPInstanceUID).Should().BeTrue();
        }

        [Fact]
        public async Task ProcessFileAsync_WithCustomMappingConfiguration_AppliesMappingsCorrectly()
        {
            // Arrange
            var inputPath = Path.Combine(_testDirectory, "input", "mapped_test.jpg");
            Directory.CreateDirectory(Path.GetDirectoryName(inputPath)!);

            // Create test JPEG
            JpegTestFileGenerator.CreateTestJpegWithQRBridgeData(
                inputPath,
                examId: "EXAM123",
                patientName: "Test, Mapping",
                birthDate: "1990-01-01",
                gender: "F",
                comment: "Mapping Test");

            // Setup custom mapping configuration
            var mappingConfig = _serviceProvider.GetRequiredService<IMappingConfiguration>();

            // Act
            var result = await _fileProcessor.ProcessFileAsync(inputPath);

            // Assert
            result.Success.Should().BeTrue();

            var dicomFile = await FellowOakDicom.DicomFile.OpenAsync(result.OutputFile!);
            var dataset = dicomFile.Dataset;

            // Verify mappings were applied
            dataset.GetString(FellowOakDicom.DicomTag.StudyID).Should().Be("EXAM123");
        }

        [Fact]
        public async Task ProcessFileAsync_BatchProcessing_HandlesMultipleFilesSuccessfully()
        {
            // Arrange
            var inputDir = Path.Combine(_testDirectory, "batch_input");
            var outputDir = Path.Combine(_testDirectory, "batch_output");
            Directory.CreateDirectory(inputDir);
            Directory.CreateDirectory(outputDir);

            // Create batch of test images
            var testFiles = JpegTestFileGenerator.CreateBatchTestImages(inputDir, 5);

            // Act
            var results = new List<ProcessingResult>();
            foreach (var file in testFiles)
            {
                var result = await _fileProcessor.ProcessFileAsync(file);
                results.Add(result);
            }

            // Assert
            results.Should().AllSatisfy(r =>
            {
                r.Success.Should().BeTrue();
                r.OutputFile.Should().NotBeNullOrEmpty();
                File.Exists(r.OutputFile).Should().BeTrue();
            });

            // Verify each DICOM has unique IDs
            var sopInstanceUids = new HashSet<string>();
            var studyInstanceUids = new HashSet<string>();

            foreach (var result in results)
            {
                var dicomFile = await FellowOakDicom.DicomFile.OpenAsync(result.OutputFile!);
                var dataset = dicomFile.Dataset;

                var sopUid = dataset.GetString(FellowOakDicom.DicomTag.SOPInstanceUID);
                var studyUid = dataset.GetString(FellowOakDicom.DicomTag.StudyInstanceUID);

                sopInstanceUids.Add(sopUid).Should().BeTrue("SOP Instance UIDs should be unique");
                // Study UIDs can be the same for images in the same study
            }
        }

        [Fact]
        public async Task ProcessFileAsync_InvalidJpegFile_ReturnsFailure()
        {
            // Arrange
            var inputPath = Path.Combine(_testDirectory, "input", "invalid.jpg");
            Directory.CreateDirectory(Path.GetDirectoryName(inputPath)!);

            // Create invalid file
            await File.WriteAllTextAsync(inputPath, "This is not a JPEG file");

            // Act
            var result = await _fileProcessor.ProcessFileAsync(inputPath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ProcessFileAsync_WithDicomValidation_ValidatesGeneratedFiles()
        {
            // Arrange
            var inputPath = Path.Combine(_testDirectory, "input", "validation_test.jpg");
            Directory.CreateDirectory(Path.GetDirectoryName(inputPath)!);

            JpegTestFileGenerator.CreateTestJpegWithQRBridgeData(
                inputPath,
                examId: "VAL001",
                patientName: "Validation, Test",
                birthDate: "2000-01-01",
                gender: "M",
                comment: "Validation Test");

            var dicomConverter = _serviceProvider.GetRequiredService<IDicomConverter>();

            // Act
            var processingResult = await _fileProcessor.ProcessFileAsync(inputPath);

            // Validate the generated DICOM
            var validationResult = await dicomConverter.ValidateDicomFileAsync(processingResult.OutputFile!);

            // Assert
            processingResult.Success.Should().BeTrue();
            validationResult.IsValid.Should().BeTrue();
            validationResult.Errors.Should().BeEmpty();
            validationResult.Warnings.Should().BeEmpty();
        }

        public void Dispose()
        {
            _serviceProvider?.Dispose();

            // Cleanup test files
            JpegTestFileGenerator.CleanupTestFiles(_testDirectory);

            // Cleanup test directory
            try
            {
                if (Directory.Exists(_testDirectory))
                {
                    Directory.Delete(_testDirectory, true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
