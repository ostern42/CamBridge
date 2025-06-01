using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace CamBridge.TestConsole
{
    /// <summary>
    /// Debug console to diagnose the parser bug
    /// </summary>
    class ParserDebugConsole
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== CamBridge Parser Debug Console ===\n");

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: ParserDebugConsole <jpeg-file-path>");
                Console.WriteLine("\nThis tool will:");
                Console.WriteLine("1. Read EXIF data using multiple methods");
                Console.WriteLine("2. Show raw UserComment bytes");
                Console.WriteLine("3. Try different decodings");
                Console.WriteLine("4. Display parsed QRBridge fields");
                return;
            }

            var filePath = args[0];
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: File not found: {filePath}");
                return;
            }

            Console.WriteLine($"Analyzing: {filePath}\n");

            try
            {
                // Method 1: Using MetadataExtractor
                Console.WriteLine("=== Method 1: MetadataExtractor ===");
                await AnalyzeWithMetadataExtractor(filePath);

                // Method 2: Raw EXIF reading
                Console.WriteLine("\n=== Method 2: Raw EXIF Data ===");
                await AnalyzeRawExifData(filePath);

                // Method 3: Direct UserComment extraction
                Console.WriteLine("\n=== Method 3: Direct UserComment ===");
                await ExtractUserCommentDirectly(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static async Task AnalyzeWithMetadataExtractor(string filePath)
        {
            var directories = ImageMetadataReader.ReadMetadata(filePath);

            // First, list ALL tags to find where Barcode is stored
            Console.WriteLine("=== ALL EXIF TAGS ===");
            foreach (var directory in directories)
            {
                Console.WriteLine($"\nDirectory: {directory.Name}");
                foreach (var tag in directory.Tags)
                {
                    var value = tag.Description;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        // Highlight barcode-related tags
                        if (tag.Name.Contains("Barcode", StringComparison.OrdinalIgnoreCase) ||
                            (value.Contains('|') && value.Contains("EX")))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"  >>> {tag.Name}: {value}");
                            Console.ResetColor();
                        }
                        else if (tag.Name.Contains("User", StringComparison.OrdinalIgnoreCase) ||
                                 tag.Name.Contains("Comment", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"  >> {tag.Name}: {value}");
                            Console.ResetColor();
                        }
                        else if (value.Length < 50) // Only show short values
                        {
                            Console.WriteLine($"  {tag.Name}: {value}");
                        }
                    }
                }
            }

            Console.WriteLine("\n=== FOCUSED ANALYSIS ===");

            // Look for UserComment in EXIF
            foreach (var directory in directories)
            {
                if (directory is ExifSubIfdDirectory subIfd)
                {
                    if (subIfd.HasTagName(ExifDirectoryBase.TagUserComment))
                    {
                        // Get raw bytes
                        var userCommentBytes = subIfd.GetByteArray(ExifDirectoryBase.TagUserComment);
                        Console.WriteLine($"\nUserComment raw bytes length: {userCommentBytes?.Length ?? 0}");

                        if (userCommentBytes != null && userCommentBytes.Length > 0)
                        {
                            // Show hex dump of first 100 bytes
                            Console.WriteLine("\nHex dump (first 100 bytes):");
                            ShowHexDump(userCommentBytes, Math.Min(100, userCommentBytes.Length));

                            // Try to decode character code
                            if (userCommentBytes.Length > 8)
                            {
                                var charCode = Encoding.ASCII.GetString(userCommentBytes, 0, 8);
                                Console.WriteLine($"\nCharacter code: '{charCode.Replace("\0", "\\0")}'");

                                // Extract data portion
                                var dataBytes = new byte[userCommentBytes.Length - 8];
                                Array.Copy(userCommentBytes, 8, dataBytes, 0, dataBytes.Length);

                                // Find actual length (last non-null byte)
                                int actualLength = dataBytes.Length;
                                for (int i = dataBytes.Length - 1; i >= 0; i--)
                                {
                                    if (dataBytes[i] != 0)
                                    {
                                        actualLength = i + 1;
                                        break;
                                    }
                                }
                                Console.WriteLine($"Actual data length: {actualLength} (total: {dataBytes.Length})");

                                // Try different encodings
                                Console.WriteLine("\nDecoded as Latin-1:");
                                var latin1 = Encoding.GetEncoding("ISO-8859-1").GetString(dataBytes, 0, actualLength);
                                Console.WriteLine($"'{latin1}'");
                                Console.WriteLine($"Length: {latin1.Length}, Contains pipe: {latin1.Contains('|')}");

                                Console.WriteLine("\nDecoded as UTF-8:");
                                try
                                {
                                    var utf8 = Encoding.UTF8.GetString(dataBytes, 0, actualLength);
                                    Console.WriteLine($"'{utf8}'");
                                    Console.WriteLine($"Length: {utf8.Length}, Contains pipe: {utf8.Contains('|')}");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"UTF-8 decode failed: {ex.Message}");
                                }

                                // Parse pipe-delimited data
                                if (latin1.Contains('|'))
                                {
                                    Console.WriteLine("\n=== Parsing UserComment Pipe-Delimited Data ===");
                                    ParseAndDisplayPipeData(latin1);
                                }
                            }
                        }

                        // Also get the string representation
                        var description = directory.GetDescription(ExifDirectoryBase.TagUserComment);
                        Console.WriteLine($"\nMetadataExtractor UserComment description: '{description}'");
                    }
                }
            }
        }

        static async Task AnalyzeRawExifData(string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(fs);

            // Check JPEG SOI marker
            if (reader.ReadUInt16() != 0xD8FF) // Note: little-endian
            {
                Console.WriteLine("Not a valid JPEG file");
                return;
            }

            // Look for EXIF APP1 segment
            while (fs.Position < fs.Length - 2)
            {
                var marker = ReadBigEndianUInt16(reader);

                if (marker == 0xFFE1) // APP1
                {
                    var segmentLength = ReadBigEndianUInt16(reader);
                    Console.WriteLine($"Found APP1 segment, length: {segmentLength}");

                    var startPos = fs.Position;
                    var identifier = reader.ReadBytes(6);
                    if (Encoding.ASCII.GetString(identifier) == "Exif\0\0")
                    {
                        Console.WriteLine("Found EXIF data");

                        // Read some EXIF data to show structure
                        var tiffHeader = reader.ReadBytes(8);
                        Console.WriteLine($"TIFF header: {BitConverter.ToString(tiffHeader)}");

                        // Skip to where UserComment might be
                        // This is just for demonstration
                        break;
                    }
                    else
                    {
                        // Skip rest of segment
                        fs.Seek(startPos + segmentLength - 8, SeekOrigin.Begin);
                    }
                }
                else if ((marker & 0xFF00) == 0xFF00)
                {
                    var segmentLength = ReadBigEndianUInt16(reader);
                    fs.Seek(segmentLength - 2, SeekOrigin.Current);
                }
                else
                {
                    fs.Seek(-1, SeekOrigin.Current);
                }
            }
        }

        static async Task ExtractUserCommentDirectly(string filePath)
        {
            // This method tries to find UserComment by searching for known patterns
            var fileBytes = await File.ReadAllBytesAsync(filePath);

            // Look for "GCM_TAG " pattern (Ricoh specific)
            var gcmPattern = Encoding.ASCII.GetBytes("GCM_TAG ");
            var gcmIndex = FindPattern(fileBytes, gcmPattern);

            if (gcmIndex >= 0)
            {
                Console.WriteLine($"Found 'GCM_TAG ' at offset: {gcmIndex}");

                // Extract data after GCM_TAG
                var dataStart = gcmIndex + gcmPattern.Length;
                var sb = new StringBuilder();

                // Read until we hit multiple nulls or end of reasonable data
                int nullCount = 0;
                for (int i = dataStart; i < fileBytes.Length && i < dataStart + 500; i++)
                {
                    var b = fileBytes[i];
                    if (b == 0)
                    {
                        nullCount++;
                        if (nullCount > 2) break;
                    }
                    else
                    {
                        nullCount = 0;
                        if (b >= 32 || b == '|') // Printable or pipe
                        {
                            sb.Append((char)b);
                        }
                    }
                }

                var extracted = sb.ToString();
                Console.WriteLine($"\nExtracted data: '{extracted}'");
                Console.WriteLine($"Length: {extracted.Length}");

                if (extracted.Contains('|'))
                {
                    ParseAndDisplayPipeData(extracted);
                }
            }
            else
            {
                Console.WriteLine("'GCM_TAG ' pattern not found");

                // Try to find pipe-delimited pattern
                var exPattern = Encoding.ASCII.GetBytes("EX");
                var exIndex = FindPattern(fileBytes, exPattern);
                if (exIndex >= 0)
                {
                    Console.WriteLine($"\nFound 'EX' pattern at offset: {exIndex}");
                    // Extract some context
                    var contextStart = Math.Max(0, exIndex - 20);
                    var contextEnd = Math.Min(fileBytes.Length, exIndex + 200);
                    var context = Encoding.GetEncoding("ISO-8859-1").GetString(
                        fileBytes, contextStart, contextEnd - contextStart);
                    Console.WriteLine($"Context: '{context.Replace("\0", "\\0")}'");
                }
            }
        }

        static void ParseAndDisplayPipeData(string data)
        {
            // Clean the data
            data = data.Trim();

            // Remove GCM_TAG if present
            if (data.StartsWith("GCM_TAG ", StringComparison.OrdinalIgnoreCase))
            {
                data = data.Substring(8);
            }

            Console.WriteLine($"Parsing: '{data}'");
            var parts = data.Split('|');

            Console.WriteLine($"\nFound {parts.Length} parts:");
            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i].Trim();
                var fieldName = i switch
                {
                    0 => "examid",
                    1 => "name",
                    2 => "birthdate",
                    3 => "gender",
                    4 => "comment",
                    _ => $"field{i}"
                };

                Console.WriteLine($"  [{i}] {fieldName}: '{part}' (length: {part.Length})");
            }
        }

        static int FindPattern(byte[] data, byte[] pattern)
        {
            for (int i = 0; i <= data.Length - pattern.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (data[i + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found) return i;
            }
            return -1;
        }

        static void ShowHexDump(byte[] data, int length)
        {
            for (int i = 0; i < length; i += 16)
            {
                // Hex
                Console.Write($"{i:X4}: ");
                for (int j = 0; j < 16; j++)
                {
                    if (i + j < length)
                        Console.Write($"{data[i + j]:X2} ");
                    else
                        Console.Write("   ");
                }

                // ASCII
                Console.Write(" | ");
                for (int j = 0; j < 16 && i + j < length; j++)
                {
                    var b = data[i + j];
                    Console.Write(b >= 32 && b < 127 ? (char)b : '.');
                }
                Console.WriteLine();
            }
        }

        static ushort ReadBigEndianUInt16(BinaryReader reader)
        {
            var b1 = reader.ReadByte();
            var b2 = reader.ReadByte();
            return (ushort)((b1 << 8) | b2);
        }
    }
}
