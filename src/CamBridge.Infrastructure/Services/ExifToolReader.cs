using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Core.Entities;
using CamBridge.Core.ValueObjects;
using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Reads EXIF metadata from JPEG files using ExifTool
    /// Specifically handles Ricoh G900SE II barcode field
    /// </summary>
    public class ExifToolReader
    {
        private readonly ILogger<ExifToolReader> _logger;
        private readonly string _exifToolPath;
        private readonly int _timeoutMs;

        /// <summary>
        /// Creates a new ExifToolReader instance
        /// This constructor matches what ServiceCollectionExtensions expects
        /// </summary>
        /// <param name="logger">Logger for diagnostics</param>
        /// <param name="timeoutMs">Timeout for ExifTool execution in milliseconds</param>
        public ExifToolReader(ILogger<ExifToolReader> logger, int timeoutMs = 5000)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _timeoutMs = timeoutMs;

            // Try to find ExifTool in various locations
            var possiblePaths = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools", "exiftool.exe"),
                Path.Combine(Directory.GetCurrentDirectory(), "Tools", "exiftool.exe"),
                @"Tools\exiftool.exe",
                @"C:\Tools\exiftool.exe"
            };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    _exifToolPath = path;
                    _logger.LogInformation("Found ExifTool at: {Path}", path);
                    break;
                }
            }

            if (string.IsNullOrEmpty(_exifToolPath))
            {
                throw new FileNotFoundException("ExifTool not found in any expected location");
            }
        }

        /// <summary>
        /// Extracts complete metadata from a JPEG file
        /// This method name matches what FileProcessor expects
        /// </summary>
        public async Task<ImageMetadata> ExtractMetadataAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            _logger.LogDebug("Extracting metadata from: {FilePath}", filePath);

            try
            {
                // Run ExifTool and get all EXIF data
                var output = await RunExifToolAsync(filePath);
                var exifData = ParseExifToolOutput(output);

                // Log what we found
                if (exifData.TryGetValue("Barcode", out var barcode))
                {
                    _logger.LogInformation("Found Ricoh barcode data: '{Barcode}'", barcode);
                }
                if (exifData.TryGetValue("UserComment", out var userComment))
                {
                    _logger.LogDebug("UserComment field: '{UserComment}'", userComment);
                }

                // Look for QRBridge data in the Barcode field (Ricoh-specific)
                string? barcodeData = null;
                if (exifData.TryGetValue("Barcode", out var barcodeValue))
                {
                    barcodeData = barcodeValue;
                    _logger.LogInformation("Found barcode data in Barcode field: '{BarcodeData}'", barcodeData);
                }
                else if (exifData.TryGetValue("UserComment", out var userCommentValue) &&
                         userCommentValue != "GCM_TAG" &&
                         userCommentValue.Contains("|"))
                {
                    // Fallback to UserComment if it contains pipe-delimited data
                    barcodeData = userCommentValue;
                    _logger.LogInformation("Found barcode data in UserComment field: '{BarcodeData}'", barcodeData);
                }
                else
                {
                    _logger.LogWarning("No barcode data found in EXIF");
                }

                // Parse patient and study info from barcode
                PatientInfo patientInfo;
                StudyInfo studyInfo;

                if (!string.IsNullOrEmpty(barcodeData))
                {
                    (patientInfo, studyInfo) = ParseBarcodeData(barcodeData);
                }
                else
                {
                    (patientInfo, studyInfo) = CreateDefaultPatientAndStudy();
                }

                // Extract technical data
                var technicalData = new ImageTechnicalData
                {
                    ImageWidth = GetIntValue(exifData, "ImageWidth", "ExifImageWidth"),
                    ImageHeight = GetIntValue(exifData, "ImageHeight", "ExifImageHeight"),
                    BitsPerSample = GetIntValue(exifData, "BitsPerSample") ?? 8,
                    ColorSpace = exifData.GetValueOrDefault("ColorSpace", "RGB"),
                    Manufacturer = exifData.GetValueOrDefault("Make", "Unknown"),
                    Model = exifData.GetValueOrDefault("Model", "Unknown"),
                    Software = exifData.GetValueOrDefault("Software", "Unknown"),
                    Compression = exifData.GetValueOrDefault("Compression"),
                    Orientation = GetIntValue(exifData, "Orientation")
                };

                // Get capture date/time
                var captureDateTime = GetDateTime(exifData, "DateTimeOriginal", "CreateDate", "DateTime") ?? DateTime.Now;

                // Generate instance UID
                var instanceUid = GenerateUid();

                // Create and return metadata using the proper constructor
                return new ImageMetadata(
                    sourceFilePath: filePath,
                    captureDateTime: captureDateTime,
                    patient: patientInfo,
                    study: studyInfo,
                    technicalData: technicalData,
                    userComment: exifData.GetValueOrDefault("UserComment"),
                    barcodeData: barcodeData,
                    instanceNumber: 1,
                    instanceUid: instanceUid,
                    exifData: new Dictionary<string, string>(exifData) // Create a copy to ensure no issues
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to extract metadata from {FilePath}", filePath);
                throw new InvalidOperationException($"Failed to extract metadata from {filePath}: {ex.Message}", ex);
            }
        }

        private async Task<string> RunExifToolAsync(string filePath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = _exifToolPath,
                Arguments = $"-s -a -u -charset exiftool=UTF8 -charset filename=UTF8 \"{filePath}\"", // Force UTF-8
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
            };

            using var process = new Process { StartInfo = startInfo };
            using var cts = new CancellationTokenSource(_timeoutMs);

            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                    outputBuilder.AppendLine(e.Data);
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                    errorBuilder.AppendLine(e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            try
            {
                await process.WaitForExitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                process.Kill();
                throw new TimeoutException($"ExifTool timed out after {_timeoutMs}ms");
            }

            if (process.ExitCode != 0)
            {
                var error = errorBuilder.ToString();
                _logger.LogWarning("ExifTool returned exit code {ExitCode}. Error: {Error}",
                    process.ExitCode, error);
            }

            return outputBuilder.ToString();
        }

        private Dictionary<string, string> ParseExifToolOutput(string output)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(output))
                return result;

            var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var colonIndex = line.IndexOf(':');
                if (colonIndex > 0 && colonIndex < line.Length - 1)
                {
                    var key = line.Substring(0, colonIndex).Trim();
                    var value = line.Substring(colonIndex + 1).Trim();

                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    {
                        // Skip keys that are just encoding artifacts
                        if (key.Length == 1 && (key[0] == '�' || key[0] < 32 || key[0] > 126))
                        {
                            _logger.LogDebug("Skipping invalid key: {Key} (char code: {Code})", key, (int)key[0]);
                            continue;
                        }

                        // Handle duplicate keys by making them unique
                        var finalKey = key;
                        var counter = 1;
                        while (result.ContainsKey(finalKey))
                        {
                            finalKey = $"{key}_{counter}";
                            counter++;
                            _logger.LogDebug("Duplicate key found: {Key}, renamed to: {FinalKey}", key, finalKey);
                        }

                        result[finalKey] = value;
                    }
                }
            }

            return result;
        }

        private (PatientInfo, StudyInfo) ParseBarcodeData(string barcodeData)
        {
            if (string.IsNullOrWhiteSpace(barcodeData))
            {
                _logger.LogWarning("Barcode data is empty or null");
                return CreateDefaultPatientAndStudy();
            }

            // Clean the barcode data first
            var cleanedData = CleanBarcodeData(barcodeData);
            _logger.LogDebug("Parsing cleaned barcode data: '{CleanedData}'", cleanedData);

            var parts = cleanedData.Split('|');

            // QRBridge format validation
            if (parts.Length < 3)
            {
                _logger.LogWarning("Invalid barcode format. Expected at least 3 fields, got {Count}", parts.Length);
                return CreateDefaultPatientAndStudy();
            }

            // Log each field for debugging
            for (int i = 0; i < parts.Length; i++)
            {
                _logger.LogDebug("Barcode field [{Index}]: '{Value}'", i, parts[i]);
            }

            try
            {
                // Parse patient info
                var examId = parts[0].Trim();
                var patientName = parts[1].Trim();

                // Parse birth date - handle various formats
                DateTime? birthDate = null;
                if (parts.Length > 2 && !string.IsNullOrWhiteSpace(parts[2]))
                {
                    var dateStr = parts[2].Trim();
                    if (DateTime.TryParseExact(dateStr, "yyyy-MM-dd", null, DateTimeStyles.None, out var date1))
                        birthDate = date1;
                    else if (DateTime.TryParseExact(dateStr, "dd.MM.yyyy", null, DateTimeStyles.None, out var date2))
                        birthDate = date2;
                    else if (DateTime.TryParse(dateStr, out var date3))
                        birthDate = date3;
                    else
                        _logger.LogWarning("Could not parse birth date: '{DateStr}'", dateStr);
                }

                // Parse gender
                Gender gender = Gender.Other;
                if (parts.Length > 3 && !string.IsNullOrWhiteSpace(parts[3]))
                {
                    var genderStr = parts[3].Trim().ToUpperInvariant();
                    gender = genderStr switch
                    {
                        "M" => Gender.Male,
                        "F" => Gender.Female,
                        "W" => Gender.Female, // German: Weiblich
                        _ => Gender.Other
                    };
                }

                // Parse study description
                string? studyDescription = null;
                if (parts.Length > 4 && !string.IsNullOrWhiteSpace(parts[4]))
                {
                    studyDescription = parts[4].Trim();
                }

                // Create patient info
                var patientInfo = new PatientInfo(
                    id: new PatientId(examId),
                    name: patientName,
                    birthDate: birthDate,
                    gender: gender
                );

                // Create study info
                var studyInfo = new StudyInfo(
                    studyId: new StudyId($"S{examId.Substring(0, Math.Min(examId.Length, 14))}"), // Max 16 chars
                    examId: examId,
                    description: studyDescription ?? "Clinical Photography",
                    modality: "VL", // Visible Light photography
                    studyDate: DateTime.Now
                );

                _logger.LogInformation("Successfully parsed barcode: ExamId={ExamId}, Patient={PatientName}, Study={StudyDescription}",
                    examId, patientName, studyDescription);

                return (patientInfo, studyInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing barcode data");
                return CreateDefaultPatientAndStudy();
            }
        }

        /// <summary>
        /// Cleans barcode data by fixing common encoding issues from Ricoh cameras
        /// </summary>
        private string CleanBarcodeData(string barcodeData)
        {
            if (string.IsNullOrEmpty(barcodeData))
                return barcodeData;

            // Ricoh G900 II specific character replacements
            // The camera seems to use Windows-1252 encoding
            var cleaned = barcodeData;

            // Apply replacements one by one to avoid dictionary key conflicts
            // Common German umlauts
            cleaned = cleaned.Replace("�", "ö");    // Most common encoding issue
            cleaned = cleaned.Replace("÷", "ö");    // Alternative encoding
            cleaned = cleaned.Replace("õ", "ä");
            cleaned = cleaned.Replace("³", "ü");
            cleaned = cleaned.Replace("Í", "Ö");
            cleaned = cleaned.Replace("─", "Ä");
            cleaned = cleaned.Replace("▄", "Ü");
            cleaned = cleaned.Replace("á", "ß");

            // French characters
            cleaned = cleaned.Replace("Ó", "à");
            cleaned = cleaned.Replace("Þ", "è");
            cleaned = cleaned.Replace("Ú", "é");
            cleaned = cleaned.Replace("Û", "ê");
            cleaned = cleaned.Replace("¶", "ç");

            // Spanish/Portuguese
            cleaned = cleaned.Replace("±", "ñ");
            cleaned = cleaned.Replace("ã", "ã");
            cleaned = cleaned.Replace("Ž", "õ");

            // Other common replacements
            cleaned = cleaned.Replace("Æ", "°");    // degree symbol
            cleaned = cleaned.Replace("º", "€");    // Euro symbol

            // Remove any control characters
            cleaned = Regex.Replace(cleaned, @"[\x00-\x1F\x7F]", "");

            // Trim whitespace
            cleaned = cleaned.Trim();

            if (cleaned != barcodeData)
            {
                _logger.LogDebug("Cleaned barcode data: '{Original}' -> '{Cleaned}'", barcodeData, cleaned);
            }

            return cleaned;
        }

        private (PatientInfo, StudyInfo) CreateDefaultPatientAndStudy()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var shortTimestamp = DateTime.Now.ToString("MMddHHmm"); // Shorter for StudyId

            var patientInfo = new PatientInfo(
                id: new PatientId($"AUTO_{timestamp}"),
                name: "Unknown Patient",
                birthDate: null,
                gender: Gender.Other
            );

            var studyInfo = new StudyInfo(
                studyId: new StudyId($"S{shortTimestamp}"), // Max 16 chars
                examId: $"AUTO_{timestamp}",
                description: "Unidentified Clinical Photography",
                modality: "VL",
                studyDate: DateTime.Now
            );

            _logger.LogWarning("Created default patient/study info with ID: AUTO_{Timestamp}", timestamp);

            return (patientInfo, studyInfo);
        }

        private int? GetIntValue(Dictionary<string, string> exifData, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (exifData.TryGetValue(key, out var value))
                {
                    // Handle values like "8 8 8" for BitsPerSample
                    var firstValue = value.Split(' ')[0];

                    if (int.TryParse(firstValue, out var result))
                        return result;
                }
            }
            return null;
        }

        private DateTime? GetDateTime(Dictionary<string, string> exifData, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (exifData.TryGetValue(key, out var value))
                {
                    // EXIF datetime format: "yyyy:MM:dd HH:mm:ss"
                    if (DateTime.TryParseExact(value, "yyyy:MM:dd HH:mm:ss",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                        return result;

                    // Try standard parse as fallback
                    if (DateTime.TryParse(value, out result))
                        return result;
                }
            }
            return null;
        }

        private string GenerateUid()
        {
            // Simple UID generation - in production use proper DICOM UID generation
            return $"1.2.826.0.1.3680043.8.498.{DateTime.Now.Ticks}";
        }
    }
}
