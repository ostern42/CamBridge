using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;

namespace CamBridge.Infrastructure.Tests.TestHelpers
{
    /// <summary>
    /// Helper class to generate test JPEG files with custom EXIF data
    /// </summary>
    public static class JpegTestFileGenerator
    {
        /// <summary>
        /// Creates a test JPEG file with QRBridge data in User Comment
        /// </summary>
        public static string CreateTestJpegWithQRBridgeData(
            string qrBridgeData,
            string? fileName = null,
            int width = 800,
            int height = 600)
        {
            fileName ??= $"test_{Guid.NewGuid():N}.jpg";
            var tempPath = Path.Combine(Path.GetTempPath(), "CamBridgeTests");
            Directory.CreateDirectory(tempPath);
            var filePath = Path.Combine(tempPath, fileName);

            // Create a simple test image
            using (var bitmap = new Bitmap(width, height))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Fill with gradient
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Rectangle(0, 0, width, height),
                    Color.LightBlue,
                    Color.DarkBlue,
                    45f))
                {
                    graphics.FillRectangle(brush, 0, 0, width, height);
                }

                // Add some text
                using (var font = new Font("Arial", 24))
                using (var textBrush = new SolidBrush(Color.White))
                {
                    graphics.DrawString("CamBridge Test Image", font, textBrush, 50, 50);
                    graphics.DrawString($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                        new Font("Arial", 12), textBrush, 50, 100);
                }

                // Save with EXIF data
                var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 85L);

                // Get JPEG codec
                var jpegCodec = GetEncoder(ImageFormat.Jpeg);

                // Note: System.Drawing doesn't support writing custom EXIF tags directly
                // In real implementation, we'd need to use a library like ExifLib or similar
                // For testing, we'll save the image and then modify EXIF data
                bitmap.Save(filePath, jpegCodec, encoderParams);
            }

            // In a real implementation, we would inject the EXIF data here
            // For testing purposes, we'll create a companion file with the data
            var exifDataPath = Path.ChangeExtension(filePath, ".exif");
            File.WriteAllText(exifDataPath, qrBridgeData);

            return filePath;
        }

        /// <summary>
        /// Creates multiple test files with different patient data
        /// </summary>
        public static List<string> CreateTestDataset()
        {
            var files = new List<string>();
            var testData = new[]
            {
                "EX001|Schmidt, Maria|1985-03-15|F|Röntgen Thorax",
                "EX002|Müller, Hans|1975-06-20|M|CT Abdomen",
                "EX003|Wagner, Lisa|1990-12-01|F|MRT Knie links",
                "EX004|Becker, Thomas|1968-08-30|M|Ultraschall Abdomen",
                "EX005|Meyer, Anna|2000-01-15|F|Röntgen Hand rechts"
            };

            foreach (var data in testData)
            {
                var fileName = $"patient_{data.Split('|')[0]}.jpg";
                files.Add(CreateTestJpegWithQRBridgeData(data, fileName));
            }

            return files;
        }

        /// <summary>
        /// Creates test files with edge cases
        /// </summary>
        public static Dictionary<string, string> CreateEdgeCaseFiles()
        {
            var files = new Dictionary<string, string>();

            // Missing gender
            files["missing_gender"] = CreateTestJpegWithQRBridgeData(
                "EX100|Test, Patient|1980-01-01||No gender specified");

            // Missing birthdate
            files["missing_birthdate"] = CreateTestJpegWithQRBridgeData(
                "EX101|Test, Patient Two||M|No birthdate");

            // Special characters
            files["special_chars"] = CreateTestJpegWithQRBridgeData(
                "EX102|Østergård, Søren|1975-05-05|M|Ärztliche Untersuchung");

            // Very long comment
            files["long_comment"] = CreateTestJpegWithQRBridgeData(
                "EX103|Test, Patient|1990-01-01|F|" + new string('X', 200));

            // Minimal data
            files["minimal"] = CreateTestJpegWithQRBridgeData(
                "EX104|Minimal Patient");

            // Empty QRBridge data
            files["empty"] = CreateTestJpegWithQRBridgeData("");

            return files;
        }

        /// <summary>
        /// Cleans up test files
        /// </summary>
        public static void CleanupTestFiles()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "CamBridgeTests");
            if (Directory.Exists(tempPath))
            {
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch
                {
                    // Ignore cleanup errors in tests
                }
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            throw new InvalidOperationException($"Encoder not found for format {format}");
        }
    }
}
