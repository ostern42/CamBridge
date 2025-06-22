// src/CamBridge.Infrastructure/Services/FileProcessor.cs
// Version: 0.7.29
// Description: Pipeline-aware file processor with FIXED DICOM output handling
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Core;
using CamBridge.Core.Entities;
using CamBridge.Core.Interfaces;
using CamBridge.Core.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Orchestrates the complete JPEG to DICOM conversion process for a specific pipeline
    /// FIXED: Now properly handles DICOM files in output with ABSOLUTE paths!
    /// </summary>
    public class FileProcessor
    {
        private readonly ILogger _logger;
        private readonly ExifToolReader _exifToolReader;
        private readonly DicomConverter _dicomConverter;
        private readonly PipelineConfiguration _pipelineConfig;
        private readonly DicomSettings _dicomSettings;
        private readonly IDicomTagMapper? _tagMapper;
        private readonly IMappingConfiguration? _mappingConfiguration;

        public event EventHandler<FileProcessingEventArgs>? ProcessingStarted;
        public event EventHandler<FileProcessingEventArgs>? ProcessingCompleted;
        public event EventHandler<FileProcessingErrorEventArgs>? ProcessingError;

        /// <summary>
        /// Creates a FileProcessor for a specific pipeline
        /// </summary>
        public FileProcessor(
            ILogger logger,
            ExifToolReader exifToolReader,
            DicomConverter dicomConverter,
            PipelineConfiguration pipelineConfig,
            DicomSettings globalDicomSettings,
            IDicomTagMapper? tagMapper = null,
            IMappingConfiguration? mappingConfiguration = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _exifToolReader = exifToolReader ?? throw new ArgumentNullException(nameof(exifToolReader));
            _dicomConverter = dicomConverter ?? throw new ArgumentNullException(nameof(dicomConverter));
            _pipelineConfig = pipelineConfig ?? throw new ArgumentNullException(nameof(pipelineConfig));
            _dicomSettings = ApplyDicomOverrides(globalDicomSettings, pipelineConfig.DicomOverrides);
            _tagMapper = tagMapper;
            _mappingConfiguration = mappingConfiguration;

            _logger.LogDebug("Created FileProcessor for pipeline: {PipelineName} (\"{PipelineId}\")",
                pipelineConfig.Name, pipelineConfig.Id);
        }

        /// <summary>
        /// Determines if a file should be processed based on pipeline configuration
        /// </summary>
        public bool ShouldProcessFile(string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                var extension = fileInfo.Extension.ToLowerInvariant();

                // Check file extension
                var filePattern = _pipelineConfig.WatchSettings.FilePattern;
                var patterns = string.IsNullOrEmpty(filePattern)
                    ? new[] { "*.jpg", "*.jpeg" }
                    : filePattern.Split(';', StringSplitOptions.RemoveEmptyEntries);

                var isValidExtension = patterns.Any(pattern =>
                {
                    var patternExt = Path.GetExtension(pattern).ToLowerInvariant();
                    return patternExt == extension || patternExt == ".*";
                });

                if (!isValidExtension)
                {
                    return false;
                }

                // Check file age
                if (_pipelineConfig.ProcessingOptions.MaxFileAge.HasValue)
                {
                    var age = DateTime.UtcNow - fileInfo.CreationTimeUtc;
                    if (age > _pipelineConfig.ProcessingOptions.MaxFileAge.Value)
                    {
                        _logger.LogDebug("File {FileName} is too old ({Age} days)",
                            fileInfo.Name, age.TotalDays);
                        return false;
                    }
                }

                // Check file size
                if (_pipelineConfig.ProcessingOptions.MinimumFileSizeBytes.HasValue &&
                    fileInfo.Length < _pipelineConfig.ProcessingOptions.MinimumFileSizeBytes.Value)
                {
                    _logger.LogDebug("File {FileName} is too small ({Size} bytes)",
                        fileInfo.Name, fileInfo.Length);
                    return false;
                }

                if (_pipelineConfig.ProcessingOptions.MaximumFileSizeBytes.HasValue &&
                    fileInfo.Length > _pipelineConfig.ProcessingOptions.MaximumFileSizeBytes.Value)
                {
                    _logger.LogDebug("File {FileName} is too large ({Size} bytes)",
                        fileInfo.Name, fileInfo.Length);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error checking file {FilePath}", filePath);
                return false;
            }
        }

        /// <summary>
        /// Processes a single JPEG file through the pipeline
        /// </summary>
        public async Task<FileProcessingResult> ProcessFileAsync(string filePath)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new FileProcessingResult
            {
                SourceFile = filePath,
                StartTime = DateTime.UtcNow,
                PipelineId = _pipelineConfig.Id
            };

            try
            {
                _logger.LogInformation("Processing file: {FileName}", Path.GetFileName(filePath));
                ProcessingStarted?.Invoke(this, new FileProcessingEventArgs { FilePath = filePath });

                // Validate input file
                ValidateInputFile(filePath);

                // Extract EXIF data
                _logger.LogDebug("Extracting EXIF data from {FilePath}", filePath);
                var exifStopwatch = Stopwatch.StartNew();
                var metadata = await _exifToolReader.ExtractMetadataAsync(filePath);
                exifStopwatch.Stop();
                _logger.LogDebug("EXIF extraction completed in {ElapsedMs}ms", exifStopwatch.ElapsedMilliseconds);

                if (metadata == null)
                {
                    // FIXED: Create default metadata instead of failing!
                    _logger.LogWarning("Failed to extract metadata, creating default DICOM with minimal tags");
                    metadata = CreateDefaultMetadata(filePath);
                }

                // Determine output path based on pipeline configuration
                var outputPath = DetermineOutputPath(metadata, filePath);

                // Ensure output directory exists
                var outputDir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                // Convert to DICOM using pipeline-specific settings and mapping
                _logger.LogInformation("Converting JPEG to DICOM: {Source} → {Destination}",
                    Path.GetFullPath(filePath), Path.GetFullPath(outputPath));

                var dicomStopwatch = Stopwatch.StartNew();

                // Create converter with mapper if available
                var converterWithMapping = new DicomConverter(
                    _logger as ILogger<DicomConverter> ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<DicomConverter>.Instance,
                    _tagMapper,
                    _mappingConfiguration);

                var conversionResult = await converterWithMapping.ConvertToDicomAsync(
                    filePath,
                    outputPath,
                    metadata);

                dicomStopwatch.Stop();
                _logger.LogDebug("DICOM conversion completed in {ElapsedMs}ms", dicomStopwatch.ElapsedMilliseconds);

                result.Success = conversionResult.Success;
                result.OutputFile = outputPath;
                result.DicomFile = outputPath; // NEW: Track DICOM file location
                result.EndTime = DateTime.UtcNow;
                result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

                if (result.Success)
                {
                    _logger.LogInformation(
                        "Successfully converted {FileName} to DICOM at: {FullPath} ({ElapsedMs}ms)",
                        Path.GetFileName(filePath),
                        Path.GetFullPath(outputPath),
                        result.ProcessingTimeMs);

                    // Performance warning for slow processing
                    if (result.ProcessingTimeMs > 5000)
                    {
                        _logger.LogWarning("Slow processing detected for {FileName}: {ElapsedMs}ms",
                            Path.GetFileName(filePath), result.ProcessingTimeMs);
                    }

                    // FIXED: Handle post-processing for SOURCE file only
                    await HandlePostProcessingAsync(filePath, outputPath, result.Success);

                    ProcessingCompleted?.Invoke(this, new FileProcessingEventArgs
                    {
                        FilePath = filePath,
                        OutputPath = outputPath
                    });
                }
                else
                {
                    throw new InvalidOperationException(
                        $"DICOM conversion failed: {conversionResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.EndTime = DateTime.UtcNow;
                result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

                _logger.LogError(ex,
                    "Failed to process {FileName}. Error: {ErrorMessage}",
                    Path.GetFileName(filePath), ex.Message);

                // Critical error detection
                if (ex is UnauthorizedAccessException && filePath.StartsWith(_pipelineConfig.WatchSettings.Path))
                {
                    _logger.LogCritical(ex, "Cannot access watch folder {Path} - pipeline will fail!",
                        _pipelineConfig.WatchSettings.Path);
                }

                // Handle failure post-processing
                await HandlePostProcessingAsync(filePath, null, false);

                ProcessingError?.Invoke(this, new FileProcessingErrorEventArgs
                {
                    FilePath = filePath,
                    Error = ex
                });
            }

            return result;
        }

        private void ValidateInputFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Source file not found: {filePath}");
            }

            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (extension != ".jpg" && extension != ".jpeg")
            {
                throw new InvalidOperationException($"Invalid file type: {extension}. Expected JPEG file.");
            }
        }

        private string DetermineOutputPath(ImageMetadata metadata, string sourceFile)
        {
            // FIXED: Use ArchiveFolder as DICOM output if no OutputPath configured
            // This matches the current config structure where ArchiveFolder is used for output
            var baseOutputPath = _pipelineConfig.WatchSettings.OutputPath;

            if (string.IsNullOrEmpty(baseOutputPath))
            {
                // Fallback to ArchiveFolder if no OutputPath (current config behavior)
                baseOutputPath = _pipelineConfig.ProcessingOptions.ArchiveFolder;
                _logger.LogWarning("No OutputPath in WatchSettings, using ArchiveFolder as output: {Path}",
                    Path.GetFullPath(baseOutputPath));
            }

            // ALWAYS make path absolute!
            if (!Path.IsPathRooted(baseOutputPath))
            {
                // If relative path, make it relative to service executable location
                var serviceDir = AppDomain.CurrentDomain.BaseDirectory;
                baseOutputPath = Path.Combine(serviceDir, baseOutputPath);
                _logger.LogWarning("OutputPath was relative, converted to absolute: {Path}",
                    Path.GetFullPath(baseOutputPath));
            }

            _logger.LogDebug("Base output path for pipeline {Pipeline}: {Path}",
                _pipelineConfig.Name, Path.GetFullPath(baseOutputPath));

            var organization = _pipelineConfig.ProcessingOptions.OutputOrganization;
            var fileName = Path.GetFileNameWithoutExtension(sourceFile);

            // Build organized path based on configuration
            var outputDir = baseOutputPath;

            switch (organization)
            {
                case OutputOrganization.ByPatient:
                    if (!string.IsNullOrEmpty(metadata.Patient?.PatientName))
                    {
                        var safeName = SanitizeForPath(metadata.Patient.PatientName);
                        outputDir = Path.Combine(baseOutputPath, safeName);
                        _logger.LogDebug("Output organized by patient: {PatientName} -> {SafeName}",
                            metadata.Patient.PatientName, safeName);
                    }
                    else
                    {
                        outputDir = Path.Combine(baseOutputPath, "Unknown Patient");
                        _logger.LogWarning("No patient name found, using 'Unknown Patient' folder");
                    }
                    break;

                case OutputOrganization.ByDate:
                    // ALWAYS use current date for organization
                    var dateFolder = DateTime.Now.ToString("yyyy-MM-dd");
                    outputDir = Path.Combine(baseOutputPath, dateFolder);
                    _logger.LogDebug("Output organized by date: {Date}", dateFolder);
                    break;

                case OutputOrganization.ByPatientAndDate:
                    if (!string.IsNullOrEmpty(metadata.Patient?.PatientName))
                    {
                        var safeName = SanitizeForPath(metadata.Patient.PatientName);
                        // ALWAYS use current date for organization
                        var dateFolder2 = DateTime.Now.ToString("yyyy-MM-dd");
                        outputDir = Path.Combine(baseOutputPath, safeName, dateFolder2);
                        _logger.LogDebug("Output organized by patient/date: {PatientName}/{Date}",
                            safeName, dateFolder2);
                    }
                    else
                    {
                        var dateFolder3 = DateTime.Now.ToString("yyyy-MM-dd");
                        outputDir = Path.Combine(baseOutputPath, "Unknown Patient", dateFolder3);
                        _logger.LogWarning("No patient name found, using 'Unknown Patient/{Date}' folder", dateFolder3);
                    }
                    break;

                case OutputOrganization.None:
                default:
                    // Use base output path as-is
                    break;
            }

            // Add DICOM extension
            var dicomPath = Path.Combine(outputDir, $"{fileName}.dcm");

            // CRITICAL: ALWAYS return ABSOLUTE path!
            var absolutePath = Path.GetFullPath(dicomPath);
            _logger.LogInformation("Determined DICOM output path: {FullPath}", absolutePath);
            return absolutePath;
        }

        private string SanitizeForPath(string input)
        {
            var invalid = Path.GetInvalidFileNameChars()
                .Concat(Path.GetInvalidPathChars())
                .Distinct()
                .ToArray();

            return string.Join("_", input.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// FIXED: Now handles source and DICOM files separately!
        /// </summary>
        private async Task HandlePostProcessingAsync(string sourceFilePath, string? dicomFilePath, bool success)
        {
            var action = success
                ? _pipelineConfig.ProcessingOptions.SuccessAction
                : _pipelineConfig.ProcessingOptions.FailureAction;

            try
            {
                switch (action)
                {
                    case PostProcessingAction.Delete:
                        // Delete only the source file
                        if (File.Exists(sourceFilePath))
                        {
                            File.Delete(sourceFilePath);
                            _logger.LogDebug("Deleted source file: {FilePath}", sourceFilePath);
                        }
                        break;

                    case PostProcessingAction.Archive:
                        // FIXED: Create separate archive folder for processed JPEGs
                        // Don't mix them with DICOM output!
                        var jpegArchiveFolder = _pipelineConfig.ProcessingOptions.BackupFolder
                            ?? Path.Combine(_pipelineConfig.ProcessingOptions.ArchiveFolder, "ProcessedJPEGs");

                        var archivePath = Path.Combine(
                            jpegArchiveFolder,
                            Path.GetFileName(sourceFilePath));

                        Directory.CreateDirectory(Path.GetDirectoryName(archivePath)!);

                        if (File.Exists(sourceFilePath))
                        {
                            File.Move(sourceFilePath, archivePath, true);
                            _logger.LogDebug("Archived source JPEG: {FullSourcePath} -> {FullArchivePath}",
                                Path.GetFullPath(sourceFilePath), Path.GetFullPath(archivePath));
                        }

                        // DICOM file stays in output folder - don't move it!
                        if (success && !string.IsNullOrEmpty(dicomFilePath))
                        {
                            _logger.LogInformation("DICOM file created at: {FullDicomPath}",
                                Path.GetFullPath(dicomFilePath));
                        }
                        break;

                    case PostProcessingAction.MoveToError:
                        // Move source file to error folder (typically for failures)
                        if (!success)
                        {
                            await MoveToErrorFolderAsync(sourceFilePath, "Processing failed");

                            // If DICOM was partially created, clean it up
                            if (!string.IsNullOrEmpty(dicomFilePath) && File.Exists(dicomFilePath))
                            {
                                try
                                {
                                    File.Delete(dicomFilePath);
                                    _logger.LogDebug("Cleaned up partial DICOM file: {DicomPath}", dicomFilePath);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning(ex, "Failed to clean up partial DICOM file: {DicomPath}", dicomFilePath);
                                }
                            }
                        }
                        break;

                    default:
                        // Leave files as-is
                        _logger.LogDebug("No post-processing action for {FilePath}", sourceFilePath);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to perform post-processing action {Action} on {FilePath}",
                    action, sourceFilePath);
                // Don't fail the overall processing for post-processing errors
            }
        }

        private async Task MoveToErrorFolderAsync(string filePath, string errorMessage)
        {
            try
            {
                var errorFolder = _pipelineConfig.ProcessingOptions.ErrorFolder;
                Directory.CreateDirectory(errorFolder);

                var errorFileName = $"{Path.GetFileNameWithoutExtension(filePath)}" +
                                   $"_{DateTime.Now:yyyyMMdd_HHmmss}" +
                                   $"{Path.GetExtension(filePath)}";

                var errorPath = Path.Combine(errorFolder, errorFileName);

                // Write error info file
                var errorInfoPath = Path.ChangeExtension(errorPath, ".error.txt");
                await File.WriteAllTextAsync(errorInfoPath,
                    $"Error Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                    $"Pipeline: {_pipelineConfig.Name}\n" +
                    $"Source File: {filePath}\n" +
                    $"Error: {errorMessage}");

                // Move the file
                if (File.Exists(filePath))
                {
                    File.Move(filePath, errorPath, true);
                    _logger.LogDebug("Moved failed file to error folder: {FullErrorPath}",
                        Path.GetFullPath(errorPath));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to move file to error folder: {FilePath}", filePath);
            }
        }

        private DicomSettings ApplyDicomOverrides(DicomSettings global, DicomOverrides? overrides)
        {
            if (overrides == null)
                return global;

            // Create a copy of global settings with overrides applied
            var settings = new DicomSettings
            {
                InstitutionName = overrides.InstitutionName ?? global.InstitutionName,
                InstitutionDepartment = overrides.InstitutionDepartment ?? global.InstitutionDepartment,
                StationName = global.StationName,
                SourceApplicationEntityTitle = global.SourceApplicationEntityTitle,
                ImplementationVersionName = global.ImplementationVersionName,
                ImplementationClassUid = global.ImplementationClassUid,
                Modality = global.Modality,
                ValidateAfterCreation = global.ValidateAfterCreation
            };

            _logger.LogDebug("Applied DICOM overrides for pipeline: {PipelineName}", _pipelineConfig.Name);
            return settings;
        }

        /// <summary>
        /// Creates minimal default metadata when extraction fails
        /// DICOM MUST be created regardless of metadata availability!
        /// </summary>
        private ImageMetadata CreateDefaultMetadata(string sourceFile)
        {
            var fileName = Path.GetFileNameWithoutExtension(sourceFile);
            var now = DateTime.Now;

            _logger.LogWarning("Creating default metadata for {FileName} with date {Date}",
                fileName, now.ToString("yyyy-MM-dd"));

            // Create default patient info with constructor
            var patientId = new PatientId($"DEFAULT_{now:yyyyMMddHHmmss}");
            var patient = new PatientInfo(
                id: patientId,
                name: "Unknown Patient",
                birthDate: null,
                gender: Gender.Other
            );

            // Create default study info with constructor
            var studyId = new StudyId(Guid.NewGuid().ToString().Substring(0, 16)); // Max 16 chars
            var study = new StudyInfo(
                studyId: studyId,
                examId: null,
                description: "CamBridge JPEG to DICOM Conversion",
                modality: "XC",  // Photographic Image
                studyDate: now,
                accessionNumber: $"ACC{now:yyyyMMddHHmmss}",
                referringPhysician: null,
                comment: null
            );

            // Create technical data (minimal)
            var technicalData = new ImageTechnicalData
            {
                Manufacturer = "Unknown",
                Model = "Unknown"
            };

            // Create EXIF data dictionary
            var exifData = new Dictionary<string, string>
            {
                ["FileName"] = fileName,
                ["FileDate"] = now.ToString("yyyy-MM-dd HH:mm:ss"),
                ["Source"] = "CamBridge Default"
            };

            // Create metadata with full constructor
            var metadata = new ImageMetadata(
                sourceFilePath: sourceFile,
                captureDateTime: now,  // Note: captureDateTime, not captureDate!
                patient: patient,
                study: study,
                technicalData: technicalData,
                userComment: null,
                barcodeData: null,
                instanceNumber: 1,
                instanceUid: null,  // Will be auto-generated
                exifData: exifData
            );

            return metadata;
        }
    }

    /// <summary>
    /// Event arguments for file processing events
    /// </summary>
    public class FileProcessingEventArgs : EventArgs
    {
        public string FilePath { get; set; } = string.Empty;
        public string? OutputPath { get; set; }
    }

    /// <summary>
    /// Event arguments for file processing errors
    /// </summary>
    public class FileProcessingErrorEventArgs : EventArgs
    {
        public string FilePath { get; set; } = string.Empty;
        public Exception Error { get; set; } = null!;
    }

    /// <summary>
    /// Result of file processing operation
    /// </summary>
    public class FileProcessingResult
    {
        public string SourceFile { get; set; } = string.Empty;
        public string? OutputFile { get; set; }
        public string? DicomFile { get; set; }  // NEW: Track DICOM file separately
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long ProcessingTimeMs { get; set; }
        public Guid PipelineId { get; set; }
    }
}
