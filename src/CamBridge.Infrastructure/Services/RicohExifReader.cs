using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Specialized EXIF reader for Ricoh cameras that extracts barcode data from the correct tag
    /// </summary>
    public class RicohExifReader : ExifReader
    {
        private readonly ILogger<RicohExifReader> _logger;

        // Ricoh-specific EXIF tags (from Pentax MakerNotes)
        private const int PENTAX_TAG_BARCODE = 0x0423; // Barcode tag in Pentax MakerNotes

        public RicohExifReader(ILogger<RicohExifReader> logger) : base(logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Override to specifically look for Barcode tag in Ricoh/Pentax cameras
        /// </summary>
        public override async Task<string?> GetUserCommentAsync(string filePath)
        {
            try
            {
                var directories = await Task.Run(() => ImageMetadataReader.ReadMetadata(filePath));

                // PRIORITY 1: Look for Barcode in Pentax MakerNotes
                foreach (var directory in directories)
                {
                    if (directory.Name.Contains("Pentax") || directory.Name.Contains("Makernote"))
                    {
                        // Try to get the Barcode tag
                        if (directory.HasTagName(PENTAX_TAG_BARCODE))
                        {
                            var barcodeValue = directory.GetString(PENTAX_TAG_BARCODE);
                            if (!string.IsNullOrWhiteSpace(barcodeValue))
                            {
                                _logger.LogInformation("Found QRBridge data in Pentax Barcode tag: {Barcode}", barcodeValue);
                                return CleanBarcodeData(barcodeValue);
                            }
                        }

                        // Also try by tag name
                        foreach (var tag in directory.Tags)
                        {
                            if (tag.Name.Contains("Barcode", StringComparison.OrdinalIgnoreCase))
                            {
                                var value = tag.Description;
                                if (!string.IsNullOrWhiteSpace(value) && value.Contains('|'))
                                {
                                    _logger.LogInformation("Found QRBridge data in tag {TagName}: {Value}",
                                        tag.Name, value);
                                    return CleanBarcodeData(value);
                                }
                            }
                        }
                    }
                }

                // PRIORITY 2: Check all directories for any Barcode tag
                foreach (var directory in directories)
                {
                    foreach (var tag in directory.Tags)
                    {
                        if (tag.Name.Equals("Barcode", StringComparison.OrdinalIgnoreCase) ||
                            tag.Name.Contains("Barcode", StringComparison.OrdinalIgnoreCase))
                        {
                            var value = tag.Description;
                            if (!string.IsNullOrWhiteSpace(value) && value.Contains('|'))
                            {
                                _logger.LogInformation("Found QRBridge data in {Directory} - {TagName}: {Value}",
                                    directory.Name, tag.Name, value);
                                return CleanBarcodeData(value);
                            }
                        }
                    }
                }

                // PRIORITY 3: Fall back to UserComment (original behavior)
                _logger.LogWarning("No Barcode tag found, falling back to UserComment search");
                return await base.GetUserCommentAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading Barcode tag from {FilePath}", filePath);
                // Fall back to base implementation
                return await base.GetUserCommentAsync(filePath);
            }
        }

        /// <summary>
        /// Clean barcode data from potential encoding issues
        /// </summary>
        private string CleanBarcodeData(string barcodeData)
        {
            if (string.IsNullOrWhiteSpace(barcodeData))
                return barcodeData;

            // The barcode might have encoding issues (like "R÷ntgen" instead of "Röntgen")
            // This is likely because ExifTool is interpreting UTF-8 as Latin-1 or vice versa

            // Try to fix common encoding issues
            var cleaned = barcodeData;

            // Fix known encoding problems
            cleaned = cleaned.Replace("÷", "ö");  // Common UTF-8/Latin-1 confusion
            cleaned = cleaned.Replace("á", " ");  // Space misencoded

            // Remove any control characters but keep printable ones
            var sb = new StringBuilder();
            foreach (char c in cleaned)
            {
                if (c >= 32 || c == '\t' || c == '\n' || c == '\r')
                {
                    sb.Append(c);
                }
            }

            var result = sb.ToString().Trim();

            if (result != barcodeData)
            {
                _logger.LogDebug("Cleaned barcode data from '{Original}' to '{Cleaned}'",
                    barcodeData, result);
            }

            return result;
        }

        /// <summary>
        /// Attempts to extract barcode data directly from JPEG EXIF segment
        /// This is a fallback method when MetadataExtractor doesn't support the tag
        /// </summary>
        public async Task<string?> ExtractBarcodeFromRawExifAsync(string filePath)
        {
            try
            {
                // First try the standard approach
                var result = await GetUserCommentAsync(filePath);
                if (!string.IsNullOrWhiteSpace(result))
                    return result;

                // If that fails, try raw EXIF extraction
                var exifData = await ExtractRawExifDataAsync(filePath);
                if (exifData == null || exifData.Length == 0)
                {
                    _logger.LogWarning("No EXIF data found in file: {FilePath}", filePath);
                    return null;
                }

                // Look for barcode pattern in EXIF data
                var barcodeData = FindBarcodePattern(exifData);
                if (!string.IsNullOrEmpty(barcodeData))
                {
                    _logger.LogInformation("Found barcode data in raw EXIF: {Barcode}", barcodeData);
                    return barcodeData;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting barcode from raw EXIF");
                return null;
            }
        }

        private async Task<byte[]?> ExtractRawExifDataAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    using var reader = new BinaryReader(fs);

                    // Check JPEG SOI marker
                    if (reader.ReadUInt16() != 0xFFD8)
                    {
                        _logger.LogWarning("Not a valid JPEG file: {FilePath}", filePath);
                        return null;
                    }

                    // Look for EXIF APP1 segment
                    while (fs.Position < fs.Length - 2)
                    {
                        var marker = reader.ReadUInt16();

                        // Check for APP1 marker (0xFFE1)
                        if (marker == 0xFFE1)
                        {
                            var segmentLength = ReadBigEndianUInt16(reader);

                            // Check for "Exif\0\0" identifier
                            var exifIdentifier = reader.ReadBytes(6);
                            if (Encoding.ASCII.GetString(exifIdentifier) == "Exif\0\0")
                            {
                                // Read EXIF data
                                var exifDataLength = segmentLength - 8; // Subtract marker and identifier length
                                return reader.ReadBytes(exifDataLength);
                            }
                        }
                        else if ((marker & 0xFF00) == 0xFF00)
                        {
                            // Skip segment
                            var segmentLength = ReadBigEndianUInt16(reader);
                            fs.Seek(segmentLength - 2, SeekOrigin.Current);
                        }
                        else
                        {
                            // Not a valid marker, try next byte
                            fs.Seek(-1, SeekOrigin.Current);
                        }
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading raw EXIF data");
                    return null;
                }
            });
        }

        private string? FindBarcodePattern(byte[] exifData)
        {
            // Look for pipe-delimited pattern that matches our expected format
            // Pattern: EX###|Name|Date|Gender|Comment

            // Try different encodings
            string[] possibleStrings = {
                Encoding.UTF8.GetString(exifData),
                Encoding.GetEncoding("ISO-8859-1").GetString(exifData),
                Encoding.ASCII.GetString(exifData)
            };

            foreach (var dataString in possibleStrings)
            {
                // Look for our QRBridge patterns
                var patterns = new[]
                {
                    @"EX\d+\|[^|]+\|[\d-]+\|[MFO]\|[^|]+",  // Full format with 5 fields
                    @"EX\d+\|[^|]+\|[\d-]+\|[MFO]",          // 4 fields
                    @"EX\d+\|[^|]+\|[\d-]+",                 // 3 fields
                    @"EX\d+\|[^|]+",                          // 2 fields
                };

                foreach (var pattern in patterns)
                {
                    var match = System.Text.RegularExpressions.Regex.Match(
                        dataString,
                        pattern,
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        _logger.LogDebug("Found barcode pattern in raw EXIF: {Pattern}", match.Value);
                        return CleanBarcodeData(match.Value);
                    }
                }
            }

            // Alternative: Look for ASCII strings that might be barcode data
            var asciiStrings = ExtractAsciiStrings(exifData, 10); // Min length 10
            foreach (var str in asciiStrings)
            {
                if (str.Contains('|') && str.StartsWith("EX", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogDebug("Found potential barcode string: {String}", str);
                    return CleanBarcodeData(str);
                }
            }

            return null;
        }

        private List<string> ExtractAsciiStrings(byte[] data, int minLength)
        {
            var strings = new List<string>();
            var currentString = new StringBuilder();

            foreach (var b in data)
            {
                if (b >= 32 && b <= 126) // Printable ASCII
                {
                    currentString.Append((char)b);
                }
                else
                {
                    if (currentString.Length >= minLength)
                    {
                        strings.Add(currentString.ToString());
                    }
                    currentString.Clear();
                }
            }

            if (currentString.Length >= minLength)
            {
                strings.Add(currentString.ToString());
            }

            return strings;
        }

        private ushort ReadBigEndianUInt16(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(2);
            return (ushort)((bytes[0] << 8) | bytes[1]);
        }
    }
}
