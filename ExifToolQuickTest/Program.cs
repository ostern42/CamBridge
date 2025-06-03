using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ExifToolQuickTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== CamBridge ExifTool Direct Test ===");
            Console.WriteLine();

            var exifToolPath = @"C:\Users\aiadmin\source\repos\CamBridge\Tools\exiftool.exe";
            if (!File.Exists(exifToolPath))
            {
                Console.WriteLine("ERROR: ExifTool not found!");
                return;
            }
            
            var imagePath = args.Length > 0 ? args[0] : @"C:\Users\aiadmin\source\repos\CamBridge\R0010168.JPG";
            
            Console.WriteLine("Reading EXIF from: " + imagePath);
            
            // Get all tags as JSON
            var jsonOutput = await RunExifToolCommand(exifToolPath, "-j -a -G -s \"" + imagePath + "\"");
            
            // Save for debugging
            File.WriteAllText("exif_output.json", jsonOutput);
            Console.WriteLine("Saved full output to exif_output.json");
            
            // Look for Barcode field
            Console.WriteLine("\n=== SEARCHING FOR BARCODE ===");
            
            // Try different possible field names
            var barcodeFields = new[] { 
                "\"Barcode\":", 
                "\"MakerNotesPentax:Barcode\":", 
                "\"Pentax:Barcode\":",
                "\"MakerNotes:Barcode\":"
            };
            
            string barcodeValue = null;
            foreach (var field in barcodeFields)
            {
                if (jsonOutput.Contains(field))
                {
                    Console.WriteLine("FOUND: " + field);
                    
                    // Extract value
                    var startIdx = jsonOutput.IndexOf(field);
                    var valueStart = jsonOutput.IndexOf("\"", startIdx + field.Length) + 1;
                    var valueEnd = jsonOutput.IndexOf("\"", valueStart);
                    
                    if (valueStart > 0 && valueEnd > valueStart)
                    {
                        barcodeValue = jsonOutput.Substring(valueStart, valueEnd - valueStart);
                        Console.WriteLine("VALUE: " + barcodeValue);
                        
                        // Parse pipe-delimited data
                        var parts = barcodeValue.Split('|');
                        Console.WriteLine("\nPARSED DATA:");
                        if (parts.Length >= 1) Console.WriteLine("  Exam ID: " + parts[0]);
                        if (parts.Length >= 2) Console.WriteLine("  Name: " + parts[1]);
                        if (parts.Length >= 3) Console.WriteLine("  Birth Date: " + parts[2]);
                        if (parts.Length >= 4) Console.WriteLine("  Gender: " + parts[3]);
                        if (parts.Length >= 5) Console.WriteLine("  Comment: " + parts[4]);
                    }
                    break;
                }
            }
            
            if (barcodeValue == null)
            {
                Console.WriteLine("NO BARCODE FIELD FOUND!");
                Console.WriteLine("\nSearching for any field with 'barcode' (case insensitive)...");
                
                var lines = jsonOutput.Split('\n');
                foreach (var line in lines)
                {
                    if (line.ToLower().Contains("barcode"))
                    {
                        Console.WriteLine("Found line: " + line.Trim());
                    }
                }
            }
            
            // Also check UserComment
            if (jsonOutput.Contains("\"UserComment\":"))
            {
                var ucStart = jsonOutput.IndexOf("\"UserComment\":") + 15;
                var ucValueStart = jsonOutput.IndexOf("\"", ucStart) + 1;
                var ucValueEnd = jsonOutput.IndexOf("\"", ucValueStart);
                var userComment = jsonOutput.Substring(ucValueStart, ucValueEnd - ucValueStart);
                Console.WriteLine("\nUserComment: " + userComment);
            }
            
            Console.WriteLine("\nDone! Check exif_output.json for full data.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        
        static async Task<string> RunExifToolCommand(string exifToolPath, string arguments)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exifToolPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                }
            };
            
            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            await Task.Run(() => process.WaitForExit());
            return output.Trim();
        }
    }
}
