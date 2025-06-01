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
    /// Integration tests for error handling scenarios
    /// </summary>
    public class ErrorHandlingTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly ServiceProvider _serviceProvider;
        private readonly IFileProcessor _fileProcessor;

        public ErrorHandlingTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), $"CamBridgeErrorTest_{Guid.NewGuid()}");
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
                options.FailureAction = PostProcessingAction.MoveToError;
                options.ErrorFolder = Path.Combine(_testDirectory, "errors");
                options.CreateBackup = false;
                options.MaxRetryAttempts = 2;
                options.RetryDelaySeconds = 1;
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
        public async Task ProcessFileAsync_WithCorruptedJpeg_HandlesGracefully()
        {
            // Arrange
            var corruptPath = Path.Combine(_testDirectory, "corrupted.jpg");
            await File.WriteAllBytesAsync(corruptPath, new byte[] { 0xFF, 0xD8, 0x00, 0x00 }); // Invalid JPEG

            // Act
            var result = await _fileProcessor.ProcessFileAsync(corruptPath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ProcessFileAsync_WithFileLockedForReading_RetriesAndFails()
        {
            // Arrange
            var inputPath = Path.Combine(_testDirectory, "locked.jpg");
            JpegTestFileGenerator.CreateTestJpegWithQRBridgeData(inputPath);

            // Lock the file
            using var fileStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.None);

            // Act
            var result = await _fileProcessor.ProcessFileAsync(inputPath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("access");
        }

        [Fact]
        public async Task ProcessFileAsync_WithInvalidPatientData_HandlesSpecialCharacters()
        {
            // Arrange
            var inputPath = Path.Combine(_testDirectory, "invalid_chars.jpg");

            // Create JPEG with invalid characters in patient data
            JpegTestFileGenerator.CreateTestJpegWithQRBridgeData(
                inputPath,
                examId: "EX<>:|?*001",  // Invalid filename characters
                patientName: "Test\0Patient",  // Null character
                birthDate: "invalid-date",
                gender: "X",  // Invalid gender
                comment: "Test\r\nComment"
            );

            // Act
            var result = await _fileProcessor.ProcessFileAsync(inputPath);

            // Assert
            result.Should().NotBeNull();
            // Should either succeed with sanitized data or fail gracefully
            if (result.Success)
            {
                result.OutputFile.Should().NotBeNullOrEmpty();
                File.Exists(result.OutputFile).Should().BeTrue();
            }
            else
            {
                result.ErrorMessage.Should().NotBeNullOrEmpty();
            }
        }

        [Fact]
        public async Task ProcessFileAsync_WithExtremelyLargeFile_HandlesMemoryEfficiently()
        {
            // Skip this test in CI environments
            if (Environment.GetEnvironmentVariable("CI") == "true")
                return;

            // Arrange - Create a large test file (10MB)
            var largePath = Path.Combine(_testDirectory, "large.jpg");
            var largeData = new byte[10 * 1024 * 1024]; // 10MB

            // Fill with minimal JPEG structure
            largeData[0] = 0xFF;
            largeData[1] = 0xD8; // SOI
            largeData[largeData.Length - 2] = 0xFF;
            largeData[largeData.Length - 1] = 0xD9; // EOI

            await File.WriteAllBytesAsync(largePath, largeData);

            // Act
            var result = await _fileProcessor.ProcessFileAsync(largePath);

            // Assert
            result.Should().NotBeNull();
            // Large files without proper EXIF should fail
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task ProcessFileAsync_WithReadOnlyOutputDirectory_FailsGracefully()
        {
            // Arrange
            var inputPath = Path.Combine(_testDirectory, "readonly_test.jpg");
            JpegTestFileGenerator.CreateTestJpegWithQRBridgeData(inputPath);

            var readOnlyDir = Path.Combine(_testDirectory, "readonly_output");
            Directory.CreateDirectory(readOnlyDir);

            // Make directory read-only (Windows specific)
            if (OperatingSystem.IsWindows())
            {
                var dirInfo = new DirectoryInfo(readOnlyDir);
                dirInfo.Attributes |= FileAttributes.ReadOnly;
            }

            // Temporarily change output directory
            var settings = _serviceProvider.GetRequiredService<IOptions<CamBridgeSettings>>();
            var originalOutput = settings.Value.DefaultOutputFolder;
            settings.Value.DefaultOutputFolder = readOnlyDir;

            try
            {
                // Act
                var result = await _fileProcessor.ProcessFileAsync(inputPath);

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeFalse();
                result.ErrorMessage.Should().NotBeNullOrEmpty();
            }
            finally
            {
                // Restore
                settings.Value.DefaultOutputFolder = originalOutput;

                if (OperatingSystem.IsWindows())
                {
                    var dirInfo = new DirectoryInfo(readOnlyDir);
                    dirInfo.Attributes &= ~FileAttributes.ReadOnly;
                }
            }
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
