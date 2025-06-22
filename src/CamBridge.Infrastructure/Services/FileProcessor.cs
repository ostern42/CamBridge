// src/CamBridge.Infrastructure/Services/FileProcessor.cs
// Version: 0.7.28
// Description: Pipeline-aware file processor with correct business event logging
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Entities;
using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Orchestrates the complete JPEG to DICOM conversion process for a specific pipeline
    /// KISS UPDATE: No more IFileProcessor interface - direct dependency pattern!
    /// PIPELINE UPDATE: Each pipeline gets its own FileProcessor instance with isolated logger!
    /// </summary>
    public class FileProcessor
    {
        private readonly ILogger _logger;  // Pipeline-specific logger!
        private readonly ExifToolReader _exifToolReader;
        private readonly DicomConverter _dicomConverter;
        private readonly PipelineConfiguration _pipelineConfig;
        private readonly DicomSettings _dicomSettings;

        public event EventHandler<FileProcessingEventArgs>? ProcessingStarted;
        public event EventHandler<FileProcessingEventArgs>? ProcessingCompleted;
        public event EventHandler<FileProcessingErrorEventArgs>? ProcessingError;

        /// <summary>
        /// Creates a FileProcessor for a specific pipeline
        /// Each pipeline gets its own instance with its own configuration AND logger!
        /// </summary>
        public FileProcessor(
            ILogger logger,  // Pipeline-specific logger passed from PipelineManager
            ExifToolReader exifToolReader,
            DicomConverter dicomConverter,
            PipelineConfiguration pipelineConfig,
            DicomSettings globalDicomSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _exifToolReader = exifToolReader ?? throw new ArgumentNullException(nameof(exifToolReader));
            _dicomConverter = dicomConverter ?? throw new ArgumentNullException(nameof(dicomConverter));
            _pipelineConfig = pipelineConfig ?? throw new ArgumentNullException(nameof(pipelineConfig));
            _dicomSettings = ApplyDicomOverrides(globalDicomSettings, pipelineConfig.DicomOverrides);

            // Changed to DEBUG - technical initialization detail
            _logger.LogDebug("Created FileProcessor for pipeline: {PipelineName} ({PipelineId})",
                _pipelineConfig.Name, _pipelineConfig.Id);
        }

        /// <summary>
        /// Determines if a file should be processed based on pipeline configuration
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
                var validExtensions = new[] { ".jpg", ".jpeg", ".jpe", ".jfif" };
                if (!validExtensions.Contains(extension))
                    return false;

                // Check file size limits if configured
                var fileInfo = new FileInfo(filePath);

                // Minimum size check
                if (_pipelineConfig.ProcessingOptions.MinimumFileSizeBytes.HasValue &&
                    fileInfo.Length < _pipelineConfig.ProcessingOptions.MinimumFileSizeBytes.Value)
                {
                    _logger.LogDebug("File {FilePath} is too small ({Size} bytes)", filePath, fileInfo.Length);
                    return false;
                }

                // Maximum size check
                if (_pipelineConfig.ProcessingOptions.MaximumFileSizeBytes.HasValue &&
                    fileInfo.Length > _pipelineConfig.ProcessingOptions.MaximumFileSizeBytes.Value)
                {
                    _logger.LogDebug("File {FilePath} is too large ({Size} bytes)", filePath, fileInfo.Length);
                    return false;
                }

                // Check file age if configured
                if (_pipelineConfig.ProcessingOptions.MaxFileAge.HasValue)
                {
                    var fileAge = DateTime.Now - fileInfo.CreationTime;
                    if (fileAge > _pipelineConfig.ProcessingOptions.MaxFileAge.Value)
                    {
                        _logger.LogDebug("File {FilePath} is too old ({Age})", filePath, fileAge);
                        return false;
                    }
                }

                // Check if file is still being written (wait for minimum age)
                var minimumAgeSeconds = _pipelineConfig.WatchSettings.MinimumFileAgeSeconds;
                var fileAgeSeconds = (DateTime.Now - fileInfo.LastWriteTime).TotalSeconds;
                if (fileAgeSeconds < minimumAgeSeconds)
                {
                    _logger.LogDebug("File {FilePath} is too new, waiting for stability ({Age}s < {Min}s)",
                        filePath, fileAgeSeconds, minimumAgeSeconds);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if file should be processed: {FilePath}", filePath);
                return false;
            }
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
                // Keep as INFO - important business event
                _logger.LogInformation("Processing file: {FileName}", Path.GetFileName(filePath));
                ProcessingStarted?.Invoke(this, new FileProcessingEventArgs { FilePath = filePath });

                // Validate input file
                ValidateInputFile(filePath);

                // Extract EXIF data
                _logger.LogDebug("Extracting EXIF data from {FilePath}", filePath);
                var exifStopwatch = Stopwatch.StartNew();
                var metadata = await _exifToolReader.ExtractMetadataAsync(filePath);
                exifStopwatch.Stop();

                // Changed to DEBUG - performance metric
                _logger.LogDebug("EXIF extraction completed in {ElapsedMs}ms", exifStopwatch.ElapsedMilliseconds);

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

                // Convert to DICOM using pipeline-specific settings
                _logger.LogDebug("Converting to DICOM: {OutputPath}", outputPath);
                var dicomStopwatch = Stopwatch.StartNew();
                var conversionResult = await _dicomConverter.ConvertToDicomAsync(
                    filePath,
                    outputPath,
                    metadata);
                dicomStopwatch.Stop();

                // Changed to DEBUG - performance metric
                _logger.LogDebug("DICOM conversion completed in {ElapsedMs}ms", dicomStopwatch.ElapsedMilliseconds);

                result.Success = conversionResult.Success;
                result.OutputFile = outputPath;
                result.EndTime = DateTime.UtcNow;
                result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

                if (result.Success)
                {
                    // Keep as INFO - important business event
                    _logger.LogInformation(
                        "Successfully converted {FileName} to DICOM in {ElapsedMs}ms",
                        Path.GetFileName(filePath), result.ProcessingTimeMs);

                    // NEW: Add performance warning if processing was slow
                    if (result.ProcessingTimeMs > 5000)
                    {
                        _logger.LogWarning("Slow processing detected for {FileName}: {ElapsedMs}ms",
                            Path.GetFileName(filePath), result.ProcessingTimeMs);
                    }

                    // Handle post-processing based on pipeline configuration
                    await HandlePostProcessingAsync(filePath, result.Success);

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

                // Check if this is a critical error that threatens the pipeline
                if (ex is UnauthorizedAccessException && filePath.StartsWith(_pipelineConfig.WatchSettings.Path))
                {
                    _logger.LogCritical(ex, "Cannot access watch folder {Path} - pipeline will fail!",
                        _pipelineConfig.WatchSettings.Path);
                }

                // Move to error folder if configured
                await MoveToErrorFolderAsync(filePath, ex.Message);

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
                throw new FileNotFoundException($"Input file not found: {filePath}");
            }

            var fileInfo = new FileInfo(filePath);

            // Check file size limits if configured
            if (_pipelineConfig.ProcessingOptions.MaximumFileSizeBytes.HasValue)
            {
                if (fileInfo.Length > _pipelineConfig.ProcessingOptions.MaximumFileSizeBytes.Value)
                {
                    throw new InvalidOperationException(
                        $"File size ({fileInfo.Length / 1024 / 1024}MB) exceeds maximum allowed size " +
                        $"({_pipelineConfig.ProcessingOptions.MaximumFileSizeBytes.Value / 1024 / 1024}MB)");
                }
            }

            // Validate extension
            var validExtensions = new[] { ".jpg", ".jpeg", ".jpe", ".jfif" };
            if (!validExtensions.Contains(fileInfo.Extension.ToLowerInvariant()))
            {
                throw new InvalidOperationException(
                    $"Invalid file extension: {fileInfo.Extension}. Expected JPEG file.");
            }
        }

        private string DetermineOutputPath(ImageMetadata metadata, string sourceFile)
        {
            // Get base output path from watch settings
            var baseOutputPath = _pipelineConfig.WatchSettings.OutputPath
                ?? Path.Combine(Path.GetDirectoryName(_pipelineConfig.WatchSettings.Path) ?? "", "Output");

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
                    }
                    break;

                case OutputOrganization.ByDate:
                    var dateFolder = metadata.Study != null
                        ? metadata.Study.StudyDate.ToString("yyyy-MM-dd")
                        : DateTime.Now.ToString("yyyy-MM-dd");
                    outputDir = Path.Combine(baseOutputPath, dateFolder);
                    break;

                case OutputOrganization.ByPatientAndDate:
                    if (!string.IsNullOrEmpty(metadata.Patient?.PatientName))
                    {
                        var safeName = SanitizeForPath(metadata.Patient.PatientName);
                        var dateFolder2 = metadata.Study != null
                            ? metadata.Study.StudyDate.ToString("yyyy-MM-dd")
                            : DateTime.Now.ToString("yyyy-MM-dd");
                        outputDir = Path.Combine(baseOutputPath, safeName, dateFolder2);
                    }
                    break;

                case OutputOrganization.None:
                default:
                    // Use base output path as-is
                    break;
            }

            // Add DICOM extension
            return Path.Combine(outputDir, $"{fileName}.dcm");
        }

        private string SanitizeForPath(string input)
        {
            var invalid = Path.GetInvalidFileNameChars()
                .Concat(Path.GetInvalidPathChars())
                .Distinct()
                .ToArray();

            return string.Join("_", input.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
        }

        private async Task HandlePostProcessingAsync(string filePath, bool success)
        {
            var action = success
                ? _pipelineConfig.ProcessingOptions.SuccessAction
                : _pipelineConfig.ProcessingOptions.FailureAction;

            try
            {
                switch (action)
                {
                    case PostProcessingAction.Delete:
                        File.Delete(filePath);
                        _logger.LogDebug("Deleted source file: {FilePath}", filePath);
                        break;

                    case PostProcessingAction.Archive:
                        var archivePath = Path.Combine(
                            _pipelineConfig.ProcessingOptions.ArchiveFolder,
                            Path.GetFileName(filePath));

                        Directory.CreateDirectory(Path.GetDirectoryName(archivePath)!);
                        File.Move(filePath, archivePath, true);
                        _logger.LogDebug("Archived source file: {FilePath} -> {ArchivePath}",
                            filePath, archivePath);
                        break;

                    case PostProcessingAction.MoveToError:
                        // This is typically for failure action
                        if (!success)
                        {
                            await MoveToErrorFolderAsync(filePath, "Processing failed");
                        }
                        break;

                    default:
                        // Leave file as-is
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to perform post-processing action {Action} on {FilePath}",
                    action, filePath);
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
                File.Move(filePath, errorPath, true);

                _logger.LogDebug("Moved failed file to error folder: {ErrorPath}", errorPath);
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

            // Create a copy of global settings
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
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long ProcessingTimeMs { get; set; }
        public Guid PipelineId { get; set; }
    }
}
