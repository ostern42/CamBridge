using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CamBridge.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CamBridge.Infrastructure.Tests.Services
{
    public class ExifReaderTests
    {
        private readonly ExifReader _sut;
        private readonly Mock<ILogger<ExifReader>> _loggerMock;

        public ExifReaderTests()
        {
            _loggerMock = new Mock<ILogger<ExifReader>>();
            _sut = new ExifReader(_loggerMock.Object);
        }

        [Fact]
        public async Task ReadExifDataAsync_WithNullPath_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ReadExifDataAsync(null!));
        }

        [Fact]
        public async Task ReadExifDataAsync_WithEmptyPath_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ReadExifDataAsync(string.Empty));
        }

        [Fact]
        public async Task ReadExifDataAsync_WithNonExistentFile_ThrowsFileNotFoundException()
        {
            // Arrange
            var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".jpg");

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => _sut.ReadExifDataAsync(nonExistentPath));
        }

        [Fact]
        public void ParseQRBridgeData_WithPipeDelimitedFormat_ParsesCorrectly()
        {
            // Arrange
            var input = "EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax";

            // Act
            var result = _sut.ParseQRBridgeData(input);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(5);
            result["examid"].Should().Be("EX002");
            result["name"].Should().Be("Schmidt, Maria");
            result["birthdate"].Should().Be("1985-03-15");
            result["gender"].Should().Be("F");
            result["comment"].Should().Be("Röntgen Thorax");
        }

        [Fact]
        public void ParseQRBridgeData_WithCommandLineFormat_ParsesCorrectly()
        {
            // Arrange
            var input = "-examid \"EX002\" -name \"Schmidt, Maria\" -birthdate \"1985-03-15\" -gender \"F\" -comment \"Röntgen Thorax\"";

            // Act
            var result = _sut.ParseQRBridgeData(input);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(5);
            result["examid"].Should().Be("EX002");
            result["name"].Should().Be("Schmidt, Maria");
            result["birthdate"].Should().Be("1985-03-15");
            result["gender"].Should().Be("F");
            result["comment"].Should().Be("Röntgen Thorax");
        }

        [Fact]
        public void ParseQRBridgeData_WithPartialPipeDelimitedData_ParsesAvailableFields()
        {
            // Arrange
            var input = "EX003|Müller, Hans|1975-06-20";

            // Act
            var result = _sut.ParseQRBridgeData(input);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result["examid"].Should().Be("EX003");
            result["name"].Should().Be("Müller, Hans");
            result["birthdate"].Should().Be("1975-06-20");
            result.Should().NotContainKey("gender");
            result.Should().NotContainKey("comment");
        }

        [Fact]
        public void ParseQRBridgeData_WithEmptyString_ReturnsEmptyDictionary()
        {
            // Act
            var result = _sut.ParseQRBridgeData(string.Empty);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void ParseQRBridgeData_WithNull_ReturnsEmptyDictionary()
        {
            // Act
            var result = _sut.ParseQRBridgeData(null!);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void ParseQRBridgeData_WithMalformedData_HandlesGracefully()
        {
            // Arrange
            var input = "This is not valid QRBridge data";

            // Act
            var result = _sut.ParseQRBridgeData(input);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void ParseQRBridgeData_IsCaseInsensitive()
        {
            // Arrange
            var input = "-ExamID \"EX002\" -NAME \"Schmidt, Maria\" -BirthDate \"1985-03-15\"";

            // Act
            var result = _sut.ParseQRBridgeData(input);

            // Assert
            result.Should().ContainKey("examid");
            result.Should().ContainKey("name");
            result.Should().ContainKey("birthdate");
        }

        // Integration test - requires a real JPEG file with EXIF data
        [Fact(Skip = "Requires test JPEG file with EXIF data")]
        public async Task ReadExifDataAsync_WithValidJpeg_ReturnsExifData()
        {
            // This test would require a test JPEG file in TestData folder
            // For now, it's skipped but shows the intended test structure
            
            // Arrange
            var testImagePath = Path.Combine("TestData", "test-image-with-exif.jpg");

            // Act
            var result = await _sut.ReadExifDataAsync(testImagePath);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("Make");
            result.Should().ContainKey("Model");
            result.Should().ContainKey("DateTime");
        }
    }
}