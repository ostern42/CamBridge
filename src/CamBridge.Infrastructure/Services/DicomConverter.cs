// src/CamBridge.Infrastructure/Services/DicomConverter.cs
// Version: 0.8.10
// Description: DICOM conversion service with enhanced error handling and user-friendly messages
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Entities;
using CamBridge.Core.Interfaces;
using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.IO.Buffer;
using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Service responsible for converting JPEG images to DICOM format
    /// </summary>
    public class DicomConverter
    {
        private readonly ILogger<DicomConverter> _logger;
        private readonly IDicomTagMapper? _tagMapper;
        private readonly IMappingConfiguration? _mappingConfiguration;
        private readonly DicomSettings _dicomSettings;

        /// <summary>
        /// Creates a DicomConverter with optional tag mapping support
        /// </summary>
        public DicomConverter(
            ILogger<DicomConverter> logger,
            IDicomTagMapper? tagMapper = null,
            IMappingConfiguration? mappingConfiguration = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tagMapper = tagMapper;
            _mappingConfiguration = mappingConfiguration;

            // Default DICOM settings if not injected
            _dicomSettings = new DicomSettings
            {
                InstitutionName = "CamBridge Medical Imaging",
                InstitutionDepartment = "Radiology",
                StationName = "CAMBRIDGE01",
                SourceApplicationEntityTitle = "CAMBRIDGE",
                ImplementationVersionName = "CAMBRIDGE_0.8.10",
                ImplementationClassUid = "1.2.276.0.7230010.3.0.3.8.10",
                Modality = "XC",
                ValidateAfterCreation = true
            };
        }

        /// <summary>
        /// Creates a DicomConverter with specific DICOM settings
        /// </summary>
        public DicomConverter(
            ILogger<DicomConverter> logger,
            DicomSettings dicomSettings,
            IDicomTagMapper? tagMapper = null,
            IMappingConfiguration? mappingConfiguration = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dicomSettings = dicomSettings ?? throw new ArgumentNullException(nameof(dicomSettings));
            _tagMapper = tagMapper;
            _mappingConfiguration = mappingConfiguration;
        }

        /// <summary>
        /// Converts a JPEG file to DICOM format
        /// </summary>
        public async Task<ConversionResult> ConvertToDicomAsync(
            string sourceJpegPath,
            string outputDicomPath,
            ImageMetadata metadata,
            string? correlationId = null)
        {
            if (string.IsNullOrWhiteSpace(sourceJpegPath))
                throw new ArgumentException("Source JPEG path cannot be empty", nameof(sourceJpegPath));

            if (string.IsNullOrWhiteSpace(outputDicomPath))
                throw new ArgumentException("Output DICOM path cannot be empty", nameof(outputDicomPath));

            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            _logger.LogInformation("[{CorrelationId}] [DicomConversion] Starting JPEG to DICOM conversion: {Source} -> {Target}",
                correlationId ?? "NO-ID", Path.GetFileName(sourceJpegPath), Path.GetFileName(outputDicomPath));

            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Validate source file exists
                if (!File.Exists(sourceJpegPath))
                {
                    throw new FileNotFoundException($"Source JPEG file not found: {sourceJpegPath}");
                }

                // Ensure output directory exists
                var outputDir = Path.GetDirectoryName(outputDicomPath);
                if (!string.IsNullOrEmpty(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                // Create DICOM dataset with JPEG transfer syntax
                var dicomFile = new DicomFile();
                var dataset = new DicomDataset(DicomTransferSyntax.JPEGProcess1);

                // Add required DICOM tags
                AddRequiredDicomTags(dataset, metadata);

                // Apply custom mappings if available
                if (_tagMapper != null && _mappingConfiguration != null)
                {
                    try
                    {
                        _logger.LogDebug("[{CorrelationId}] [DicomConversion] Applying custom tag mappings",
                            correlationId ?? "NO-ID");

                        // Convert metadata to dictionary for mapping
                        var sourceData = CreateSourceDataDictionary(metadata);
                        var mappingRules = _mappingConfiguration.GetMappingRules();

                        // FIXED: Pass correlationId to mapper!
                        if (_tagMapper is DicomTagMapper mapper)
                        {
                            // Use the overloaded method with correlationId
                            mapper.MapToDataset(dataset, sourceData, mappingRules, correlationId);
                        }
                        else
                        {
                            // Fallback to interface method without correlationId
                            _tagMapper.MapToDataset(dataset, sourceData, mappingRules);
                        }

                        _logger.LogDebug("[{CorrelationId}] [DicomConversion] Applied {Count} mapping rules",
                            correlationId ?? "NO-ID", mappingRules.Count());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "[{CorrelationId}] [DicomConversion] Failed to apply custom mappings, continuing with default tags",
                            correlationId ?? "NO-ID");
                        // Continue with default tags only
                    }
                }

                // Load JPEG and add as pixel data
                await AddJpegAsPixelDataAsync(dataset, sourceJpegPath);

                // Set file meta info
                dicomFile.FileMetaInfo.TransferSyntax = DicomTransferSyntax.JPEGProcess1;
                dicomFile.FileMetaInfo.MediaStorageSOPClassUID = DicomUID.SecondaryCaptureImageStorage;
                dicomFile.FileMetaInfo.MediaStorageSOPInstanceUID = dataset.GetSingleValue<DicomUID>(DicomTag.SOPInstanceUID);
                dicomFile.FileMetaInfo.ImplementationClassUID = DicomUID.Parse(_dicomSettings.ImplementationClassUid);
                dicomFile.FileMetaInfo.ImplementationVersionName = _dicomSettings.ImplementationVersionName;

                // Add dataset to file
                dicomFile.Dataset.AddOrUpdate(dataset);

                // Validate if configured
                if (_dicomSettings.ValidateAfterCreation)
                {
                    var validationResult = ValidateDicomFile(dicomFile);
                    if (!validationResult.IsValid)
                    {
                        _logger.LogWarning("[{CorrelationId}] [DicomConversion] DICOM validation warnings: {Warnings}",
                            correlationId ?? "NO-ID", string.Join(", ", validationResult.Warnings));
                    }
                }

                // Save DICOM file
                await dicomFile.SaveAsync(outputDicomPath);

                stopwatch.Stop();

                var fileInfo = new FileInfo(outputDicomPath);
                _logger.LogInformation("[{CorrelationId}] [DicomConversion] DICOM conversion successful in {ElapsedMs}ms, output size: {FileSize} bytes",
                    correlationId ?? "NO-ID", stopwatch.ElapsedMilliseconds, fileInfo.Length);

                return new ConversionResult
                {
                    Success = true,
                    DicomFilePath = outputDicomPath,
                    FileSizeBytes = fileInfo.Length,
                    ProcessingTime = stopwatch.Elapsed
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[{CorrelationId}] [DicomConversion] DICOM conversion failed after {ElapsedMs}ms",
                    correlationId ?? "NO-ID", stopwatch.ElapsedMilliseconds);

                return new ConversionResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    ProcessingTime = stopwatch.Elapsed
                };
            }
        }

        /// <summary>
        /// Adds required DICOM tags to the dataset
        /// </summary>
        private void AddRequiredDicomTags(DicomDataset dataset, ImageMetadata metadata)
        {
            // Patient Module
            if (metadata.Patient != null)
            {
                dataset.Add(DicomTag.PatientName, metadata.Patient.PatientName);
                dataset.Add(DicomTag.PatientID, metadata.Patient.PatientId.ToString());

                if (metadata.Patient.BirthDate.HasValue)
                {
                    dataset.Add(DicomTag.PatientBirthDate, metadata.Patient.BirthDate.Value);
                }

                dataset.Add(DicomTag.PatientSex, metadata.Patient.Gender.ToString().Substring(0, 1).ToUpperInvariant());
            }

            // Study Module
            if (metadata.Study != null)
            {
                dataset.Add(DicomTag.StudyInstanceUID, DicomUID.Generate());
                dataset.Add(DicomTag.StudyID, metadata.Study.StudyId.ToString());
                dataset.Add(DicomTag.StudyDate, metadata.Study.StudyDate);
                dataset.Add(DicomTag.StudyTime, metadata.Study.StudyDate);
                dataset.Add(DicomTag.StudyDescription, metadata.Study.Description ?? string.Empty);
                dataset.Add(DicomTag.AccessionNumber, metadata.Study.AccessionNumber ?? string.Empty);
                dataset.Add(DicomTag.ReferringPhysicianName, metadata.Study.ReferringPhysician ?? string.Empty);
            }

            // Series Module
            dataset.Add(DicomTag.SeriesInstanceUID, DicomUID.Generate());
            dataset.Add(DicomTag.SeriesNumber, "1");
            dataset.Add(DicomTag.Modality, _dicomSettings.Modality);
            dataset.Add(DicomTag.SeriesDescription, "CamBridge JPEG to DICOM Conversion");

            // General Image Module
            dataset.Add(DicomTag.InstanceNumber, metadata.InstanceNumber.ToString());
            dataset.Add(DicomTag.ContentDate, metadata.CaptureDateTime);
            dataset.Add(DicomTag.ContentTime, metadata.CaptureDateTime);
            dataset.Add(DicomTag.AcquisitionDateTime, metadata.CaptureDateTime);

            // SOP Common Module
            dataset.Add(DicomTag.SOPClassUID, DicomUID.SecondaryCaptureImageStorage);
            dataset.Add(DicomTag.SOPInstanceUID, string.IsNullOrEmpty(metadata.InstanceUid) ? DicomUID.Generate() : DicomUID.Parse(metadata.InstanceUid));

            // Equipment Module
            dataset.Add(DicomTag.Manufacturer, metadata.TechnicalData?.Manufacturer ?? "Unknown");
            dataset.Add(DicomTag.ManufacturerModelName, metadata.TechnicalData?.Model ?? "Unknown");
            dataset.Add(DicomTag.StationName, _dicomSettings.StationName);
            dataset.Add(DicomTag.InstitutionName, _dicomSettings.InstitutionName);
            dataset.Add(DicomTag.InstitutionalDepartmentName, _dicomSettings.InstitutionDepartment);

            // SC Equipment Module
            dataset.Add(DicomTag.ConversionType, "WSD"); // Workstation
            dataset.Add(DicomTag.SecondaryCaptureDeviceManufacturer, "Claude's Improbably Reliable Software Solutions");
            dataset.Add(DicomTag.SecondaryCaptureDeviceManufacturerModelName, "CamBridge");
            dataset.Add(DicomTag.SecondaryCaptureDeviceSoftwareVersions, _dicomSettings.ImplementationVersionName);

            // Image Pixel Module
            dataset.Add(DicomTag.SamplesPerPixel, (ushort)3);
            dataset.Add(DicomTag.PhotometricInterpretation, PhotometricInterpretation.Rgb.Value);
            dataset.Add(DicomTag.PlanarConfiguration, (ushort)0);

            // Add barcode data as private tag if present
            if (!string.IsNullOrEmpty(metadata.BarcodeData))
            {
                // Private Creator
                dataset.Add(new DicomTag(0x0009, 0x0010), "CAMBRIDGE");
                // Private Tag for Barcode
                dataset.Add(new DicomTag(0x0009, 0x1001), metadata.BarcodeData);
            }
        }

        /// <summary>
        /// Creates a dictionary from metadata for tag mapping
        /// </summary>
        private Dictionary<string, string> CreateSourceDataDictionary(ImageMetadata metadata)
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Patient data
            if (metadata.Patient != null)
            {
                data["PatientName"] = metadata.Patient.PatientName;
                data["PatientID"] = metadata.Patient.PatientId.ToString();
                data["PatientBirthDate"] = metadata.Patient.BirthDate?.ToString("yyyyMMdd") ?? "";
                data["PatientSex"] = metadata.Patient.Gender.ToString().Substring(0, 1).ToUpperInvariant();
            }

            // Study data
            if (metadata.Study != null)
            {
                data["StudyID"] = metadata.Study.StudyId.ToString();
                data["StudyDescription"] = metadata.Study.Description ?? "";
                data["StudyDate"] = metadata.Study.StudyDate.ToString("yyyyMMdd");
                data["StudyTime"] = metadata.Study.StudyDate.ToString("HHmmss");
                data["AccessionNumber"] = metadata.Study.AccessionNumber ?? "";
                data["ReferringPhysicianName"] = metadata.Study.ReferringPhysician ?? "";
                data["ExamId"] = metadata.Study.ExamId ?? "";
            }

            // Technical data
            if (metadata.TechnicalData != null)
            {
                data["Manufacturer"] = metadata.TechnicalData.Manufacturer ?? "";
                data["ManufacturerModelName"] = metadata.TechnicalData.Model ?? "";
                data["ImageWidth"] = metadata.TechnicalData.ImageWidth.ToString();
                data["ImageHeight"] = metadata.TechnicalData.ImageHeight.ToString();
            }

            // General data
            data["CaptureDateTime"] = metadata.CaptureDateTime.ToString("yyyyMMddHHmmss");
            data["CaptureDate"] = metadata.CaptureDateTime.ToString("yyyyMMdd");
            data["CaptureTime"] = metadata.CaptureDateTime.ToString("HHmmss");
            data["InstanceNumber"] = metadata.InstanceNumber.ToString();
            data["SourceFilePath"] = metadata.SourceFilePath;
            data["BarcodeData"] = metadata.BarcodeData ?? "";
            data["UserComment"] = metadata.UserComment ?? "";

            // Add all EXIF data
            foreach (var kvp in metadata.ExifData)
            {
                // Avoid overwriting existing keys
                if (!data.ContainsKey(kvp.Key))
                {
                    data[kvp.Key] = kvp.Value;
                }
            }

            return data;
        }

        /// <summary>
        /// Adds JPEG image as pixel data to DICOM dataset
        /// </summary>
        private async Task AddJpegAsPixelDataAsync(DicomDataset dataset, string jpegPath)
        {
            // Read JPEG file
            var jpegBytes = await File.ReadAllBytesAsync(jpegPath);

            // Get image dimensions from JPEG
            // For JPEG, we can read the dimensions from the JPEG header
            // This is a simplified approach - in production you might use a library
            var (width, height) = GetJpegDimensions(jpegBytes);

            // Add image dimensions
            dataset.Add(DicomTag.Rows, (ushort)height);
            dataset.Add(DicomTag.Columns, (ushort)width);
            dataset.Add(DicomTag.BitsAllocated, (ushort)8);
            dataset.Add(DicomTag.BitsStored, (ushort)8);
            dataset.Add(DicomTag.HighBit, (ushort)7);
            dataset.Add(DicomTag.PixelRepresentation, (ushort)0);

            // Create pixel data element with JPEG data
            var pixelData = new DicomOtherByteFragment(DicomTag.PixelData);

            // Add offset table (required for encapsulated data)
            pixelData.Fragments.Add(new MemoryByteBuffer(new byte[0]));

            // Add JPEG data as fragment
            pixelData.Fragments.Add(new MemoryByteBuffer(jpegBytes));

            dataset.AddOrUpdate(pixelData);
        }

        /// <summary>
        /// Gets JPEG dimensions by reading the JPEG header
        /// </summary>
        private (int width, int height) GetJpegDimensions(byte[] jpegData)
        {
            // Default dimensions if we can't read them
            int width = 0;
            int height = 0;

            // JPEG starts with FFD8
            if (jpegData.Length < 2 || jpegData[0] != 0xFF || jpegData[1] != 0xD8)
                return (width, height);

            int offset = 2;
            while (offset < jpegData.Length - 9)
            {
                if (jpegData[offset] != 0xFF)
                {
                    offset++;
                    continue;
                }

                byte marker = jpegData[offset + 1];

                // Skip any padding FF bytes
                if (marker == 0xFF)
                {
                    offset++;
                    continue;
                }

                offset += 2;

                // Check for SOF markers (C0-CF, except C4, C8, CC)
                if ((marker >= 0xC0 && marker <= 0xCF) &&
                    marker != 0xC4 && marker != 0xC8 && marker != 0xCC)
                {
                    // Skip length (2 bytes) and precision (1 byte)
                    offset += 3;

                    // Read dimensions
                    height = (jpegData[offset] << 8) | jpegData[offset + 1];
                    width = (jpegData[offset + 2] << 8) | jpegData[offset + 3];
                    break;
                }
                else
                {
                    // Read the length and skip this segment
                    int segmentLength = (jpegData[offset] << 8) | jpegData[offset + 1];
                    offset += segmentLength;
                }
            }

            // Fallback to reasonable defaults if parsing fails
            if (width == 0 || height == 0)
            {
                _logger.LogWarning("Failed to read JPEG dimensions, using defaults");
                width = 1024;
                height = 768;
            }

            return (width, height);
        }

        /// <summary>
        /// Validates DICOM file for common issues
        /// </summary>
        private ValidationResult ValidateDicomFile(DicomFile dicomFile)
        {
            var result = new ValidationResult { IsValid = true };
            var warnings = new List<string>();

            // Check required patient tags
            if (!dicomFile.Dataset.Contains(DicomTag.PatientName))
                warnings.Add("Missing PatientName");

            if (!dicomFile.Dataset.Contains(DicomTag.PatientID))
                warnings.Add("Missing PatientID");

            // Check required study tags
            if (!dicomFile.Dataset.Contains(DicomTag.StudyInstanceUID))
                warnings.Add("Missing StudyInstanceUID");

            // Check image data
            if (!dicomFile.Dataset.Contains(DicomTag.PixelData))
            {
                warnings.Add("Missing PixelData");
                result.IsValid = false;
            }

            // Check transfer syntax
            if (dicomFile.FileMetaInfo.TransferSyntax != DicomTransferSyntax.JPEGProcess1)
            {
                warnings.Add($"Unexpected transfer syntax: {dicomFile.FileMetaInfo.TransferSyntax}");
            }

            result.Warnings = warnings;
            return result;
        }
    }

    /// <summary>
    /// Result of DICOM conversion operation
    /// </summary>
    public class ConversionResult
    {
        public bool Success { get; set; }
        public string? DicomFilePath { get; set; }
        public string? ErrorMessage { get; set; }
        public long FileSizeBytes { get; set; }
        public TimeSpan ProcessingTime { get; set; }
    }

    /// <summary>
    /// Result of DICOM validation
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
