using System;
using System.Diagnostics;
using System.IO;

namespace CamBridge.PipelineTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== CamBridge ExifTool Direct Test v0.5.21 ===");
            Console.WriteLine("© 2025 Claude's Improbably Reliable Software Solutions\n");

            // Test file
            var testFile = "R0010168.JPG";
            
            if (!File.Exists(testFile))
            {
                Console.WriteLine($"ERROR: Test file not found: {testFile}");
                Console.WriteLine($"Current directory: {Directory.GetCurrentDirectory()}");
                Console.WriteLine("\nPlease copy R0010168.JPG to this directory!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"✅ Found test file: {testFile}");
            var fileInfo = new FileInfo(testFile);
            Console.WriteLine($"   Size: {fileInfo.Length:N0} bytes");
            Console.WriteLine($"   Modified: {fileInfo.LastWriteTime}");

            // Find ExifTool
            var exifToolPaths = new[]
            {
                @"..\..\Tools\exiftool.exe",
                @"..\..\..\Tools\exiftool.exe",
                @"Tools\exiftool.exe",
                @"C:\Users\aiadmin\source\repos\CamBridge\Tools\exiftool.exe"
            };

            string? exifToolPath = null;
            foreach (var path in exifToolPaths)
            {
                if (File.Exists(path))
                {
                    exifToolPath = path;
                    break;
                }
            }

            if (exifToolPath == null)
            {
                Console.WriteLine("\n❌ ERROR: ExifTool not found!");
                Console.WriteLine("Searched in:");
                foreach (var path in exifToolPaths)
                {
                    Console.WriteLine($"  - {Path.GetFullPath(path)}");
                }
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\n✅ Found ExifTool: {exifToolPath}");

            try
            {
                // Test 1: Get all EXIF data
                Console.WriteLine("\n=== Test 1: Reading ALL EXIF Data ===");
                RunExifTool(exifToolPath, $"-s \"{testFile}\"");

                // Test 2: Get specific fields
                Console.WriteLine("\n=== Test 2: Looking for QRBridge Data ===");
                RunExifTool(exifToolPath, $"-UserComment -ImageDescription -XPComment -Subject -Keywords \"{testFile}\"");

                // Test 3: Search for 'EX002' in all fields
                Console.WriteLine("\n=== Test 3: Searching for 'EX002' ===");
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = exifToolPath,
                        Arguments = $"-a -u -s \"{testFile}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                // Search for EX002 in output
                var lines = output.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Contains("EX002") || line.Contains("Schmidt") || line.Contains("|"))
                    {
                        Console.WriteLine($"🎯 FOUND: {line.Trim()}");
                    }
                }

                // Test 4: Get Ricoh-specific fields
                Console.WriteLine("\n=== Test 4: Ricoh-Specific Fields ===");
                RunExifTool(exifToolPath, $"-Make -Model -Ricoh:* \"{testFile}\"");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void RunExifTool(string exifToolPath, string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exifToolPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(output))
            {
                Console.WriteLine(output);
            }

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine($"Error: {error}");
            }
        }
    }
}