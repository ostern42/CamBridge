// src\CamBridge.Infrastructure\Services\PacsUploadQueue.cs
// Version: 0.8.3
// Description: Queue for PACS uploads with enhanced retry logging
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CamBridge.Core;
using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Item queued for PACS upload
    /// </summary>
    public class PacsUploadItem
    {
        public string DicomFilePath { get; init; } = string.Empty;
        public DateTime EnqueuedAt { get; init; } = DateTime.UtcNow;
        public int AttemptCount { get; set; }
        public DateTime? LastAttemptAt { get; set; }
        public string? LastError { get; set; }
        public Guid CorrelationId { get; init; } = Guid.NewGuid();
    }

    /// <summary>
    /// Error information for failed uploads
    /// </summary>
    public class PacsUploadError
    {
        public string FilePath { get; init; } = string.Empty;
        public string Error { get; init; } = string.Empty;
        public string UserFriendlyError { get; init; } = string.Empty;
        public DicomErrorType ErrorType { get; init; }
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
        public int AttemptNumber { get; init; }
        public bool WillRetry { get; init; }
    }

    /// <summary>
    /// Queue for uploading DICOM files to PACS with enhanced logging
    /// </summary>
    public class PacsUploadQueue : IDisposable
    {
        private readonly PipelineConfiguration _pipelineConfig;
        private readonly DicomStoreService _storeService;
        private readonly ILogger<PacsUploadQueue> _logger;
        private readonly Channel<PacsUploadItem> _queue;
        private readonly List<PacsUploadError> _recentErrors = new();
        private readonly object _errorLock = new();
        private bool _disposed;

        // Statistics
        private int _totalQueued;
        private int _totalSuccessful;
        private int _totalFailed;
        private DateTime? _lastSuccessfulUpload;
        private DateTime? _lastFailedUpload;

        public PacsUploadQueue(
            PipelineConfiguration pipelineConfig,
            DicomStoreService storeService,
            ILogger<PacsUploadQueue> logger)
        {
            _pipelineConfig = pipelineConfig ?? throw new ArgumentNullException(nameof(pipelineConfig));
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var pacsConfig = _pipelineConfig.PacsConfiguration;
            if (pacsConfig == null || !pacsConfig.Enabled)
            {
                throw new InvalidOperationException("PACS configuration is not enabled for this pipeline");
            }

            // Create bounded channel with capacity
            var channelOptions = new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            };

            _queue = Channel.CreateBounded<PacsUploadItem>(channelOptions);

            _logger.LogInformation("Created PACS upload queue for pipeline {Pipeline} with concurrency {Concurrency}",
                _pipelineConfig.Name,
                pacsConfig.MaxConcurrentUploads);
        }

        /// <summary>
        /// Queue a DICOM file for upload
        /// </summary>
        public async Task<bool> EnqueueAsync(string dicomFilePath, CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(PacsUploadQueue));

            if (!File.Exists(dicomFilePath))
            {
                _logger.LogWarning("Cannot queue non-existent file for PACS upload: {Path}", dicomFilePath);
                return false;
            }

            var item = new PacsUploadItem
            {
                DicomFilePath = dicomFilePath,
                CorrelationId = Guid.NewGuid()
            };

            try
            {
                await _queue.Writer.WriteAsync(item, cancellationToken);
                Interlocked.Increment(ref _totalQueued);

                _logger.LogInformation("[{CorrelationId}] DICOM queued for PACS upload: {FileName} → {Host}:{Port}",
                    item.CorrelationId.ToString().Substring(0, 8),
                    Path.GetFileName(dicomFilePath),
                    _pipelineConfig.PacsConfiguration!.Host,
                    _pipelineConfig.PacsConfiguration.Port);

                return true;
            }
            catch (OperationCanceledException)
            {
                _logger.LogDebug("Queue operation cancelled");
                return false;
            }
        }

        /// <summary>
        /// Process the upload queue
        /// </summary>
        public async Task ProcessQueueAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PACS upload queue started for pipeline {Pipeline}", _pipelineConfig.Name);

            var pacsConfig = _pipelineConfig.PacsConfiguration!;
            var semaphore = new SemaphoreSlim(pacsConfig.MaxConcurrentUploads, pacsConfig.MaxConcurrentUploads);

            try
            {
                await foreach (var item in _queue.Reader.ReadAllAsync(cancellationToken))
                {
                    await semaphore.WaitAsync(cancellationToken);

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await ProcessUploadAsync(item, cancellationToken);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("PACS upload queue cancelled for pipeline {Pipeline}", _pipelineConfig.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error in PACS upload queue for pipeline {Pipeline}", _pipelineConfig.Name);
                throw;
            }
            finally
            {
                semaphore?.Dispose();
            }
        }

        private async Task ProcessUploadAsync(PacsUploadItem item, CancellationToken cancellationToken)
        {
            var fileName = Path.GetFileName(item.DicomFilePath);
            var correlationId = item.CorrelationId.ToString().Substring(0, 8);

            item.AttemptCount++;
            item.LastAttemptAt = DateTime.UtcNow;

            _logger.LogInformation("[{CorrelationId}] Processing PACS upload for {FileName} (attempt {Attempt})",
                correlationId,
                fileName,
                item.AttemptCount);

            try
            {
                var result = await _storeService.StoreFileWithRetryAsync(
                    item.DicomFilePath,
                    _pipelineConfig.PacsConfiguration!,
                    cancellationToken);

                if (result.Success)
                {
                    Interlocked.Increment(ref _totalSuccessful);
                    _lastSuccessfulUpload = DateTime.UtcNow;

                    var uploadTime = DateTime.UtcNow - item.EnqueuedAt;
                    _logger.LogInformation("[{CorrelationId}] Successfully uploaded {FileName} to PACS. " +
                        "Transaction: {TransactionId}, Upload time: {Time:F1}s",
                        correlationId,
                        fileName,
                        result.TransactionUid,
                        uploadTime.TotalSeconds);
                }
                else
                {
                    HandleUploadFailure(item, result, correlationId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{CorrelationId}] Unexpected error uploading {FileName} to PACS",
                    correlationId, fileName);

                var error = new PacsUploadError
                {
                    FilePath = item.DicomFilePath,
                    Error = ex.Message,
                    UserFriendlyError = "Unerwarteter Fehler beim Upload. Details siehe Log.",
                    ErrorType = DicomErrorType.Unknown,
                    AttemptNumber = item.AttemptCount,
                    WillRetry = false
                };

                RecordError(error);
                Interlocked.Increment(ref _totalFailed);
                _lastFailedUpload = DateTime.UtcNow;
            }
        }

        private void HandleUploadFailure(PacsUploadItem item, StoreResult result, string correlationId)
        {
            var fileName = Path.GetFileName(item.DicomFilePath);
            item.LastError = result.ErrorMessage;

            var willRetry = !IsNonRetryableError(result.ErrorType) &&
                           item.AttemptCount < _pipelineConfig.PacsConfiguration!.MaxRetryAttempts;

            var error = new PacsUploadError
            {
                FilePath = item.DicomFilePath,
                Error = result.ErrorMessage ?? "Unknown error",
                UserFriendlyError = result.UserFriendlyMessage ?? result.ErrorMessage ?? "Unknown error",
                ErrorType = result.ErrorType,
                AttemptNumber = item.AttemptCount,
                WillRetry = willRetry
            };

            RecordError(error);

            if (willRetry)
            {
                _logger.LogWarning("[{CorrelationId}] PACS upload failed for {FileName} (attempt {Attempt}/{Max}). " +
                    "Error: {Error}. Will retry...",
                    correlationId,
                    fileName,
                    item.AttemptCount,
                    _pipelineConfig.PacsConfiguration.MaxRetryAttempts,
                    result.UserFriendlyMessage);

                // Re-queue for retry
                _ = Task.Run(async () =>
                {
                    var delay = TimeSpan.FromSeconds(
                        _pipelineConfig.PacsConfiguration.RetryDelaySeconds * item.AttemptCount);

                    await Task.Delay(delay);
                    await _queue.Writer.WriteAsync(item);
                });
            }
            else
            {
                Interlocked.Increment(ref _totalFailed);
                _lastFailedUpload = DateTime.UtcNow;

                _logger.LogError("[{CorrelationId}] PACS upload permanently failed for {FileName} after {Attempts} attempts. " +
                    "Final error: {Error}\n" +
                    "Handlungsempfehlung:\n{UserMessage}",
                    correlationId,
                    fileName,
                    item.AttemptCount,
                    result.ErrorMessage,
                    GetActionRecommendation(result.ErrorType, result.UserFriendlyMessage));
            }
        }

        private bool IsNonRetryableError(DicomErrorType errorType)
        {
            return errorType switch
            {
                DicomErrorType.FileNotFound => true,
                DicomErrorType.InvalidConfiguration => true,
                DicomErrorType.Authentication => true,
                _ => false
            };
        }

        private string GetActionRecommendation(DicomErrorType errorType, string? userMessage)
        {
            var recommendation = errorType switch
            {
                DicomErrorType.NetworkConnection =>
                    "1. PACS-Server Status prüfen (läuft Orthanc?)\n" +
                    "2. Netzwerkverbindung testen\n" +
                    "3. Firewall-Einstellungen prüfen",

                DicomErrorType.Authentication =>
                    "1. AE Titles in Config prüfen\n" +
                    "2. PACS-Server Konfiguration checken\n" +
                    "3. Bei Orthanc: DICOM_CHECK_CALLED_AE_TITLE=false setzen",

                DicomErrorType.Timeout =>
                    "1. PACS-Server Performance prüfen\n" +
                    "2. Netzwerk-Latenz testen\n" +
                    "3. Timeout-Wert erhöhen",

                DicomErrorType.PacsRejection =>
                    "1. DICOM-Datei mit dcmdump validieren\n" +
                    "2. PACS-Server Logs prüfen\n" +
                    "3. Transfer Syntax Kompatibilität checken",

                _ => "1. Log-Dateien prüfen\n" +
                     "2. IT-Support kontaktieren\n" +
                     "3. Datei manuell hochladen"
            };

            if (!string.IsNullOrEmpty(userMessage))
            {
                return $"{userMessage}\n\nEmpfohlene Maßnahmen:\n{recommendation}";
            }

            return recommendation;
        }

        private void RecordError(PacsUploadError error)
        {
            lock (_errorLock)
            {
                _recentErrors.Add(error);

                // Keep only last 100 errors
                while (_recentErrors.Count > 100)
                {
                    _recentErrors.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Get queue statistics
        /// </summary>
        public (int queued, int successful, int failed) GetStatistics()
        {
            return (_totalQueued, _totalSuccessful, _totalFailed);
        }

        /// <summary>
        /// Get recent errors
        /// </summary>
        public List<PacsUploadError> GetRecentErrors()
        {
            lock (_errorLock)
            {
                return new List<PacsUploadError>(_recentErrors);
            }
        }

        /// <summary>
        /// Get current queue length
        /// </summary>
        public int QueueLength => _queue.Reader.Count;

        public void Dispose()
        {
            if (_disposed) return;

            _queue.Writer.TryComplete();
            _disposed = true;
        }
    }
}
