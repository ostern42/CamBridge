using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// ExifTool-based EXIF reader that provides access to ALL EXIF tags including proprietary ones.
    /// This is our primary reader for Ricoh G900 II cameras as it can read the Barcode tag reliably.
    /// </summary>
    public class ExifToolReader : IExifReader
    {
        private readonly ILogger<ExifToolReader> _logger;
        private readonly string? _exifToolPath;
        private readonly int _timeoutMs;

        // Cache for parsed data to avoid re-parsing
        private readonly Dictionary<string, ExifToolData> _cache = new();
        private readonly SemaphoreSlim _cacheLock = new(1, 1);

        public ExifToolReader(ILogger<ExifToolReader> logger, string? exifToolPath = null, int timeoutMs = 5000)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _timeoutMs = timeoutMs;

            // Discovery order for ExifTool
            _exifToolPath = exifToolPath ?? DiscoverExifTool();

            if (string.IsNullOrEmpty(_exifToolPath))
            {
                _logger.LogWarning("ExifTool not found. Reader will not be available.");
            }
            else
            {
                _logger.LogInformation("ExifTool found at: {Path}", _exifToolPath);
            }
        }

        /// <summary>
        /// Discovers ExifTool in common locations
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
                
                // 3. System PATH
                "exiftool.exe",
                
                // 4. Common installation paths
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
                // where command might not exist or fail
            }

            _logger.LogWarning("ExifTool not found in any common location");
            return null;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, string>> ReadExifDataAsync(string filePath)
        {
            if (!IsAvailable())
            {
                _logger.LogWarning("ExifTool not available, returning empty dictionary");
                return new Dictionary<string, string>();
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Image file not found: {filePath}", filePath);
            }

            // Check cache first
            await _cacheLock.WaitAsync();
            try
            {
                if (_cache.TryGetValue(filePath, out var cached))
                {
                    var fileInfo = new FileInfo(filePath);
                    if (cached.LastModified == fileInfo.LastWriteTimeUtc)
                    {
                        _logger.LogDebug("Using cached EXIF data for: {FilePath}", filePath);
                        return cached.Tags;
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

            // Read fresh data
            var exifData = await ReadExifToolDataAsync(filePath);

            // Cache the result
            await _cacheLock.WaitAsync();
            try
            {
                var fileInfo = new FileInfo(filePath);
                _cache[filePath] = new ExifToolData
                {
                    Tags = exifData,
                    LastModified = fileInfo.LastWriteTimeUtc
                };
            }
            finally
            {
                _cacheLock.Release();
            }

            return exifData;
        }

        /// <inheritdoc />
        public async Task<string?> GetUserCommentAsync(string filePath)
        {
            if (!IsAvailable())
            {
                _logger.LogWarning("ExifTool not available");
                return null;
            }

            var exifData = await ReadExifDataAsync(filePath);

            // Priority order for QRBridge data
            var priorityFields = new[]
            {
                "Barcode",              // Ricoh/Pentax specific
                "PentaxBarcode",        // Alternative naming
                "MakerNotesPentax:Barcode",  // Full tag path
                "UserComment",          // Standard EXIF
                "Comment",              // Alternative
                "ImageDescription"      // Fallback
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
                    if (value.Contains('|') || value.StartsWith("EX", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation("Found QRBridge data in {Field}: {Value}", field, value);
                        return value;
                    }
                }
            }

            _logger.LogWarning("No QRBridge data found in any field for: {FilePath}", filePath);
            return null;
        }

        /// <inheritdoc />
        public Dictionary<string, string> ParseQRBridgeData(string userComment)
        {
            if (string.IsNullOrWhiteSpace(userComment))
            {
                return new Dictionary<string, string>();
            }

            // Clean the data first
            userComment = CleanBarcodeData(userComment);

            _logger.LogDebug("Parsing QRBridge data: '{Data}'", userComment);

            // Detect format
            if (userComment.Contains('|'))
            {
                return ParsePipeDelimitedFormat(userComment);
            }
            else if (userComment.StartsWith("v2:", StringComparison.OrdinalIgnoreCase))
            {
                return ParseJsonFormat(userComment);
            }
            else if (userComment.Contains("-examid") || userComment.Contains("-name"))
            {
                return ParseCommandLineFormat(userComment);
            }

            // Unknown format, try to be smart
            _logger.LogWarning("Unknown QRBridge format: {Data}", userComment);
            return new Dictionary<string, string> { ["raw"] = userComment };
        }

        /// <inheritdoc />
        public async Task<bool> HasExifDataAsync(string filePath)
        {
            if (!IsAvailable()) return false;

            try
            {
                var data = await ReadExifDataAsync(filePath);
                return data.Count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking EXIF data for: {FilePath}", filePath);
                return false;
            }
        }

        /// <summary>
        /// Checks if ExifTool is available
        /// </summary>
        public bool IsAvailable() => !string.IsNullOrEmpty(_exifToolPath) && File.Exists(_exifToolPath);

        /// <summary>
        /// Reads EXIF data using ExifTool process
        /// </summary>
        private async Task<Dictionary<string, string>> ReadExifToolDataAsync(string filePath)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _exifToolPath!,
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

                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null) outputBuilder.AppendLine(e.Data);
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null) errorBuilder.AppendLine(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                var completed = await process.WaitForExitAsync(_timeoutMs);

                if (!completed)
                {
                    _logger.LogWarning("ExifTool process timed out after {Timeout}ms", _timeoutMs);
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
                    throw new InvalidOperationException($"ExifTool exited with code {process.ExitCode}: {error}");
                }

                // Parse JSON output
                result = ParseExifToolJson(output);

                _logger.LogDebug("ExifTool extracted {Count} tags from {FilePath}", result.Count, filePath);

                // Log interesting tags for debugging
                foreach (var key in new[] { "Barcode", "UserComment", "PentaxBarcode" })
                {
                    if (result.TryGetValue(key, out var value))
                    {
                        _logger.LogDebug("  {Key}: {Value}", key, value);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running ExifTool on {FilePath}", filePath);
                throw;
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
                // ExifTool returns an array with one object
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

                        // Also store without group prefix (e.g., "MakerNotes:Barcode" -> "Barcode")
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing ExifTool JSON output");
            }

            return result;
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
                    data = data.Substring(prefix.Length);
                    _logger.LogDebug("Removed prefix '{Prefix}' from data", prefix);
                    break;
                }
            }

            // Fix common encoding issues
            data = data.Replace("÷", "ö")    // UTF-8/Latin-1 confusion
                      .Replace("ä", "ä")      // Common for umlauts
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
        /// Parses pipe-delimited format: EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax
        /// </summary>
        private Dictionary<string, string> ParsePipeDelimitedFormat(string data)
        {
            var parts = data.Split('|');
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Map to expected field names
            string[] fieldNames = { "examid", "name", "birthdate", "gender", "comment" };

            for (int i = 0; i < Math.Min(parts.Length, fieldNames.Length); i++)
            {
                var value = parts[i].Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    result[fieldNames[i]] = value;
                }
            }

            _logger.LogInformation("Parsed {Count} fields from pipe-delimited data", result.Count);

            if (parts.Length < 5)
            {
                _logger.LogWarning("Only {Count} of 5 expected fields found (Ricoh G900 II limitation)", parts.Length);
            }

            return result;
        }

        /// <summary>
        /// Parses JSON format: v2:{"e":"EX002","n":"Schmidt, Maria","b":"19850315","g":"F"}
        /// </summary>
        private Dictionary<string, string> ParseJsonFormat(string data)
        {
            if (!data.StartsWith("v2:", StringComparison.OrdinalIgnoreCase))
                return new Dictionary<string, string>();

            var jsonPart = data.Substring(3);
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
                _logger.LogError(ex, "Error parsing JSON format: {Data}", jsonPart);
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
        /// Helper class for caching
        /// </summary>
        private class ExifToolData
        {
            public Dictionary<string, string> Tags { get; set; } = new();
            public DateTime LastModified { get; set; }
        }
    }

    /// <summary>
    /// Extension methods for Process
    /// </summary>
    public static class ProcessExtensions
    {
        public static async Task<bool> WaitForExitAsync(this Process process, int timeoutMs)
        {
            using var cts = new CancellationTokenSource(timeoutMs);
            try
            {
                await process.WaitForExitAsync(cts.Token);
                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }
    }
}
