namespace CamBridge.Core.ValueObjects
{
    /// <summary>
    /// Represents an EXIF tag with its ID and name
    /// </summary>
    public record ExifTag
    {
        public int Id { get; }
        public string Name { get; }
        public string Category { get; }

        public ExifTag(int id, string name, string category = "Unknown")
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Category = category;
        }

        public override string ToString() => $"{Name} (0x{Id:X4})";

        // Common EXIF Tags
        public static class Image
        {
            public static readonly ExifTag Make = new(0x010F, "Make", "Image");
            public static readonly ExifTag Model = new(0x0110, "Model", "Image");
            public static readonly ExifTag Software = new(0x0131, "Software", "Image");
            public static readonly ExifTag DateTime = new(0x0132, "DateTime", "Image");
            public static readonly ExifTag ImageDescription = new(0x010E, "ImageDescription", "Image");
            public static readonly ExifTag Orientation = new(0x0112, "Orientation", "Image");
            public static readonly ExifTag XResolution = new(0x011A, "XResolution", "Image");
            public static readonly ExifTag YResolution = new(0x011B, "YResolution", "Image");
        }

        public static class Photo
        {
            public static readonly ExifTag ExposureTime = new(0x829A, "ExposureTime", "Photo");
            public static readonly ExifTag FNumber = new(0x829D, "FNumber", "Photo");
            public static readonly ExifTag ISOSpeedRatings = new(0x8827, "ISOSpeedRatings", "Photo");
            public static readonly ExifTag DateTimeOriginal = new(0x9003, "DateTimeOriginal", "Photo");
            public static readonly ExifTag Flash = new(0x9209, "Flash", "Photo");
            public static readonly ExifTag FocalLength = new(0x920A, "FocalLength", "Photo");
            public static readonly ExifTag UserComment = new(0x9286, "UserComment", "Photo");
            public static readonly ExifTag ExifVersion = new(0x9000, "ExifVersion", "Photo");
        }

        public static class GPS
        {
            public static readonly ExifTag GPSLatitudeRef = new(0x0001, "GPSLatitudeRef", "GPS");
            public static readonly ExifTag GPSLatitude = new(0x0002, "GPSLatitude", "GPS");
            public static readonly ExifTag GPSLongitudeRef = new(0x0003, "GPSLongitudeRef", "GPS");
            public static readonly ExifTag GPSLongitude = new(0x0004, "GPSLongitude", "GPS");
            public static readonly ExifTag GPSAltitude = new(0x0006, "GPSAltitude", "GPS");
            public static readonly ExifTag GPSTimeStamp = new(0x0007, "GPSTimeStamp", "GPS");
        }

        // Ricoh specific
        public static readonly ExifTag RicohUserComment = Photo.UserComment;
    }
}