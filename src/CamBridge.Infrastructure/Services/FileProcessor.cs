// src/CamBridge.Infrastructure/Services/FileProcessor.cs
// Version: 0.8.10
// Description: Pipeline-aware file processor with LogContext integration
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Core;
using CamBridge.Core.Entities;
using CamBridge.Core.Interfaces;
using CamBridge.Core.ValueObjects;
using CamBridge.Core.Logging;
using CamBridge.Core.Enums;
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
    /// ENHANCED: Now uses LogContext for structured hierarchical logging
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
        private readonly PacsUploadQueue? _pacsUploadQueue;
        private readonly LogVerbosity _logVerbosity;

        public event EventHandler<FileProcessingEventArgs>? ProcessingStarted;
        public event EventHandler<FileProcessingEventArgs>? ProcessingCompleted;
        public event EventHandler<FileProcessingErrorEventArgs>? ProcessingError;

        /// <summary>
        /// Creates a FileProcessor for a specific pipeline with optional PACS upload support
        /// </summary>
        public FileProcessor(
            ILogger logger,
            ExifToolReader exifToolReader,
            DicomConverter dicomConverter,
            PipelineConfiguration pipelineConfig,
            DicomSettings globalDicomSettings,
            IDicomTagMapper? tagMapper = null,
            IMappingConfiguration? mappingConfiguration = null,
            PacsUploadQueue? pacsUploadQueue = null,
            LogVerbosity logVerbosity = LogVerbosity.Detailed,
            string? correlationId = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _exifToolReader = exifToolReader ?? throw new ArgumentNullException(nameof(exifToolReader));
            _dicomConverter = dicomConverter ?? throw new ArgumentNullException(nameof(dicomConverter));
            _pipelineConfig = pipelineConfig ?? throw new ArgumentNullException(nameof(pipelineConfig));
            _dicomSettings = ApplyDicomOverrides(globalDicomSettings, pipelineConfig.DicomOverrides);
            _tagMapper = tagMapper;
            _mappingConfiguration = mappingConfiguration;
            _pacsUploadQueue = pacsUploadQueue;
            _logVerbosity = logVerbosity;

            // Log with correlation ID if provided
            if (!string.IsNullOrEmpty(correlationId))
            {
                _logger.LogDebug("[{CorrelationId}] [ProcessorInit] Created FileProcessor for pipeline: {PipelineName} (\"{PipelineId}\")",
                    correlationId, pipelineConfig.Name, pipelineConfig.Id);

                if (_pacsUploadQueue != null)
                {
                    _logger.LogInformation("[{CorrelationId}] [PacsInit] PACS upload queue attached to pipeline: {PipelineName}",
                        correlationId, pipelineConfig.Name);
                }
            }
            else
            {
                // Fallback without correlation ID
                _logger.LogDebug("Created FileProcessor for pipeline: {PipelineName} (\"{PipelineId}\")",
                    pipelineConfig.Name, pipelineConfig.Id);

                if (_pacsUploadQueue != null)
                {
                    _logger.LogInformation("PACS upload queue attached to pipeline: {PipelineName}",
                        pipelineConfig.Name);
                }
            }
        }

        /// <summary>
        /// Processes a single JPEG file through the pipeline using LogContext
        /// </summary>
        public async Task<FileProcessingResult> ProcessFileAsync(string filePath)
        {
            // Generate correlation ID and create LogContext
            var correlationId = GenerateCorrelationId(filePath);
            var logContext = _logger.CreateContext(correlationId, _pipelineConfig.Name, _logVerbosity);

            var result = new FileProcessingResult
            {
                SourceFile = filePath,
                StartTime = DateTime.UtcNow,
                PipelineId = _pipelineConfig.Id,
                CorrelationId = correlationId
            };

            try
            {
                using (logContext.BeginStage(ProcessingStage.FileDetected, $"Processing file: {Path.GetFileName(filePath)}"))
                {
                    ProcessingStarted?.Invoke(this, new FileProcessingEventArgs
                    {
                        FilePath = filePath,
                        CorrelationId = correlationId,
                        PipelineName = _pipelineConfig.Name,
                        Stage = ProcessingStage.FileDetected
                    });

                    // Validate input file
                    ValidateInputFile(filePath);
                }

                // Extract EXIF data with timing
                ImageMetadata? metadata;
                using (logContext.BeginStage(ProcessingStage.ExifExtraction, "Extracting EXIF metadata"))
                {
                    // FIXED: Pass correlationId to ExtractMetadataAsync!
                    metadata = await _exifToolReader.ExtractMetadataAsync(filePath, correlationId);

                    if (metadata == null)
                    {
                        logContext.LogWarning("Failed to extract metadata, creating default DICOM with minimal tags");
                        metadata = CreateDefaultMetadata(filePath);
                    }
                    else if (metadata.Patient != null)
                    {
                        logContext.LogInformation($"Patient: {metadata.Patient.PatientName}");
                    }
                }

                // Apply tag mapping if configured
                if (_tagMapper != null && _mappingConfiguration != null)
                {
                    using (logContext.BeginStage(ProcessingStage.TagMapping, "Applying tag mapping rules"))
                    {
                        var mappingRules = _mappingConfiguration.GetMappingRules();
                        if (mappingRules.Any())
                        {
                            logContext.LogDebug($"Applying {mappingRules.Count()} mapping rules");
                        }
                    }
                }

                // Determine output path based on pipeline configuration
                // FIXED: Pass correlationId to DetermineOutputPath!
                var outputPath = DetermineOutputPath(metadata, filePath, correlationId);

                // Ensure output directory exists
                var outputDir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                // Convert to DICOM with timing
                ConversionResult conversionResult;
                using (logContext.BeginStage(ProcessingStage.DicomConversion, $"Converting to DICOM: {Path.GetFileName(outputPath)}"))
                {
                    // Create converter with mapper if available
                    var converterWithMapping = new DicomConverter(
                        _logger as ILogger<DicomConverter> ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<DicomConverter>.Instance,
                        _tagMapper,
                        _mappingConfiguration);

                    // FIXED: Pass correlationId to ConvertToDicomAsync!
                    conversionResult = await converterWithMapping.ConvertToDicomAsync(
                        filePath,
                        outputPath,
                        metadata,
                        correlationId);
                }

                result.Success = conversionResult.Success;
                result.OutputFile = outputPath;
                result.DicomFile = outputPath;
                result.EndTime = DateTime.UtcNow;
                result.ProcessingTimeMs = (long)(result.EndTime - result.StartTime).TotalMilliseconds;

                if (result.Success)
                {
                    // Queue for PACS upload if enabled
                    if (_pacsUploadQueue != null && _pipelineConfig.PacsConfiguration?.Enabled == true)
                    {
                        using (logContext.BeginStage(ProcessingStage.PacsUpload, $"Queueing for PACS upload to {_pipelineConfig.PacsConfiguration.Host}:{_pipelineConfig.PacsConfiguration.Port}"))
                        {
                            try
                            {
                                await _pacsUploadQueue.EnqueueAsync(conversionResult.DicomFilePath!, correlationId);
                            }
                            catch (Exception ex)
                            {
                                logContext.LogError(ex, "Failed to queue DICOM for PACS upload");
                                // Don't fail the overall processing if PACS queue fails
                            }
                        }
                    }

                    // Handle post-processing
                    using (logContext.BeginStage(ProcessingStage.PostProcessing, "Performing post-processing"))
                    {
                        await HandlePostProcessingAsync(filePath, outputPath, result.Success, logContext);
                    }

                    // Final success log
                    using (logContext.BeginStage(ProcessingStage.Complete, $"Successfully processed {Path.GetFileName(filePath)}"))
                    {
                        // Performance warning for slow processing
                        if (result.ProcessingTimeMs > 5000)
                        {
                            logContext.LogWarning($"Slow processing detected: {result.ProcessingTimeMs}ms");
                        }

                        ProcessingCompleted?.Invoke(this, new FileProcessingEventArgs
                        {
                            FilePath = filePath,
                            OutputPath = outputPath,
                            CorrelationId = correlationId,
                            PipelineName = _pipelineConfig.Name,
                            Stage = ProcessingStage.Complete
                        });
                    }
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
                result.ProcessingTimeMs = (long)(result.EndTime - result.StartTime).TotalMilliseconds;

                using (logContext.BeginStage(ProcessingStage.Error, $"Processing failed: {ex.Message}"))
                {
                    logContext.LogError(ex, $"Failed to process {Path.GetFileName(filePath)}");

                    // Critical error detection
                    if (ex is UnauthorizedAccessException && filePath.StartsWith(_pipelineConfig.WatchSettings.Path))
                    {
                        _logger.LogCritical(ex, "[{CorrelationId}] [Error] Cannot access watch folder {Path} - pipeline will fail! [{Pipeline}]",
                            correlationId, _pipelineConfig.WatchSettings.Path, _pipelineConfig.Name);
                    }

                    // Handle failure post-processing
                    await HandlePostProcessingAsync(filePath, null, false, logContext);

                    ProcessingError?.Invoke(this, new FileProcessingErrorEventArgs
                    {
                        FilePath = filePath,
                        Error = ex,
                        CorrelationId = correlationId,
                        PipelineName = _pipelineConfig.Name,
                        Stage = ProcessingStage.Error
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Handles post-processing with LogContext
        /// </summary>
        private async Task HandlePostProcessingAsync(string sourceFilePath, string? dicomFilePath, bool success, LogContext logContext)
        {
            var action = success
                ? _pipelineConfig.ProcessingOptions.SuccessAction
                : _pipelineConfig.ProcessingOptions.FailureAction;

            try
            {
                logContext.LogDebug($"Performing {action} on source file");

                switch (action)
                {
                    case PostProcessingAction.Delete:
                        // Delete only the source file
                        if (File.Exists(sourceFilePath))
                        {
                            File.Delete(sourceFilePath);
                            logContext.LogDebug("Deleted source file");
                        }
                        break;

                    case PostProcessingAction.Archive:
                        var jpegArchiveFolder = _pipelineConfig.ProcessingOptions.BackupFolder
                            ?? Path.Combine(_pipelineConfig.ProcessingOptions.ArchiveFolder, "ProcessedJPEGs");

                        var archivePath = Path.Combine(
                            jpegArchiveFolder,
                            Path.GetFileName(sourceFilePath));

                        Directory.CreateDirectory(Path.GetDirectoryName(archivePath)!);

                        if (File.Exists(sourceFilePath))
                        {
                            File.Move(sourceFilePath, archivePath, true);
                            logContext.LogDebug($"Archived source JPEG to {Path.GetFullPath(archivePath)}");
                        }

                        // DICOM file stays in output folder
                        if (success && !string.IsNullOrEmpty(dicomFilePath))
                        {
                            logContext.LogInformation($"DICOM file created at: {Path.GetFullPath(dicomFilePath)}");
                        }
                        break;

                    case PostProcessingAction.MoveToError:
                        // Move source file to error folder (typically for failures)
                        if (!success)
                        {
                            await MoveToErrorFolderAsync(sourceFilePath, "Processing failed", logContext);

                            // If DICOM was partially created, clean it up
                            if (!string.IsNullOrEmpty(dicomFilePath) && File.Exists(dicomFilePath))
                            {
                                try
                                {
                                    File.Delete(dicomFilePath);
                                    logContext.LogDebug("Cleaned up partial DICOM file");
                                }
                                catch (Exception ex)
                                {
                                    logContext.LogWarning($"Failed to clean up partial DICOM: {ex.Message}");
                                }
                            }
                        }
                        break;

                    default:
                        // Leave files as-is
                        logContext.LogDebug("No post-processing action");
                        break;
                }
            }
            catch (Exception ex)
            {
                logContext.LogWarning($"Failed to perform {action}: {ex.Message}");
                // Don't fail the overall processing for post-processing errors
            }
        }

        private async Task MoveToErrorFolderAsync(string filePath, string errorMessage, LogContext logContext)
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
                    $"Correlation ID: {logContext.CorrelationId}\n" +
                    $"Error: {errorMessage}");

                // Move the file
                if (File.Exists(filePath))
                {
                    File.Move(filePath, errorPath, true);
                    logContext.LogDebug("Moved failed file to error folder");
                }
            }
            catch (Exception ex)
            {
                logContext.LogError(ex, "Failed to move file to error folder");
            }
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

        private string GenerateCorrelationId(string filePath)
        {
            // Format: F{HHmmssff}-{FilePrefix8}
            // Example: F10234512-IMG_1234
            var timestamp = DateTime.Now.ToString("HHmmssff");
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            var filePrefix = fileNameWithoutExt.Length > 8
                ? fileNameWithoutExt.Substring(0, 8)
                : fileNameWithoutExt;

            return $"F{timestamp}-{filePrefix}";
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

        private string DetermineOutputPath(ImageMetadata metadata, string sourceFile, string correlationId)
        {
            // FIXED: Use ArchiveFolder as DICOM output if no OutputPath configured
            var baseOutputPath = _pipelineConfig.WatchSettings.OutputPath;

            if (string.IsNullOrEmpty(baseOutputPath))
            {
                // Fallback to ArchiveFolder if no OutputPath (current config behavior)
                baseOutputPath = _pipelineConfig.ProcessingOptions.ArchiveFolder;
                _logger.LogWarning("[{CorrelationId}] [PathResolution] No OutputPath in WatchSettings, using ArchiveFolder as output: {Path}",
                    correlationId, Path.GetFullPath(baseOutputPath));
            }

            // ALWAYS make path absolute!
            if (!Path.IsPathRooted(baseOutputPath))
            {
                // If relative path, make it relative to service executable location
                var serviceDir = AppDomain.CurrentDomain.BaseDirectory;
                baseOutputPath = Path.Combine(serviceDir, baseOutputPath);
                _logger.LogWarning("[{CorrelationId}] [PathResolution] OutputPath was relative, converted to absolute: {Path}",
                    correlationId, Path.GetFullPath(baseOutputPath));
            }

            _logger.LogDebug("[{CorrelationId}] [PathResolution] Base output path for pipeline {Pipeline}: {Path}",
                correlationId, _pipelineConfig.Name, Path.GetFullPath(baseOutputPath));

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
                        _logger.LogDebug("[{CorrelationId}] [PathResolution] Output organized by patient: {PatientName} -> {SafeName}",
                            correlationId, metadata.Patient.PatientName, safeName);
                    }
                    else
                    {
                        outputDir = Path.Combine(baseOutputPath, "Unknown Patient");
                        _logger.LogWarning("[{CorrelationId}] [PathResolution] No patient name found, using 'Unknown Patient' folder",
                            correlationId);
                    }
                    break;

                case OutputOrganization.ByDate:
                    // ALWAYS use current date for organization
                    var dateFolder = DateTime.Now.ToString("yyyy-MM-dd");
                    outputDir = Path.Combine(baseOutputPath, dateFolder);
                    _logger.LogDebug("[{CorrelationId}] [PathResolution] Output organized by date: {Date}",
                        correlationId, dateFolder);
                    break;

                case OutputOrganization.ByPatientAndDate:
                    if (!string.IsNullOrEmpty(metadata.Patient?.PatientName))
                    {
                        var safeName = SanitizeForPath(metadata.Patient.PatientName);
                        // ALWAYS use current date for organization
                        var dateFolder2 = DateTime.Now.ToString("yyyy-MM-dd");
                        outputDir = Path.Combine(baseOutputPath, safeName, dateFolder2);
                        _logger.LogDebug("[{CorrelationId}] [PathResolution] Output organized by patient/date: {PatientName}/{Date}",
                            correlationId, safeName, dateFolder2);
                    }
                    else
                    {
                        var dateFolder3 = DateTime.Now.ToString("yyyy-MM-dd");
                        outputDir = Path.Combine(baseOutputPath, "Unknown Patient", dateFolder3);
                        _logger.LogWarning("[{CorrelationId}] [PathResolution] No patient name found, using 'Unknown Patient/{Date}' folder",
                            correlationId, dateFolder3);
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
            _logger.LogInformation("[{CorrelationId}] [PathResolution] Determined DICOM output path: {FullPath}",
                correlationId, absolutePath);
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
                captureDateTime: now,
                patient: patient,
                study: study,
                technicalData: technicalData,
                userComment: null,
                barcodeData: null,
                instanceNumber: 1,
                instanceUid: null,
                exifData: exifData
            );

            return metadata;
        }
    }

    /// <summary>
    /// Event arguments for file processing events with correlation support
    /// </summary>
    public class FileProcessingEventArgs : EventArgs
    {
        public string FilePath { get; set; } = string.Empty;
        public string? OutputPath { get; set; }
        public string CorrelationId { get; set; } = string.Empty;
        public string PipelineName { get; set; } = string.Empty;
        public ProcessingStage Stage { get; set; } = ProcessingStage.FileDetected;
    }

    /// <summary>
    /// Event arguments for file processing errors with correlation support
    /// </summary>
    public class FileProcessingErrorEventArgs : EventArgs
    {
        public string FilePath { get; set; } = string.Empty;
        public Exception Error { get; set; } = null!;
        public string CorrelationId { get; set; } = string.Empty;
        public string PipelineName { get; set; } = string.Empty;
        public ProcessingStage Stage { get; set; } = ProcessingStage.Error;
    }

    /// <summary>
    /// Result of file processing operation with correlation tracking
    /// </summary>
    public class FileProcessingResult
    {
        public string SourceFile { get; set; } = string.Empty;
        public string? OutputFile { get; set; }
        public string? DicomFile { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long ProcessingTimeMs { get; set; }
        public Guid PipelineId { get; set; }
        public string CorrelationId { get; set; } = string.Empty;
    }
}
