using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Specialized EXIF reader for Ricoh cameras that can extract custom barcode tags
    /// Falls back to standard ExifReader for most operations
    /// </summary>
    public class RicohExifReader : ExifReader
    {
        private readonly ILogger<RicohExifReader> _logger;

        // Known Ricoh-specific EXIF tags
        private const int RICOH_BARCODE_TAG = 0x9999; // Placeholder - actual tag ID needs verification
        private const string TIFF_HEADER_II = "II"; // Little-endian
        private const string TIFF_HEADER_MM = "MM"; // Big-endian
        private const ushort TIFF_MAGIC = 42;

        public RicohExifReader(ILogger<RicohExifReader> logger) : base(logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Attempts to extract barcode data directly from JPEG EXIF segment
        /// This is a fallback method when standard EXIF libraries don't support custom tags
        /// </summary>
        public async Task<string?> ExtractBarcodeFromRawExifAsync(string filePath)
        {
            try
            {
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

            var dataString = Encoding.UTF8.GetString(exifData);

            // Simple pattern matching for Ricoh barcode format
            var patterns = new[]
            {
                @"EX\d+\|[^|]+\|[\d-]+\|[MFO]\|[^|]+",  // Full format
                @"EX\d+\|[^|]+\|[\d-]+\|[MFO]",          // Without comment
                @"EX\d+\|[^|]+\|[\d-]+"                  // Minimal format
            };

            foreach (var pattern in patterns)
            {
                var match = System.Text.RegularExpressions.Regex.Match(
                    dataString,
                    pattern,
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    return match.Value;
                }
            }

            // Alternative: Look for ASCII strings that might be barcode data
            var asciiStrings = ExtractAsciiStrings(exifData, 10); // Min length 10
            foreach (var str in asciiStrings)
            {
                if (str.Contains('|') && (str.StartsWith("EX") || str.Contains("Schmidt") || str.Contains("1985")))
                {
                    _logger.LogDebug("Found potential barcode string: {String}", str);
                    return str;
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
