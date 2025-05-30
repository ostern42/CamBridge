using System;
using System.Collections.Generic;

namespace CamBridge.Core.Entities
{
    /// <summary>
    /// Represents comprehensive metadata for an image file
    /// </summary>
    public class ImageMetadata
    {
        public string SourceFilePath { get; }
        public DateTime CaptureDateTime { get; }
        public PatientInfo Patient { get; }
        public StudyInfo Study { get; }
        public Dictionary<string, string> ExifData { get; }
        public ImageTechnicalData TechnicalData { get; }
        public string InstanceUid { get; }
        public int InstanceNumber { get; }

        public ImageMetadata(
            string sourceFilePath,
            DateTime captureDateTime,
            PatientInfo patient,
            StudyInfo study,
            Dictionary<string, string> exifData,
            ImageTechnicalData technicalData,
            int instanceNumber = 1)
        {
            SourceFilePath = sourceFilePath ?? throw new ArgumentNullException(nameof(sourceFilePath));
            CaptureDateTime = captureDateTime;
            Patient = patient ?? throw new ArgumentNullException(nameof(patient));
            Study = study ?? throw new ArgumentNullException(nameof(study));
            ExifData = exifData ?? new Dictionary<string, string>();
            TechnicalData = technicalData ?? throw new ArgumentNullException(nameof(technicalData));
            InstanceNumber = instanceNumber;

            // Generate unique instance UID
            InstanceUid = GenerateInstanceUid();
        }

        private string GenerateInstanceUid()
        {
            // Simple UID generation (in production, use proper DICOM UID generation)
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var random = new Random().Next(1000, 9999);
            return $"1.2.276.0.7230010.3.1.4.{timestamp}.{random}";
        }
    }

    /// <summary>
    /// Technical data extracted from image EXIF
    /// </summary>
    public class ImageTechnicalData
    {
        public string? Manufacturer { get; init; }
        public string? Model { get; init; }
        public string? Software { get; init; }
        public int? ImageWidth { get; init; }
        public int? ImageHeight { get; init; }
        public double? ExposureTime { get; init; }
        public double? FNumber { get; init; }
        public int? IsoSpeed { get; init; }
        public DateTime? DateTimeOriginal { get; init; }
        public string? Flash { get; init; }
        public double? FocalLength { get; init; }

        /// <summary>
        /// Creates technical data from EXIF dictionary
        /// </summary>
        public static ImageTechnicalData FromExifDictionary(Dictionary<string, string> exif)
        {
            return new ImageTechnicalData
            {
                Manufacturer = exif.GetValueOrDefault("Make"),
                Model = exif.GetValueOrDefault("Model"),
                Software = exif.GetValueOrDefault("Software"),
                ImageWidth = ParseInt(exif.GetValueOrDefault("ImageWidth")),
                ImageHeight = ParseInt(exif.GetValueOrDefault("ImageHeight")),
                ExposureTime = ParseDouble(exif.GetValueOrDefault("ExposureTime")),
                FNumber = ParseDouble(exif.GetValueOrDefault("FNumber")),
                IsoSpeed = ParseInt(exif.GetValueOrDefault("ISOSpeedRatings")),
                DateTimeOriginal = ParseDateTime(exif.GetValueOrDefault("DateTimeOriginal")),
                Flash = exif.GetValueOrDefault("Flash"),
                FocalLength = ParseDouble(exif.GetValueOrDefault("FocalLength"))
            };
        }

        private static int? ParseInt(string? value)
        {
            return int.TryParse(value, out var result) ? result : null;
        }

        private static double? ParseDouble(string? value)
        {
            return double.TryParse(value, out var result) ? result : null;
        }

        private static DateTime? ParseDateTime(string? value)
        {
            // EXIF datetime format: "yyyy:MM:dd HH:mm:ss"
            if (string.IsNullOrEmpty(value)) return null;

            var formats = new[] {
                "yyyy:MM:dd HH:mm:ss",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy/MM/dd HH:mm:ss"
            };

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(value, format, null,
                    System.Globalization.DateTimeStyles.None, out var result))
                {
                    return result;
                }
            }

            return null;
        }
    }
}