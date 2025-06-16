// src/CamBridge.Infrastructure/Services/FileProcessor.cs
// Version: 0.7.20
// Description: Pipeline-aware file processor - NO MORE SINGLETON!
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Entities;
using CamBridge.Core.Interfaces;
using CamBridge.Core.ValueObjects;
using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Orchestrates the complete JPEG to DICOM conversion process for a specific pipeline
    /// KISS UPDATE: No more IFileProcessor interface - direct dependency pattern!
    /// PIPELINE UPDATE: Each pipeline gets its own FileProcessor instance!
    /// </summary>
    public class FileProcessor // KISS: No interface inheritance!
    {
        private readonly ILogger<FileProcessor> _logger;
        private readonly ExifToolReader _exifToolReader;
        private readonly DicomConverter _dicomConverter;
        private readonly PipelineConfiguration _pipelineConfig;
        private readonly DicomSettings _dicomSettings;

        public event EventHandler<FileProcessingEventArgs>? ProcessingStarted;
        public event EventHandler<FileProcessingEventArgs>? ProcessingCompleted;
        public event EventHandler<FileProcessingErrorEventArgs>? ProcessingError;

        /// <summary>
        /// Creates a FileProcessor for a specific pipeline
        /// Each pipeline gets its own instance with its own configuration!
        /// </summary>
        public FileProcessor(
            ILogger<FileProcessor> logger,
            ExifToolReader exifToolReader,
            DicomConverter dicomConverter,
            PipelineConfiguration pipelineConfig,
            DicomSettings globalDicomSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _exifToolReader = exifToolReader ?? throw new ArgumentNullException(nameof(exifToolReader));
            _dicomConverter = dicomConverter ?? throw new ArgumentNullException(nameof(dicomConverter));
            _pipelineConfig = pipelineConfig ?? throw new ArgumentNullException(nameof(pipelineConfig));

            // Apply pipeline-specific DICOM overrides to global settings
            _dicomSettings = ApplyDicomOverrides(globalDicomSettings, pipelineConfig.DicomOverrides);

            _logger.LogInformation("Created FileProcessor for pipeline: {PipelineName} ({PipelineId})",
                _pipelineConfig.Name, _pipelineConfig.Id);
        }

        /// <summary>
        /// Processes a single JPEG file according to this pipeline's configuration
        /// </summary>
        public async Task<FileProcessingResult> ProcessFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            var stopwatch = Stopwatch.StartNew();
            var result = new FileProcessingResult
            {
                SourceFile = filePath,
                StartTime = DateTime.UtcNow,
                PipelineId = _pipelineConfig.Id
            };

            try
            {
                _logger.LogInformation("Pipeline {Pipeline}: Starting processing of {FilePath}",
                    _pipelineConfig.Name, filePath);
                ProcessingStarted?.Invoke(this, new FileProcessingEventArgs { FilePath = filePath });

                // Validate input file
                ValidateInputFile(filePath);

                // Extract EXIF data
                _logger.LogDebug("Extracting EXIF data from {FilePath}", filePath);
                var metadata = await _exifToolReader.ExtractMetadataAsync(filePath);

                if (metadata == null)
                {
                    throw new InvalidOperationException("Failed to extract metadata from image");
                }

                // Determine output path based on THIS pipeline's configuration
                var outputPath = DetermineOutputPath(metadata, filePath);

                // Ensure output directory exists
                var outputDir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                // Convert to DICOM
                _logger.LogDebug("Converting {FilePath} to DICOM at {OutputPath}", filePath, outputPath);
                var conversionResult = await _dicomConverter.ConvertToDicomAsync(filePath, outputPath, metadata);

                if (!conversionResult.Success)
                {
                    throw new InvalidOperationException($"DICOM conversion failed: {conversionResult.ErrorMessage}");
                }

                // Validate DICOM if configured
                if (_dicomSettings.ValidateAfterCreation)
                {
                    var validationResult = await _dicomConverter.ValidateDicomFileAsync(outputPath);
                    if (!validationResult.IsValid)
                    {
                        // FIX: Use Errors list instead of non-existent ErrorMessage
                        throw new InvalidOperationException($"DICOM validation failed: {string.Join("; ", validationResult.Errors)}");
                    }
                }

                // Handle post-processing according to pipeline settings
                await HandleProcessingSuccess(filePath, outputPath, _pipelineConfig.ProcessingOptions);

                result.Success = true;
                result.OutputFile = outputPath;
                result.EndTime = DateTime.UtcNow;
                result.ProcessingDuration = stopwatch.Elapsed;

                _logger.LogInformation("Pipeline {Pipeline}: Successfully processed {FilePath} to {OutputPath} in {Duration}ms",
                    _pipelineConfig.Name, filePath, outputPath, stopwatch.ElapsedMilliseconds);

                ProcessingCompleted?.Invoke(this, new FileProcessingEventArgs
                {
                    FilePath = filePath,
                    OutputPath = outputPath,
                    Success = true
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pipeline {Pipeline}: Error processing file {FilePath}",
                    _pipelineConfig.Name, filePath);

                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.Exception = ex;
                result.EndTime = DateTime.UtcNow;
                result.ProcessingDuration = stopwatch.Elapsed;

                // Handle post-processing for failures
                await HandleProcessingFailure(filePath, ex.Message, _pipelineConfig.ProcessingOptions);

                ProcessingError?.Invoke(this, new FileProcessingErrorEventArgs
                {
                    FilePath = filePath,
                    Error = ex
                });

                return result;
            }
        }

        /// <summary>
        /// Determines if a file should be processed according to this pipeline's rules
        /// </summary>
        public bool ShouldProcessFile(string filePath)
        {
            try
            {
                // Check if file exists
                if (!File.Exists(filePath))
                    return false;

                // Check file extension
                var extension = Path.GetExtension(filePath).ToLowerInvariant();
                if (extension != ".jpg" && extension != ".jpeg")
                    return false;

                // Check file size limits from pipeline config
                var fileInfo = new FileInfo(filePath);
                var options = _pipelineConfig.ProcessingOptions;

                if (options.MinimumFileSizeBytes.HasValue &&
                    fileInfo.Length < options.MinimumFileSizeBytes.Value)
                {
                    _logger.LogDebug("File {FilePath} is too small ({Size} bytes)", filePath, fileInfo.Length);
                    return false;
                }

                if (options.MaximumFileSizeBytes.HasValue &&
                    fileInfo.Length > options.MaximumFileSizeBytes.Value)
                {
                    _logger.LogDebug("File {FilePath} is too large ({Size} bytes)", filePath, fileInfo.Length);
                    return false;
                }

                // Check file age
                if (options.MaxFileAge.HasValue)
                {
                    var age = DateTime.UtcNow - fileInfo.LastWriteTimeUtc;
                    if (age > options.MaxFileAge.Value)
                    {
                        _logger.LogDebug("File {FilePath} is too old ({Age})", filePath, age);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error checking if file should be processed: {FilePath}", filePath);
                return false;
            }
        }

        private void ValidateInputFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Input file not found", filePath);

            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Length == 0)
                throw new InvalidOperationException("Input file is empty");

            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (extension != ".jpg" && extension != ".jpeg")
                throw new InvalidOperationException($"Invalid file type: {extension}. Only JPEG files are supported.");
        }

        private string DetermineOutputPath(ImageMetadata metadata, string sourcePath)
        {
            // Use THIS pipeline's output configuration
            var baseOutputFolder = _pipelineConfig.WatchSettings.OutputPath
                                   ?? _pipelineConfig.ProcessingOptions.ArchiveFolder
                                   ?? @"C:\CamBridge\Output";

            // Apply output organization from pipeline settings
            var outputPath = baseOutputFolder;

            switch (_pipelineConfig.ProcessingOptions.OutputOrganization)
            {
                case OutputOrganization.ByPatient:
                    outputPath = Path.Combine(baseOutputFolder, metadata.Patient.Id.Value);
                    break;

                case OutputOrganization.ByDate:
                    outputPath = Path.Combine(baseOutputFolder, metadata.Study.StudyDate.ToString("yyyy-MM-dd"));
                    break;

                case OutputOrganization.ByPatientAndDate:
                    outputPath = Path.Combine(baseOutputFolder,
                        metadata.Patient.Id.Value,
                        metadata.Study.StudyDate.ToString("yyyy-MM-dd"));
                    break;
            }

            // Generate filename
            var fileName = GenerateFileName(metadata);
            return Path.Combine(outputPath, fileName);
        }

        private string GenerateFileName(ImageMetadata metadata)
        {
            if (!string.IsNullOrEmpty(_pipelineConfig.ProcessingOptions.OutputFilePattern))
            {
                var fileName = _pipelineConfig.ProcessingOptions.OutputFilePattern
                    .Replace("{PatientID}", metadata.Patient.Id.Value)
                    .Replace("{StudyDate}", metadata.Study.StudyDate.ToString("yyyyMMdd"))
                    .Replace("{StudyTime}", metadata.Study.StudyDate.ToString("HHmmss"))
                    .Replace("{InstanceNumber}", metadata.InstanceNumber.ToString("D4"));

                return $"{fileName}.dcm";
            }

            // Default pattern
            return $"{metadata.Patient.Id.Value}_{metadata.Study.StudyDate:yyyyMMdd}_{metadata.InstanceNumber:D4}.dcm";
        }

        private async Task HandleProcessingSuccess(string inputPath, string outputPath, ProcessingOptions options)
        {
            try
            {
                switch (options.SuccessAction)
                {
                    case PostProcessingAction.Delete:
                        File.Delete(inputPath);
                        _logger.LogDebug("Deleted source file: {FilePath}", inputPath);
                        break;

                    case PostProcessingAction.Archive:
                        if (!string.IsNullOrEmpty(options.ArchiveFolder))
                        {
                            var archivePath = GetArchivePath(inputPath, options.ArchiveFolder);
                            Directory.CreateDirectory(Path.GetDirectoryName(archivePath)!);
                            File.Move(inputPath, archivePath, true);
                            _logger.LogDebug("Archived source file to: {ArchivePath}", archivePath);
                        }
                        break;

                    case PostProcessingAction.Leave:
                    default:
                        // Do nothing
                        break;
                }

                // Create backup if configured
                if (options.CreateBackup && !string.IsNullOrEmpty(options.BackupFolder))
                {
                    var backupPath = GetArchivePath(inputPath, options.BackupFolder);
                    Directory.CreateDirectory(Path.GetDirectoryName(backupPath)!);
                    File.Copy(inputPath, backupPath, true);
                    _logger.LogDebug("Created backup at: {BackupPath}", backupPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during post-processing of {FilePath}", inputPath);
                // Don't fail the entire operation for post-processing errors
            }

            await Task.CompletedTask;
        }

        private async Task HandleProcessingFailure(string inputPath, string error, ProcessingOptions options)
        {
            try
            {
                switch (options.FailureAction)
                {
                    case PostProcessingAction.MoveToError:
                        if (!string.IsNullOrEmpty(options.ErrorFolder))
                        {
                            var errorPath = GetArchivePath(inputPath, options.ErrorFolder);
                            Directory.CreateDirectory(Path.GetDirectoryName(errorPath)!);

                            // Move file to error folder
                            File.Move(inputPath, errorPath, true);

                            // Create error details file
                            var errorDetailsPath = Path.ChangeExtension(errorPath, ".error.txt");
                            var errorDetails = $"Error processing file: {inputPath}\n" +
                                             $"Pipeline: {_pipelineConfig.Name} ({_pipelineConfig.Id})\n" +
                                             $"Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}\n" +
                                             $"Error: {error}";
                            await File.WriteAllTextAsync(errorDetailsPath, errorDetails);

                            _logger.LogDebug("Moved failed file to: {ErrorPath}", errorPath);
                        }
                        break;

                    case PostProcessingAction.Leave:
                    default:
                        // Do nothing
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during failure handling of {FilePath}", inputPath);
                // Don't fail the entire operation for post-processing errors
            }
        }

        private string GetArchivePath(string sourcePath, string archiveFolder)
        {
            var fileName = Path.GetFileName(sourcePath);
            var relativePath = DateTime.UtcNow.ToString("yyyy-MM-dd");
            return Path.Combine(archiveFolder, relativePath, fileName);
        }

        /// <summary>
        /// Applies pipeline-specific DICOM overrides to global settings
        /// </summary>
        private DicomSettings ApplyDicomOverrides(DicomSettings globalSettings, DicomOverrides? overrides)
        {
            if (overrides == null)
                return globalSettings;

            // Create a copy of global settings
            var settings = new DicomSettings
            {
                ImplementationClassUid = globalSettings.ImplementationClassUid,
                ImplementationVersionName = globalSettings.ImplementationVersionName,
                SourceApplicationEntityTitle = globalSettings.SourceApplicationEntityTitle,
                InstitutionName = overrides.InstitutionName ?? globalSettings.InstitutionName,
                InstitutionDepartment = overrides.InstitutionDepartment ?? globalSettings.InstitutionDepartment,
                StationName = overrides.StationName ?? globalSettings.StationName,
                Modality = globalSettings.Modality,
                ValidateAfterCreation = globalSettings.ValidateAfterCreation
            };

            return settings;
        }
    }

    // Event argument classes remain the same
    public class FileProcessingEventArgs : EventArgs
    {
        public string FilePath { get; set; } = string.Empty;
        public string? OutputPath { get; set; }
        public bool Success { get; set; }
    }

    public class FileProcessingErrorEventArgs : EventArgs
    {
        public string FilePath { get; set; } = string.Empty;
        public Exception Error { get; set; } = null!;
    }

    // Result class with pipeline tracking
    public class FileProcessingResult
    {
        public string SourceFile { get; set; } = string.Empty;
        public string? OutputFile { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public Exception? Exception { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan ProcessingDuration { get; set; }
        public Guid PipelineId { get; set; } // NEW: Track which pipeline processed this
    }
}
