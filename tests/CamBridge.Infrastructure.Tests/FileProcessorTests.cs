using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Entities;
using CamBridge.Core.Interfaces;
using CamBridge.Core.ValueObjects;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CamBridge.Infrastructure.Tests
{
    public class FileProcessorTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly Mock<ILogger<FileProcessor>> _loggerMock;
        private readonly Mock<IExifReader> _exifReaderMock;
        private readonly Mock<IDicomConverter> _dicomConverterMock;
        private readonly ProcessingOptions _processingOptions;
        private readonly CamBridgeSettings _settings;
        private readonly FileProcessor _fileProcessor;

        public FileProcessorTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), $"CamBridgeTest_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);

            _loggerMock = new Mock<ILogger<FileProcessor>>();
            _exifReaderMock = new Mock<IExifReader>();
            _dicomConverterMock = new Mock<IDicomConverter>();

            _processingOptions = new ProcessingOptions
            {
                ArchiveFolder = Path.Combine(_testDirectory, "Archive"),
                ErrorFolder = Path.Combine(_testDirectory, "Errors"),
                BackupFolder = Path.Combine(_testDirectory, "Backup"),
                CreateBackup = false,
                SuccessAction = PostProcessingAction.Archive,
                FailureAction = PostProcessingAction.MoveToError,
                MinimumFileSizeBytes = 1024,
                MaximumFileSizeBytes = 10 * 1024 * 1024,
                OutputFilePattern = "{PatientID}_{StudyDate}_{InstanceNumber}.dcm",
                OutputOrganization = OutputOrganization.ByPatient
            };

            _settings = new CamBridgeSettings
            {
                DefaultOutputFolder = Path.Combine(_testDirectory, "Output"),
                WatchFolders = new List<FolderConfiguration>
                {
                    new() { Path = _testDirectory, OutputPath = Path.Combine(_testDirectory, "Output") }
                },
                Dicom = new DicomSettings { ValidateAfterCreation = false }
            };

            var processingOptionsWrapper = Options.Create(_processingOptions);
            var settingsWrapper = Options.Create(_settings);

            _fileProcessor = new FileProcessor(
                _loggerMock.Object,
                _exifReaderMock.Object,
                _dicomConverterMock.Object,
                processingOptionsWrapper,
                settingsWrapper);
        }

        [Fact]
        public async Task ProcessFileAsync_ValidFile_Success()
        {
            // Arrange
            var testFile = CreateTestJpegFile();
            var exifData = new Dictionary<string, string>
            {
                ["UserComment"] = "EX001|Test Patient|1980-01-01|M|Test Study",
                ["Make"] = "RICOH",
                ["Model"] = "G900 II"
            };

            var qrBridgeData = new Dictionary<string, string>
            {
                ["examid"] = "EX001",
                ["name"] = "Test Patient",
                ["birthdate"] = "1980-01-01",
                ["gender"] = "M",
                ["comment"] = "Test Study"
            };

            _exifReaderMock.Setup(x => x.ReadExifDataAsync(testFile))
                .ReturnsAsync(exifData);
            _exifReaderMock.Setup(x => x.GetUserCommentAsync(testFile))
                .ReturnsAsync("EX001|Test Patient|1980-01-01|M|Test Study");
            _exifReaderMock.Setup(x => x.ParseQRBridgeData(It.IsAny<string>()))
                .Returns(qrBridgeData);

            _dicomConverterMock.Setup(x => x.ConvertToDicomAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ImageMetadata>()))
                .ReturnsAsync(ConversionResult.CreateSuccess("output.dcm", "1.2.3.4", 1024));

            // Act
            var result = await _fileProcessor.ProcessFileAsync(testFile);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.OutputFile);
            Assert.Equal(testFile, result.SourceFile);

            // Verify DICOM conversion was called
            _dicomConverterMock.Verify(x => x.ConvertToDicomAsync(
                testFile,
                It.IsAny<string>(),
                It.IsAny<ImageMetadata>()), Times.Once);
        }

        [Fact]
        public async Task ProcessFileAsync_MissingFile_ThrowsException()
        {
            // Arrange
            var missingFile = Path.Combine(_testDirectory, "missing.jpg");

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(
                () => _fileProcessor.ProcessFileAsync(missingFile));
        }

        [Fact]
        public async Task ProcessFileAsync_ConversionFails_ReturnsFailure()
        {
            // Arrange
            var testFile = CreateTestJpegFile();

            _exifReaderMock.Setup(x => x.ReadExifDataAsync(testFile))
                .ReturnsAsync(new Dictionary<string, string>());
            _exifReaderMock.Setup(x => x.GetUserCommentAsync(testFile))
                .ReturnsAsync((string?)null);

            _dicomConverterMock.Setup(x => x.ConvertToDicomAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ImageMetadata>()))
                .ReturnsAsync(ConversionResult.CreateFailure("Conversion error"));

            // Act
            var result = await _fileProcessor.ProcessFileAsync(testFile);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Conversion error", result.ErrorMessage);
        }

        [Fact]
        public void ShouldProcessFile_ValidJpeg_ReturnsTrue()
        {
            // Arrange
            var testFile = CreateTestJpegFile();

            // Act
            var result = _fileProcessor.ShouldProcessFile(testFile);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ShouldProcessFile_NonJpegFile_ReturnsFalse()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "Not a JPEG");

            // Act
            var result = _fileProcessor.ShouldProcessFile(testFile);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ShouldProcessFile_FileTooSmall_ReturnsFalse()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "small.jpg");
            File.WriteAllBytes(testFile, new byte[100]); // Less than minimum

            // Act
            var result = _fileProcessor.ShouldProcessFile(testFile);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ProcessFileAsync_SuccessAction_Archive_MovesFile()
        {
            // Arrange
            var testFile = CreateTestJpegFile();
            SetupSuccessfulProcessing(testFile);

            // Act
            var result = await _fileProcessor.ProcessFileAsync(testFile);

            // Assert
            Assert.True(result.Success);
            Assert.False(File.Exists(testFile)); // Original file should be moved

            // Check if file was archived
            var archiveFiles = Directory.GetFiles(_processingOptions.ArchiveFolder, "*", SearchOption.AllDirectories);
            Assert.Single(archiveFiles);
        }

        [Fact]
        public async Task ProcessFileAsync_WithBackup_CreatesBackup()
        {
            // Arrange
            _processingOptions.CreateBackup = true;
            var testFile = CreateTestJpegFile();
            SetupSuccessfulProcessing(testFile);

            // Act
            var result = await _fileProcessor.ProcessFileAsync(testFile);

            // Assert
            Assert.True(result.Success);

            // Check if backup was created
            var backupFiles = Directory.GetFiles(_processingOptions.BackupFolder, "*", SearchOption.AllDirectories);
            Assert.Single(backupFiles);
        }

        private string CreateTestJpegFile()
        {
            var fileName = $"test_{Guid.NewGuid()}.jpg";
            var filePath = Path.Combine(_testDirectory, fileName);

            // Create a minimal JPEG file (just for testing)
            var jpegHeader = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };
            var fileContent = new byte[2048];
            Array.Copy(jpegHeader, fileContent, jpegHeader.Length);

            File.WriteAllBytes(filePath, fileContent);
            return filePath;
        }

        private void SetupSuccessfulProcessing(string testFile)
        {
            var exifData = new Dictionary<string, string>
            {
                ["UserComment"] = "EX001|Test Patient|1980-01-01|M|Test Study",
                ["Make"] = "RICOH",
                ["Model"] = "G900 II"
            };

            var qrBridgeData = new Dictionary<string, string>
            {
                ["examid"] = "EX001",
                ["name"] = "Test Patient",
                ["birthdate"] = "1980-01-01",
                ["gender"] = "M",
                ["comment"] = "Test Study"
            };

            _exifReaderMock.Setup(x => x.ReadExifDataAsync(testFile))
                .ReturnsAsync(exifData);
            _exifReaderMock.Setup(x => x.GetUserCommentAsync(testFile))
                .ReturnsAsync("EX001|Test Patient|1980-01-01|M|Test Study");
            _exifReaderMock.Setup(x => x.ParseQRBridgeData(It.IsAny<string>()))
                .Returns(qrBridgeData);

            _dicomConverterMock.Setup(x => x.ConvertToDicomAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ImageMetadata>()))
                .ReturnsAsync(ConversionResult.CreateSuccess("output.dcm", "1.2.3.4", 1024));
        }

        public void Dispose()
        {
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
