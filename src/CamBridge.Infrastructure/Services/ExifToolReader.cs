// src/CamBridge.Infrastructure/Services/ExifToolReader.cs
// Version: 0.8.10
// Description: EXIF data extraction service - with correlation ID support
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Entities;
using CamBridge.Core.ValueObjects;
using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Service for reading EXIF data from image files using ExifTool
    /// Clean UTF-8 implementation without workarounds
    /// </summary>
    public class ExifToolReader
    {
        private readonly ILogger<ExifToolReader> _logger;
        private readonly string _exifToolPath;
        private readonly string? _correlationId;

        public ExifToolReader(ILogger<ExifToolReader> logger, string exifToolPath, string? correlationId = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _correlationId = correlationId;

            if (string.IsNullOrWhiteSpace(exifToolPath))
            {
                throw new ArgumentException("ExifTool path cannot be empty", nameof(exifToolPath));
            }

            // Resolve to absolute path if relative
            _exifToolPath = Path.IsPathRooted(exifToolPath)
                ? exifToolPath
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, exifToolPath);

            // Try to find ExifTool in various locations
            if (!File.Exists(_exifToolPath))
            {
                var searchPaths = new[]
                {
                    _exifToolPath,
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools", "exiftool.exe"),
                    Path.Combine(Environment.CurrentDirectory, "Tools", "exiftool.exe"),
                    Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, "Tools", "exiftool.exe")
                };

                _exifToolPath = searchPaths.FirstOrDefault(File.Exists) ?? _exifToolPath;
            }

            if (!File.Exists(_exifToolPath))
            {
                throw new FileNotFoundException($"ExifTool not found at: {_exifToolPath}");
            }

            if (!string.IsNullOrEmpty(_correlationId))
            {
                _logger.LogInformation("[{CorrelationId}] [ExifInit] ExifToolReader initialized with path: {Path}", _correlationId, _exifToolPath);
            }
            else
            {
                _logger.LogInformation("ExifToolReader initialized with path: {Path}", _exifToolPath);
            }
        }

        /// <summary>
        /// Extract metadata from an image file
        /// </summary>
        public async Task<ImageMetadata> ExtractMetadataAsync(string imagePath, string? correlationId = null)
        {
            // FIXED: Use passed correlationId instead of stored _correlationId!
            var logCorrelationId = correlationId ?? _correlationId;

            // CHANGED TO INFORMATION LEVEL
            if (!string.IsNullOrEmpty(logCorrelationId))
            {
                _logger.LogInformation("[{CorrelationId}] [ExifExtraction] Extracting metadata from: {ImagePath}",
                    logCorrelationId, imagePath);
            }
            else
            {
                _logger.LogInformation("Extracting metadata from: {ImagePath}", imagePath);
            }

            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException($"Image file not found: {imagePath}");
            }

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var exifData = await ReadExifDataAsync(imagePath, logCorrelationId);  // Pass correlationId!
                var metadata = ParseExifData(exifData, imagePath, logCorrelationId);  // Pass correlationId!

                stopwatch.Stop();

                // CHANGED TO INFORMATION LEVEL
                if (!string.IsNullOrEmpty(logCorrelationId))
                {
                    _logger.LogInformation("[{CorrelationId}] [ExifExtraction] EXIF extraction completed in {ElapsedMs}ms",
                        logCorrelationId, stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    _logger.LogInformation("EXIF extraction completed in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                }

                return metadata;
            }
            catch (Exception ex)
            {
                // FIXED: Use correlation ID in error log
                if (!string.IsNullOrEmpty(logCorrelationId))
                {
                    _logger.LogError(ex, "[{CorrelationId}] [ExifError] Failed to extract EXIF data from {ImagePath}",
                        logCorrelationId, imagePath);
                }
                else
                {
                    _logger.LogError(ex, "Failed to extract EXIF data from {ImagePath}", imagePath);
                }
                throw new InvalidOperationException($"Failed to extract EXIF data: {ex.Message}", ex);
            }
        }

        private async Task<Dictionary<string, string>> ReadExifDataAsync(string imagePath, string? correlationId = null)
        {
            var arguments = $"-j -a -G1 -s \"{imagePath}\"";

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _exifToolPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                }
            };

            // Keep as DEBUG - less important
            if (!string.IsNullOrEmpty(correlationId))
            {
                _logger.LogDebug("[{CorrelationId}] [ExifExtraction] Executing: {FileName} {Arguments}",
                    correlationId, _exifToolPath, arguments);
            }
            else
            {
                _logger.LogDebug("Executing: {FileName} {Arguments}", _exifToolPath, arguments);
            }

            process.Start();

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await Task.WhenAll(outputTask, errorTask);
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var error = await errorTask;
                throw new InvalidOperationException($"ExifTool returned exit code {process.ExitCode}: {error}");
            }

            var output = await outputTask;
            return ParseExifToolOutput(output);
        }

        private Dictionary<string, string> ParseExifToolOutput(string output)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                using var doc = JsonDocument.Parse(output);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                {
                    var firstElement = root[0];
                    var duplicateKeys = new HashSet<string>();

                    foreach (var property in firstElement.EnumerateObject())
                    {
                        var key = property.Name;
                        // Handle different JSON value types
                        var value = property.Value.ValueKind switch
                        {
                            JsonValueKind.String => property.Value.GetString() ?? string.Empty,
                            JsonValueKind.Number => property.Value.GetRawText(),
                            JsonValueKind.True => "true",
                            JsonValueKind.False => "false",
                            JsonValueKind.Null => string.Empty,
                            _ => property.Value.GetRawText()
                        };

                        // Handle duplicate keys
                        if (result.ContainsKey(key))
                        {
                            duplicateKeys.Add(key);
                            var newKey = $"{key}_{duplicateKeys.Count(k => k == key)}";

                            // Keep as DEBUG - less important
                            if (!string.IsNullOrEmpty(_correlationId))
                            {
                                _logger.LogDebug("[{CorrelationId}] [ExifExtraction] Duplicate key found: {Key}, renamed to: {NewKey}",
                                    _correlationId, key, newKey);
                            }
                            else
                            {
                                _logger.LogDebug("Duplicate key found: {Key}, renamed to: {NewKey}", key, newKey);
                            }
                            result[newKey] = value;
                        }
                        else
                        {
                            result[key] = value;
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                // FIXED: Use correlation ID in error log
                if (!string.IsNullOrEmpty(_correlationId))
                {
                    _logger.LogError(ex, "[{CorrelationId}] [ExifError] Failed to parse ExifTool JSON output", _correlationId);
                }
                else
                {
                    _logger.LogError(ex, "Failed to parse ExifTool JSON output");
                }
                throw new InvalidOperationException("Failed to parse ExifTool output", ex);
            }

            return result;
        }

        private ImageMetadata ParseExifData(Dictionary<string, string> exifData, string imagePath, string? correlationId = null)
        {
            // Log barcode data if present - ALREADY INFORMATION LEVEL
            if ((exifData.TryGetValue("RMETA:Barcode", out var barcodeData) ||
                 exifData.TryGetValue("Barcode", out barcodeData)) &&
                !string.IsNullOrEmpty(barcodeData))
            {
                // ALREADY LogInformation - good!
                if (!string.IsNullOrEmpty(correlationId))
                {
                    _logger.LogInformation("[{CorrelationId}] [ExifExtraction] Found Ricoh barcode data: '{BarcodeData}'",
                        correlationId, barcodeData);
                }
                else
                {
                    _logger.LogInformation("Found Ricoh barcode data: '{BarcodeData}'", barcodeData);
                }
            }

            // Check for UserComment (alternative location for camera data)
            if (exifData.TryGetValue("ExifIFD:UserComment", out var userComment) ||
                exifData.TryGetValue("UserComment", out userComment))
            {
                // Keep as DEBUG - less important
                if (!string.IsNullOrEmpty(correlationId))
                {
                    _logger.LogDebug("[{CorrelationId}] [ExifExtraction] UserComment field: '{UserComment}'",
                        correlationId, userComment);
                }
                else
                {
                    _logger.LogDebug("UserComment field: '{UserComment}'", userComment);
                }
            }

            // Extract patient and study info from barcode data
            var (patientInfo, studyInfo) = ParsePatientAndStudyInfo(exifData, correlationId); // Pass correlationId!

            // Extract technical data
            var technicalData = new ImageTechnicalData
            {
                ImageWidth = this.GetIntValue(exifData, "File:ImageWidth", "ExifIFD:ExifImageWidth", "ImageWidth", "ExifImageWidth") ?? 0,
                ImageHeight = this.GetIntValue(exifData, "File:ImageHeight", "ExifIFD:ExifImageHeight", "ImageHeight", "ExifImageHeight") ?? 0,
                BitsPerSample = this.GetIntValue(exifData, "File:BitsPerSample", "BitsPerSample") ?? 8,
                Manufacturer = exifData.GetValueOrDefault("IFD0:Make") ?? exifData.GetValueOrDefault("Make") ?? "Unknown",
                Model = exifData.GetValueOrDefault("IFD0:Model") ?? exifData.GetValueOrDefault("Model") ?? "Unknown",
                Software = exifData.GetValueOrDefault("IFD0:Software") ?? exifData.GetValueOrDefault("Software"),
                ColorSpace = exifData.GetValueOrDefault("ExifIFD:ColorSpace") ?? exifData.GetValueOrDefault("ColorSpace"),
                Compression = exifData.GetValueOrDefault("File:Compression") ?? exifData.GetValueOrDefault("Compression"),
                Orientation = this.GetIntValue(exifData, "IFD0:Orientation", "Orientation")
            };

            // Get capture date
            var captureDateTime = ParseDateTime(
                exifData.GetValueOrDefault("ExifIFD:DateTimeOriginal") ??
                exifData.GetValueOrDefault("DateTimeOriginal") ??
                exifData.GetValueOrDefault("ExifIFD:CreateDate") ??
                exifData.GetValueOrDefault("CreateDate") ??
                exifData.GetValueOrDefault("IFD0:ModifyDate") ??
                exifData.GetValueOrDefault("ModifyDate")) ?? DateTime.Now;

            // Create metadata using the actual constructor
            return new ImageMetadata(
                sourceFilePath: imagePath,
                captureDateTime: captureDateTime,
                patient: patientInfo,
                study: studyInfo,
                technicalData: technicalData,
                userComment: userComment,
                barcodeData: barcodeData,
                instanceNumber: 1,
                instanceUid: null, // Will be auto-generated
                exifData: exifData
            );
        }

        private (PatientInfo, StudyInfo) ParsePatientAndStudyInfo(Dictionary<string, string> exifData, string? correlationId = null)
        {
            // Check for barcode data first
            if ((exifData.TryGetValue("RMETA:Barcode", out var barcodeData) ||
                 exifData.TryGetValue("Barcode", out barcodeData)) &&
                !string.IsNullOrEmpty(barcodeData))
            {
                // FIXED: Use correlationId parameter instead of _correlationId
                if (!string.IsNullOrEmpty(correlationId))
                {
                    _logger.LogDebug("[{CorrelationId}] [ExifExtraction] Found barcode data in Barcode field: '{BarcodeData}'",
                        correlationId, barcodeData);
                }
                else
                {
                    _logger.LogDebug("Found barcode data in Barcode field: '{BarcodeData}'", barcodeData);
                }
                return ParseBarcodeData(barcodeData, correlationId); // Pass correlationId!
            }

            // Check UserComment as fallback
            if (exifData.TryGetValue("UserComment", out var userComment) &&
                !string.IsNullOrEmpty(userComment) &&
                userComment.Contains("|"))
            {
                // FIXED: Use correlationId parameter instead of _correlationId
                if (!string.IsNullOrEmpty(correlationId))
                {
                    _logger.LogDebug("[{CorrelationId}] [ExifExtraction] Found barcode data in UserComment field: '{UserComment}'",
                        correlationId, userComment);
                }
                else
                {
                    _logger.LogDebug("Found barcode data in UserComment field: '{UserComment}'", userComment);
                }
                return ParseBarcodeData(userComment, correlationId); // Pass correlationId!
            }

            // No barcode data found
            return CreateDefaultPatientAndStudy(correlationId); // Pass correlationId!
        }

        private (PatientInfo, StudyInfo) ParseBarcodeData(string barcodeData, string? correlationId = null)
        {
            try
            {
                // FIXED: Use correlationId parameter instead of _correlationId
                if (!string.IsNullOrEmpty(correlationId))
                {
                    _logger.LogDebug("[{CorrelationId}] [ExifExtraction] Parsing barcode data: '{BarcodeData}'",
                        correlationId, barcodeData);
                }
                else
                {
                    _logger.LogDebug("Parsing barcode data: '{BarcodeData}'", barcodeData);
                }

                // Expected format: "ExamId|PatientName|BirthDate|Gender|StudyDescription"
                var parts = barcodeData.Split('|');

                for (int i = 0; i < parts.Length; i++)
                {
                    // FIXED: Use correlationId parameter instead of _correlationId
                    if (!string.IsNullOrEmpty(correlationId))
                    {
                        _logger.LogDebug("[{CorrelationId}] [ExifExtraction] Barcode field [{Index}]: '{Value}'",
                            correlationId, i, parts[i]);
                    }
                    else
                    {
                        _logger.LogDebug("Barcode field [{Index}]: '{Value}'", i, parts[i]);
                    }
                }

                if (parts.Length >= 4)
                {
                    // Parse patient info
                    var examId = parts[0]?.Trim() ?? string.Empty;
                    var patientName = parts[1]?.Trim() ?? "Unknown";
                    var birthDateStr = parts[2]?.Trim();
                    var genderStr = parts[3]?.Trim();
                    var studyDescription = parts.Length > 4 ? parts[4]?.Trim() : null;

                    // Parse birth date
                    DateTime? birthDate = null;
                    if (!string.IsNullOrEmpty(birthDateStr))
                    {
                        if (DateTime.TryParse(birthDateStr, out var parsed))
                        {
                            birthDate = parsed;
                        }
                    }

                    // Parse gender
                    var gender = ParseGender(genderStr);

                    // Create patient info
                    var patientInfo = new PatientInfo(
                        id: new PatientId(examId),
                        name: patientName,
                        birthDate: birthDate,
                        gender: gender
                    );

                    // Create study info
                    var studyIdValue = examId.Length > 14 ? examId.Substring(0, 14) : examId;
                    var studyInfo = new StudyInfo(
                        studyId: new StudyId($"S{studyIdValue}"), // Max 16 chars total
                        examId: examId,
                        description: studyDescription ?? "Clinical Photography",
                        modality: "VL", // Visible Light photography
                        studyDate: DateTime.Now
                    );

                    // CHANGED TO INFORMATION LEVEL - Important info
                    if (!string.IsNullOrEmpty(correlationId))
                    {
                        _logger.LogInformation("[{CorrelationId}] [ExifExtraction] Successfully parsed barcode: ExamId={ExamId}, Patient={PatientName}, Study={StudyDescription}",
                            correlationId, examId, patientName, studyDescription);
                    }
                    else
                    {
                        _logger.LogInformation("Successfully parsed barcode: ExamId={ExamId}, Patient={PatientName}, Study={StudyDescription}",
                            examId, patientName, studyDescription);
                    }

                    return (patientInfo, studyInfo);
                }
            }
            catch (Exception ex)
            {
                // FIXED: Use correlationId parameter instead of _correlationId
                if (!string.IsNullOrEmpty(correlationId))
                {
                    _logger.LogError(ex, "[{CorrelationId}] [ExifError] Error parsing barcode data", correlationId);
                }
                else
                {
                    _logger.LogError(ex, "Error parsing barcode data");
                }
            }

            return CreateDefaultPatientAndStudy(correlationId); // Pass correlationId!
        }

        private Gender ParseGender(string? genderStr)
        {
            if (string.IsNullOrWhiteSpace(genderStr))
                return Gender.Other;

            return genderStr.ToUpperInvariant() switch
            {
                "M" or "MALE" => Gender.Male,
                "F" or "FEMALE" => Gender.Female,
                "O" or "OTHER" => Gender.Other,
                _ => Gender.Other
            };
        }

        private (PatientInfo, StudyInfo) CreateDefaultPatientAndStudy(string? correlationId = null)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var shortTimestamp = DateTime.Now.ToString("MMddHHmm");

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

            // Keep as WARNING - important to know
            if (!string.IsNullOrEmpty(correlationId))
            {
                _logger.LogWarning("[{CorrelationId}] [ExifExtraction] Created default patient/study info with ID: AUTO_{Timestamp}",
                    correlationId, timestamp);
            }
            else
            {
                _logger.LogWarning("Created default patient/study info with ID: AUTO_{Timestamp}", timestamp);
            }

            return (patientInfo, studyInfo);
        }

        private DateTime? ParseDateTime(string? dateTimeStr)
        {
            if (string.IsNullOrWhiteSpace(dateTimeStr))
                return null;

            // ExifTool format: "2023:12:25 14:30:45"
            if (DateTime.TryParseExact(dateTimeStr, "yyyy:MM:dd HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var result))
            {
                return result;
            }

            // Fallback to standard parsing
            if (DateTime.TryParse(dateTimeStr, out result))
            {
                return result;
            }

            return null;
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
    }
}
