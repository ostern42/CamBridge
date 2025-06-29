// src\CamBridge.Infrastructure\Services\DicomStoreService.cs
// Version: 0.8.3
// Description: DICOM C-STORE service with enhanced error handling and user-friendly messages
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Core;
using Microsoft.Extensions.Logging;
using FellowOakDicom;
using FellowOakDicom.Network;
using FellowOakDicom.Network.Client;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Result of DICOM C-STORE operation
    /// </summary>
    public class StoreResult
    {
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }
        public string? UserFriendlyMessage { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? TransactionUid { get; init; }
        public DicomErrorType ErrorType { get; init; } = DicomErrorType.None;

        public static StoreResult CreateSuccess(string transactionUid = "")
            => new() { Success = true, TransactionUid = transactionUid };

        public static StoreResult CreateFailure(string errorMessage, string? userFriendlyMessage = null, DicomErrorType errorType = DicomErrorType.Unknown)
            => new()
            {
                Success = false,
                ErrorMessage = errorMessage,
                UserFriendlyMessage = userFriendlyMessage ?? errorMessage,
                ErrorType = errorType
            };
    }

    /// <summary>
    /// Types of DICOM errors for better handling
    /// </summary>
    public enum DicomErrorType
    {
        None,
        FileNotFound,
        NetworkConnection,
        Authentication,
        Timeout,
        InvalidConfiguration,
        PacsRejection,
        Unknown
    }

    /// <summary>
    /// Service for DICOM C-STORE operations to PACS with enhanced error handling
    /// </summary>
    public class DicomStoreService
    {
        private readonly ILogger<DicomStoreService> _logger;

        public DicomStoreService(ILogger<DicomStoreService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Store DICOM file to PACS with enhanced error messages
        /// </summary>
        public async Task<StoreResult> StoreFileAsync(string dicomPath, PacsConfiguration config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (string.IsNullOrEmpty(dicomPath)) throw new ArgumentException("Path required", nameof(dicomPath));

            if (!File.Exists(dicomPath))
            {
                _logger.LogError("DICOM file not found: {Path}", dicomPath);
                return StoreResult.CreateFailure(
                    $"File not found: {dicomPath}",
                    $"DICOM-Datei nicht gefunden: {Path.GetFileName(dicomPath)}",
                    DicomErrorType.FileNotFound);
            }

            _logger.LogInformation("Starting C-STORE to {Host}:{Port} for {File}",
                config.Host, config.Port, Path.GetFileName(dicomPath));

            try
            {
                // Load DICOM file
                var dicomFile = await DicomFile.OpenAsync(dicomPath);
                var sopInstanceUid = dicomFile.Dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID);
                var patientName = dicomFile.Dataset.GetSingleValueOrDefault<string>(DicomTag.PatientName, "Unknown");

                _logger.LogDebug("Loaded DICOM: SOP Instance UID={Uid}, Patient={Patient}",
                    sopInstanceUid, patientName);

                // Create client
                var client = DicomClientFactory.Create(
                    config.Host,
                    config.Port,
                    false,  // No TLS for now
                    config.CallingAeTitle,
                    config.CalledAeTitle);

                client.NegotiateAsyncOps();

                // Setup response handling with TaskCompletionSource
                DicomCStoreResponse? response = null;
                var responseReceived = new TaskCompletionSource<bool>();

                var request = new DicomCStoreRequest(dicomFile)
                {
                    OnResponseReceived = (req, res) =>
                    {
                        response = res;
                        _logger.LogDebug("C-STORE Response: Status={Status} ({Code:X4})",
                            res.Status.State, res.Status.Code);
                        responseReceived.TrySetResult(true);
                    }
                };

                await client.AddRequestAsync(request);

                // Send with timeout
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(config.TimeoutSeconds));

                try
                {
                    await Task.WhenAll(
                        client.SendAsync(cts.Token),
                        responseReceived.Task);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogError("C-STORE timeout after {Timeout}s", config.TimeoutSeconds);
                    return StoreResult.CreateFailure(
                        $"Timeout after {config.TimeoutSeconds} seconds",
                        $"PACS antwortet nicht nach {config.TimeoutSeconds} Sekunden. Ist der PACS-Server erreichbar?",
                        DicomErrorType.Timeout);
                }

                // Check response
                if (response?.Status == DicomStatus.Success)
                {
                    _logger.LogInformation("C-STORE successful for {File}, SOP Instance UID: {Uid}",
                        Path.GetFileName(dicomPath), sopInstanceUid);
                    return StoreResult.CreateSuccess(sopInstanceUid);
                }
                else
                {
                    var errorMsg = $"C-STORE failed with status: {response?.Status?.Description ?? "Unknown"}";
                    var userMsg = TranslateDicomStatus(response?.Status);
                    _logger.LogWarning(errorMsg);
                    return StoreResult.CreateFailure(errorMsg, userMsg, DicomErrorType.PacsRejection);
                }
            }
            catch (DicomAssociationRejectedException ex)
            {
                _logger.LogError(ex, "DICOM association rejected");
                var userMsg = TranslateAssociationRejection(ex);
                return StoreResult.CreateFailure(
                    $"Association rejected: {ex.Message}",
                    userMsg,
                    DicomErrorType.Authentication);
            }
            catch (DicomNetworkException ex) when (ex.InnerException is SocketException socketEx)
            {
                _logger.LogError(ex, "Network error during C-STORE");
                var userMsg = TranslateSocketError(socketEx, config);
                return StoreResult.CreateFailure(
                    $"Network error: {ex.Message}",
                    userMsg,
                    DicomErrorType.NetworkConnection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "C-STORE failed for {File}", Path.GetFileName(dicomPath));
                var userMsg = $"Unerwarteter Fehler beim PACS-Upload: {ex.GetType().Name}. Details siehe Log.";
                return StoreResult.CreateFailure($"Store failed: {ex.Message}", userMsg);
            }
        }

        /// <summary>
        /// Test PACS connection with C-ECHO and enhanced feedback
        /// </summary>
        public async Task<StoreResult> TestConnectionAsync(PacsConfiguration config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (!config.IsValid())
            {
                var issues = new System.Text.StringBuilder();
                if (string.IsNullOrWhiteSpace(config.Host))
                    issues.AppendLine("- Host/IP-Adresse fehlt");
                if (config.Port <= 0 || config.Port > 65535)
                    issues.AppendLine($"- Ungültiger Port: {config.Port}");
                if (string.IsNullOrWhiteSpace(config.CallingAeTitle))
                    issues.AppendLine("- Calling AE Title fehlt");
                if (string.IsNullOrWhiteSpace(config.CalledAeTitle))
                    issues.AppendLine("- Called AE Title fehlt");

                return StoreResult.CreateFailure(
                    "Invalid PACS configuration",
                    $"PACS-Konfiguration ungültig:\n{issues}",
                    DicomErrorType.InvalidConfiguration);
            }

            _logger.LogInformation("Testing connection to {Host}:{Port} as {CallingAe} → {CalledAe}",
                config.Host, config.Port, config.CallingAeTitle, config.CalledAeTitle);

            try
            {
                // Create client
                var client = DicomClientFactory.Create(
                    config.Host,
                    config.Port,
                    false,  // No TLS
                    config.CallingAeTitle,
                    config.CalledAeTitle);

                client.NegotiateAsyncOps();

                // Setup response handling
                DicomCEchoResponse? response = null;
                var responseReceived = new TaskCompletionSource<bool>();

                var request = new DicomCEchoRequest
                {
                    OnResponseReceived = (req, res) =>
                    {
                        response = res;
                        _logger.LogDebug("C-ECHO Response: {Status}", res.Status);
                        responseReceived.TrySetResult(true);
                    }
                };

                await client.AddRequestAsync(request);

                // Send with timeout
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(config.TimeoutSeconds));

                try
                {
                    await Task.WhenAll(
                        client.SendAsync(cts.Token),
                        responseReceived.Task);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogError("C-ECHO timeout after {Timeout}s", config.TimeoutSeconds);
                    return StoreResult.CreateFailure(
                        $"Connection timeout after {config.TimeoutSeconds} seconds",
                        $"Verbindungstimeout nach {config.TimeoutSeconds} Sekunden.\n" +
                        $"Mögliche Ursachen:\n" +
                        $"- PACS-Server läuft nicht (z.B. Orthanc Docker Container gestoppt)\n" +
                        $"- Falsche IP/Port ({config.Host}:{config.Port})\n" +
                        $"- Firewall blockiert Verbindung",
                        DicomErrorType.Timeout);
                }

                // Check response
                if (response?.Status == DicomStatus.Success)
                {
                    _logger.LogInformation("C-ECHO successful - PACS connection verified");
                    return StoreResult.CreateSuccess("ECHO-OK");
                }
                else
                {
                    var errorMsg = $"C-ECHO failed: {response?.Status?.Description ?? "No response"}";
                    var userMsg = TranslateDicomStatus(response?.Status);
                    _logger.LogWarning(errorMsg);
                    return StoreResult.CreateFailure(errorMsg, userMsg, DicomErrorType.PacsRejection);
                }
            }
            catch (DicomAssociationRejectedException ex)
            {
                _logger.LogError(ex, "C-ECHO association rejected");
                var userMsg = TranslateAssociationRejection(ex);
                return StoreResult.CreateFailure(
                    $"Association rejected: {ex.Message}",
                    userMsg,
                    DicomErrorType.Authentication);
            }
            catch (DicomNetworkException ex) when (ex.InnerException is SocketException socketEx)
            {
                _logger.LogError(ex, "Network error during C-ECHO");
                var userMsg = TranslateSocketError(socketEx, config);
                return StoreResult.CreateFailure(
                    $"Network error: {ex.Message}",
                    userMsg,
                    DicomErrorType.NetworkConnection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "C-ECHO failed");
                return StoreResult.CreateFailure(
                    $"Connection test failed: {ex.Message}",
                    $"Verbindungstest fehlgeschlagen: {ex.GetType().Name}\nDetails siehe Log.",
                    DicomErrorType.Unknown);
            }
        }

        /// <summary>
        /// Store DICOM file with retry logic and improved logging
        /// </summary>
        public async Task<StoreResult> StoreFileWithRetryAsync(
            string dicomPath,
            PacsConfiguration config,
            CancellationToken cancellationToken = default)
        {
            if (!config.RetryOnFailure)
            {
                return await StoreFileAsync(dicomPath, config);
            }

            var attempts = 0;
            StoreResult? lastResult = null;
            var fileName = Path.GetFileName(dicomPath);

            while (attempts < config.MaxRetryAttempts && !cancellationToken.IsCancellationRequested)
            {
                attempts++;
                _logger.LogInformation("C-STORE attempt {Attempt}/{Max} for {File}",
                    attempts, config.MaxRetryAttempts, fileName);

                lastResult = await StoreFileAsync(dicomPath, config);

                if (lastResult.Success)
                {
                    if (attempts > 1)
                    {
                        _logger.LogInformation("C-STORE succeeded after {Attempts} attempts for {File}",
                            attempts, fileName);
                    }
                    return lastResult;
                }

                // Check if error is retryable
                if (IsNonRetryableError(lastResult.ErrorType))
                {
                    _logger.LogWarning("Non-retryable error detected for {File}: {ErrorType}",
                        fileName, lastResult.ErrorType);
                    break;
                }

                if (attempts < config.MaxRetryAttempts)
                {
                    var delay = TimeSpan.FromSeconds(config.RetryDelaySeconds * attempts);
                    _logger.LogWarning("C-STORE failed for {File}, retrying in {Delay}s. Error: {Error}",
                        fileName, delay.TotalSeconds, lastResult.UserFriendlyMessage);

                    await Task.Delay(delay, cancellationToken);
                }
            }

            if (attempts >= config.MaxRetryAttempts)
            {
                _logger.LogError("C-STORE failed after {Attempts} attempts for {File}. Final error: {Error}",
                    attempts, fileName, lastResult?.UserFriendlyMessage);
            }

            return lastResult ?? StoreResult.CreateFailure("No attempts made");
        }

        /// <summary>
        /// Determine if an error should not be retried
        /// </summary>
        private bool IsNonRetryableError(DicomErrorType errorType)
        {
            return errorType switch
            {
                DicomErrorType.FileNotFound => true,
                DicomErrorType.InvalidConfiguration => true,
                DicomErrorType.Authentication => true,  // Wrong AE Title won't fix itself
                _ => false  // Network errors, timeouts, etc. are retryable
            };
        }

        /// <summary>
        /// Translate DICOM status codes to user-friendly messages
        /// </summary>
        private string TranslateDicomStatus(DicomStatus? status)
        {
            if (status == null)
                return "Keine Antwort vom PACS-Server erhalten.";

            // Check common status codes
            if (status == DicomStatus.QueryRetrieveOutOfResources)
                return "PACS-Server hat nicht genügend Ressourcen. Speicherplatz voll?";

            if (status == DicomStatus.StorageStorageOutOfResources)
                return "PACS-Speicher voll. Administrator kontaktieren.";

            if (status == DicomStatus.ProcessingFailure)
                return "PACS konnte die Datei nicht verarbeiten. DICOM-Format prüfen.";

            if (status == DicomStatus.NoSuchObjectInstance)
                return "PACS erkennt das Bildformat nicht. Transfer Syntax prüfen.";

            // Generic message with status code
            return $"PACS-Server meldet: {status.Description} (Code: {status.Code:X4})";
        }

        /// <summary>
        /// Translate association rejection reasons
        /// </summary>
        private string TranslateAssociationRejection(DicomAssociationRejectedException ex)
        {
            var msg = new System.Text.StringBuilder();
            msg.AppendLine("PACS-Verbindung abgelehnt!");

            // Check common rejection reasons in the message
            if (ex.Message.Contains("Called AE Title Not Recognized", StringComparison.OrdinalIgnoreCase))
            {
                msg.AppendLine($"Der Called AE Title wird vom PACS nicht erkannt.");
                msg.AppendLine($"Tipp: Bei Orthanc muss 'DICOM_CHECK_CALLED_AE_TITLE=false' gesetzt sein.");
            }
            else if (ex.Message.Contains("Calling AE Title Not Recognized", StringComparison.OrdinalIgnoreCase))
            {
                msg.AppendLine($"Der Calling AE Title wird vom PACS nicht akzeptiert.");
                msg.AppendLine($"Tipp: AE Title im PACS als bekannter Client registrieren.");
            }
            else
            {
                msg.AppendLine($"Grund: {ex.RejectReason}");
                msg.AppendLine($"Source: {ex.RejectSource}");
                msg.AppendLine($"Result: {ex.RejectResult}");
            }

            return msg.ToString();
        }

        /// <summary>
        /// Translate socket errors to user-friendly messages
        /// </summary>
        private string TranslateSocketError(SocketException socketEx, PacsConfiguration config)
        {
            var msg = new System.Text.StringBuilder();
            msg.AppendLine($"Netzwerkfehler beim Verbinden zu {config.Host}:{config.Port}");

            switch (socketEx.SocketErrorCode)
            {
                case SocketError.ConnectionRefused:
                    msg.AppendLine("Verbindung verweigert!");
                    msg.AppendLine("Mögliche Ursachen:");
                    msg.AppendLine("- PACS-Server läuft nicht (Orthanc Docker Container prüfen)");
                    msg.AppendLine($"- Falscher Port (aktuell: {config.Port}, Orthanc Standard: 4242)");
                    msg.AppendLine("- Firewall blockiert Verbindung");
                    break;

                case SocketError.HostNotFound:
                case SocketError.HostUnreachable:
                    msg.AppendLine("Host nicht erreichbar!");
                    msg.AppendLine($"- Hostname/IP prüfen: {config.Host}");
                    msg.AppendLine("- Netzwerkverbindung prüfen");
                    msg.AppendLine("- DNS-Auflösung prüfen");
                    break;

                case SocketError.TimedOut:
                    msg.AppendLine("Verbindung Timeout!");
                    msg.AppendLine("- PACS-Server überlastet?");
                    msg.AppendLine("- Netzwerk zu langsam?");
                    msg.AppendLine($"- Timeout erhöhen (aktuell: {config.TimeoutSeconds}s)");
                    break;

                default:
                    msg.AppendLine($"Socket-Fehler: {socketEx.SocketErrorCode}");
                    msg.AppendLine($"Details: {socketEx.Message}");
                    break;
            }

            return msg.ToString();
        }
    }
}
