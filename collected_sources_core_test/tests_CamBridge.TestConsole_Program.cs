using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Entities;
using CamBridge.Core.ValueObjects;
using CamBridge.Core.Interfaces;
using CamBridge.Infrastructure.Services;
using FellowOakDicom;
using FoDicomTag = FellowOakDicom.DicomTag;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;

namespace CamBridge.TestConsole
{
    /// <summary>
    /// Test console application for validating Ricoh G900 II JPEG to DICOM conversion
    /// </summary>
    [SupportedOSPlatform("windows")]
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("CamBridge Core Test - Ricoh G900 II JPEG");
            Console.WriteLine("© 2025 Claude's Improbably Reliable Software Solutions");
            Console.WriteLine("===========================================\n");

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: CamBridge.TestConsole <path-to-ricoh-jpeg>");
                Console.WriteLine("Example: CamBridge.TestConsole C:\\Test\\ricoh_test.jpg");
                return;
            }

            var inputFile = args[0];
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"ERROR: File not found: {inputFile}");
                return;
            }

            try
            {
                // Setup dependency injection
                var services = new ServiceCollection();
                ConfigureServices(services);

                using var serviceProvider = services.BuildServiceProvider();
                var testRunner = serviceProvider.GetRequiredService<RicohTestRunner>();

                await testRunner.RunTestAsync(inputFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFATAL ERROR: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void ConfigureServices(IServiceCollection services)
        {
            // Logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            // CamBridge Core Services
            services.AddSingleton<IExifReader, RicohExifReader>();
            services.AddSingleton<IDicomTagMapper, DicomTagMapper>();
            services.AddSingleton<IMappingConfiguration, CustomMappingConfiguration>(sp =>
            {
                var config = new CustomMappingConfiguration();
                // Load default mappings
                var loader = new MappingConfigurationLoader(sp.GetRequiredService<ILogger<MappingConfigurationLoader>>());
                var mappingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mappings.json");

                if (File.Exists(mappingsPath))
                {
                    var loadedConfig = loader.LoadFromFileAsync(mappingsPath).Result;
                    foreach (var rule in loadedConfig.GetMappingRules())
                    {
                        config.AddRule(rule);
                    }
                    Console.WriteLine($"Loaded {loadedConfig.GetMappingRules().Count} mapping rules from {mappingsPath}");
                }
                else
                {
                    Console.WriteLine("No mappings.json found, using default configuration");
                    return IMappingConfiguration.GetDefault() as CustomMappingConfiguration ?? new CustomMappingConfiguration();
                }

                return config;
            });

            // DicomConverter with dependencies
            services.AddSingleton<IDicomConverter, DicomConverter>();
            services.AddSingleton<IFileProcessor, FileProcessor>();

            // Settings
            services.Configure<ProcessingOptions>(options =>
            {
                options.CreateBackup = false;
                options.SuccessAction = PostProcessingAction.Leave;
                options.MinimumFileSizeBytes = 1024;
                options.MaximumFileSizeBytes = 100 * 1024 * 1024;
            });

            services.Configure<CamBridgeSettings>(settings =>
            {
                settings.UseRicohExifReader = true;
                settings.DefaultOutputFolder = Path.Combine(Environment.CurrentDirectory, "output");
                settings.Dicom.ValidateAfterCreation = true;
                settings.Dicom.InstitutionName = "Test Hospital";
            });

            // Test runner
            services.AddTransient<RicohTestRunner>();
        }
    }

    /// <summary>
    /// Main test runner for Ricoh JPEG processing
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class RicohTestRunner
    {
        private readonly ILogger<RicohTestRunner> _logger;
        private readonly IExifReader _exifReader;
        private readonly IDicomConverter _dicomConverter;
        private readonly IFileProcessor _fileProcessor;

        public RicohTestRunner(
            ILogger<RicohTestRunner> logger,
            IExifReader exifReader,
            IDicomConverter dicomConverter,
            IFileProcessor fileProcessor)
        {
            _logger = logger;
            _exifReader = exifReader;
            _dicomConverter = dicomConverter;
            _fileProcessor = fileProcessor;
        }

        public async Task RunTestAsync(string inputFile)
        {
            Console.WriteLine($"\n1. ANALYZING INPUT FILE: {Path.GetFileName(inputFile)}");
            Console.WriteLine($"   File size: {new FileInfo(inputFile).Length:N0} bytes");

            // Step 1: Read EXIF data
            Console.WriteLine("\n2. EXTRACTING EXIF DATA:");
            var exifData = await _exifReader.ReadExifDataAsync(inputFile);

            Console.WriteLine($"   Found {exifData.Count} EXIF tags");
            foreach (var kvp in exifData.Take(10))
            {
                Console.WriteLine($"   - {kvp.Key}: {kvp.Value}");
            }
            if (exifData.Count > 10)
            {
                Console.WriteLine($"   ... and {exifData.Count - 10} more tags");
            }

            // Step 2: Look for QRBridge data
            Console.WriteLine("\n3. SEARCHING FOR QRBRIDGE DATA:");
            var userComment = await _exifReader.GetUserCommentAsync(inputFile);

            if (string.IsNullOrEmpty(userComment))
            {
                Console.WriteLine("   ❌ NO QRBridge data found in User Comment!");
                Console.WriteLine("   Checking for Barcode field...");

                if (exifData.TryGetValue("Barcode", out var barcode))
                {
                    Console.WriteLine($"   ✓ Found in Barcode field: {barcode}");
                    userComment = barcode;
                }
                else
                {
                    Console.WriteLine("   ❌ No Barcode field found either!");
                }
            }
            else
            {
                Console.WriteLine($"   ✓ Found User Comment: {userComment}");

                // Debug output - show hex values
                Console.WriteLine("\n   DEBUG - Hex dump of User Comment:");
                var bytes = Encoding.UTF8.GetBytes(userComment);
                for (int i = 0; i < Math.Min(bytes.Length, 100); i += 16)
                {
                    var hex = string.Join(" ", bytes.Skip(i).Take(16).Select(b => b.ToString("X2")));
                    var ascii = string.Join("", bytes.Skip(i).Take(16).Select(b =>
                        (b >= 32 && b <= 126) ? (char)b : '.'));
                    Console.WriteLine($"   {i:D4}: {hex,-48} {ascii}");
                }

                // Check for line breaks or special characters
                if (userComment.Contains('\r') || userComment.Contains('\n'))
                {
                    Console.WriteLine("\n   ⚠️ Found line breaks in data!");
                    Console.WriteLine($"   CR (\\r) count: {userComment.Count(c => c == '\r')}");
                    Console.WriteLine($"   LF (\\n) count: {userComment.Count(c => c == '\n')}");
                }

                // Check for encoding issues
                if (userComment.Contains('�') || userComment.Contains('\ufffd'))
                {
                    Console.WriteLine("\n   ⚠️ Found encoding issues (� characters)!");
                    Console.WriteLine("   This usually means UTF-8 data was interpreted as Latin-1");
                }
            }

            // Step 3: Parse QRBridge data
            if (!string.IsNullOrEmpty(userComment))
            {
                Console.WriteLine("\n4. PARSING QRBRIDGE DATA:");
                var qrBridgeData = _exifReader.ParseQRBridgeData(userComment);

                if (qrBridgeData.Count > 0)
                {
                    Console.WriteLine("   Parsed fields:");
                    foreach (var field in qrBridgeData)
                    {
                        Console.WriteLine($"   - {field.Key}: {field.Value}");
                    }
                }
                else
                {
                    Console.WriteLine("   ❌ Failed to parse QRBridge data!");
                }
            }

            // Step 4: Process file through full pipeline
            Console.WriteLine("\n5. PROCESSING THROUGH FULL PIPELINE:");
            var outputDir = Path.Combine(Environment.CurrentDirectory, "output");
            Directory.CreateDirectory(outputDir);

            var result = await _fileProcessor.ProcessFileAsync(inputFile);

            if (result.Success && !string.IsNullOrEmpty(result.OutputFile))
            {
                Console.WriteLine($"   ✓ SUCCESS! Output: {result.OutputFile}");
                Console.WriteLine($"   Processing time: {result.ProcessingTime.TotalMilliseconds:F0}ms");

                // Step 5: Analyze generated DICOM
                await AnalyzeDicomFileAsync(result.OutputFile);
            }
            else
            {
                Console.WriteLine($"   ❌ FAILED: {result.ErrorMessage}");
            }

            // Step 6: Direct converter test (for debugging)
            if (!result.Success)
            {
                Console.WriteLine("\n6. ATTEMPTING DIRECT CONVERSION (DEBUG MODE):");
                await TestDirectConversionAsync(inputFile);
            }
        }

        private async Task AnalyzeDicomFileAsync(string dicomPath)
        {
            Console.WriteLine("\n6. ANALYZING GENERATED DICOM:");

            try
            {
                var dicomFile = await DicomFile.OpenAsync(dicomPath);
                var dataset = dicomFile.Dataset;

                Console.WriteLine("   Patient Information:");
                Console.WriteLine($"   - Name: {dataset.GetString(FoDicomTag.PatientName)}");
                Console.WriteLine($"   - ID: {dataset.GetString(FoDicomTag.PatientID)}");
                Console.WriteLine($"   - Birth Date: {dataset.GetString(FoDicomTag.PatientBirthDate)}");
                Console.WriteLine($"   - Sex: {dataset.GetString(FoDicomTag.PatientSex)}");

                Console.WriteLine("\n   Study Information:");
                Console.WriteLine($"   - Study ID: {dataset.GetString(FoDicomTag.StudyID)}");
                Console.WriteLine($"   - Study Date: {dataset.GetString(FoDicomTag.StudyDate)}");
                Console.WriteLine($"   - Study Description: {dataset.GetString(FoDicomTag.StudyDescription)}");
                Console.WriteLine($"   - Modality: {dataset.GetString(FoDicomTag.Modality)}");

                Console.WriteLine("\n   Technical Information:");
                Console.WriteLine($"   - Manufacturer: {dataset.GetString(FoDicomTag.Manufacturer)}");
                Console.WriteLine($"   - Model: {dataset.GetString(FoDicomTag.ManufacturerModelName)}");
                Console.WriteLine($"   - Transfer Syntax: {dicomFile.FileMetaInfo.TransferSyntax}");
                Console.WriteLine($"   - SOP Class: {dataset.GetString(FoDicomTag.SOPClassUID)}");

                // Check for pixel data
                if (dataset.Contains(FoDicomTag.PixelData))
                {
                    var pixelData = dataset.GetDicomItem<DicomOtherByteFragment>(FoDicomTag.PixelData);
                    Console.WriteLine($"\n   Pixel Data: ✓ Present ({pixelData.Fragments.Count} fragments)");
                }
                else
                {
                    Console.WriteLine("\n   Pixel Data: ❌ MISSING!");
                }

                // Validate
                var validationResult = await _dicomConverter.ValidateDicomFileAsync(dicomPath);
                Console.WriteLine($"\n   Validation: {(validationResult.IsValid ? "✓ PASSED" : "❌ FAILED")}");

                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        Console.WriteLine($"   - ERROR: {error}");
                    }
                }

                foreach (var warning in validationResult.Warnings)
                {
                    Console.WriteLine($"   - WARNING: {warning}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Error analyzing DICOM: {ex.Message}");
            }
        }

        private async Task TestDirectConversionAsync(string inputFile)
        {
            try
            {
                // Create minimal metadata for testing
                var metadata = new ImageMetadata(
                    inputFile,
                    DateTime.Now,
                    new PatientInfo(
                        new PatientId("TEST001"),
                        "Debug, Test",
                        null,
                        Gender.Other),
                    new StudyInfo(
                        new StudyId("STUDY001"),
                        DateTime.Now,
                        "XC"),
                    new Dictionary<string, string>(),
                    new ImageTechnicalData(),
                    1);

                var outputPath = Path.Combine(Environment.CurrentDirectory, "output", "debug_test.dcm");
                var result = await _dicomConverter.ConvertToDicomAsync(inputFile, outputPath, metadata);

                if (result.Success)
                {
                    Console.WriteLine($"   ✓ Direct conversion succeeded: {result.DicomFilePath}");
                    if (!string.IsNullOrEmpty(result.DicomFilePath))
                    {
                        await AnalyzeDicomFileAsync(result.DicomFilePath);
                    }
                }
                else
                {
                    Console.WriteLine($"   ❌ Direct conversion failed: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Exception during direct conversion: {ex.Message}");
            }
        }
    }
}
