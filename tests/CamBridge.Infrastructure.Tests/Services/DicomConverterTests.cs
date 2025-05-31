using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using CamBridge.Core.Entities;
using CamBridge.Core.ValueObjects;
using CamBridge.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CamBridge.Infrastructure.Tests.Services
{
    [SupportedOSPlatform("windows")]
    public class DicomConverterTests : IDisposable
    {
        private readonly DicomConverter _sut;
        private readonly Mock<ILogger<DicomConverter>> _loggerMock;
        private readonly string _tempPath;

        public DicomConverterTests()
        {
            _loggerMock = new Mock<ILogger<DicomConverter>>();
            _sut = new DicomConverter(_loggerMock.Object);
            _tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempPath);
        }

        [Fact]
        public async Task ConvertToDicomAsync_WithValidInputs_ShouldSucceed()
        {
            // Arrange
            var jpegPath = CreateTestJpeg();
            var dicomPath = Path.Combine(_tempPath, "test.dcm");
            var metadata = CreateTestMetadata();

            // Act
            var result = await _sut.ConvertToDicomAsync(jpegPath, dicomPath, metadata);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.DicomFilePath.Should().Be(dicomPath);
            result.SopInstanceUid.Should().NotBeNullOrEmpty();
            result.FileSizeBytes.Should().BeGreaterThan(0);
            File.Exists(dicomPath).Should().BeTrue();
        }

        [Fact]
        public async Task ConvertToDicomAsync_WithNullSourcePath_ShouldThrowArgumentException()
        {
            // Arrange
            var metadata = CreateTestMetadata();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.ConvertToDicomAsync(null!, "output.dcm", metadata));
        }

        [Fact]
        public async Task ConvertToDicomAsync_WithNullDestinationPath_ShouldThrowArgumentException()
        {
            // Arrange
            var metadata = CreateTestMetadata();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.ConvertToDicomAsync("input.jpg", null!, metadata));
        }

        [Fact]
        public async Task ConvertToDicomAsync_WithNullMetadata_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _sut.ConvertToDicomAsync("input.jpg", "output.dcm", null!));
        }

        [Fact]
        public async Task ConvertToDicomAsync_WithNonExistentSourceFile_ShouldReturnFailure()
        {
            // Arrange
            var nonExistentPath = Path.Combine(_tempPath, "nonexistent.jpg");
            var dicomPath = Path.Combine(_tempPath, "test.dcm");
            var metadata = CreateTestMetadata();

            // Act
            var result = await _sut.ConvertToDicomAsync(nonExistentPath, dicomPath, metadata);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Conversion failed");
        }

        [Fact]
        public async Task ConvertToDicomAsync_CreatesOutputDirectory_WhenNotExists()
        {
            // Arrange
            var jpegPath = CreateTestJpeg();
            var outputDir = Path.Combine(_tempPath, "subdir");
            var dicomPath = Path.Combine(outputDir, "test.dcm");
            var metadata = CreateTestMetadata();

            Directory.Exists(outputDir).Should().BeFalse();

            // Act
            var result = await _sut.ConvertToDicomAsync(jpegPath, dicomPath, metadata);

            // Assert
            result.Success.Should().BeTrue();
            Directory.Exists(outputDir).Should().BeTrue();
            File.Exists(dicomPath).Should().BeTrue();
        }

        [Fact]
        public async Task ValidateDicomFileAsync_WithValidFile_ShouldReturnValid()
        {
            // Arrange
            var jpegPath = CreateTestJpeg();
            var dicomPath = Path.Combine(_tempPath, "valid.dcm");
            var metadata = CreateTestMetadata();

            await _sut.ConvertToDicomAsync(jpegPath, dicomPath, metadata);

            // Act
            var result = await _sut.ValidateDicomFileAsync(dicomPath);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateDicomFileAsync_WithEmptyPath_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.ValidateDicomFileAsync(string.Empty));
        }

        [Fact]
        public async Task ValidateDicomFileAsync_WithNonExistentFile_ShouldReturnInvalid()
        {
            // Arrange
            var nonExistentPath = Path.Combine(_tempPath, "nonexistent.dcm");

            // Act
            var result = await _sut.ValidateDicomFileAsync(nonExistentPath);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors[0].Should().Contain("Validation error");
        }

        [Fact]
        public void GetPhotographicSopClassUid_ShouldReturnCorrectUid()
        {
            // Act
            var uid = _sut.GetPhotographicSopClassUid();

            // Assert
            uid.Should().Be("1.2.840.10008.5.1.4.1.1.77.1.4");
        }

        private string CreateTestJpeg()
        {
            var path = Path.Combine(_tempPath, "test.jpg");

            using (var bitmap = new Bitmap(100, 100))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                graphics.DrawString("Test", new Font("Arial", 12), Brushes.Black, 10, 10);
                graphics.DrawRectangle(Pens.Blue, 20, 20, 60, 60);

                bitmap.Save(path, ImageFormat.Jpeg);
            }

            return path;
        }

        private ImageMetadata CreateTestMetadata()
        {
            var patient = new PatientInfo(
                new PatientId("TEST123"),
                "Test, Patient",
                new DateTime(1980, 1, 1),
                Gender.Male);

            var study = new StudyInfo(
                new StudyId("STUDY123"),
                DateTime.Now,
                "XC",
                examId: "EX123",
                studyDescription: "Test Study");

            var technicalData = new ImageTechnicalData
            {
                Manufacturer = "Test Manufacturer",
                Model = "Test Model",
                Software = "Test Software v1.0",
                ImageWidth = 100,
                ImageHeight = 100
            };

            return new ImageMetadata(
                "test.jpg",
                DateTime.Now,
                patient,
                study,
                new Dictionary<string, string>(),
                technicalData,
                1);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempPath))
            {
                Directory.Delete(_tempPath, true);
            }
        }
    }
}
