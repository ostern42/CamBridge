// src/CamBridge.Infrastructure/Services/DicomConverter.cs
// Version: 0.7.31
// Description: DICOM converter with FIXED property names and JPEG encapsulation
// Copyright: Â© 2025 Claude's Improbably Reliable Software Solutions

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
    /// DICOM converter using fo-dicom library
    /// Converts JPEG images to DICOM format while preserving JPEG compression
    /// v0.7.31: FIXED property names and JPEG encapsulation
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class DicomConverter
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

        /// <summary>
        /// Converts a JPEG file to DICOM using the provided metadata
        /// </summary>
        public async Task<ConversionResult> ConvertToDicomAsync(
            string sourceJpegPath,
            string destinationDicomPath,
            ImageMetadata metadata)
        {
            try
            {
                _logger.LogInformation("Converting JPEG to DICOM: {Source} â†’ {Destination}",
                    sourceJpegPath, destinationDicomPath);

                // Validate inputs
                if (!File.Exists(sourceJpegPath))
                {
                    return ConversionResult.CreateFailure($"Source file not found: {sourceJpegPath}");
                }

                // Load JPEG data
                var jpegData = await File.ReadAllBytesAsync(sourceJpegPath);

                // Create DICOM dataset with JPEG transfer syntax for proper encapsulation
                var dataset = CreateDicomDataset(metadata, jpegData);

                // Map custom tags if mapper is available
                if (_tagMapper != null && _mappingConfiguration != null)
                {
                    var mappingRules = _mappingConfiguration.GetMappingRules();

                    if (mappingRules.Any())
                    {
                        _tagMapper.MapToDataset(dataset, metadata.ExifData, mappingRules);
                    }
                }

                // Create DICOM file with correct structure
                var dicomFile = new DicomFile(dataset);

                // CRITICAL FIX: Set Transfer Syntax on FileMetaInfo!
                dicomFile.FileMetaInfo.TransferSyntax = DicomTransferSyntax.JPEGProcess1;

                // Also ensure File Meta Information has required tags
                dicomFile.FileMetaInfo.MediaStorageSOPClassUID = dataset.GetSingleValue<DicomUID>(DicomTag.SOPClassUID);
                dicomFile.FileMetaInfo.MediaStorageSOPInstanceUID = dataset.GetSingleValue<DicomUID>(DicomTag.SOPInstanceUID);
                dicomFile.FileMetaInfo.ImplementationClassUID = DicomUID.Parse(IMPLEMENTATION_CLASS_UID);
                dicomFile.FileMetaInfo.ImplementationVersionName = IMPLEMENTATION_VERSION_NAME;

                // Ensure output directory exists
                var outputDir = Path.GetDirectoryName(destinationDicomPath);
                if (!string.IsNullOrEmpty(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                // Save DICOM file - fo-dicom should handle undefined length automatically
                // when dataset has JPEG transfer syntax
                await dicomFile.SaveAsync(destinationDicomPath);

                var fileInfo = new FileInfo(destinationDicomPath);
                var sopInstanceUid = dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID);

                _logger.LogInformation("Successfully created DICOM file: {Path} ({Size} bytes, Transfer Syntax: {TransferSyntax})",
                    destinationDicomPath, fileInfo.Length, dicomFile.FileMetaInfo.TransferSyntax.UID.UID);

                return ConversionResult.CreateSuccess(destinationDicomPath, sopInstanceUid, fileInfo.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to convert JPEG to DICOM");
                return ConversionResult.CreateFailure($"Conversion failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates if the generated DICOM file is compliant
        /// </summary>
        public async Task<ValidationResult> ValidateDicomFileAsync(string dicomFilePath)
        {
            try
            {
                if (!File.Exists(dicomFilePath))
                {
                    return ValidationResult.Invalid($"DICOM file not found: {dicomFilePath}");
                }

                // Load DICOM file for validation
                var dicomFile = await DicomFile.OpenAsync(dicomFilePath);
                var dataset = dicomFile.Dataset;

                var errors = new List<string>();
                var warnings = new List<string>();

                // Validate required tags
                ValidateRequiredTags(dataset, errors);

                // Validate image module
                ValidateImageModule(dataset, warnings);

                // Validate patient module
                ValidatePatientModule(dataset, warnings);

                // Validate study module
                ValidateStudyModule(dataset, warnings);

                // Check for critical errors
                if (errors.Any())
                {
                    return ValidationResult.Invalid(string.Join("; ", errors));
                }

                // Return valid with warnings if any
                if (warnings.Any())
                {
                    return ValidationResult.ValidWithWarnings(string.Join("; ", warnings));
                }

                return ValidationResult.Valid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate DICOM file: {Path}", dicomFilePath);
                return ValidationResult.Invalid($"Validation error: {ex.Message}");
            }
        }

        private DicomDataset CreateDicomDataset(ImageMetadata metadata, byte[] jpegData)
        {
            // CRITICAL FIX: Create dataset WITH transfer syntax for proper JPEG encapsulation!
            // This ensures fo-dicom uses undefined length for pixel data as required by DICOM standard
            var dataset = new DicomDataset(DicomTransferSyntax.JPEGProcess1);

            // Generate UIDs
            var studyInstanceUid = GenerateUid();
            var seriesInstanceUid = GenerateUid();
            var sopInstanceUid = GenerateUid();

            // SOP Common Module
            dataset.Add(DicomTag.SOPClassUID, PHOTOGRAPHIC_SOP_CLASS_UID);
            dataset.Add(DicomTag.SOPInstanceUID, sopInstanceUid);

            // Study Module - FIXED property names
            dataset.Add(DicomTag.StudyInstanceUID, studyInstanceUid);
            dataset.Add(DicomTag.StudyDate, metadata.Study.StudyDate.ToString("yyyyMMdd"));
            dataset.Add(DicomTag.StudyTime, metadata.Study.StudyDate.ToString("HHmmss.fff"));
            dataset.Add(DicomTag.StudyID, metadata.Study.StudyId.Value ?? "");
            dataset.Add(DicomTag.AccessionNumber, metadata.Study.AccessionNumber ?? "");
            dataset.Add(DicomTag.StudyDescription, metadata.Study.Description ?? "");

            // Series Module
            dataset.Add(DicomTag.SeriesInstanceUID, seriesInstanceUid);
            dataset.Add(DicomTag.SeriesNumber, "1"); // IS expects string!
            dataset.Add(DicomTag.Modality, metadata.Study.Modality ?? "OT"); // Other

            // Patient Module - FIXED property names
            dataset.Add(DicomTag.PatientName, metadata.Patient.Name ?? "");
            dataset.Add(DicomTag.PatientID, metadata.Patient.Id.Value ?? "");
            dataset.Add(DicomTag.PatientBirthDate, metadata.Patient.BirthDate?.ToString("yyyyMMdd") ?? "");
            dataset.Add(DicomTag.PatientSex, MapGenderToString(metadata.Patient.Gender) ?? "");

            // General Image Module
            dataset.Add(DicomTag.InstanceNumber, "1"); // IS expects string!
            dataset.Add(DicomTag.PatientOrientation, "");

            // Image Pixel Module - FIXED: No SamplesPerPixel property
            dataset.Add(DicomTag.SamplesPerPixel, (ushort)3); // Assume RGB JPEG
            dataset.Add(DicomTag.PhotometricInterpretation, "YBR_FULL_422"); // JPEG requires YBR!
            dataset.Add(DicomTag.Rows, (ushort)(metadata.TechnicalData.ImageHeight ?? 0));
            dataset.Add(DicomTag.Columns, (ushort)(metadata.TechnicalData.ImageWidth ?? 0));
            dataset.Add(DicomTag.BitsAllocated, (ushort)8);
            dataset.Add(DicomTag.BitsStored, (ushort)8);
            dataset.Add(DicomTag.HighBit, (ushort)7);
            dataset.Add(DicomTag.PixelRepresentation, (ushort)0);
            dataset.Add(DicomTag.PlanarConfiguration, (ushort)0);

            // Add JPEG data as encapsulated pixel data
            var pixelData = DicomPixelData.Create(dataset, true);
            var buffer = new MemoryByteBuffer(jpegData);
            pixelData.AddFrame(buffer);

            // General Equipment Module - FIXED property names
            dataset.Add(DicomTag.Manufacturer, metadata.TechnicalData.Manufacturer ?? "Unknown");
            dataset.Add(DicomTag.ManufacturerModelName, metadata.TechnicalData.Model ?? "Unknown");
            dataset.Add(DicomTag.SoftwareVersions, metadata.TechnicalData.Software ?? "CamBridge");

            // Add character set for proper encoding (UTF-8)
            dataset.Add(DicomTag.SpecificCharacterSet, "ISO_IR 192");

            return dataset;
        }

        /// <summary>
        /// Generates a DICOM compliant UID with only numeric characters
        /// </summary>
        private string GenerateUid()
        {
            // Use shorter components to ensure we stay under 64 characters
            // Only numeric characters allowed in DICOM UIDs!
            var shortTicks = (DateTime.UtcNow.Ticks % 10000000000).ToString();
            var processId = (Environment.ProcessId % 10000).ToString();
            var random = new Random().Next(1000, 9999);

            var uid = $"{IMPLEMENTATION_CLASS_UID}.{shortTicks}.{processId}.{random}";

            // Ensure UID doesn't exceed 64 characters
            if (uid.Length > 64)
            {
                _logger.LogWarning("Generated UID too long ({Length} chars), truncating: {UID}",
                    uid.Length, uid);
                uid = uid.Substring(0, 64);
            }

            return uid;
        }

        private void ValidateRequiredTags(DicomDataset dataset, List<string> errors)
        {
            // Check for essential tags
            if (!dataset.Contains(DicomTag.SOPClassUID))
                errors.Add("Missing SOP Class UID");

            if (!dataset.Contains(DicomTag.SOPInstanceUID))
                errors.Add("Missing SOP Instance UID");

            if (!dataset.Contains(DicomTag.StudyInstanceUID))
                errors.Add("Missing Study Instance UID");

            if (!dataset.Contains(DicomTag.SeriesInstanceUID))
                errors.Add("Missing Series Instance UID");

            if (!dataset.Contains(DicomTag.Rows) || !dataset.Contains(DicomTag.Columns))
                errors.Add("Missing image dimensions (Rows/Columns)");
        }

        private void ValidateImageModule(DicomDataset dataset, List<string> warnings)
        {
            // Validate pixel data exists
            try
            {
                var pixelData = DicomPixelData.Create(dataset);
                if (pixelData.NumberOfFrames == 0)
                {
                    warnings.Add("No pixel data frames found");
                }
            }
            catch
            {
                warnings.Add("Unable to read pixel data");
            }

            // Check photometric interpretation
            var photometric = dataset.GetSingleValueOrDefault<string>(DicomTag.PhotometricInterpretation, string.Empty);
            if (string.IsNullOrEmpty(photometric))
            {
                warnings.Add("Missing Photometric Interpretation");
            }
        }

        private void ValidatePatientModule(DicomDataset dataset, List<string> warnings)
        {
            if (!dataset.Contains(DicomTag.PatientName) || string.IsNullOrWhiteSpace(dataset.GetSingleValueOrDefault<string>(DicomTag.PatientName, string.Empty)))
                warnings.Add("Missing or empty Patient Name");

            if (!dataset.Contains(DicomTag.PatientID) || string.IsNullOrWhiteSpace(dataset.GetSingleValueOrDefault<string>(DicomTag.PatientID, string.Empty)))
                warnings.Add("Missing or empty Patient ID");
        }

        private void ValidateStudyModule(DicomDataset dataset, List<string> warnings)
        {
            if (!dataset.Contains(DicomTag.StudyDate))
                warnings.Add("Missing Study Date");

            if (!dataset.Contains(DicomTag.StudyTime))
                warnings.Add("Missing Study Time");
        }

        private string MapGenderToString(Gender gender)
        {
            return gender switch
            {
                Gender.Male => "M",
                Gender.Female => "F",
                Gender.Other => "O",
                _ => "O"
            };
        }
    }

    /// <summary>
    /// Result of DICOM conversion operation
    /// </summary>
    public class ConversionResult
    {
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }
        public string? DicomFilePath { get; init; }
        public string? SopInstanceUid { get; init; }
        public long FileSizeBytes { get; init; }

        public static ConversionResult CreateSuccess(string dicomFilePath, string sopInstanceUid, long fileSizeBytes)
        {
            return new ConversionResult
            {
                Success = true,
                DicomFilePath = dicomFilePath,
                SopInstanceUid = sopInstanceUid,
                FileSizeBytes = fileSizeBytes
            };
        }

        public static ConversionResult CreateFailure(string errorMessage)
        {
            return new ConversionResult
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }

    /// <summary>
    /// Result of DICOM validation
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; init; }
        public string? Message { get; init; }

        public static ValidationResult Valid() => new() { IsValid = true };
        public static ValidationResult ValidWithWarnings(string warnings) => new() { IsValid = true, Message = warnings };
        public static ValidationResult Invalid(string errors) => new() { IsValid = false, Message = errors };
    }
}
