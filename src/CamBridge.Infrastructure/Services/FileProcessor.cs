using System;
using System.IO;
using System.Threading.Tasks;
using CamBridge.Core.Entities;
using CamBridge.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Updated FileProcessor to use the new simplified ExifTool pipeline
    /// </summary>
    public class FileProcessor : IFileProcessor
    {
        private readonly ILogger<FileProcessor> _logger;
        private readonly ExifToolReader _exifToolReader;
        private readonly IDicomConverter _dicomConverter;
        private readonly ProcessingQueue _processingQueue;
        private readonly DeadLetterQueue _deadLetterQueue;

        public event EventHandler<FileProcessingEventArgs>? ProcessingStarted;
        public event EventHandler<FileProcessingEventArgs>? ProcessingCompleted;
        public event EventHandler<FileProcessingErrorEventArgs>? ProcessingError;

        public FileProcessor(
            ILogger<FileProcessor> logger,
            ExifToolReader exifToolReader,
            IDicomConverter dicomConverter,
            ProcessingQueue processingQueue,
            DeadLetterQueue deadLetterQueue)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _exifToolReader = exifToolReader ?? throw new ArgumentNullException(nameof(exifToolReader));
            _dicomConverter = dicomConverter ?? throw new ArgumentNullException(nameof(dicomConverter));
            _processingQueue = processingQueue ?? throw new ArgumentNullException(nameof(processingQueue));
            _deadLetterQueue = deadLetterQueue ?? throw new ArgumentNullException(nameof(deadLetterQueue));
        }

        /// <inheritdoc />
        public async Task<FileProcessingResult> ProcessFileAsync(string filePath)
        {
            var startTime = DateTime.UtcNow;

            try
            {
                _logger.LogInformation("Starting processing of file: {FilePath}", filePath);
                ProcessingStarted?.Invoke(this, new FileProcessingEventArgs(filePath));

                // Validate file
                if (!ShouldProcessFile(filePath))
                {
                    var message = $"File {filePath} is not a valid JPEG for processing";
                    _logger.LogWarning(message);
                    return FileProcessingResult.CreateFailure(filePath, message, DateTime.UtcNow - startTime);
                }

                // Add to processing queue
                await _processingQueue.EnqueueAsync(filePath);

                try
                {
                    // Extract metadata using ExifTool (the ONLY way!)
                    _logger.LogDebug("Extracting metadata from {FilePath}", filePath);
                    var metadata = await _exifToolReader.ExtractMetadataAsync(filePath);

                    // Validate metadata
                    if (string.IsNullOrWhiteSpace(metadata.Patient.PatientId) ||
                        metadata.Patient.PatientId == "UNKNOWN")
                    {
                        throw new InvalidOperationException(
                            "No valid patient ID found in image. Ensure QRBridge data is present.");
                    }

                    // Convert to DICOM
                    _logger.LogDebug("Converting to DICOM: {PatientId} - {StudyId}",
                        metadata.Patient.PatientId, metadata.Study.StudyId);

                    var dicomResult = await _dicomConverter.ConvertAsync(metadata);

                    if (!dicomResult.Success)
                    {
                        throw new InvalidOperationException(
                            $"DICOM conversion failed: {dicomResult.ErrorMessage}");
                    }

                    // Success!
                    var processingTime = DateTime.UtcNow - startTime;
                    _logger.LogInformation(
                        "Successfully processed {FilePath} -> {OutputPath} in {Time:F2}s",
                        filePath, dicomResult.OutputPath, processingTime.TotalSeconds);

                    ProcessingCompleted?.Invoke(this, new FileProcessingEventArgs(filePath));

                    return FileProcessingResult.CreateSuccess(
                        filePath,
                        dicomResult.OutputPath!,
                        processingTime);
                }
                finally
                {
                    // Remove from processing queue
                    await _processingQueue.DequeueAsync(filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file: {FilePath}", filePath);

                // Add to dead letter queue
                await _deadLetterQueue.AddAsync(filePath, ex.Message);

                ProcessingError?.Invoke(this, new FileProcessingErrorEventArgs(filePath, ex));

                return FileProcessingResult.CreateFailure(
                    filePath,
                    ex.Message,
                    DateTime.UtcNow - startTime);
            }
        }

        /// <inheritdoc />
        public bool ShouldProcessFile(string filePath)
        {
            try
            {
                // Check if file exists
                if (!File.Exists(filePath))
                {
                    _logger.LogDebug("File does not exist: {FilePath}", filePath);
                    return false;
                }

                // Check file extension
                var extension = Path.GetExtension(filePath)?.ToLower();
                if (string.IsNullOrEmpty(extension) ||
                    !(extension == ".jpg" || extension == ".jpeg" || extension == ".jpe" || extension == ".jfif"))
                {
                    _logger.LogDebug("Unsupported file extension: {Extension}", extension);
                    return false;
                }

                // Check if file is already being processed
                if (_processingQueue.IsProcessing(filePath))
                {
                    _logger.LogDebug("File is already being processed: {FilePath}", filePath);
                    return false;
                }

                // Check if file is in dead letter queue (optional: could have retry logic)
                if (_deadLetterQueue.Contains(filePath))
                {
                    _logger.LogDebug("File is in dead letter queue: {FilePath}", filePath);
                    return false;
                }

                // Check if it's a valid JPEG by reading the header
                try
                {
                    using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                    if (fs.Length < 2)
                        return false;

                    var header = new byte[2];
                    fs.Read(header, 0, 2);

                    // JPEG files start with 0xFFD8
                    if (header[0] != 0xFF || header[1] != 0xD8)
                    {
                        _logger.LogDebug("File is not a valid JPEG: {FilePath}", filePath);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Cannot read file: {FilePath}", filePath);
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
    }
}
