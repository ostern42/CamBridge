using System;
using System.Collections.Generic;

namespace CamBridge.Core.Entities
{
    /// <summary>
    /// Technical metadata extracted from image EXIF data
    /// </summary>
    public class ImageTechnicalData
    {
        public string? Manufacturer { get; init; }
        public string? Model { get; init; }
        public string? Software { get; init; }
        public int? ImageWidth { get; init; }
        public int? ImageHeight { get; init; }
        public string? ColorSpace { get; init; }
        public int? BitsPerSample { get; init; }
        public string? Compression { get; init; }
        public int? Orientation { get; init; }

        /// <summary>
        /// Creates ImageTechnicalData from raw EXIF dictionary
        /// </summary>
        public static ImageTechnicalData FromExifDictionary(Dictionary<string, string> exifData)
        {
            return new ImageTechnicalData
            {
                Manufacturer = GetValue(exifData, "Make", "Manufacturer"),
                Model = GetValue(exifData, "Model", "CameraModel"),
                Software = GetValue(exifData, "Software"),
                ImageWidth = GetIntValue(exifData, "ImageWidth", "PixelXDimension"),
                ImageHeight = GetIntValue(exifData, "ImageHeight", "PixelYDimension"),
                ColorSpace = GetValue(exifData, "ColorSpace"),
                BitsPerSample = GetIntValue(exifData, "BitsPerSample"),
                Compression = GetValue(exifData, "Compression"),
                Orientation = GetIntValue(exifData, "Orientation")
            };
        }

        private static string? GetValue(Dictionary<string, string> data, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (data.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
                    return value;
            }
            return null;
        }

        private static int? GetIntValue(Dictionary<string, string> data, params string[] keys)
        {
            var value = GetValue(data, keys);
            if (value != null && int.TryParse(value, out var result))
                return result;
            return null;
        }
    }
}
