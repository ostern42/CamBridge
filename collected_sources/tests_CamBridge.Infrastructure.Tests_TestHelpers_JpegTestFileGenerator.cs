using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace CamBridge.Infrastructure.Tests.TestHelpers
{
    /// <summary>
    /// Helper class to generate test JPEG files with EXIF data
    /// </summary>
    public static class JpegTestFileGenerator
    {
        /// <summary>
        /// Creates a test JPEG file with embedded EXIF user comment
        /// </summary>
        public static string CreateTestJpegWithQRBridgeData(
            string outputPath,
            string examId = "EX002",
            string patientName = "Schmidt, Maria",
            string birthDate = "1985-03-15",
            string gender = "F",
            string comment = "Röntgen Thorax")
        {
            // Create QRBridge formatted data
            var qrBridgeData = $"{examId}|{patientName}|{birthDate}|{gender}|{comment}";

            // Create a simple test image
            using var bitmap = new Bitmap(800, 600);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                graphics.DrawString("Test Medical Image",
                    new Font("Arial", 24),
                    Brushes.Black,
                    new PointF(250, 250));

                // Add some test pattern
                for (int i = 0; i < 10; i++)
                {
                    graphics.DrawRectangle(Pens.Gray,
                        50 + i * 20,
                        50 + i * 20,
                        100,
                        100);
                }
            }

            // Save with EXIF data
            var encoderParameters = new EncoderParameters(1);
            // Fix: Explicitly use System.Drawing.Imaging.Encoder
            encoderParameters.Param[0] = new EncoderParameter(
                System.Drawing.Imaging.Encoder.Quality,
                90L);

            // Get JPEG codec
            var jpegCodec = GetEncoder(ImageFormat.Jpeg);

            // Create property items for EXIF data
            var propertyItems = new[]
            {
                CreatePropertyItem(0x010F, "RICOH"), // Make
                CreatePropertyItem(0x0110, "G900 II"), // Model
                CreatePropertyItem(0x0131, "CamBridge Test"), // Software
                CreatePropertyItem(0x0132, DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss")), // DateTime
                CreatePropertyItem(0x9003, DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss")), // DateTimeOriginal
                CreatePropertyItem(0x9286, qrBridgeData) // UserComment
            };

            // Add properties to bitmap
            foreach (var prop in propertyItems)
            {
                bitmap.SetPropertyItem(prop);
            }

            // Ensure directory exists
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            // Save the image
            bitmap.Save(outputPath, jpegCodec, encoderParameters);

            return outputPath;
        }

        /// <summary>
        /// Creates a test JPEG without QRBridge data
        /// </summary>
        public static string CreateTestJpegWithoutQRBridgeData(string outputPath)
        {
            using var bitmap = new Bitmap(800, 600);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.LightGray);
                graphics.DrawString("Test Image - No Patient Data",
                    new Font("Arial", 20),
                    Brushes.Red,
                    new PointF(200, 280));
            }

            // Basic EXIF data only
            var propertyItems = new[]
            {
                CreatePropertyItem(0x010F, "Generic"), // Make
                CreatePropertyItem(0x0110, "Camera"), // Model
                CreatePropertyItem(0x0132, DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss")) // DateTime
            };

            foreach (var prop in propertyItems)
            {
                bitmap.SetPropertyItem(prop);
            }

            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            bitmap.Save(outputPath, ImageFormat.Jpeg);
            return outputPath;
        }

        /// <summary>
        /// Creates a batch of test images with sequential data
        /// </summary>
        public static string[] CreateBatchTestImages(string outputDirectory, int count)
        {
            var paths = new string[count];

            for (int i = 0; i < count; i++)
            {
                var examId = $"EX{(i + 1):D3}";
                var patientName = $"Test{i + 1}, Patient";
                var birthDate = new DateTime(1980 + i % 40, (i % 12) + 1, (i % 28) + 1).ToString("yyyy-MM-dd");
                var gender = i % 2 == 0 ? "M" : "F";
                var comment = $"Test procedure {i + 1}";

                var filename = Path.Combine(outputDirectory, $"test_{examId}.jpg");
                paths[i] = CreateTestJpegWithQRBridgeData(
                    filename, examId, patientName, birthDate, gender, comment);
            }

            return paths;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                    return codec;
            }
            throw new InvalidOperationException($"Encoder for {format} not found");
        }

        private static PropertyItem CreatePropertyItem(int id, string value)
        {
            // Create a dummy image to get a PropertyItem template
            using var dummy = new Bitmap(1, 1);
            var prop = dummy.PropertyItems.Length > 0
                ? dummy.PropertyItems[0]
                : (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true)!;

            prop.Id = id;
            prop.Type = 2; // ASCII
            prop.Value = Encoding.UTF8.GetBytes(value + '\0');
            prop.Len = prop.Value.Length;

            return prop;
        }

        /// <summary>
        /// Cleans up test files from a directory
        /// </summary>
        public static void CleanupTestFiles(string directory)
        {
            if (Directory.Exists(directory))
            {
                try
                {
                    var files = Directory.GetFiles(directory, "*.jpg", SearchOption.AllDirectories)
                        .Concat(Directory.GetFiles(directory, "*.dcm", SearchOption.AllDirectories));

                    foreach (var file in files)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch
                        {
                            // Ignore individual file deletion errors
                        }
                    }
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
    }
}
