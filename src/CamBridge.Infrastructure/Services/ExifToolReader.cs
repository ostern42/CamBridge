using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Entities;
using CamBridge.Core.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Reads EXIF metadata from images using ExifTool
    /// </summary>
    public class ExifToolReader
    {
        private readonly ILogger<ExifToolReader> _logger;
        private readonly string _exifToolPath;

        public ExifToolReader(ILogger<ExifToolReader> logger, IOptions<ExifToolSettings> settings)
        {
            _logger = logger;
            _exifToolPath = settings.Value.ExifToolPath ?? @"Tools\exiftool.exe";

            if (!File.Exists(_exifToolPath))
            {
                _logger.LogWarning($"ExifTool not found at configured path: {_exifToolPath}");

                // Try alternative paths
                var alternativePaths = new[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools", "exiftool.exe"),
                    Path.Combine(Directory.GetCurrentDirectory(), "Tools", "exiftool.exe"),
                    @"C:\Tools\exiftool.exe"
                };

                foreach (var path in alternativePaths)
                {
                    if (File.Exists(path))
                    {
                        _exifToolPath = path;
                        _logger.LogInformation($"Found ExifTool at: {path}");
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Reads metadata from an image file using ExifTool
        /// </summary>
        public async Task<ImageMetadata?> ReadMetadataAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                _logger.LogError($"File not found: {filePath}");
                return null;
            }

            if (!File.Exists(_exifToolPath))
            {
                _logger.LogError($"ExifTool not found at: {_exifToolPath}");
                return null;
            }

            try
            {
                var output = await RunExifToolAsync(filePath);
                return await ExtractMetadataAsync(output, filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading metadata from {filePath}");
                return null;
            }
        }

        private async Task<string> RunExifToolAsync(string filePath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = _exifToolPath,
                Arguments = $"-s -a -u \"{filePath}\"", // -s: short output, -a: allow duplicates, -u: extract unknown tags
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
            };

            using var process = new Process { StartInfo = startInfo };

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

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var error = errorBuilder.ToString();
                _logger.LogWarning($"ExifTool returned exit code {process.ExitCode}. Error: {error}");
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
                        result[key] = value;
                    }
                }
            }

            return result;
        }

        private async Task<ImageMetadata?> ExtractMetadataAsync(string output, string filePath)
        {
            var exifData = ParseExifToolOutput(output);

            if (exifData.Count == 0)
            {
                _logger.LogWarning($"No EXIF data found in file: {filePath}");
                return null;
            }

            // Look for QRBridge data in the Barcode field (Ricoh-specific)
            string? barcodeData = null;
            if (exifData.TryGetValue("Barcode", out var barcode))
            {
                barcodeData = barcode;
                _logger.LogInformation($"Found barcode data in Barcode field: '{barcodeData}'");
            }
            else if (exifData.TryGetValue("UserComment", out var userComment) && userComment != "GCM_TAG")
            {
                // Fallback to UserComment if it contains actual data
                barcodeData = userComment;
                _logger.LogInformation($"Found barcode data in UserComment field: '{barcodeData}'");
            }
            else
            {
                _logger.LogWarning("No barcode data found in EXIF");
            }

            // Parse patient and study info from barcode
            PatientInfo? patientInfo = null;
            StudyInfo? studyInfo = null;

            if (!string.IsNullOrEmpty(barcodeData))
            {
                (patientInfo, studyInfo) = ParseBarcodeData(barcodeData);
            }

            // Extract technical data
            var technicalData = new ImageTechnicalData
            {
                Width = GetIntValue(exifData, "ImageWidth", "ExifImageWidth") ?? 0,
                Height = GetIntValue(exifData, "ImageHeight", "ExifImageHeight") ?? 0,
                BitsPerSample = GetIntValue(exifData, "BitsPerSample") ?? 8,
                PhotometricInterpretation = exifData.GetValueOrDefault("ColorSpace", "RGB"),
                Manufacturer = exifData.GetValueOrDefault("Make", "Unknown"),
                Model = exifData.GetValueOrDefault("Model", "Unknown"),
                Software = exifData.GetValueOrDefault("Software", "Unknown"),
                AcquisitionDateTime = GetDateTime(exifData, "DateTimeOriginal", "CreateDate") ?? DateTime.Now
            };

            // Generate instance UID
            var instanceUid = GenerateUid();

            return new ImageMetadata
            {
                PatientInfo = patientInfo,
                StudyInfo = studyInfo,
                TechnicalData = technicalData,
                ExifData = exifData,
                InstanceNumber = 1,
                InstanceUid = instanceUid,
                SourceFile = filePath
            };
        }

        private (PatientInfo?, StudyInfo?) ParseBarcodeData(string barcodeData)
        {
            if (string.IsNullOrWhiteSpace(barcodeData))
            {
                _logger.LogWarning("Barcode data is empty or null");
                return (null, null);
            }

            // Clean the barcode data first
            var cleanedData = CleanBarcodeData(barcodeData);
            _logger.LogDebug($"Parsing cleaned barcode data: '{cleanedData}'");

            var parts = cleanedData.Split('|');

            // QRBridge format validation
            if (parts.Length < 3)
            {
                _logger.LogWarning($"Invalid barcode format. Expected at least 3 fields, got {parts.Length}");
                return (null, null);
            }

            // Log each field for debugging
            for (int i = 0; i < parts.Length; i++)
            {
                _logger.LogDebug($"Barcode field [{i}]: '{parts[i]}'");
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
                        _logger.LogWarning($"Could not parse birth date: '{dateStr}'");
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
                var studyInfo = new StudyInfo
                {
                    StudyId = new StudyId($"STU-{examId}-{DateTime.Now:yyyyMMddHHmmss}"),
                    Description = studyDescription ?? "Clinical Photography",
                    StudyDate = DateTime.Now,
                    ExamId = examId
                };

                _logger.LogInformation($"Successfully parsed barcode: ExamId={examId}, Patient={patientName}, Study={studyDescription}");

                return (patientInfo, studyInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing barcode data");
                return (null, null);
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
            var replacements = new Dictionary<string, string>
            {
                // Common German umlauts
                { "�", "ö" },    // Most common encoding issue
                { "÷", "ö" },    // Alternative encoding
                { "õ", "ä" },
                { "�", "ä" },    // Alternative encoding
                { "³", "ü" },
                { "�", "ü" },    // Alternative encoding
                { "Í", "Ö" },
                { "─", "Ä" },
                { "▄", "Ü" },
                { "á", "ß" },    
                
                // French characters
                { "Ó", "à" },
                { "Þ", "è" },
                { "Ú", "é" },
                { "Û", "ê" },
                { "¶", "ç" },    
                
                // Spanish/Portuguese
                { "±", "ñ" },
                { "ã", "ã" },
                { "Ž", "õ" },    
                
                // Other common replacements
                { "Æ", "°" },    // degree symbol
                { "º", "€" },    // Euro symbol
            };

            var cleaned = barcodeData;
            foreach (var replacement in replacements)
            {
                cleaned = cleaned.Replace(replacement.Key, replacement.Value);
            }

            // Remove any control characters
            cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"[\x00-\x1F\x7F]", "");

            // Trim whitespace
            cleaned = cleaned.Trim();

            if (cleaned != barcodeData)
            {
                _logger.LogDebug($"Cleaned barcode data: '{barcodeData}' -> '{cleaned}'");
            }

            return cleaned;
        }

        private int? GetIntValue(Dictionary<string, string> exifData, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (exifData.TryGetValue(key, out var value))
                {
                    if (int.TryParse(value, out var result))
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

    public class ExifToolSettings
    {
        public string ExifToolPath { get; set; } = @"Tools\exiftool.exe";
    }
}
