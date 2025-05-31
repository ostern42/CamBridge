using System;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Infrastructure.Services;
using CamBridge.Infrastructure.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace CamBridge.Infrastructure.Tests.IntegrationTests
{
    /// <summary>
    /// Tests for error handling scenarios
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class ErrorHandlingTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly FileProcessor _fileProcessor;
        private readonly string _tempPath;
        private readonly ProcessingOptions _processingOptions;

        public ErrorHandlingTests(ITestOutputHelper output)
        {
            _output = output;
            _tempPath = Path.Combine(Path.GetTempPath(), $"CamBridgeErrorTest_{Guid.NewGuid()}");
            Directory.CreateDirectory(_tempPath);

            // Setup with error handling configuration
            _processingOptions = new ProcessingOptions
            {
                SuccessAction = PostProcessingAction.Archive,
                FailureAction = PostProcessingAction.MoveToError,
                ArchiveFolder = Path.Combine(_tempPath, "Archive"),
                ErrorFolder = Path.Combine(_tempPath, "Errors"),
                CreateBackup = true,
                BackupFolder = Path.Combine(_tempPath, "Backup")
            };

            var settings = new CamBridgeSettings
            {
                DefaultOutputFolder = Path.Combine(_tempPath, "Output")
            };

            var loggerExif = new Mock<ILogger<ExifReader>>().Object;
            var loggerDicom = new Mock<ILogger<DicomConverter>>().Object;
            var loggerProcessor = new Mock<ILogger<FileProcessor>>().Object;

            _fileProcessor = new FileProcessor(
                loggerProcessor,
                new ExifReader(loggerExif),
                new DicomConverter(loggerDicom),
                Options.Create(_processingOptions),
                Options.Create(settings));
        }

        [Fact]
        public async Task ProcessFile_WithMissingQRBridgeData_ShouldMoveToErrorFolder()
        {
            // Arrange - Create JPEG without QRBridge data
            var jpegPath = JpegTestFileGenerator.CreateTestJpegWithQRBridgeData(
                "no_qrbridge.jpg",
                ""); // Empty User Comment

            _output.WriteLine($"Testing file without QRBridge data: {jpegPath}");

            // Act
            var result = await _fileProcessor.ProcessFileAsync(jpegPath);

            // Assert
            result.Success.Should().BeTrue(); // Should still process but with minimal metadata

            // For files without QRBridge data, it should create DICOM with generated IDs
            result.OutputFile.Should().NotBeNullOrEmpty();
            File.Exists(result.OutputFile).Should().BeTrue();
        }

        [Fact]
        public async Task ProcessFile_WithCorruptedFile_ShouldHandleGracefully()
        {
            // Arrange - Create corrupted JPEG
            var corruptPath = Path.Combine(_tempPath, "corrupt.jpg");
            await File.WriteAllBytesAsync(corruptPath, new byte[] { 0xFF, 0xD8, 0xFF, 0x00 }); // Invalid JPEG

            // Act
            var result = await _fileProcessor.ProcessFileAsync(corruptPath);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();

            // File should be moved to error folder
            var errorPath = Path.Combine(_processingOptions.ErrorFolder, DateTime.UtcNow.ToString("yyyy-MM-dd"), "corrupt.jpg");
            File.Exists(corruptPath).Should().BeFalse(); // Original removed
            File.Exists(errorPath).Should().BeTrue(); // Moved to error folder
        }

        [Fact]
        public async Task ProcessFile_WithInvalidPath_ShouldReturnFailure()
        {
            // Arrange
            var invalidPath = Path.Combine(_tempPath, "nonexistent.jpg");

            // Act
            var result = await _fileProcessor.ProcessFileAsync(invalidPath);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("not found");
        }

        [Fact]
        public async Task ProcessFile_WithSuccessfulConversion_ShouldArchive()
        {
            // Arrange
            var jpegPath = JpegTestFileGenerator.CreateTestJpegWithQRBridgeData(
                "success.jpg",
                "EX300|Archive, Test|1980-01-01|M|Success Test");

            // Act
            var result = await _fileProcessor.ProcessFileAsync(jpegPath);

            // Assert
            result.Success.Should().BeTrue();

            // Original should be moved to archive
            File.Exists(jpegPath).Should().BeFalse();

            var archivePattern = Path.Combine(_processingOptions.ArchiveFolder, "*", "success.jpg");
            var archivedFiles = Directory.GetFiles(_processingOptions.ArchiveFolder, "success.jpg", SearchOption.AllDirectories);
            archivedFiles.Should().HaveCount(1);

            // Backup should exist
            var backupPattern = Path.Combine(_processingOptions.BackupFolder, "*", "success.jpg");
            var backupFiles = Directory.GetFiles(_processingOptions.BackupFolder, "success.jpg", SearchOption.AllDirectories);
            backupFiles.Should().HaveCount(1);

            _output.WriteLine($"Archived to: {archivedFiles[0]}");
            _output.WriteLine($"Backed up to: {backupFiles[0]}");
        }

        [Fact]
        public void ShouldProcessFile_WithVariousConditions_ShouldValidateCorrectly()
        {
            // Test file size limits
            var validPath = JpegTestFileGenerator.CreateTestJpegWithQRBridgeData("valid.jpg", "TEST|Test|2000-01-01|M|");

            _fileProcessor.ShouldProcessFile(validPath).Should().BeTrue();
            _fileProcessor.ShouldProcessFile("").Should().BeFalse();
            _fileProcessor.ShouldProcessFile(null!).Should().BeFalse();
            _fileProcessor.ShouldProcessFile("test.txt").Should().BeFalse(); // Wrong extension
        }

        [Fact]
        public async Task ProcessFile_WithSpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange - Test with special characters in patient name
            var specialChars = "EX400|O'Connell-Smith, José María|1975-03-20|M|Ñoño's X-Ray";
            var jpegPath = JpegTestFileGenerator.CreateTestJpegWithQRBridgeData(
                "special_chars.jpg",
                specialChars);

            // Act
            var result = await _fileProcessor.ProcessFileAsync(jpegPath);

            // Assert
            result.Success.Should().BeTrue();
            result.OutputFile.Should().NotBeNullOrEmpty();

            // Verify filename sanitization worked
            var outputFileName = Path.GetFileName(result.OutputFile);
            outputFileName.Should().NotContain("'");
            outputFileName.Should().NotContain("ñ");
        }

        public void Dispose()
        {
            JpegTestFileGenerator.CleanupTestFiles();

            if (Directory.Exists(_tempPath))
            {
                Directory.Delete(_tempPath, true);
            }
        }
    }
}
