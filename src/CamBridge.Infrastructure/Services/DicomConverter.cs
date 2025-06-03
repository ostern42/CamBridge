using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using CamBridge.Core.Entities;
using CamBridge.Core.Interfaces;
using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.IO.Buffer;
using Microsoft.Extensions.Logging;
using DicomTag = FellowOakDicom.DicomTag;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Implementation of IDicomConverter using fo-dicom library
    /// Converts JPEG images to DICOM format while preserving JPEG compression
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class DicomConverter : IDicomConverter
    {
        private readonly ILogger<DicomConverter> _logger;
        private readonly IDicomTagMapper? _tagMapper;
        private readonly IMappingConfiguration? _mappingConfiguration;

        private const string PHOTOGRAPHIC_SOP_CLASS_UID = "1.2.840.10008.5.1.4.1.1.77.1.4";
        private const string JPEG_BASELINE_TRANSFER_SYNTAX_UID = "1.2.840.10008.1.2.4.50";

        // Implementation Class UID prefix for CamBridge
        private const string IMPLEMENTATION_CLASS_UID = "1.2.276.0.7230010.3.0.3.6.4";
        private const string IMPLEMENTATION_VERSION_NAME = "CAMBRIDGE_001";

        // Constructor for backward compatibility
        public DicomConverter(ILogger<DicomConverter> logger)
            : this(logger, null, null)
        {
        }

        // New constructor with dependency injection
        public DicomConverter(
            ILogger<DicomConverter> logger,
            IDicomTagMapper? tagMapper,
            IMappingConfiguration? mappingConfiguration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tagMapper = tagMapper;
            _mappingConfiguration = mappingConfiguration;
        }

        /// <inheritdoc />
        public async Task<ConversionResult> ConvertToDicomAsync(
            string sourceJpegPath,
            string destinationDicomPath,
            ImageMetadata metadata)
        {
            if (string.IsNullOrWhiteSpace(sourceJpegPath))
                throw new ArgumentException("Source JPEG path cannot be null or empty", nameof(sourceJpegPath));

            if (string.IsNullOrWhiteSpace(destinationDicomPath))
                throw new ArgumentException("Destination DICOM path cannot be null or empty", nameof(destinationDicomPath));

            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            try
            {
                _logger.LogInformation("Starting DICOM conversion for {SourceFile}", sourceJpegPath);

                // Read JPEG file
                var jpegBytes = await File.ReadAllBytesAsync(sourceJpegPath);

                // Get image dimensions from metadata or read from file
                var (width, height) = await GetImageDimensionsAsync(sourceJpegPath, metadata);

                // Create DICOM dataset
                DicomDataset dataset;

                if (_tagMapper != null && _mappingConfiguration != null)
                {
                    // Use dynamic mapping
                    dataset = CreateDicomDatasetWithMapping(metadata, width, height);
                }
                else
                {
                    // Use legacy hardcoded mapping
                    dataset = CreateDicomDatasetLegacy(metadata, width, height);
                }

                // Create DICOM file with JPEG encapsulated pixel data
                var dicomFile = CreateDicomFileWithJpegData(dataset, jpegBytes);

                // Ensure output directory exists
                var outputDir = Path.GetDirectoryName(destinationDicomPath);
                if (!string.IsNullOrEmpty(outputDir))
                    Directory.CreateDirectory(outputDir);

                // Save DICOM file
                await dicomFile.SaveAsync(destinationDicomPath);

                var fileInfo = new FileInfo(destinationDicomPath);
                var sopInstanceUid = dataset.GetString(DicomTag.SOPInstanceUID);

                _logger.LogInformation("Successfully converted {SourceFile} to DICOM: {DicomFile} ({FileSize} bytes)",
                    sourceJpegPath, destinationDicomPath, fileInfo.Length);

                return ConversionResult.CreateSuccess(destinationDicomPath, sopInstanceUid, fileInfo.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting {SourceFile} to DICOM", sourceJpegPath);
                return ConversionResult.CreateFailure($"Conversion failed: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public async Task<ValidationResult> ValidateDicomFileAsync(string dicomFilePath)
        {
            if (string.IsNullOrWhiteSpace(dicomFilePath))
                throw new ArgumentException("DICOM file path cannot be null or empty", nameof(dicomFilePath));

            try
            {
                var errors = new List<string>();
                var warnings = new List<string>();

                // Try to open the file
                var dicomFile = await DicomFile.OpenAsync(dicomFilePath);
                var dataset = dicomFile.Dataset;

                // Check mandatory Patient Module tags
                ValidateTag(dataset, DicomTag.PatientName, "Patient Name", errors, warnings);
                ValidateTag(dataset, DicomTag.PatientID, "Patient ID", errors, warnings);
                ValidateTag(dataset, DicomTag.PatientBirthDate, "Patient Birth Date", errors, warnings);
                ValidateTag(dataset, DicomTag.PatientSex, "Patient Sex", errors, warnings);

                // Check mandatory Study Module tags
                ValidateTag(dataset, DicomTag.StudyInstanceUID, "Study Instance UID", errors, warnings, true);
                ValidateTag(dataset, DicomTag.StudyDate, "Study Date", errors, warnings);
                ValidateTag(dataset, DicomTag.StudyTime, "Study Time", errors, warnings);

                // Check mandatory Series Module tags
                ValidateTag(dataset, DicomTag.SeriesInstanceUID, "Series Instance UID", errors, warnings, true);
                ValidateTag(dataset, DicomTag.Modality, "Modality", errors, warnings, true);

                // Check mandatory Image Module tags
                ValidateTag(dataset, DicomTag.SOPClassUID, "SOP Class UID", errors, warnings, true);
                ValidateTag(dataset, DicomTag.SOPInstanceUID, "SOP Instance UID", errors, warnings, true);

                // Check Image Pixel Module
                ValidateTag(dataset, DicomTag.SamplesPerPixel, "Samples Per Pixel", errors, warnings, true);
                ValidateTag(dataset, DicomTag.PhotometricInterpretation, "Photometric Interpretation", errors, warnings, true);
                ValidateTag(dataset, DicomTag.Rows, "Rows", errors, warnings, true);
                ValidateTag(dataset, DicomTag.Columns, "Columns", errors, warnings, true);
                ValidateTag(dataset, DicomTag.BitsAllocated, "Bits Allocated", errors, warnings, true);
                ValidateTag(dataset, DicomTag.BitsStored, "Bits Stored", errors, warnings, true);
                ValidateTag(dataset, DicomTag.HighBit, "High Bit", errors, warnings, true);
                ValidateTag(dataset, DicomTag.PixelRepresentation, "Pixel Representation", errors, warnings, true);

                // Check pixel data
                if (!dataset.Contains(DicomTag.PixelData))
                {
                    errors.Add("Missing Pixel Data");
                }

                // Validate photometric interpretation for JPEG
                var transferSyntax = dicomFile.FileMetaInfo.TransferSyntax;
                if (transferSyntax.UID.UID == JPEG_BASELINE_TRANSFER_SYNTAX_UID)
                {
                    var photometric = dataset.GetString(DicomTag.PhotometricInterpretation);
                    if (photometric != "YBR_FULL_422")
                    {
                        warnings.Add($"Photometric Interpretation '{photometric}' may not be optimal for JPEG compression (expected YBR_FULL_422)");
                    }
                }

                if (errors.Count == 0)
                {
                    _logger.LogInformation("DICOM file validation passed for {FilePath}", dicomFilePath);
                    return ValidationResult.Valid();
                }
                else
                {
                    _logger.LogWarning("DICOM file validation failed for {FilePath}: {Errors}",
                        dicomFilePath, string.Join(", ", errors));
                    return ValidationResult.Invalid(errors.ToArray());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating DICOM file {FilePath}", dicomFilePath);
                return ValidationResult.Invalid($"Validation error: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public string GetPhotographicSopClassUid() => PHOTOGRAPHIC_SOP_CLASS_UID;

        private DicomDataset CreateDicomDatasetWithMapping(ImageMetadata metadata, int width, int height)
        {
            var dataset = new DicomDataset();

            // Add basic technical tags that aren't mapped
            AddBasicTechnicalTags(dataset, metadata, width, height);

            // Prepare source data for mapping
            var sourceData = PrepareSourceData(metadata);

            // Get mapping rules
            var mappingRules = _mappingConfiguration!.GetMappingRules();

            // Flatten the nested dictionary if needed
            var flatSourceData = new Dictionary<string, string>();
            if (sourceData.ContainsKey("QRBridge"))
            {
                foreach (var kvp in sourceData["QRBridge"])
                {
                    flatSourceData[kvp.Key] = kvp.Value;
                }
            }

            // Apply mappings (void method, no return value)
            _tagMapper!.MapToDataset(dataset, flatSourceData, mappingRules);

            _logger.LogInformation("Mapping completed successfully");

            // Add any essential tags that weren't mapped
            EnsureEssentialTags(dataset, metadata);

            return dataset;
        }

        private Dictionary<string, Dictionary<string, string>> PrepareSourceData(ImageMetadata metadata)
        {
            var sourceData = new Dictionary<string, Dictionary<string, string>>();

            // Prepare QRBridge data
            var qrBridgeData = new Dictionary<string, string>();

            // Extract from parsed user comment if available
            if (metadata.ExifData != null && metadata.ExifData.TryGetValue("QRBridgeData", out var qrData))
            {
                // Assume it's already parsed into key-value pairs
                var parts = qrData.Split('|');
                if (parts.Length >= 1) qrBridgeData["examid"] = parts[0];
                if (parts.Length >= 2) qrBridgeData["name"] = parts[1];
                if (parts.Length >= 3) qrBridgeData["birthdate"] = parts[2];
                if (parts.Length >= 4) qrBridgeData["gender"] = parts[3];
                if (parts.Length >= 5) qrBridgeData["comment"] = parts[4];
            }

            // Or use metadata directly
            qrBridgeData["name"] = metadata.Patient.Name;
            qrBridgeData["patientid"] = metadata.Patient.Id.Value;
            if (metadata.Patient.BirthDate.HasValue)
                qrBridgeData["birthdate"] = metadata.Patient.BirthDate.Value.ToString("yyyy-MM-dd");
            qrBridgeData["gender"] = metadata.Patient.Gender.ToString()[0].ToString();
            qrBridgeData["examid"] = metadata.Study.ExamId ?? metadata.Study.StudyId.Value;
            if (!string.IsNullOrEmpty(metadata.Study.Comment))
                qrBridgeData["comment"] = metadata.Study.Comment;

            sourceData["QRBridge"] = qrBridgeData;

            // Prepare EXIF data
            if (metadata.ExifData != null)
            {
                sourceData["EXIF"] = new Dictionary<string, string>(metadata.ExifData);
            }
            else
            {
                sourceData["EXIF"] = new Dictionary<string, string>();
            }

            // Add technical data to EXIF if not already present
            if (metadata.TechnicalData != null)
            {
                if (metadata.TechnicalData.Manufacturer != null)
                    sourceData["EXIF"]["Make"] = metadata.TechnicalData.Manufacturer;
                if (metadata.TechnicalData.Model != null)
                    sourceData["EXIF"]["Model"] = metadata.TechnicalData.Model;
                if (metadata.TechnicalData.Software != null)
                    sourceData["EXIF"]["Software"] = metadata.TechnicalData.Software;
            }

            return sourceData;
        }

        private void AddBasicTechnicalTags(DicomDataset dataset, ImageMetadata metadata, int width, int height)
        {
            // File Meta Information
            dataset.Add(DicomTag.SpecificCharacterSet, "ISO_IR 100"); // Latin-1 for Umlauts

            // Essential Study/Series UIDs (generate if not mapped)
            dataset.Add(DicomTag.StudyInstanceUID, DicomUID.Generate());
            dataset.Add(DicomTag.SeriesInstanceUID, DicomUID.Generate());

            // Dates and times (will be overwritten if mapped)
            dataset.Add(DicomTag.StudyDate, metadata.Study.StudyDate.ToString("yyyyMMdd"));
            dataset.Add(DicomTag.StudyTime, metadata.Study.StudyDate.ToString("HHmmss"));
            dataset.Add(DicomTag.SeriesDate, metadata.Study.StudyDate.ToString("yyyyMMdd"));
            dataset.Add(DicomTag.SeriesTime, metadata.Study.StudyDate.ToString("HHmmss"));

            // Series Module defaults
            dataset.Add(DicomTag.Modality, metadata.Study.Modality);
            dataset.Add(DicomTag.SeriesNumber, "1");
            dataset.Add(DicomTag.SeriesDescription, "VL Photographic Image");

            // General Equipment Module defaults
            dataset.Add(DicomTag.StationName, Environment.MachineName);
            dataset.Add(DicomTag.ConversionType, "DI"); // Digital Interface

            // General Image Module
            dataset.Add(DicomTag.InstanceNumber, metadata.InstanceNumber.ToString());
            dataset.Add(DicomTag.ContentDate, metadata.CaptureDateTime.ToString("yyyyMMdd"));
            dataset.Add(DicomTag.ContentTime, metadata.CaptureDateTime.ToString("HHmmss.ffffff"));
            dataset.Add(DicomTag.ImageType, "DERIVED", "PRIMARY");

            // Image Pixel Module (technical requirements)
            dataset.Add(DicomTag.SamplesPerPixel, (ushort)3); // Color image
            dataset.Add(DicomTag.PhotometricInterpretation, "YBR_FULL_422"); // Required for JPEG
            dataset.Add(DicomTag.Rows, (ushort)height);
            dataset.Add(DicomTag.Columns, (ushort)width);
            dataset.Add(DicomTag.BitsAllocated, (ushort)8);
            dataset.Add(DicomTag.BitsStored, (ushort)8);
            dataset.Add(DicomTag.HighBit, (ushort)7);
            dataset.Add(DicomTag.PixelRepresentation, (ushort)0); // Unsigned
            dataset.Add(DicomTag.PlanarConfiguration, (ushort)0); // Color-by-pixel

            // SOP Common Module
            dataset.Add(DicomTag.SOPClassUID, PHOTOGRAPHIC_SOP_CLASS_UID);
            dataset.Add(DicomTag.SOPInstanceUID, metadata.InstanceUid);
        }

        private void EnsureEssentialTags(DicomDataset dataset, ImageMetadata metadata)
        {
            // Ensure critical patient identifiers exist
            if (!dataset.Contains(DicomTag.PatientName))
            {
                dataset.Add(DicomTag.PatientName, metadata.Patient.Name);
                _logger.LogWarning("Patient Name was not mapped, using fallback");
            }

            if (!dataset.Contains(DicomTag.PatientID))
            {
                dataset.Add(DicomTag.PatientID, metadata.Patient.Id.Value);
                _logger.LogWarning("Patient ID was not mapped, using fallback");
            }

            // Ensure Study ID exists
            if (!dataset.Contains(DicomTag.StudyID))
            {
                dataset.Add(DicomTag.StudyID, metadata.Study.StudyId.Value);
            }
        }

        // Legacy method for backward compatibility
        private DicomDataset CreateDicomDatasetLegacy(ImageMetadata metadata, int width, int height)
        {
            var dataset = new DicomDataset();

            // File Meta Information
            dataset.Add(DicomTag.SpecificCharacterSet, "ISO_IR 100"); // Latin-1 for Umlauts

            // Patient Module (Type 2 - must be present, may be empty)
            dataset.Add(DicomTag.PatientName, metadata.Patient.Name);
            dataset.Add(DicomTag.PatientID, metadata.Patient.Id.Value);
            dataset.Add(DicomTag.PatientBirthDate, metadata.Patient.BirthDate?.ToString("yyyyMMdd") ?? "");
            dataset.Add(DicomTag.PatientSex, GetDicomGender(metadata.Patient.Gender));

            // General Study Module
            dataset.Add(DicomTag.StudyInstanceUID, DicomUID.Generate());
            dataset.Add(DicomTag.StudyDate, metadata.Study.StudyDate.ToString("yyyyMMdd"));
            dataset.Add(DicomTag.StudyTime, metadata.Study.StudyDate.ToString("HHmmss"));
            dataset.Add(DicomTag.AccessionNumber, metadata.Study.AccessionNumber ?? "");
            dataset.Add(DicomTag.ReferringPhysicianName, metadata.Study.ReferringPhysician ?? "");
            dataset.Add(DicomTag.StudyID, metadata.Study.StudyId.Value);
            dataset.Add(DicomTag.StudyDescription, metadata.Study.Description ?? "");

            // General Series Module
            dataset.Add(DicomTag.SeriesInstanceUID, DicomUID.Generate());
            dataset.Add(DicomTag.Modality, metadata.Study.Modality);
            dataset.Add(DicomTag.SeriesNumber, "1");
            dataset.Add(DicomTag.SeriesDate, metadata.Study.StudyDate.ToString("yyyyMMdd"));
            dataset.Add(DicomTag.SeriesTime, metadata.Study.StudyDate.ToString("HHmmss"));
            dataset.Add(DicomTag.SeriesDescription, "VL Photographic Image");
            // PerformingPhysicianName removed - not part of StudyInfo

            // General Equipment Module
            dataset.Add(DicomTag.Manufacturer, metadata.TechnicalData?.Manufacturer ?? "Unknown");
            dataset.Add(DicomTag.ManufacturerModelName, metadata.TechnicalData?.Model ?? "");
            dataset.Add(DicomTag.SoftwareVersions, metadata.TechnicalData?.Software ?? "");
            dataset.Add(DicomTag.StationName, Environment.MachineName);

            // SC Equipment Module
            dataset.Add(DicomTag.ConversionType, "DI"); // Digital Interface

            // General Image Module
            dataset.Add(DicomTag.InstanceNumber, metadata.InstanceNumber.ToString());
            dataset.Add(DicomTag.ContentDate, metadata.CaptureDateTime.ToString("yyyyMMdd"));
            dataset.Add(DicomTag.ContentTime, metadata.CaptureDateTime.ToString("HHmmss.ffffff"));
            dataset.Add(DicomTag.ImageType, "DERIVED", "PRIMARY");

            // Image Pixel Module
            dataset.Add(DicomTag.SamplesPerPixel, (ushort)3); // Color image
            dataset.Add(DicomTag.PhotometricInterpretation, "YBR_FULL_422"); // Required for JPEG
            dataset.Add(DicomTag.Rows, (ushort)height);
            dataset.Add(DicomTag.Columns, (ushort)width);
            dataset.Add(DicomTag.BitsAllocated, (ushort)8);
            dataset.Add(DicomTag.BitsStored, (ushort)8);
            dataset.Add(DicomTag.HighBit, (ushort)7);
            dataset.Add(DicomTag.PixelRepresentation, (ushort)0); // Unsigned
            dataset.Add(DicomTag.PlanarConfiguration, (ushort)0); // Color-by-pixel

            // SOP Common Module
            dataset.Add(DicomTag.SOPClassUID, PHOTOGRAPHIC_SOP_CLASS_UID);
            dataset.Add(DicomTag.SOPInstanceUID, metadata.InstanceUid);

            // VL Photographic Image Module specific
            dataset.Add(DicomTag.ImageComments, metadata.Study.Comment ?? "");

            return dataset;
        }

        private DicomFile CreateDicomFileWithJpegData(DicomDataset dataset, byte[] jpegBytes)
        {
            // Create DICOM file with proper meta information
            var file = new DicomFile(dataset);

            // Set transfer syntax to JPEG Baseline
            file.FileMetaInfo.TransferSyntax = DicomTransferSyntax.JPEGProcess1;
            file.FileMetaInfo.MediaStorageSOPClassUID = DicomUID.Parse(PHOTOGRAPHIC_SOP_CLASS_UID);
            file.FileMetaInfo.MediaStorageSOPInstanceUID = DicomUID.Parse(dataset.GetString(DicomTag.SOPInstanceUID));
            file.FileMetaInfo.ImplementationClassUID = DicomUID.Parse(IMPLEMENTATION_CLASS_UID);
            file.FileMetaInfo.ImplementationVersionName = IMPLEMENTATION_VERSION_NAME;

            // Create encapsulated pixel data
            // CRITICAL: Do NOT decompress JPEG - keep it compressed!
            var pixelData = new DicomOtherByteFragment(DicomTag.PixelData);

            // Add basic offset table (empty for single frame)
            pixelData.Fragments.Add(new MemoryByteBuffer(new byte[0]));

            // Add JPEG data as fragment
            pixelData.Fragments.Add(new MemoryByteBuffer(jpegBytes));

            // Add to dataset
            dataset.AddOrUpdate(pixelData);

            return file;
        }

        [SupportedOSPlatform("windows")]
        private async Task<(int width, int height)> GetImageDimensionsAsync(string jpegPath, ImageMetadata metadata)
        {
            // First try to get from metadata
            if (metadata.TechnicalData != null &&
                metadata.TechnicalData.ImageWidth.HasValue &&
                metadata.TechnicalData.ImageHeight.HasValue)
            {
                return (metadata.TechnicalData.ImageWidth.Value, metadata.TechnicalData.ImageHeight.Value);
            }

            // Otherwise read from file
            return await Task.Run(() =>
            {
                using (var image = Image.FromFile(jpegPath))
                {
                    return (image.Width, image.Height);
                }
            });
        }

        private string GetDicomGender(Gender gender)
        {
            return gender switch
            {
                Gender.Male => "M",
                Gender.Female => "F",
                Gender.Other => "O",
                _ => "O"
            };
        }

        private void ValidateTag(DicomDataset dataset, FellowOakDicom.DicomTag tag, string tagName,
            List<string> errors, List<string> warnings, bool isType1 = false)
        {
            if (!dataset.Contains(tag))
            {
                if (isType1)
                    errors.Add($"Missing mandatory Type 1 tag: {tagName} {tag}");
                else
                    warnings.Add($"Missing Type 2 tag: {tagName} {tag}");
            }
            else if (isType1)
            {
                var value = dataset.GetString(tag);
                if (string.IsNullOrWhiteSpace(value))
                {
                    errors.Add($"Empty Type 1 tag: {tagName} {tag}");
                }
            }
        }
    }
}
