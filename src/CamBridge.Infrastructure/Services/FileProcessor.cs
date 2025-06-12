// src/CamBridge.Infrastructure/Services/FileProcessor.cs
// Version: 0.7.8
// Description: Complete file processor with SIMPLE error handling - KISS approach!
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
using Microsoft.Extensions.Options;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Orchestrates the complete JPEG to DICOM conversion process
    /// KISS UPDATE: No more IFileProcessor interface - direct dependency pattern!
    /// </summary>
    public class FileProcessor // KISS: No interface inheritance!
    {
        private readonly ILogger<FileProcessor> _logger;
        private readonly ExifToolReader _exifToolReader;
        private readonly DicomConverter _dicomConverter;
        private readonly ProcessingOptions _processingOptions;
        private readonly CamBridgeSettings _settings;

        public event EventHandler<FileProcessingEventArgs>? ProcessingStarted;
        public event EventHandler<FileProcessingEventArgs>? ProcessingCompleted;
        public event EventHandler<FileProcessingErrorEventArgs>? ProcessingError;

        public FileProcessor(
            ILogger<FileProcessor> logger,
            ExifToolReader exifToolReader,
            DicomConverter dicomConverter,
            IOptions<ProcessingOptions> processingOptions,
            IOptions<CamBridgeSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _exifToolReader = exifToolReader ?? throw new ArgumentNullException(nameof(exifToolReader));
            _dicomConverter = dicomConverter ?? throw new ArgumentNullException(nameof(dicomConverter));
            _processingOptions = processingOptions?.Value ?? throw new ArgumentNullException(nameof(processingOptions));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Processes a single JPEG file
        /// </summary>
        public async Task<FileProcessingResult> ProcessFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Starting processing of {FilePath}", filePath);
                ProcessingStarted?.Invoke(this, new FileProcessingEventArgs(filePath));

                // Validate file
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File not found: {filePath}");
                }

                var fileInfo = new FileInfo(filePath);
                ValidateFile(fileInfo);

                // Create backup if configured
                if (_processingOptions.CreateBackup)
                {
                    await CreateBackupAsync(fileInfo);
                }

                // Extract metadata - simplified with new pipeline!
                var metadata = await ExtractMetadataAsync(filePath);

                // Determine output path
                var outputPath = DetermineOutputPath(metadata, filePath);

                // Convert to DICOM - KISS: Direct method call, no interface!
                var conversionResult = await _dicomConverter.ConvertToDicomAsync(
                    filePath,
                    outputPath,
                    metadata);

                if (!conversionResult.Success)
                {
                    throw new InvalidOperationException(
                        $"DICOM conversion failed: {conversionResult.ErrorMessage}");
                }

                // Validate DICOM if configured
                if (_settings.Dicom.ValidateAfterCreation)
                {
                    var validationResult = await _dicomConverter.ValidateDicomFileAsync(outputPath);
                    if (!validationResult.IsValid)
                    {
                        _logger.LogWarning("DICOM validation warnings: {Warnings}",
                            string.Join("; ", validationResult.Warnings));
                    }
                }

                // Handle source file based on success
                await HandleSourceFileAsync(filePath, _processingOptions.SuccessAction);

                stopwatch.Stop();
                var result = FileProcessingResult.CreateSuccess(filePath, outputPath, stopwatch.Elapsed);

                _logger.LogInformation("Successfully processed {FilePath} to {OutputPath} in {ElapsedMs}ms",
                    filePath, outputPath, stopwatch.ElapsedMilliseconds);

                ProcessingCompleted?.Invoke(this, new FileProcessingEventArgs(filePath));
                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error processing file {FilePath}", filePath);

                ProcessingError?.Invoke(this, new FileProcessingErrorEventArgs(filePath, ex));

                // KISS: Simple error handling - move to error folder!
                try
                {
                    await HandleProcessingError(filePath, ex);
                }
                catch (Exception moveEx)
                {
                    _logger.LogError(moveEx, "Error moving failed file {FilePath}", filePath);
                }

                return FileProcessingResult.CreateFailure(filePath, ex.Message, stopwatch.Elapsed);
            }
        }

        /// <summary>
        /// SIMPLE error handling - move to error folder with details
        /// </summary>
        private async Task HandleProcessingError(string filePath, Exception ex)
        {
            _logger.LogError(ex, "Failed to process file: {FilePath}", filePath);

            // Simple error handling - move to error folder
            try
            {
                var errorFolder = _processingOptions.ErrorFolder;
                if (!Directory.Exists(errorFolder))
                    Directory.CreateDirectory(errorFolder);

                var fileName = Path.GetFileName(filePath);
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var errorFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{timestamp}{Path.GetExtension(fileName)}";
                var errorPath = Path.Combine(errorFolder, errorFileName);

                // Move file
                File.Move(filePath, errorPath, overwrite: true);

                // Write error details
                var errorInfoPath = Path.ChangeExtension(errorPath, ".error.txt");
                var errorInfo = $"""
                    CamBridge Error Report
                    ======================
                    
                    Original File: {fileName}
                    Error Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
                    
                    Error Message:
                    {ex.Message}
                    
                    Error Type:
                    {ex.GetType().FullName}
                    
                    Stack Trace:
                    {ex.StackTrace}
                    
                    Inner Exception:
                    {ex.InnerException?.ToString() ?? "None"}
                    """;

                await File.WriteAllTextAsync(errorInfoPath, errorInfo);

                _logger.LogInformation("Moved failed file to error folder: {ErrorPath}", errorPath);
            }
            catch (Exception moveEx)
            {
                _logger.LogError(moveEx, "Failed to move file to error folder");
            }
        }

        /// <summary>
        /// Checks if a file should be processed
        /// </summary>
        public bool ShouldProcessFile(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                    return false;

                var fileInfo = new FileInfo(filePath);

                // Check file extension
                var extension = fileInfo.Extension.ToLowerInvariant();
                if (extension != ".jpg" && extension != ".jpeg")
                    return false;

                // Check file size
                var minSize = _processingOptions.MinimumFileSizeBytes ?? 1024; // Default 1KB
                var maxSize = _processingOptions.MaximumFileSizeBytes ?? 100 * 1024 * 1024; // Default 100MB

                if (fileInfo.Length < minSize || fileInfo.Length > maxSize)
                {
                    _logger.LogDebug("File {FilePath} size {Size} outside configured range",
                        filePath, fileInfo.Length);
                    return false;
                }

                // Check file age
                if (_processingOptions.MaxFileAge.HasValue)
                {
                    var fileAge = DateTime.UtcNow - fileInfo.CreationTimeUtc;
                    if (fileAge > _processingOptions.MaxFileAge.Value)
                    {
                        _logger.LogDebug("File {FilePath} is too old ({Age} days)",
                            filePath, fileAge.TotalDays);
                        return false;
                    }
                }

                // Check if file is still being written
                if (!IsFileReady(filePath))
                {
                    _logger.LogDebug("File {FilePath} is not ready (still being written)",
                        filePath);
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
        /// Simplified metadata extraction using the new ExifToolReader
        /// </summary>
        private async Task<ImageMetadata> ExtractMetadataAsync(string filePath)
        {
            // ExifToolReader does all the heavy lifting now!
            var metadata = await _exifToolReader.ExtractMetadataAsync(filePath);

            _logger.LogInformation("Extracted metadata for {Patient} - {Study}",
                metadata.Patient.Id.Value, metadata.Study.StudyId.Value);

            return metadata;
        }

        private string DetermineOutputPath(ImageMetadata metadata, string sourcePath)
        {
            // Find the matching watch folder configuration
            FolderConfiguration? folderConfig = null;
            foreach (var watchFolder in _settings.WatchFolders)
            {
                if (sourcePath.StartsWith(watchFolder.Path, StringComparison.OrdinalIgnoreCase))
                {
                    folderConfig = watchFolder;
                    break;
                }
            }

            // Determine base output folder
            var baseOutputFolder = folderConfig?.OutputPath ?? _settings.DefaultOutputFolder;

            // Apply output organization
            var organizedPath = ApplyOutputOrganization(baseOutputFolder, metadata);

            // Generate filename
            var fileName = GenerateOutputFileName(metadata);

            return Path.Combine(organizedPath, fileName);
        }

        private string ApplyOutputOrganization(string baseFolder, ImageMetadata metadata)
        {
            var path = baseFolder;

            switch (_processingOptions.OutputOrganization)
            {
                case OutputOrganization.ByPatient:
                    path = Path.Combine(path, SanitizePathComponent(metadata.Patient.Id.Value));
                    break;

                case OutputOrganization.ByDate:
                    path = Path.Combine(path, metadata.Study.StudyDate.ToString("yyyy-MM-dd"));
                    break;

                case OutputOrganization.ByPatientAndDate:
                    path = Path.Combine(path,
                        SanitizePathComponent(metadata.Patient.Id.Value),
                        metadata.Study.StudyDate.ToString("yyyy-MM-dd"));
                    break;
            }

            // Ensure directory exists
            Directory.CreateDirectory(path);
            return path;
        }

        private string GenerateOutputFileName(ImageMetadata metadata)
        {
            var pattern = _processingOptions.OutputFilePattern;

            // Fix: Handle null pattern with default
            if (string.IsNullOrEmpty(pattern))
            {
                pattern = "{PatientID}_{StudyDate}_{InstanceNumber}";
            }

            // Replace tokens - now pattern is guaranteed non-null
            string fileName = pattern;
            fileName = fileName.Replace("{PatientID}", SanitizeFileName(metadata.Patient.Id.Value));
            fileName = fileName.Replace("{PatientName}", SanitizeFileName(metadata.Patient.Name));
            fileName = fileName.Replace("{StudyDate}", metadata.Study.StudyDate.ToString("yyyyMMdd"));
            fileName = fileName.Replace("{StudyID}", SanitizeFileName(metadata.Study.StudyId.Value));
            fileName = fileName.Replace("{ExamID}", SanitizeFileName(metadata.Study.ExamId ?? ""));
            fileName = fileName.Replace("{InstanceNumber}", metadata.InstanceNumber.ToString("D4"));
            fileName = fileName.Replace("{Timestamp}", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));

            // Ensure .dcm extension
            if (!fileName.EndsWith(".dcm", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".dcm";
            }

            return fileName;
        }

        private async Task CreateBackupAsync(FileInfo fileInfo)
        {
            var backupPath = Path.Combine(
                _processingOptions.BackupFolder,
                DateTime.UtcNow.ToString("yyyy-MM-dd"),
                fileInfo.Name);

            var backupDir = Path.GetDirectoryName(backupPath)!;
            Directory.CreateDirectory(backupDir);

            await CopyFileAsync(fileInfo.FullName, backupPath);
            _logger.LogDebug("Created backup of {SourceFile} at {BackupPath}",
                fileInfo.FullName, backupPath);
        }

        private async Task HandleSourceFileAsync(string filePath, PostProcessingAction action)
        {
            switch (action)
            {
                case PostProcessingAction.Archive:
                    var archivePath = Path.Combine(
                        _processingOptions.ArchiveFolder,
                        DateTime.UtcNow.ToString("yyyy-MM-dd"),
                        Path.GetFileName(filePath));

                    var archiveDir = Path.GetDirectoryName(archivePath)!;
                    Directory.CreateDirectory(archiveDir);

                    await MoveFileAsync(filePath, archivePath);
                    _logger.LogInformation("Archived {SourceFile} to {ArchivePath}",
                        filePath, archivePath);
                    break;

                case PostProcessingAction.Delete:
                    File.Delete(filePath);
                    _logger.LogInformation("Deleted source file {FilePath}", filePath);
                    break;

                case PostProcessingAction.MoveToError:
                    // This is now handled by HandleProcessingError
                    break;

                case PostProcessingAction.Leave:
                default:
                    // Do nothing
                    break;
            }
        }

        private void ValidateFile(FileInfo fileInfo)
        {
            var minSize = _processingOptions.MinimumFileSizeBytes ?? 1024;
            var maxSize = _processingOptions.MaximumFileSizeBytes ?? 100 * 1024 * 1024;

            if (fileInfo.Length < minSize)
            {
                throw new InvalidOperationException(
                    $"File too small: {fileInfo.Length} bytes (minimum: {minSize})");
            }

            if (fileInfo.Length > maxSize)
            {
                throw new InvalidOperationException(
                    $"File too large: {fileInfo.Length} bytes (maximum: {maxSize})");
            }
        }

        private bool IsFileReady(string filePath)
        {
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }

        private async Task CopyFileAsync(string source, string destination)
        {
            using var sourceStream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var destStream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None);
            await sourceStream.CopyToAsync(destStream);
        }

        private async Task MoveFileAsync(string source, string destination)
        {
            // Try direct move first
            try
            {
                File.Move(source, destination, true);
            }
            catch (IOException)
            {
                // Fall back to copy and delete
                await CopyFileAsync(source, destination);
                File.Delete(source);
            }
        }

        private string SanitizeFileName(string fileName)
        {
            var invalid = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
        }

        private string SanitizePathComponent(string pathComponent)
        {
            var invalid = Path.GetInvalidPathChars();
            return string.Join("_", pathComponent.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
        }
    }

    /// <summary>
    /// Result of file processing operation
    /// </summary>
    public class FileProcessingResult
    {
        public string SourceFile { get; init; } = string.Empty;
        public string? OutputFile { get; init; }
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }
        public DateTime ProcessedAt { get; init; } = DateTime.UtcNow;
        public TimeSpan ProcessingTime { get; init; }

        public static FileProcessingResult CreateSuccess(string source, string output, TimeSpan time)
            => new()
            {
                SourceFile = source,
                OutputFile = output,
                Success = true,
                ProcessingTime = time
            };

        public static FileProcessingResult CreateFailure(string source, string error, TimeSpan time)
            => new()
            {
                SourceFile = source,
                Success = false,
                ErrorMessage = error,
                ProcessingTime = time
            };
    }

    public class FileProcessingEventArgs : EventArgs
    {
        public string FilePath { get; }
        public DateTime Timestamp { get; }

        public FileProcessingEventArgs(string filePath)
        {
            FilePath = filePath;
            Timestamp = DateTime.UtcNow;
        }
    }

    public class FileProcessingErrorEventArgs : FileProcessingEventArgs
    {
        public Exception Exception { get; }

        public FileProcessingErrorEventArgs(string filePath, Exception exception)
            : base(filePath)
        {
            Exception = exception;
        }
    }
}
