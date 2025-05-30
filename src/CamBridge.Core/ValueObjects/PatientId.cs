using System;

namespace CamBridge.Core.ValueObjects
{
    /// <summary>
    /// Strongly typed patient identifier
    /// </summary>
    public record PatientId
    {
        public string Value { get; }

        public PatientId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Patient ID cannot be empty", nameof(value));

            // Validate format (alphanumeric, hyphens, underscores)
            if (!IsValidFormat(value))
                throw new ArgumentException($"Invalid patient ID format: {value}", nameof(value));

            Value = value.Trim();
        }

        private static bool IsValidFormat(string value)
        {
            // Allow alphanumeric, hyphens, underscores, max 64 chars (DICOM limit)
            if (value.Length > 64) return false;

            foreach (char c in value)
            {
                if (!char.IsLetterOrDigit(c) && c != '-' && c != '_' && c != '.')
                    return false;
            }

            return true;
        }

        public override string ToString() => Value;

        // Implicit conversion to string
        public static implicit operator string(PatientId id) => id.Value;

        // Factory method for creating from various sources
        public static PatientId CreateFromName(string name, DateTime? birthDate = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty");

            // Create ID from name initials and timestamp/birthdate
            var parts = name.Split(new[] { ' ', ',', '-' }, StringSplitOptions.RemoveEmptyEntries);
            var initials = string.Join("", parts.Select(p => p[0])).ToUpper();

            var dateComponent = birthDate?.ToString("yyyyMMdd") ??
                               DateTime.UtcNow.ToString("yyyyMMddHHmm");

            return new PatientId($"{initials}{dateComponent}");
        }
    }
}