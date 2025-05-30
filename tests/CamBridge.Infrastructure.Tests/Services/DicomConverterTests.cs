using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using CamBridge.Core.Entities;
using CamBridge.Core.ValueObjects;
using CamBridge.Infrastructure.Services;
using FellowOakDicom;
using Microsoft.Extensions.Logging;
using Xunit;
using DicomTag = FellowOakDicom.DicomTag;

namespace CamBridge.Infrastructure.Tests.Services
{
    public class DicomConverterTests : IDisposable
    {
        private readonly DicomConverter _converter;
        private readonly ILogger<DicomConverter> _logger;
        private readonly string _testDirectory;
        private readonly string _testJpegPath;

        public DicomConverterTests()
        {
            _logger = new TestLogger<DicomConverter>();
            _converter = new DicomConverter(_logger);

            // Create test directory
            _testDirectory = Path.Combine(Path.GetTempPath(), $"CamBridgeTest_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);

            // Create test JPEG
            _testJpegPath = CreateTestJpeg();
        }

        [Fact]
        public async Task ConvertToDicomAsync_ValidInput_CreatesValidDicomFile()
        {
            // Arrange
            var outputPath = Path.Combine(_testDirectory, "test_output.dcm");
            var metadata = CreateTestMetadata();

            // Act
            var result = await _converter.ConvertToDicomAsync(_testJpegPath, outputPath, metadata);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.DicomFilePath);
            Assert.NotNull(result.SopInstanceUid);
            Assert.True(result.FileSizeBytes > 0);
            Assert.True(File.Exists(outputPath));

            // Verify DICOM file can be opened
            var dicomFile = await DicomFile.OpenAsync(outputPath);
            Assert.NotNull(dicomFile);
        }

        [Fact]
        public async Task ConvertToDicomAsync_PreservesJpegCompression()
        {
            // Arrange
            var outputPath = Path.Combine(_testDirectory, "test_compressed.dcm");
            var metadata = CreateTestMetadata();
            var originalJpegSize = new FileInfo(_testJpegPath).Length;

            // Act
            var result = await _converter.ConvertToDicomAsync(_testJpegPath, outputPath, metadata);

            // Assert
            Assert.True(result.Success);

            var dicomFile = await DicomFile.OpenAsync(outputPath);

            // Check transfer syntax is JPEG
            Assert.Equal("1.2.840.10008.1.2.4.50", dicomFile.FileMetaInfo.TransferSyntax.UID.UID);

            // Check photometric interpretation
            var photometric = dicomFile.Dataset.GetString(DicomTag.PhotometricInterpretation);
            Assert.Equal("YBR_FULL_422", photometric);

            // Check pixel data is encapsulated
            var pixelData = dicomFile.Dataset.GetDicomItem<DicomOtherByteFragment>(DicomTag.PixelData);
            Assert.NotNull(pixelData);
            Assert.True(pixelData.Fragments.Count >= 2); // Basic offset table + data
        }

        [Fact]
        public async Task ConvertToDicomAsync_SetsAllMandatoryTags()
        {
            // Arrange
            var outputPath = Path.Combine(_testDirectory, "test_tags.dcm");
            var metadata = CreateTestMetadata();

            // Act
            var result = await _converter.ConvertToDicomAsync(_testJpegPath, outputPath, metadata);

            // Assert
            Assert.True(result.Success);

            var dicomFile = await DicomFile.OpenAsync(outputPath);
            var dataset = dicomFile.Dataset;

            // Patient Module
            Assert.Equal("Schmidt, Maria", dataset.GetString(DicomTag.PatientName));
            Assert.Equal("PAT001", dataset.GetString(DicomTag.PatientID));
            Assert.Equal("19850315", dataset.GetString(DicomTag.PatientBirthDate));
            Assert.Equal("F", dataset.GetString(DicomTag.PatientSex));

            // Study Module
            Assert.True(dataset.Contains(DicomTag.StudyInstanceUID));
            Assert.Equal("20250115", dataset.GetString(DicomTag.StudyDate));
            Assert.Equal("EX002", dataset.GetString(DicomTag.StudyID));
            Assert.Equal("Röntgen Thorax", dataset.GetString(DicomTag.StudyDescription));

            // Series Module
            Assert.True(dataset.Contains(DicomTag.SeriesInstanceUID));
            Assert.Equal("XC", dataset.GetString(DicomTag.Modality));

            // Image Module
            Assert.Equal("1.2.840.10008.5.1.4.1.1.77.1.4", dataset.GetString(DicomTag.SOPClassUID));
            Assert.True(dataset.Contains(DicomTag.SOPInstanceUID));

            // Equipment Module
            Assert.Equal("RICOH", dataset.GetString(DicomTag.Manufacturer));
            Assert.Equal("G900 II", dataset.GetString(DicomTag.ManufacturerModelName));
        }

        [Fact]
        public async Task ConvertToDicomAsync_HandlesSpecialCharacters()
        {
            // Arrange
            var outputPath = Path.Combine(_testDirectory, "test_special_chars.dcm");
            var metadata = CreateTestMetadata();
            metadata = new ImageMetadata(
                metadata.SourceFilePath,
                metadata.CaptureDateTime,
                new PatientInfo(
                    new PatientId("PAT001"),
                    "Müller, Jörg",
                    DateTime.Parse("1985-03-15"),
                    Gender.Male
                ),
                metadata.Study,
                metadata.ExifData,
                metadata.TechnicalData,
                metadata.InstanceNumber
            );

            // Act
            var result = await _converter.ConvertToDicomAsync(_testJpegPath, outputPath, metadata);

            // Assert
            Assert.True(result.Success);

            var dicomFile = await DicomFile.OpenAsync(outputPath);
            var dataset = dicomFile.Dataset;

            // Check character set
            Assert.Equal("ISO_IR 100", dataset.GetString(DicomTag.SpecificCharacterSet));

            // Check name with umlauts
            Assert.Equal("Müller, Jörg", dataset.GetString(DicomTag.PatientName));
        }

        [Fact]
        public async Task ValidateDicomFileAsync_ValidFile_ReturnsValid()
        {
            // Arrange
            var outputPath = Path.Combine(_testDirectory, "test_validate.dcm");
            var metadata = CreateTestMetadata();
            await _converter.ConvertToDicomAsync(_testJpegPath, outputPath, metadata);

            // Act
            var validationResult = await _converter.ValidateDicomFileAsync(outputPath);

            // Assert
            Assert.True(validationResult.IsValid);
            Assert.Empty(validationResult.Errors);
        }

        [Fact]
        public async Task ValidateDicomFileAsync_MissingMandatoryTag_ReturnsInvalid()
        {
            // Arrange
            var outputPath = Path.Combine(_testDirectory, "test_invalid.dcm");

            // Create invalid DICOM file (missing mandatory tags)
            var dataset = new DicomDataset();
            dataset.Add(DicomTag.SOPClassUID, "1.2.840.10008.5.1.4.1.1.77.1.4");
            dataset.Add(DicomTag.SOPInstanceUID, DicomUID.Generate());
            // Missing Patient Name, ID, etc.

            var file = new DicomFile(dataset);
            await file.SaveAsync(outputPath);

            // Act
            var validationResult = await _converter.ValidateDicomFileAsync(outputPath);

            // Assert
            Assert.False(validationResult.IsValid);
            Assert.NotEmpty(validationResult.Errors);
            Assert.Contains(validationResult.Errors, e => e.Contains("Study Instance UID"));
        }

        [Fact]
        public async Task ConvertToDicomAsync_InvalidJpegPath_ReturnsFailure()
        {
            // Arrange
            var outputPath = Path.Combine(_testDirectory, "test_fail.dcm");
            var metadata = CreateTestMetadata();

            // Act
            var result = await _converter.ConvertToDicomAsync("nonexistent.jpg", outputPath, metadata);

            // Assert
            Assert.False(result.Success);
            Assert.NotNull(result.ErrorMessage);
        }

        [Fact]
        public void GetPhotographicSopClassUid_ReturnsCorrectUid()
        {
            // Act
            var uid = _converter.GetPhotographicSopClassUid();

            // Assert
            Assert.Equal("1.2.840.10008.5.1.4.1.1.77.1.4", uid);
        }

        private string CreateTestJpeg()
        {
            var jpegPath = Path.Combine(_testDirectory, "test_image.jpg");

            using (var bitmap = new Bitmap(640, 480))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Draw test pattern
                graphics.Clear(Color.White);
                graphics.DrawString("CamBridge Test Image",
                    new Font("Arial", 24),
                    Brushes.Black,
                    new PointF(100, 200));
                graphics.DrawRectangle(Pens.Blue, 50, 50, 540, 380);

                // Save as JPEG
                bitmap.Save(jpegPath, ImageFormat.Jpeg);
            }

            return jpegPath;
        }

        private ImageMetadata CreateTestMetadata()
        {
            var patient = new PatientInfo(
                new PatientId("PAT001"),
                "Schmidt, Maria",
                DateTime.Parse("1985-03-15"),
                Gender.Female
            );

            var study = new StudyInfo(
                new StudyId("EX002"),
                DateTime.Parse("2025-01-15 10:30:00"),
                "XC",
                "EX002",
                "Röntgen Thorax",
                null,
                null,
                null,
                "Test conversion"
            );

            var technicalData = new ImageTechnicalData
            {
                Manufacturer = "RICOH",
                Model = "G900 II",
                Software = "v1.0",
                ImageWidth = 640,
                ImageHeight = 480
            };

            var exifData = new Dictionary<string, string>
            {
                ["Make"] = "RICOH",
                ["Model"] = "G900 II"
            };

            return new ImageMetadata(
                _testJpegPath,
                DateTime.Now,
                patient,
                study,
                exifData,
                technicalData,
                1
            );
        }

        public void Dispose()
        {
            // Clean up test directory
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
