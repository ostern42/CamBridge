using System;
using CamBridge.Core.ValueObjects;

namespace CamBridge.Core.Entities
{
    /// <summary>
    /// Represents patient information extracted from EXIF data
    /// </summary>
    public class PatientInfo
    {
        public PatientId Id { get; }
        public string Name { get; }
        public DateTime? BirthDate { get; }
        public Gender Gender { get; }
        public string? OtherIds { get; }

        public PatientInfo(
            PatientId id,
            string name,
            DateTime? birthDate = null,
            Gender gender = Gender.Other,
            string? otherIds = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            BirthDate = birthDate;
            Gender = gender;
            OtherIds = otherIds;
        }

        /// <summary>
        /// Creates PatientInfo from parsed EXIF user comment data
        /// </summary>
        public static PatientInfo FromExifData(Dictionary<string, string> exifData)
        {
            if (!exifData.TryGetValue("name", out var name))
                throw new ArgumentException("Patient name is required");

            var patientId = new PatientId(
                exifData.GetValueOrDefault("patientid") ??
                GeneratePatientId(name));

            DateTime? birthDate = null;
            if (exifData.TryGetValue("birthdate", out var birthdateStr) &&
                DateTime.TryParse(birthdateStr, out var parsedDate))
            {
                birthDate = parsedDate;
            }

            var gender = ParseGender(exifData.GetValueOrDefault("gender"));

            return new PatientInfo(patientId, name, birthDate, gender);
        }

        private static Gender ParseGender(string? genderStr)
        {
            return genderStr?.ToUpperInvariant() switch
            {
                "M" or "MALE" => Gender.Male,
                "F" or "FEMALE" => Gender.Female,
                "O" or "OTHER" => Gender.Other,
                _ => Gender.Other
            };
        }

        private static string GeneratePatientId(string name)
        {
            // Simple ID generation based on name and timestamp
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var namePrefix = name.Length >= 3 ? name.Substring(0, 3).ToUpper() : name.ToUpper();
            return $"{namePrefix}{timestamp}";
        }
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }
}