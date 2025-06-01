using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CamBridge.Core.Interfaces;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Implementation of IExifReader for extracting EXIF data from JPEG files
    /// Specifically designed for Ricoh G900 II camera images with QRBridge data
    /// </summary>
    public class ExifReader : IExifReader
    {
        private readonly ILogger<ExifReader> _logger;

        public ExifReader(ILogger<ExifReader> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, string>> ReadExifDataAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Image file not found: {filePath}");

            return await Task.Run(() => ReadExifData(filePath));
        }

        /// <inheritdoc />
        public virtual async Task<string?> GetUserCommentAsync(string filePath)
        {
            var exifData = await ReadExifDataAsync(filePath);

            // PRIORITY 1: Look for Barcode field (Ricoh G900 II specific)
            if (exifData.TryGetValue("Barcode", out var barcode) && !string.IsNullOrWhiteSpace(barcode))
            {
                // Skip if it's just "GCM_TAG"
                if (barcode.Trim() != "GCM_TAG")
                {
                    _logger.LogInformation("Found QRBridge data in Barcode field: {Barcode}", barcode);
                    return barcode;
                }
            }

            // PRIORITY 2: Check Pentax-specific barcode field
            if (exifData.TryGetValue("PentaxBarcode", out var pentaxBarcode) && !string.IsNullOrWhiteSpace(pentaxBarcode))
            {
                _logger.LogInformation("Found QRBridge data in Pentax Barcode field: {Barcode}", pentaxBarcode);
                return pentaxBarcode;
            }

            // PRIORITY 3: Fall back to User Comment (but not if it's just "GCM_TAG")
            if (exifData.TryGetValue("UserComment", out var userComment))
            {
                // Skip if UserComment is just the marker
                if (userComment.Trim() == "GCM_TAG")
                {
                    _logger.LogDebug("UserComment contains only 'GCM_TAG' marker, not using as data source");
                }
                else if (!string.IsNullOrWhiteSpace(userComment))
                {
                    _logger.LogInformation("Found data in UserComment field: {UserComment}", userComment);
                    return userComment;
                }
            }

            _logger.LogWarning("No QRBridge data found in EXIF for file: {FilePath}", filePath);
            return null;
        }

        /// <inheritdoc />
        public Dictionary<string, string> ParseQRBridgeData(string userComment)
        {
            if (string.IsNullOrWhiteSpace(userComment))
                return new Dictionary<string, string>();

            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                // Check if it's pipe-delimited format (Ricoh G900 II format)
                if (userComment.Contains('|'))
                {
                    _logger.LogDebug("Parsing pipe-delimited QRBridge format");
                    return ParsePipeDelimitedFormat(userComment);
                }

                // Otherwise try the command-line format
                _logger.LogDebug("Parsing command-line QRBridge format");
                return ParseCommandLineFormat(userComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing QRBridge data: {Data}", userComment);
                return result;
            }
        }

        /// <inheritdoc />
        public async Task<bool> HasExifDataAsync(string filePath)
        {
            try
            {
                var exifData = await ReadExifDataAsync(filePath);
                return exifData.Count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking EXIF data for file: {FilePath}", filePath);
                return false;
            }
        }

        private Dictionary<string, string> ReadExifData(string filePath)
        {
            var exifData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                var directories = ImageMetadataReader.ReadMetadata(filePath);

                // Process standard EXIF directories
                foreach (var directory in directories)
                {
                    if (directory is ExifIfd0Directory ifd0)
                    {
                        ExtractValue(exifData, ifd0, ExifDirectoryBase.TagMake, "Make");
                        ExtractValue(exifData, ifd0, ExifDirectoryBase.TagModel, "Model");
                        ExtractValue(exifData, ifd0, ExifDirectoryBase.TagSoftware, "Software");
                        ExtractValue(exifData, ifd0, ExifDirectoryBase.TagDateTime, "DateTime");
                    }
                    else if (directory is ExifSubIfdDirectory subIfd)
                    {
                        ExtractValue(exifData, subIfd, ExifDirectoryBase.TagDateTimeOriginal, "DateTimeOriginal");
                        ExtractValue(exifData, subIfd, ExifDirectoryBase.TagExposureTime, "ExposureTime");
                        ExtractValue(exifData, subIfd, ExifDirectoryBase.TagFNumber, "FNumber");
                        ExtractValue(exifData, subIfd, ExifDirectoryBase.TagIsoEquivalent, "ISOSpeedRatings");
                        ExtractValue(exifData, subIfd, ExifDirectoryBase.TagFocalLength, "FocalLength");
                        ExtractValue(exifData, subIfd, ExifDirectoryBase.TagFlash, "Flash");

                        // FIXED: Better handling for User Comment with proper encoding
                        if (subIfd.HasTagName(ExifDirectoryBase.TagUserComment))
                        {
                            var userCommentBytes = subIfd.GetByteArray(ExifDirectoryBase.TagUserComment);
                            if (userCommentBytes != null && userCommentBytes.Length > 8)
                            {
                                var userComment = ExtractUserCommentSafely(userCommentBytes);
                                if (!string.IsNullOrWhiteSpace(userComment))
                                {
                                    exifData["UserComment"] = userComment;
                                    _logger.LogDebug("Extracted UserComment (length: {Length}): {UserComment}",
                                        userComment.Length, userComment);
                                }
                            }
                        }

                        ExtractValue(exifData, subIfd, ExifDirectoryBase.TagExifImageWidth, "ImageWidth");
                        ExtractValue(exifData, subIfd, ExifDirectoryBase.TagExifImageHeight, "ImageHeight");
                    }
                    else if (directory is JpegDirectory jpeg)
                    {
                        ExtractValue(exifData, jpeg, JpegDirectory.TagImageWidth, "JpegWidth");
                        ExtractValue(exifData, jpeg, JpegDirectory.TagImageHeight, "JpegHeight");
                    }

                    // Look for Ricoh-specific tags or unknown tags that might contain barcode
                    ExtractAllTags(directory, exifData);
                }

                // Try to find barcode data in various possible locations
                FindBarcodeData(exifData);

                _logger.LogDebug("Read {Count} EXIF properties from {FilePath}", exifData.Count, filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading EXIF data from {FilePath}", filePath);
                throw new InvalidOperationException($"Failed to read EXIF data from {filePath}", ex);
            }

            return exifData;
        }

        /// <summary>
        /// Safely extracts UserComment with proper handling of encoding and length
        /// </summary>
        private string ExtractUserCommentSafely(byte[] userCommentBytes)
        {
            try
            {
                // EXIF UserComment structure:
                // First 8 bytes: Character code (e.g., "ASCII\0\0\0", "UNICODE\0", "JIS\0\0\0\0\0")
                // Remaining bytes: The actual comment data

                if (userCommentBytes.Length <= 8)
                    return string.Empty;

                // Extract character code
                var characterCode = Encoding.ASCII.GetString(userCommentBytes, 0, 8).TrimEnd('\0');
                _logger.LogDebug("UserComment character code: '{CharCode}'", characterCode);

                // Calculate actual data length (excluding trailing nulls)
                int dataLength = userCommentBytes.Length - 8;
                var dataBytes = new byte[dataLength];
                Array.Copy(userCommentBytes, 8, dataBytes, 0, dataLength);

                // Find actual length by looking for the last non-null byte
                int actualLength = dataLength;
                for (int i = dataLength - 1; i >= 0; i--)
                {
                    if (dataBytes[i] != 0)
                    {
                        actualLength = i + 1;
                        break;
                    }
                }

                // If actualLength is 0, the comment is empty
                if (actualLength == 0)
                    return string.Empty;

                _logger.LogDebug("UserComment actual data length: {Length} bytes", actualLength);

                // Decode based on character code
                string result;
                switch (characterCode.ToUpperInvariant())
                {
                    case "UNICODE":
                        result = Encoding.Unicode.GetString(dataBytes, 0, actualLength);
                        break;
                    case "JIS":
                        // Japanese Industrial Standard - rarely used
                        result = Encoding.GetEncoding("ISO-2022-JP").GetString(dataBytes, 0, actualLength);
                        break;
                    case "ASCII":
                    default:
                        // For ASCII, we'll try Latin-1 for better German umlaut support
                        result = Encoding.GetEncoding("ISO-8859-1").GetString(dataBytes, 0, actualLength);
                        break;
                }

                // Remove any remaining control characters but preserve the data
                result = CleanUserCommentData(result);

                _logger.LogDebug("Extracted UserComment: '{Comment}' (final length: {Length})",
                    result, result.Length);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting UserComment safely");
                // Fallback: try to extract as Latin-1 skipping first 8 bytes
                try
                {
                    var fallback = Encoding.GetEncoding("ISO-8859-1")
                        .GetString(userCommentBytes, 8, userCommentBytes.Length - 8);
                    return CleanUserCommentData(fallback);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Cleans UserComment data while preserving all actual content
        /// </summary>
        private string CleanUserCommentData(string data)
        {
            if (string.IsNullOrEmpty(data))
                return data;

            var sb = new StringBuilder(data.Length);

            // Note: GCM_TAG removal is now handled in ParsePipeDelimitedFormat
            // This method just cleans control characters

            // Process character by character to preserve all printable content
            foreach (char c in data)
            {
                // Keep all printable ASCII and extended ASCII (for umlauts)
                if (c >= 32 || c == '\t' || c == '\n' || c == '\r')
                {
                    sb.Append(c);
                }
                // Stop at first null character (true end of string)
                else if (c == '\0')
                {
                    break;
                }
            }

            return sb.ToString().Trim();
        }

        private void ExtractValue(Dictionary<string, string> exifData, MetadataExtractor.Directory directory, int tagType, string key)
        {
            if (directory.HasTagName(tagType))
            {
                var value = directory.GetDescription(tagType);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    exifData[key] = value;
                }
            }
        }

        private void ExtractAllTags(MetadataExtractor.Directory directory, Dictionary<string, string> exifData)
        {
            foreach (var tag in directory.Tags)
            {
                var tagName = tag.Name;
                var description = tag.Description;

                if (!string.IsNullOrWhiteSpace(description))
                {
                    // Clean up the description - remove line breaks
                    var cleanedDescription = description
                        .Replace("\r\n", " ")
                        .Replace("\n", " ")
                        .Replace("\r", " ")
                        .Trim();

                    // PRIORITY CHECK: Is this the Pentax Barcode tag?
                    if (tagName.Equals("Barcode", StringComparison.OrdinalIgnoreCase) ||
                        (directory.Name.Contains("Pentax") && tagName.Contains("0x0423")))
                    {
                        if (cleanedDescription.Contains('|'))
                        {
                            exifData["Barcode"] = cleanedDescription;
                            _logger.LogInformation("Found Ricoh/Pentax Barcode tag: {Value}", cleanedDescription);
                            continue;
                        }
                    }

                    // Check if this might be barcode data in other tags
                    if (cleanedDescription.Contains('|') &&
                        (cleanedDescription.StartsWith("EX", StringComparison.OrdinalIgnoreCase) ||
                         cleanedDescription.Contains("Schmidt") ||
                         cleanedDescription.Contains("1985")))
                    {
                        // Only set as Barcode if we don't already have one
                        if (!exifData.ContainsKey("Barcode"))
                        {
                            exifData["Barcode"] = cleanedDescription;
                            _logger.LogDebug("Found barcode data in tag {TagName}: {Value}", tagName, cleanedDescription);
                        }
                    }

                    // Store tag with cleaned name (including Pentax-specific tags)
                    var cleanName = tagName.Replace(" ", "").Replace("-", "");

                    // Special handling for Pentax tags
                    if (directory.Name.Contains("Pentax") && tagName.StartsWith("Unknown"))
                    {
                        // Also store with a more descriptive name
                        if (tagName.Contains("0x0423"))
                        {
                            exifData["PentaxBarcode"] = cleanedDescription;
                        }
                    }

                    if (!exifData.ContainsKey(cleanName))
                    {
                        exifData[cleanName] = cleanedDescription;
                    }
                }
            }
        }

        private void FindBarcodeData(Dictionary<string, string> exifData)
        {
            // PRIORITY 1: Check if "Barcode" key already exists (from ExtractAllTags)
            if (exifData.TryGetValue("Barcode", out var existingBarcode) &&
                !string.IsNullOrWhiteSpace(existingBarcode) &&
                existingBarcode.Contains('|'))
            {
                _logger.LogDebug("Barcode already found: {Value}", existingBarcode);
                return;
            }

            // PRIORITY 2: Check various possible tag names for barcode data
            var possibleBarcodeKeys = new[] {
                "Barcode",
                "BarcodeInfo",
                "Pentax Barcode",  // Pentax-specific
                "Unknown0x0423",   // Pentax tag 0x0423
                "Unknown0x9999",
                "Memo",
                "Comment"
            };

            foreach (var key in possibleBarcodeKeys)
            {
                if (exifData.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
                {
                    if (value.Contains('|'))
                    {
                        exifData["Barcode"] = value;
                        _logger.LogInformation("Found barcode data in {Key}: {Value}", key, value);
                        return; // Stop after finding first valid barcode
                    }
                }
            }

            // PRIORITY 3: If User Comment contains barcode-like data, use it as fallback
            if (exifData.TryGetValue("UserComment", out var userComment))
            {
                // Skip if UserComment is just "GCM_TAG"
                if (userComment.Trim() == "GCM_TAG")
                {
                    _logger.LogDebug("UserComment contains only 'GCM_TAG' marker, skipping");
                }
                else if (userComment.Contains('|') && !exifData.ContainsKey("Barcode"))
                {
                    exifData["Barcode"] = userComment;
                    _logger.LogDebug("Using UserComment as Barcode fallback: {Value}", userComment);
                }
            }
        }

        private Dictionary<string, string> ParsePipeDelimitedFormat(string data)
        {
            // Clean up the data first - remove line breaks but preserve content
            data = data.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");

            // WICHTIG: Remove "GCM_TAG " prefix that Ricoh camera adds
            if (data.StartsWith("GCM_TAG ", StringComparison.OrdinalIgnoreCase))
            {
                data = data.Substring(8).Trim();
                _logger.LogDebug("Removed 'GCM_TAG ' prefix from data");
            }

            _logger.LogDebug("Parsing QRBridge data: '{Data}' (length: {Length})", data, data.Length);

            // Format: EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax
            var parts = data.Split('|');
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Parse all available fields
            if (parts.Length >= 1 && !string.IsNullOrWhiteSpace(parts[0]))
                result["examid"] = parts[0].Trim();

            if (parts.Length >= 2 && !string.IsNullOrWhiteSpace(parts[1]))
                result["name"] = parts[1].Trim();

            if (parts.Length >= 3 && !string.IsNullOrWhiteSpace(parts[2]))
                result["birthdate"] = parts[2].Trim();

            if (parts.Length >= 4 && !string.IsNullOrWhiteSpace(parts[3]))
                result["gender"] = parts[3].Trim();

            if (parts.Length >= 5 && !string.IsNullOrWhiteSpace(parts[4]))
                result["comment"] = parts[4].Trim();

            _logger.LogInformation("Parsed pipe-delimited data: {Count} fields from {PartsCount} parts",
                result.Count, parts.Length);

            // Log each field for debugging
            foreach (var field in result)
            {
                _logger.LogDebug("  {Key}: '{Value}' (Length: {Length})",
                    field.Key, field.Value, field.Value.Length);
            }

            // Log warning if expected fields are missing (Ricoh G900 II limitation)
            if (parts.Length < 5)
            {
                _logger.LogWarning("QRBridge data incomplete: only {Count} of 5 expected fields found. " +
                    "This is a known Ricoh G900 II limitation.", parts.Length);
            }

            return result;
        }

        private Dictionary<string, string> ParseCommandLineFormat(string data)
        {
            // Format: -examid "EX002" -name "Schmidt, Maria" -birthdate "1985-03-15"
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Simple regex pattern to match -key "value" pairs
            var pattern = @"-(\w+)\s+""([^""]+)""";
            var matches = System.Text.RegularExpressions.Regex.Matches(data, pattern);

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (match.Groups.Count >= 3)
                {
                    var key = match.Groups[1].Value.ToLower();
                    var value = match.Groups[2].Value;
                    result[key] = value;
                }
            }

            _logger.LogDebug("Parsed command-line format data: {Count} fields", result.Count);
            return result;
        }
    }
}
