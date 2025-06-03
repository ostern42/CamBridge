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

            // CamBridge Core Services - ExifTool only!
            services.AddSingleton<ExifToolReader>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ExifToolReader>>();
                return new ExifToolReader(logger);
            });

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
        private readonly ExifToolReader _exifToolReader;  // Direct dependency!
        private readonly IDicomConverter _dicomConverter;
        private readonly IFileProcessor _fileProcessor;

        public RicohTestRunner(
            ILogger<RicohTestRunner> logger,
            ExifToolReader exifToolReader,  // Direct dependency!
            IDicomConverter dicomConverter,
            IFileProcessor fileProcessor)
        {
            _logger = logger;
            _exifToolReader = exifToolReader;
            _dicomConverter = dicomConverter;
            _fileProcessor = fileProcessor;
        }

        public async Task RunTestAsync(string inputFile)
        {
            Console.WriteLine($"\n1. ANALYZING INPUT FILE: {Path.GetFileName(inputFile)}");
            Console.WriteLine($"   File size: {new FileInfo(inputFile).Length:N0} bytes");

            // Step 1: Extract metadata using ExifToolReader
            Console.WriteLine("\n2. EXTRACTING METADATA WITH EXIFTOOL:");
            try
            {
                var metadata = await _exifToolReader.ExtractMetadataAsync(inputFile);

                Console.WriteLine($"   ✓ Successfully extracted metadata");
                Console.WriteLine($"   Patient: {metadata.Patient.Name} (ID: {metadata.Patient.PatientId})");
                Console.WriteLine($"   Study: {metadata.Study.StudyId} - {metadata.Study.StudyDescription}");
                Console.WriteLine($"   Capture Date: {metadata.CaptureDateTime}");
                Console.WriteLine($"   EXIF Tags found: {metadata.ExifData.Count}");

                // Show some key EXIF data
                if (metadata.ExifData.TryGetValue("Barcode", out var barcode))
                {
                    Console.WriteLine($"   Barcode field: {barcode}");
                }
                if (metadata.ExifData.TryGetValue("UserComment", out var userComment))
                {
                    Console.WriteLine($"   UserComment field: {userComment}");
                }

                // Show technical data
                if (metadata.TechnicalData != null)
                {
                    Console.WriteLine($"\n   Technical Data:");
                    Console.WriteLine($"   - Camera: {metadata.TechnicalData.Manufacturer} {metadata.TechnicalData.Model}");
                    Console.WriteLine($"   - Dimensions: {metadata.TechnicalData.ImageWidth}x{metadata.TechnicalData.ImageHeight}");
                    Console.WriteLine($"   - Software: {metadata.TechnicalData.Software}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Failed to extract metadata: {ex.Message}");
                return; // Can't continue without metadata
            }

            // Step 2: Process through full pipeline
            Console.WriteLine("\n3. PROCESSING THROUGH FULL PIPELINE:");
            var outputDir = Path.Combine(Environment.CurrentDirectory, "output");
            Directory.CreateDirectory(outputDir);

            var result = await _fileProcessor.ProcessFileAsync(inputFile);

            if (result.Success && !string.IsNullOrEmpty(result.OutputFile))
            {
                Console.WriteLine($"   ✓ SUCCESS! Output: {result.OutputFile}");
                Console.WriteLine($"   Processing time: {result.ProcessingTime.TotalMilliseconds:F0}ms");

                // Step 3: Analyze generated DICOM
                await AnalyzeDicomFileAsync(result.OutputFile);
            }
            else
            {
                Console.WriteLine($"   ❌ FAILED: {result.ErrorMessage}");

                // Try direct conversion for debugging
                Console.WriteLine("\n4. ATTEMPTING DIRECT CONVERSION (DEBUG MODE):");
                await TestDirectConversionAsync(inputFile);
            }
        }

        private async Task AnalyzeDicomFileAsync(string dicomPath)
        {
            Console.WriteLine("\n4. ANALYZING GENERATED DICOM:");

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
                // Get metadata from ExifToolReader
                var metadata = await _exifToolReader.ExtractMetadataAsync(inputFile);

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
