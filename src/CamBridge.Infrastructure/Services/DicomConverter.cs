// src/CamBridge.Infrastructure/Services/DicomConverter.cs
// Version: 0.7.30
// Description: DICOM converter with FIXED Transfer Syntax for proper JPEG display
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

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
    /// v0.7.30: FIXED Transfer Syntax and Photometric Interpretation
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
                _logger.LogInformation("Converting JPEG to DICOM: {Source} → {Destination}",
                    sourceJpegPath, destinationDicomPath);

                // Validate inputs
                if (!File.Exists(sourceJpegPath))
                {
                    return ConversionResult.CreateFailure($"Source file not found: {sourceJpegPath}");
                }

                // Load JPEG data
                var jpegData = await File.ReadAllBytesAsync(sourceJpegPath);

                // Create DICOM dataset
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

                // Save DICOM file
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

                // Check Transfer Syntax
                if (dicomFile.FileMetaInfo.TransferSyntax?.UID.UID != JPEG_BASELINE_TRANSFER_SYNTAX_UID)
                {
                    warnings.Add($"Transfer Syntax is not JPEG Baseline: {dicomFile.FileMetaInfo.TransferSyntax?.UID.UID}");
                }

                if (errors.Count > 0)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Errors = errors,
                        Warnings = warnings
                    };
                }

                return new ValidationResult
                {
                    IsValid = true,
                    Warnings = warnings
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate DICOM file");
                return ValidationResult.Invalid($"Validation failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the SOP Class UID for photographic images
        /// </summary>
        public string GetPhotographicSopClassUid()
        {
            return PHOTOGRAPHIC_SOP_CLASS_UID;
        }

        private DicomDataset CreateDicomDataset(ImageMetadata metadata, byte[] jpegData)
        {
            var dataset = new DicomDataset();

            // REMOVED: Transfer Syntax from dataset (it goes in FileMetaInfo only!)

            // Set proper character encoding for German umlauts
            dataset.Add(DicomTag.SpecificCharacterSet, "ISO_IR 192"); // UTF-8

            // SOP Common Module
            dataset.Add(DicomTag.SOPClassUID, PHOTOGRAPHIC_SOP_CLASS_UID);
            dataset.Add(DicomTag.SOPInstanceUID, GenerateUID());

            // General Study Module
            dataset.Add(DicomTag.StudyInstanceUID, GenerateUID());
            dataset.Add(DicomTag.StudyDate, metadata.Study.StudyDate.ToString("yyyyMMdd"));
            dataset.Add(DicomTag.StudyTime, metadata.Study.StudyDate.ToString("HHmmss"));
            dataset.Add(DicomTag.StudyID, metadata.Study.StudyId.Value);
            dataset.Add(DicomTag.AccessionNumber, metadata.Study.AccessionNumber ?? "");
            dataset.Add(DicomTag.StudyDescription, metadata.Study.Description ?? "Photographic Image");
            dataset.Add(DicomTag.ReferringPhysicianName, metadata.Study.ReferringPhysician ?? "");

            // Patient Module
            dataset.Add(DicomTag.PatientName, metadata.Patient.Name);
            dataset.Add(DicomTag.PatientID, metadata.Patient.Id.Value);

            if (metadata.Patient.BirthDate.HasValue)
            {
                dataset.Add(DicomTag.PatientBirthDate, metadata.Patient.BirthDate.Value.ToString("yyyyMMdd"));
            }

            // Gender is NOT nullable - it's an enum!
            dataset.Add(DicomTag.PatientSex, GetDicomGender(metadata.Patient.Gender));

            // General Series Module
            dataset.Add(DicomTag.SeriesInstanceUID, GenerateUID());
            dataset.Add(DicomTag.SeriesNumber, "1");
            dataset.Add(DicomTag.SeriesDate, metadata.Study.StudyDate.ToString("yyyyMMdd"));
            dataset.Add(DicomTag.SeriesTime, metadata.Study.StudyDate.ToString("HHmmss"));
            dataset.Add(DicomTag.SeriesDescription, "Photographic Series");
            dataset.Add(DicomTag.Modality, "XC"); // Photographic Image

            // General Equipment Module - Use EXIF data if available
            dataset.Add(DicomTag.Manufacturer, metadata.ExifData.GetValueOrDefault("Make", "Unknown"));
            dataset.Add(DicomTag.ManufacturerModelName, metadata.ExifData.GetValueOrDefault("Model", "Unknown"));
            dataset.Add(DicomTag.DeviceSerialNumber, metadata.ExifData.GetValueOrDefault("SerialNumber", ""));
            dataset.Add(DicomTag.SoftwareVersions, IMPLEMENTATION_VERSION_NAME);

            // SC Equipment Module
            dataset.Add(DicomTag.ConversionType, "WSD"); // Workstation
            dataset.Add(DicomTag.SecondaryCaptureDeviceID, "CamBridge");
            dataset.Add(DicomTag.SecondaryCaptureDeviceManufacturer, "Claude's Improbably Reliable Software Solutions");
            dataset.Add(DicomTag.SecondaryCaptureDeviceManufacturerModelName, "CamBridge JPEG to DICOM Converter");
            dataset.Add(DicomTag.SecondaryCaptureDeviceSoftwareVersions, IMPLEMENTATION_VERSION_NAME);

            // General Image Module
            dataset.Add(DicomTag.InstanceNumber, metadata.InstanceNumber.ToString());
            dataset.Add(DicomTag.PatientOrientation, "");
            dataset.Add(DicomTag.ContentDate, metadata.Study.StudyDate.ToString("yyyyMMdd"));
            dataset.Add(DicomTag.ContentTime, metadata.Study.StudyDate.ToString("HHmmss"));
            dataset.Add(DicomTag.ImageType, "ORIGINAL\\PRIMARY");

            // Image Pixel Module
            AddImagePixelData(dataset, jpegData);

            // Implementation-specific tags
            dataset.Add(DicomTag.ImplementationClassUID, IMPLEMENTATION_CLASS_UID);
            dataset.Add(DicomTag.ImplementationVersionName, IMPLEMENTATION_VERSION_NAME);

            // Add comment with source file name if available
            var fileName = Path.GetFileName(metadata.SourceFilePath);
            var comment = $"Source: {fileName}";
            if (!string.IsNullOrEmpty(metadata.Study.Description))
            {
                comment += $" | {metadata.Study.Description}";
            }
            dataset.Add(DicomTag.ImageComments, comment);

            return dataset;
        }

        private void AddImagePixelData(DicomDataset dataset, byte[] jpegData)
        {
            // Read JPEG to get dimensions
            using (var ms = new MemoryStream(jpegData))
            using (var image = Image.FromStream(ms))
            {
                dataset.Add(DicomTag.Columns, (ushort)image.Width);
                dataset.Add(DicomTag.Rows, (ushort)image.Height);

                // FIXED: Set correct photometric interpretation for JPEG
                // YBR_FULL_422 is the standard for JPEG compressed images
                dataset.Add(DicomTag.PhotometricInterpretation, "YBR_FULL_422");

                // For JPEG, samples per pixel is typically 3 (color)
                dataset.Add(DicomTag.SamplesPerPixel, (ushort)3);
                dataset.Add(DicomTag.BitsAllocated, (ushort)8);
                dataset.Add(DicomTag.BitsStored, (ushort)8);
                dataset.Add(DicomTag.HighBit, (ushort)7);
                dataset.Add(DicomTag.PixelRepresentation, (ushort)0);

                // Planar configuration is 0 for JPEG (interleaved)
                dataset.Add(DicomTag.PlanarConfiguration, (ushort)0);
            }

            // Add encapsulated JPEG data
            var pixelData = DicomPixelData.Create(dataset, true); // true = encapsulated
            var buffer = new MemoryByteBuffer(jpegData);
            pixelData.AddFrame(buffer);
        }

        private void ValidateRequiredTags(DicomDataset dataset, List<string> errors)
        {
            var requiredTags = new[]
            {
                DicomTag.SOPClassUID,
                DicomTag.SOPInstanceUID,
                DicomTag.StudyInstanceUID,
                DicomTag.SeriesInstanceUID,
                DicomTag.PatientID,
                DicomTag.PatientName,
                DicomTag.Columns,
                DicomTag.Rows,
                DicomTag.BitsAllocated,
                DicomTag.BitsStored,
                DicomTag.HighBit,
                DicomTag.PixelRepresentation,
                DicomTag.PhotometricInterpretation,
                DicomTag.SamplesPerPixel
            };

            foreach (var tag in requiredTags)
            {
                if (!dataset.Contains(tag))
                {
                    errors.Add($"Required tag {tag} is missing");
                }
            }
        }

        private void ValidateImageModule(DicomDataset dataset, List<string> warnings)
        {
            if (!dataset.Contains(DicomTag.PixelData))
            {
                warnings.Add("No pixel data found");
            }

            // Check for JPEG compression indicators
            if (dataset.TryGetString(DicomTag.PhotometricInterpretation, out var photometric))
            {
                if (photometric != "YBR_FULL_422" && photometric != "YBR_FULL")
                {
                    warnings.Add($"Photometric Interpretation '{photometric}' may not be optimal for JPEG");
                }
            }
        }

        private void ValidatePatientModule(DicomDataset dataset, List<string> warnings)
        {
            if (!dataset.Contains(DicomTag.PatientBirthDate))
            {
                warnings.Add("Patient birth date is missing");
            }

            if (!dataset.Contains(DicomTag.PatientSex))
            {
                warnings.Add("Patient sex is missing");
            }
        }

        private void ValidateStudyModule(DicomDataset dataset, List<string> warnings)
        {
            if (!dataset.Contains(DicomTag.StudyDescription))
            {
                warnings.Add("Study description is missing");
            }

            if (!dataset.Contains(DicomTag.AccessionNumber))
            {
                warnings.Add("Accession number is missing");
            }
        }

        private string GenerateUID()
        {
            // Generate unique UID with our implementation prefix
            // DICOM UIDs: max 64 chars, only digits (0-9) and dots (.)

            // Our prefix is already 27 chars: "1.2.276.0.7230010.3.0.3.6.4"
            // That leaves us 37 chars for uniqueness

            var timestamp = DateTime.UtcNow;
            var ticks = timestamp.Ticks;

            // Use only last 10 digits of ticks to save space
            var shortTicks = (ticks % 10000000000).ToString();

            // Random component (4 digits)
            var random = new Random().Next(1000, 9999);

            // Process ID for additional uniqueness (last 4 digits)
            var processId = (Environment.ProcessId % 10000).ToString();

            // Format: <prefix>.<short_ticks>.<process_id>.<random>
            // Example: 1.2.276.0.7230010.3.0.3.6.4.1847714048.1234.5678
            var uid = $"{IMPLEMENTATION_CLASS_UID}.{shortTicks}.{processId}.{random}";

            // Safety check
            if (uid.Length > 64)
            {
                _logger.LogWarning("Generated UID too long ({Length} chars), truncating: {UID}",
                    uid.Length, uid);
                uid = uid.Substring(0, 64);
            }

            return uid;
        }

        private string GetDicomGender(Gender gender)
        {
            return gender switch
            {
                Gender.Male => "M",
                Gender.Female => "F",
                Gender.Other => "O",
                _ => ""
            };
        }
    }

    /// <summary>
    /// Result of DICOM conversion operation
    /// </summary>
    public class ConversionResult
    {
        public bool Success { get; init; }
        public string? DicomFilePath { get; init; }
        public string? ErrorMessage { get; init; }
        public string? SopInstanceUid { get; init; }
        public long FileSizeBytes { get; init; }

        public static ConversionResult CreateSuccess(string filePath, string sopInstanceUid, long fileSize)
            => new()
            {
                Success = true,
                DicomFilePath = filePath,
                SopInstanceUid = sopInstanceUid,
                FileSizeBytes = fileSize
            };

        public static ConversionResult CreateFailure(string error)
            => new() { Success = false, ErrorMessage = error };
    }

    /// <summary>
    /// Result of DICOM validation
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; init; }
        public List<string> Errors { get; init; } = new();
        public List<string> Warnings { get; init; } = new();

        public static ValidationResult Valid()
            => new() { IsValid = true };

        public static ValidationResult Invalid(params string[] errors)
            => new() { IsValid = false, Errors = errors.ToList() };
    }
}
