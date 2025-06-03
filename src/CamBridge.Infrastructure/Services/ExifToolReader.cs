using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Core.Entities;
using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// THE ONLY EXIF solution for CamBridge. Uses ExifTool to read ALL tags reliably.
    /// No fallbacks, no alternatives - if ExifTool is not available, processing cannot continue.
    /// </summary>
    public class ExifToolReader
    {
        private readonly ILogger<ExifToolReader> _logger;
        private readonly string _exifToolPath;
        private readonly int _timeoutMs;

        // Cache for performance
        private readonly Dictionary<string, CachedExifData> _cache = new();
        private readonly SemaphoreSlim _cacheLock = new(1, 1);

        public ExifToolReader(ILogger<ExifToolReader> logger, int timeoutMs = 5000)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _timeoutMs = timeoutMs;

            // Discovery ExifTool - MANDATORY!
            _exifToolPath = DiscoverExifTool()
                ?? throw new InvalidOperationException(
                    "ExifTool not found! CamBridge requires ExifTool to function. " +
                    "Please install ExifTool in the Tools folder or system PATH.");

            _logger.LogInformation("ExifTool initialized at: {Path}", _exifToolPath);
        }

        /// <summary>
        /// Extracts complete image metadata from JPEG file using ExifTool
        /// </summary>
        public async Task<ImageMetadata> ExtractMetadataAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Image file not found: {filePath}", filePath);
            }

            // Check cache first
            var cached = await GetCachedDataAsync(filePath);
            if (cached != null)
            {
                return cached;
            }

            // Read all EXIF data
            var exifData = await ReadExifDataAsync(filePath);

            // Extract and parse QRBridge data
            var qrBridgeData = ExtractQRBridgeData(exifData);
            var parsedData = qrBridgeData != null
                ? ParseQRBridgeData(qrBridgeData)
                : new Dictionary<string, string>();

            // Build metadata object using existing entities
            var patientInfo = new PatientInfo(
                patientId: parsedData.GetValueOrDefault("examid", "UNKNOWN"),
                name: parsedData.GetValueOrDefault("name", "Unknown Patient"),
                birthDate: ParseBirthDate(parsedData.GetValueOrDefault("birthdate")),
                gender: ParseGender(parsedData.GetValueOrDefault("gender"))
            );

            var captureDateTime = ParseDateTime(exifData.GetValueOrDefault("DateTimeOriginal"))
                ?? DateTime.UtcNow;

            var studyInfo = new StudyInfo(
                studyId: $"STUDY_{patientInfo.PatientId}_{captureDateTime:yyyyMMddHHmmss}",
                studyDate: captureDateTime,
                studyDescription: parsedData.GetValueOrDefault("comment", "CamBridge Capture"),
                modality: "XC" // Secondary Capture
            );

            var technicalData = ImageTechnicalData.FromExifDictionary(exifData);

            var metadata = new ImageMetadata(
                sourceFilePath: filePath,
                captureDateTime: captureDateTime,
                patient: patientInfo,
                study: studyInfo,
                exifData: exifData,
                technicalData: technicalData
            );

            // Cache the result
            await CacheDataAsync(filePath, metadata);

            _logger.LogInformation("Successfully extracted metadata from {File}: {Patient} - {Study}",
                filePath, patientInfo.PatientId, studyInfo.StudyId);

            return metadata;
        }

        /// <summary>
        /// Reads all EXIF data from file using ExifTool
        /// </summary>
        private async Task<Dictionary<string, string>> ReadExifDataAsync(string filePath)
        {
            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _exifToolPath,
                        Arguments = $"-j -a -G -s \"{filePath}\"",  // JSON, all tags, group names, short output
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8
                    }
                };

                var outputBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();
                var outputComplete = new TaskCompletionSource<bool>();
                var errorComplete = new TaskCompletionSource<bool>();

                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                        outputBuilder.AppendLine(e.Data);
                    else
                        outputComplete.SetResult(true);
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                        errorBuilder.AppendLine(e.Data);
                    else
                        errorComplete.SetResult(true);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Wait for process with timeout
                using var cts = new CancellationTokenSource(_timeoutMs);
                try
                {
                    await process.WaitForExitAsync(cts.Token);
                    await Task.WhenAll(outputComplete.Task, errorComplete.Task);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogError("ExifTool timed out after {Timeout}ms for file: {File}", _timeoutMs, filePath);
                    try { process.Kill(); } catch { }
                    throw new TimeoutException($"ExifTool did not complete within {_timeoutMs}ms");
                }

                var output = outputBuilder.ToString();
                var error = errorBuilder.ToString();

                if (!string.IsNullOrWhiteSpace(error))
                {
                    _logger.LogWarning("ExifTool stderr: {Error}", error);
                }

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException($"ExifTool failed with exit code {process.ExitCode}: {error}");
                }

                // Parse JSON output
                return ParseExifToolJson(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read EXIF data from {FilePath}", filePath);
                throw new InvalidOperationException($"Failed to extract EXIF data from {filePath}", ex);
            }
        }

        /// <summary>
        /// Parses ExifTool JSON output
        /// </summary>
        private Dictionary<string, string> ParseExifToolJson(string json)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                {
                    var obj = root[0];

                    foreach (var prop in obj.EnumerateObject())
                    {
                        var key = prop.Name;
                        var value = prop.Value.ToString();

                        // Store both with and without group prefix
                        result[key] = value;

                        // Also store without group prefix (e.g., "MakerNotesPentax:Barcode" -> "Barcode")
                        var colonIndex = key.IndexOf(':');
                        if (colonIndex > 0)
                        {
                            var shortKey = key.Substring(colonIndex + 1);
                            if (!result.ContainsKey(shortKey))
                            {
                                result[shortKey] = value;
                            }
                        }
                    }
                }

                _logger.LogDebug("Parsed {Count} EXIF tags", result.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing ExifTool JSON output");
                throw new InvalidOperationException("Failed to parse ExifTool output", ex);
            }

            return result;
        }

        /// <summary>
        /// Extracts QRBridge data from EXIF tags
        /// </summary>
        private string? ExtractQRBridgeData(Dictionary<string, string> exifData)
        {
            // Priority order for QRBridge data
            var priorityFields = new[]
            {
                "Barcode",                    // Ricoh/Pentax specific
                "MakerNotesPentax:Barcode",  // Full tag path
                "PentaxBarcode",             // Alternative naming
                "UserComment",               // Standard EXIF
                "Comment",                   // Alternative
                "ImageDescription"           // Fallback
            };

            foreach (var field in priorityFields)
            {
                if (exifData.TryGetValue(field, out var value) && !string.IsNullOrWhiteSpace(value))
                {
                    // Clean the value
                    value = CleanBarcodeData(value);

                    // Skip if it's just the marker
                    if (value.Trim() == "GCM_TAG")
                    {
                        _logger.LogDebug("Field {Field} contains only 'GCM_TAG' marker, skipping", field);
                        continue;
                    }

                    // Check if it looks like QRBridge data
                    if (value.Contains('|') ||
                        value.StartsWith("EX", StringComparison.OrdinalIgnoreCase) ||
                        value.StartsWith("v2:", StringComparison.OrdinalIgnoreCase) ||
                        value.Contains("-examid"))
                    {
                        _logger.LogInformation("Found QRBridge data in {Field}: {Value}", field, value);
                        return value;
                    }
                }
            }

            _logger.LogWarning("No QRBridge data found in any EXIF field");
            return null;
        }

        /// <summary>
        /// Cleans barcode data from encoding issues and prefixes
        /// </summary>
        private string CleanBarcodeData(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return data;

            // Remove common prefixes
            var prefixes = new[] { "GCM_TAG ", "GCM_TAG", "GCM " };
            foreach (var prefix in prefixes)
            {
                if (data.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    data = data.Substring(prefix.Length).Trim();
                    _logger.LogDebug("Removed prefix '{Prefix}' from data", prefix);
                    break;
                }
            }

            // Fix common encoding issues (UTF-8/Latin-1 confusion)
            data = data.Replace("÷", "ö")
                      .Replace("ä", "ä")
                      .Replace("ü", "ü")
                      .Replace("Ä", "Ä")
                      .Replace("Ö", "Ö")
                      .Replace("Ü", "Ü")
                      .Replace("ß", "ß");

            // Remove control characters but keep printable content
            var cleaned = new StringBuilder();
            foreach (char c in data)
            {
                if (c >= 32 || c == '\t' || c == '\n' || c == '\r')
                {
                    cleaned.Append(c);
                }
                else if (c == '\0')
                {
                    break; // Stop at null terminator
                }
            }

            return cleaned.ToString().Trim();
        }

        /// <summary>
        /// Parses QRBridge data from any supported format
        /// </summary>
        private Dictionary<string, string> ParseQRBridgeData(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return new Dictionary<string, string>();
            }

            _logger.LogDebug("Parsing QRBridge data: '{Data}'", data);

            // Detect and parse format
            try
            {
                if (data.Contains('|'))
                {
                    return ParsePipeDelimitedFormat(data);
                }
                else if (data.StartsWith("v2:", StringComparison.OrdinalIgnoreCase))
                {
                    return ParseJsonV2Format(data);
                }
                else if (data.Contains("-examid") || data.Contains("-name"))
                {
                    return ParseCommandLineFormat(data);
                }
                else
                {
                    _logger.LogWarning("Unknown QRBridge format: {Data}", data);
                    return new Dictionary<string, string> { ["raw"] = data };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing QRBridge data: {Data}", data);
                return new Dictionary<string, string> { ["raw"] = data };
            }
        }

        /// <summary>
        /// Parses pipe-delimited format: EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax
        /// </summary>
        private Dictionary<string, string> ParsePipeDelimitedFormat(string data)
        {
            var parts = data.Split('|');
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Standard field order
            string[] fieldNames = { "examid", "name", "birthdate", "gender", "comment" };

            for (int i = 0; i < Math.Min(parts.Length, fieldNames.Length); i++)
            {
                var value = parts[i].Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    result[fieldNames[i]] = value;
                }
            }

            _logger.LogInformation("Parsed {Count} fields from pipe-delimited format", result.Count);

            if (parts.Length < 5)
            {
                _logger.LogWarning("Only {Count} of 5 expected fields found (Ricoh G900 II limitation)", parts.Length);
            }

            return result;
        }

        /// <summary>
        /// Parses JSON v2 format: v2:{"e":"EX002","n":"Schmidt, Maria","b":"19850315","g":"F"}
        /// </summary>
        private Dictionary<string, string> ParseJsonV2Format(string data)
        {
            var jsonPart = data.Substring(3); // Remove "v2:" prefix
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                using var doc = JsonDocument.Parse(jsonPart);
                var root = doc.RootElement;

                // Map short keys to full names
                var keyMap = new Dictionary<string, string>
                {
                    ["e"] = "examid",
                    ["n"] = "name",
                    ["b"] = "birthdate",
                    ["g"] = "gender",
                    ["c"] = "comment"
                };

                foreach (var prop in root.EnumerateObject())
                {
                    var key = keyMap.TryGetValue(prop.Name, out var fullKey) ? fullKey : prop.Name;
                    result[key] = prop.Value.GetString() ?? string.Empty;
                }

                _logger.LogInformation("Parsed {Count} fields from JSON v2 format", result.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing JSON v2 format: {Data}", jsonPart);
            }

            return result;
        }

        /// <summary>
        /// Parses command-line format: -examid "EX002" -name "Schmidt, Maria"
        /// </summary>
        private Dictionary<string, string> ParseCommandLineFormat(string data)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Pattern: -key "value" or -key value
            var pattern = @"-(\w+)\s+(?:""([^""]+)""|(\S+))";
            var matches = System.Text.RegularExpressions.Regex.Matches(data, pattern);

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (match.Success && match.Groups.Count > 1)
                {
                    var key = match.Groups[1].Value.ToLower();
                    var value = match.Groups[2].Success ? match.Groups[2].Value : match.Groups[3].Value;
                    result[key] = value;
                }
            }

            _logger.LogInformation("Parsed {Count} fields from command-line format", result.Count);
            return result;
        }

        /// <summary>
        /// Discovers ExifTool executable
        /// </summary>
        private string? DiscoverExifTool()
        {
            var searchPaths = new List<string>
            {
                // 1. Tools folder in solution root
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Tools", "exiftool.exe"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools", "exiftool.exe"),
                
                // 2. Same directory as executable
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "exiftool.exe"),
                
                // 3. Common installation paths
                @"C:\Tools\exiftool.exe",
                @"C:\Program Files\ExifTool\exiftool.exe",
                @"C:\Program Files (x86)\ExifTool\exiftool.exe"
            };

            foreach (var path in searchPaths)
            {
                try
                {
                    var fullPath = Path.GetFullPath(path);
                    if (File.Exists(fullPath))
                    {
                        _logger.LogDebug("Found ExifTool at: {Path}", fullPath);
                        return fullPath;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Error checking path: {Path}", path);
                }
            }

            // Try to find in PATH
            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "where",
                        Arguments = "exiftool.exe",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(output))
                {
                    var path = output.Split('\n')[0].Trim();
                    if (File.Exists(path))
                    {
                        _logger.LogDebug("Found ExifTool in PATH: {Path}", path);
                        return path;
                    }
                }
            }
            catch
            {
                // where command might not exist
            }

            _logger.LogError("ExifTool not found in any location!");
            return null;
        }

        // Helper methods
        private DateTime? ParseBirthDate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            // Try YYYY-MM-DD format
            if (DateTime.TryParse(value, out var date))
                return date;

            // Try YYYYMMDD format
            if (value.Length == 8 && DateTime.TryParseExact(value, "yyyyMMdd", null,
                System.Globalization.DateTimeStyles.None, out date))
                return date;

            return null;
        }

        private string ParseGender(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "O";

            return value.ToUpper() switch
            {
                "M" or "MALE" => "M",
                "F" or "FEMALE" => "F",
                "O" or "OTHER" => "O",
                _ => "O"
            };
        }

        private DateTime? ParseDateTime(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            // ExifTool format: "2024:01:15 14:30:45"
            var formats = new[] {
                "yyyy:MM:dd HH:mm:ss",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy/MM/dd HH:mm:ss"
            };

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(value, format, null,
                    System.Globalization.DateTimeStyles.None, out var result))
                {
                    return result;
                }
            }

            // Fallback to standard parsing
            if (DateTime.TryParse(value, out var fallback))
            {
                return fallback;
            }

            return null;
        }

        // Cache management
        private async Task<ImageMetadata?> GetCachedDataAsync(string filePath)
        {
            await _cacheLock.WaitAsync();
            try
            {
                if (_cache.TryGetValue(filePath, out var cached))
                {
                    var fileInfo = new FileInfo(filePath);
                    if (cached.LastModified == fileInfo.LastWriteTimeUtc)
                    {
                        _logger.LogDebug("Using cached metadata for: {FilePath}", filePath);
                        return cached.Metadata;
                    }
                    else
                    {
                        _cache.Remove(filePath);
                    }
                }
            }
            finally
            {
                _cacheLock.Release();
            }

            return null;
        }

        private async Task CacheDataAsync(string filePath, ImageMetadata metadata)
        {
            await _cacheLock.WaitAsync();
            try
            {
                var fileInfo = new FileInfo(filePath);
                _cache[filePath] = new CachedExifData
                {
                    Metadata = metadata,
                    LastModified = fileInfo.LastWriteTimeUtc
                };

                // Limit cache size
                if (_cache.Count > 100)
                {
                    var oldest = _cache.OrderBy(x => x.Value.LastModified).First();
                    _cache.Remove(oldest.Key);
                }
            }
            finally
            {
                _cacheLock.Release();
            }
        }

        private class CachedExifData
        {
            public ImageMetadata Metadata { get; set; } = null!;
            public DateTime LastModified { get; set; }
        }
    }

        /// <summary>
        /// Helper method to wait for process exit with cancellation
        /// </summary>
        private static async Task WaitForProcessExitAsync(Process process, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();

            process.EnableRaisingEvents = true;
            process.Exited += (sender, e) => tcs.TrySetResult(true);

            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await tcs.Task.ConfigureAwait(false);
            }
        }
    }
}
