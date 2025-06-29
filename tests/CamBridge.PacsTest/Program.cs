// tests\CamBridge.PacsTest\Program.cs
// Version: 0.8.2
// Description: Mini test program to verify fo-dicom PACS communication
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.IO;
using System.Threading.Tasks;
using FellowOakDicom;
using FellowOakDicom.Network;
using FellowOakDicom.Network.Client;
using FellowOakDicom.Imaging;
using FellowOakDicom.IO.Buffer;

namespace CamBridge.PacsTest
{
    class Program
    {
        // Test PACS configuration - adjust for your Orthanc instance
        private const string PACS_HOST = "127.0.0.1";
        private const int PACS_PORT = 4242;
        private const string CALLED_AE = "ORTHANC";
        private const string CALLING_AE = "CAMBRIDGE_TEST";

        static async Task Main(string[] args)
        {
            Console.WriteLine("CamBridge PACS Test Program v0.8.2");
            Console.WriteLine("==================================");
            Console.WriteLine($"Testing connection to {PACS_HOST}:{PACS_PORT}");
            Console.WriteLine($"AE Titles: {CALLING_AE} → {CALLED_AE}");
            Console.WriteLine();

            // fo-dicom 5.2.2 uses Microsoft.Extensions.Logging
            // No need to setup logging manually

            // Test 1: C-ECHO (Connection Test)
            Console.WriteLine("Test 1: C-ECHO (Connection Test)");
            Console.WriteLine("---------------------------------");
            var echoResult = await TestCEchoAsync();
            Console.WriteLine($"C-ECHO Result: {(echoResult ? "SUCCESS ✓" : "FAILED ✗")}");
            Console.WriteLine();

            if (!echoResult)
            {
                Console.WriteLine("C-ECHO failed, skipping C-STORE test");
                Console.WriteLine("Please verify Orthanc is running:");
                Console.WriteLine("  docker run -p 4242:4242 -p 104:104 jodogne/orthanc");
                return;
            }

            // Test 2: C-STORE (File Upload)
            Console.WriteLine("Test 2: C-STORE (File Upload)");
            Console.WriteLine("------------------------------");

            // Check if test DICOM file exists
            var testDicomPath = "test.dcm";
            if (!File.Exists(testDicomPath))
            {
                Console.WriteLine($"Test DICOM file not found: {testDicomPath}");
                Console.WriteLine("Creating a simple test DICOM file...");
                CreateTestDicomFile(testDicomPath);
            }

            var storeResult = await TestCStoreAsync(testDicomPath);
            Console.WriteLine($"C-STORE Result: {(storeResult ? "SUCCESS ✓" : "FAILED ✗")}");

            Console.WriteLine();
            Console.WriteLine("Test completed. Press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Test C-ECHO (connection test)
        /// </summary>
        private static async Task<bool> TestCEchoAsync()
        {
            try
            {
                var client = DicomClientFactory.Create(PACS_HOST, PACS_PORT, false, CALLING_AE, CALLED_AE);
                client.NegotiateAsyncOps();

                // Note: Timeout setting has changed in fo-dicom 5.x
                // client.Options.RequestTimeout is no longer available

                DicomCEchoResponse? response = null;
                var request = new DicomCEchoRequest
                {
                    OnResponseReceived = (req, res) =>
                    {
                        response = res;
                        Console.WriteLine($"C-ECHO Response Status: {res.Status}");
                    }
                };

                await client.AddRequestAsync(request);
                await client.SendAsync();

                return response?.Status == DicomStatus.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"C-ECHO Exception: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Test C-STORE (file upload)
        /// </summary>
        private static async Task<bool> TestCStoreAsync(string dicomFilePath)
        {
            try
            {
                // Load DICOM file
                var dicomFile = await DicomFile.OpenAsync(dicomFilePath);
                Console.WriteLine($"Loaded DICOM file: {dicomFilePath}");
                Console.WriteLine($"  SOP Instance UID: {dicomFile.Dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID)}");
                Console.WriteLine($"  Patient Name: {dicomFile.Dataset.GetSingleValueOrDefault<string>(DicomTag.PatientName, "Unknown")}");

                var client = DicomClientFactory.Create(PACS_HOST, PACS_PORT, false, CALLING_AE, CALLED_AE);
                client.NegotiateAsyncOps();

                // Note: Timeout setting has changed in fo-dicom 5.x

                DicomCStoreResponse? response = null;
                var request = new DicomCStoreRequest(dicomFile)
                {
                    OnResponseReceived = (req, res) =>
                    {
                        response = res;
                        Console.WriteLine($"C-STORE Response Status: {res.Status}");
                        if (res.Status != DicomStatus.Success)
                        {
                            Console.WriteLine($"  Status Description: {res.Status.Description}");
                            Console.WriteLine($"  Status Code: {res.Status.Code:X4}");
                        }
                    }
                };

                await client.AddRequestAsync(request);
                await client.SendAsync();

                return response?.Status == DicomStatus.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"C-STORE Exception: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Create a simple test DICOM file
        /// </summary>
        private static void CreateTestDicomFile(string path)
        {
            var dataset = new DicomDataset();

            // Patient Module
            dataset.Add(DicomTag.PatientName, "Test^Patient");
            dataset.Add(DicomTag.PatientID, "TEST001");
            dataset.Add(DicomTag.PatientBirthDate, "20000101");
            dataset.Add(DicomTag.PatientSex, "O");

            // Study Module
            dataset.Add(DicomTag.StudyInstanceUID, DicomUID.Generate());
            dataset.Add(DicomTag.StudyDate, DateTime.Now.ToString("yyyyMMdd"));
            dataset.Add(DicomTag.StudyTime, DateTime.Now.ToString("HHmmss"));
            dataset.Add(DicomTag.StudyID, "TEST_STUDY_001");

            // Series Module
            dataset.Add(DicomTag.SeriesInstanceUID, DicomUID.Generate());
            dataset.Add(DicomTag.SeriesNumber, "1");
            dataset.Add(DicomTag.Modality, "OT");

            // SOP Common Module
            dataset.Add(DicomTag.SOPClassUID, "1.2.840.10008.5.1.4.1.1.7"); // Secondary Capture
            dataset.Add(DicomTag.SOPInstanceUID, DicomUID.Generate());

            // Create a simple 100x100 white image
            dataset.Add(DicomTag.SamplesPerPixel, (ushort)1);
            dataset.Add(DicomTag.PhotometricInterpretation, "MONOCHROME2");
            dataset.Add(DicomTag.Rows, (ushort)100);
            dataset.Add(DicomTag.Columns, (ushort)100);
            dataset.Add(DicomTag.BitsAllocated, (ushort)8);
            dataset.Add(DicomTag.BitsStored, (ushort)8);
            dataset.Add(DicomTag.HighBit, (ushort)7);
            dataset.Add(DicomTag.PixelRepresentation, (ushort)0);

            var pixelData = DicomPixelData.Create(dataset, true);
            var buffer = new byte[100 * 100];
            Array.Fill<byte>(buffer, 255); // White pixels
            pixelData.AddFrame(new MemoryByteBuffer(buffer));

            var file = new DicomFile(dataset);
            file.Save(path);

            Console.WriteLine($"Created test DICOM file: {path}");
        }
    }
}
