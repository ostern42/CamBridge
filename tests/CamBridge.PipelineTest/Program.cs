using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace CamBridge.PipelineTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== CamBridge Pipeline Test v0.5.22 ===");
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

            // Run direct ExifTool test first
            Console.WriteLine("\n=== Part 1: Direct ExifTool Test ===");
            RunDirectExifToolTest(testFile);

            // Now test our ExifToolReader
            Console.WriteLine("\n=== Part 2: ExifToolReader Integration Test ===");
            await TestExifToolReader(testFile);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void RunDirectExifToolTest(string testFile)
        {
            // Find ExifTool
            var exifToolPaths = new[]
            {
                @"..\..\Tools\exiftool.exe",
                @"..\..\..\Tools\exiftool.exe",
                @"Tools\exiftool.exe",
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools", "exiftool.exe")
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
                Console.WriteLine("❌ ExifTool not found for direct test");
                return;
            }

            Console.WriteLine($"✅ Found ExifTool: {exifToolPath}");

            // Look specifically for Barcode field
            Console.WriteLine("\nSearching for Barcode field:");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exifToolPath,
                    Arguments = $"-Barcode -UserComment \"{testFile}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            Console.WriteLine(output);
        }

        static async Task TestExifToolReader(string testFile)
        {
            try
            {
                // Create a simple console logger
                var logger = new ConsoleLogger();

                // Create ExifToolReader with 5 second timeout
                var reader = new ExifToolReader(logger, 5000);
                Console.WriteLine("✅ ExifToolReader created successfully");

                // Extract metadata
                Console.WriteLine("\nExtracting metadata...");
                var metadata = await reader.ExtractMetadataAsync(testFile);

                Console.WriteLine("\n✅ Metadata extracted successfully!");

                // Display results
                Console.WriteLine("\n--- PATIENT INFO ---");
                Console.WriteLine($"ID: {metadata.Patient.Id.Value}");
                Console.WriteLine($"Name: {metadata.Patient.Name}");
                Console.WriteLine($"Birth Date: {metadata.Patient.BirthDate?.ToString("yyyy-MM-dd") ?? "N/A"}");
                Console.WriteLine($"Gender: {metadata.Patient.Gender}");

                Console.WriteLine("\n--- STUDY INFO ---");
                Console.WriteLine($"Study ID: {metadata.Study.StudyId.Value}");
                Console.WriteLine($"Exam ID: {metadata.Study.ExamId}");
                Console.WriteLine($"Description: {metadata.Study.Description}");
                Console.WriteLine($"Modality: {metadata.Study.Modality}");
                Console.WriteLine($"Study Date: {metadata.Study.StudyDate:yyyy-MM-dd HH:mm:ss}");

                Console.WriteLine("\n--- TECHNICAL DATA ---");
                Console.WriteLine($"Manufacturer: {metadata.TechnicalData.Manufacturer}");
                Console.WriteLine($"Model: {metadata.TechnicalData.Model}");
                Console.WriteLine($"Image Size: {metadata.TechnicalData.ImageWidth}x{metadata.TechnicalData.ImageHeight}");

                Console.WriteLine("\n--- RAW DATA ---");
                Console.WriteLine($"Barcode Data: {metadata.BarcodeData ?? "N/A"}");
                Console.WriteLine($"User Comment: {metadata.UserComment ?? "N/A"}");

                Console.WriteLine("\n--- EXIF FIELDS (First 10) ---");
                int count = 0;
                foreach (var kvp in metadata.ExifData)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                    if (++count >= 10) break;
                }

                // Success!
                Console.WriteLine("\n🎉 SUCCESS! ExifToolReader is working correctly!");
                
                // Check if we found the expected data
                if (metadata.Patient.Id.Value == "EX002" && 
                    metadata.Patient.Name.Contains("Schmidt"))
                {
                    Console.WriteLine("✅ QRBridge data parsed correctly!");
                }
                else
                {
                    Console.WriteLine("⚠️ QRBridge data may not have been parsed correctly");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ ERROR: {ex.Message}");
                Console.WriteLine($"Stack trace:\n{ex.StackTrace}");
            }
        }
    }

    // Simple console logger for testing
    public class ConsoleLogger : ILogger<ExifToolReader>
    {
        public IDisposable BeginScope<TState>(TState state) => null!;
        public bool IsEnabled(LogLevel logLevel) => true;
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);
            var prefix = logLevel switch
            {
                LogLevel.Error => "❌ ERROR",
                LogLevel.Warning => "⚠️  WARN",
                LogLevel.Information => "ℹ️  INFO",
                LogLevel.Debug => "🔍 DEBUG",
                _ => "📝 LOG"
            };
            
            Console.WriteLine($"{prefix}: {message}");
            
            if (exception != null)
            {
                Console.WriteLine($"   Exception: {exception.Message}");
            }
        }
    }
}