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
        public async Task<string?> GetUserCommentAsync(string filePath)
        {
            var exifData = await ReadExifDataAsync(filePath);

            // First try to get the barcode field (Ricoh G900 II specific)
            if (exifData.TryGetValue("Barcode", out var barcode) && !string.IsNullOrWhiteSpace(barcode))
            {
                _logger.LogInformation("Found QRBridge data in Barcode field: {Barcode}", barcode);
                return barcode;
            }

            // Fallback to User Comment
            if (exifData.TryGetValue("UserComment", out var userComment))
            {
                _logger.LogInformation("Found data in UserComment field: {UserComment}", userComment);
                return userComment;
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

                        // Special handling for User Comment with proper encoding
                        if (subIfd.HasTagName(ExifDirectoryBase.TagUserComment))
                        {
                            var userCommentBytes = subIfd.GetByteArray(ExifDirectoryBase.TagUserComment);
                            if (userCommentBytes != null && userCommentBytes.Length > 8)
                            {
                                // EXIF UserComment has 8-byte encoding prefix
                                // Skip it and use Latin-1 for German umlauts
                                var dataBytes = userCommentBytes.Skip(8).ToArray();
                                var userComment = Encoding.GetEncoding("ISO-8859-1").GetString(dataBytes).TrimEnd('\0');

                                if (!string.IsNullOrWhiteSpace(userComment))
                                {
                                    exifData["UserComment"] = userComment;
                                    _logger.LogDebug("Extracted UserComment with Latin-1 encoding: {UserComment}", userComment);
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
                    // Clean up the description - remove line breaks and fix encoding
                    var cleanedDescription = description
                        .Replace("\r\n", "")
                        .Replace("\n", "")
                        .Replace("\r", "");

                    // Check if this might be barcode data (pipe-delimited = QRBridge format)
                    if (cleanedDescription.Contains('|') &&
                        (cleanedDescription.Contains("EX") || cleanedDescription.Contains("Schmidt") ||
                         cleanedDescription.Contains("1985")))
                    {
                        // Check if it's the User Comment with encoding prefix
                        if (tagName.Contains("User Comment") && description.Length > 8)
                        {
                            // Try to clean it
                            var cleaned = CleanUserComment(description);
                            if (cleaned != description && cleaned.Contains('|'))
                            {
                                exifData["Barcode"] = cleaned;
                                _logger.LogDebug("Found barcode data in tag {TagName}: {Value}", tagName, cleaned);
                                continue;
                            }
                        }

                        exifData["Barcode"] = cleanedDescription;
                        _logger.LogDebug("Found barcode data in tag {TagName}: {Value}", tagName, cleanedDescription);
                    }

                    // Store tag with cleaned name
                    var cleanName = tagName.Replace(" ", "").Replace("-", "");
                    if (!exifData.ContainsKey(cleanName))
                    {
                        exifData[cleanName] = cleanedDescription;
                    }
                }
            }
        }

        private void FindBarcodeData(Dictionary<string, string> exifData)
        {
            // Check various possible tag names for barcode data
            var possibleBarcodeKeys = new[] { "Barcode", "BarcodeInfo", "Unknown0x9999", "Memo", "Comment" };

            foreach (var key in possibleBarcodeKeys)
            {
                if (exifData.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
                {
                    if (value.Contains('|'))
                    {
                        exifData["Barcode"] = value;
                        _logger.LogDebug("Found barcode data in {Key}: {Value}", key, value);
                        break;
                    }
                }
            }

            // If User Comment contains barcode-like data, also consider it
            if (exifData.TryGetValue("UserComment", out var userComment))
            {
                if (userComment.Contains('|') && !exifData.ContainsKey("Barcode"))
                {
                    exifData["Barcode"] = userComment;
                }
            }
        }

        private string CleanUserComment(string userComment)
        {
            // First, try to detect and fix encoding issues
            // The � character often indicates UTF-8 interpreted as Latin-1
            if (userComment.Contains("�") || userComment.Contains("\ufffd"))
            {
                _logger.LogDebug("Detected encoding issue in user comment, attempting to fix");

                // Try different approaches to fix the encoding
                try
                {
                    // If it's UTF-8 bytes interpreted as Latin-1, convert back
                    var latin1Bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(userComment);
                    var utf8String = Encoding.UTF8.GetString(latin1Bytes);

                    // Check if this improved the string (should contain German umlauts now)
                    if (!utf8String.Contains("�") &&
                        (utf8String.Contains("ö") || utf8String.Contains("ä") || utf8String.Contains("ü") ||
                         utf8String.Contains("Ö") || utf8String.Contains("Ä") || utf8String.Contains("Ü")))
                    {
                        userComment = utf8String;
                        _logger.LogDebug("Fixed encoding using UTF-8 interpretation");
                    }
                }
                catch
                {
                    // If conversion fails, continue with original
                }
            }

            // EXIF UserComment often has encoding prefix like "ASCII\0\0\0" or "UNICODE\0"
            if (userComment.StartsWith("ASCII", StringComparison.OrdinalIgnoreCase))
            {
                var idx = userComment.IndexOf('\0');
                if (idx > 0 && idx < userComment.Length - 1)
                {
                    return userComment.Substring(idx + 1).TrimStart('\0');
                }
            }

            // Check if first 8 chars look like encoding marker
            if (userComment.Length > 8 && !userComment.Substring(0, 8).Contains('|') && userComment.Substring(8).Contains('|'))
            {
                return userComment.Substring(8).TrimStart('\0');
            }

            return userComment;
        }

        private Dictionary<string, string> ParsePipeDelimitedFormat(string data)
        {
            // Clean up the data first - remove any line breaks the camera might have added
            // The Ricoh G900 II displays barcode on 2 lines and might insert line breaks
            data = data.Replace("\r\n", "").Replace("\n", "").Replace("\r", "");

            // Also remove any non-printable characters except pipe
            data = System.Text.RegularExpressions.Regex.Replace(data, @"[^\x20-\x7E|äöüÄÖÜß]", "");

            _logger.LogDebug("Cleaned QRBridge data: {Data}", data);

            // Format: EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax
            var parts = data.Split('|');
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (parts.Length >= 1) result["examid"] = parts[0].Trim();
            if (parts.Length >= 2) result["name"] = parts[1].Trim();
            if (parts.Length >= 3) result["birthdate"] = parts[2].Trim();
            if (parts.Length >= 4) result["gender"] = parts[3].Trim();
            if (parts.Length >= 5) result["comment"] = parts[4].Trim();

            _logger.LogDebug("Parsed pipe-delimited data: {Count} fields", result.Count);

            // Log each field for debugging
            foreach (var field in result)
            {
                _logger.LogDebug("  {Key}: '{Value}' (Length: {Length})",
                    field.Key, field.Value, field.Value.Length);
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
