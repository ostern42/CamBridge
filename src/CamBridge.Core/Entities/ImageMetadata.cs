using System;
using System.Collections.Generic;

namespace CamBridge.Core.Entities
{
    /// <summary>
    /// Complete metadata extracted from a medical image file
    /// </summary>
    public class ImageMetadata
    {
        public string SourceFilePath { get; }
        public DateTime CaptureDateTime { get; }
        public PatientInfo Patient { get; }
        public StudyInfo Study { get; }
        public ImageTechnicalData TechnicalData { get; }
        public string? UserComment { get; }
        public string? BarcodeData { get; }
        public int InstanceNumber { get; }
        public string InstanceUid { get; }
        public Dictionary<string, string> ExifData { get; }

        public ImageMetadata(
            string sourceFilePath,
            DateTime captureDateTime,
            PatientInfo patient,
            StudyInfo study,
            ImageTechnicalData technicalData,
            string? userComment = null,
            string? barcodeData = null,
            int instanceNumber = 1,
            string? instanceUid = null,
            Dictionary<string, string>? exifData = null)
        {
            SourceFilePath = sourceFilePath ?? throw new ArgumentNullException(nameof(sourceFilePath));
            CaptureDateTime = captureDateTime;
            Patient = patient ?? throw new ArgumentNullException(nameof(patient));
            Study = study ?? throw new ArgumentNullException(nameof(study));
            TechnicalData = technicalData ?? throw new ArgumentNullException(nameof(technicalData));
            UserComment = userComment;
            BarcodeData = barcodeData;
            InstanceNumber = instanceNumber;
            InstanceUid = instanceUid ?? GenerateUid();
            ExifData = exifData ?? new Dictionary<string, string>();
        }

        private static string GenerateUid()
        {
            // Generate a DICOM compliant UID
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"1.2.276.0.7230010.3.1.4.{timestamp}.{random}";
        }
    }
}
