using System;

namespace CamBridge.Core.ValueObjects
{
    /// <summary>
    /// Strongly typed study identifier
    /// </summary>
    public record StudyId
    {
        public string Value { get; }

        public StudyId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Study ID cannot be empty", nameof(value));

            // Validate format (max 16 chars for DICOM Study ID)
            if (value.Length > 16)
                throw new ArgumentException($"Study ID too long (max 16 chars): {value}", nameof(value));

            Value = value.Trim().ToUpper();
        }

        public override string ToString() => Value;

        // Implicit conversion to string
        public static implicit operator string(StudyId id) => id.Value;

        // Factory methods
        public static StudyId CreateFromExamId(string examId)
        {
            if (string.IsNullOrWhiteSpace(examId))
                return Generate();

            // Truncate if necessary and ensure uppercase
            var cleanId = examId.Trim().ToUpper();
            if (cleanId.Length > 16)
                cleanId = cleanId.Substring(0, 16);

            return new StudyId(cleanId);
        }

        public static StudyId Generate()
        {
            // Generate unique study ID
            // Format: SYYYYMMDDHHMMSS (max 16 chars)
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            return new StudyId($"S{timestamp}");
        }

        public static StudyId GenerateForDate(DateTime date)
        {
            // Generate study ID for specific date
            var dateStr = date.ToString("yyyyMMdd");
            var random = new Random().Next(1000, 9999);
            return new StudyId($"S{dateStr}{random}");
        }
    }
}