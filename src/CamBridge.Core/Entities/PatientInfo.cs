using System;
using System.Collections.Generic;
using CamBridge.Core.ValueObjects;

namespace CamBridge.Core.Entities
{
    /// <summary>
    /// Patient information extracted from image metadata
    /// </summary>
    public class PatientInfo
    {
        public PatientId Id { get; }
        public string Name { get; }
        public string PatientName => Name; // Alias for compatibility
        public string PatientId => Id.Value; // Alias for compatibility
        public string? StudyId { get; set; }
        public DateTime? BirthDate { get; }
        public Gender Gender { get; }

        public PatientInfo(PatientId id, string name, DateTime? birthDate, Gender gender)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            BirthDate = birthDate;
            Gender = gender;
        }

        /// <summary>
        /// Creates PatientInfo from EXIF/QRBridge data
        /// </summary>
        public static PatientInfo FromExifData(Dictionary<string, string> exifData)
        {
            // Extract patient ID - try multiple possible field names
            var patientIdValue = exifData.GetValueOrDefault("patientid") ??
                                exifData.GetValueOrDefault("id") ??
                                exifData.GetValueOrDefault("examid") ??
                                $"AUTO_{DateTime.UtcNow:yyyyMMddHHmmss}";

            var patientId = new PatientId(patientIdValue);

            // Extract name
            var name = exifData.GetValueOrDefault("name") ??
                      exifData.GetValueOrDefault("patientname") ??
                      "Unknown";

            // Extract birth date
            DateTime? birthDate = null;
            var birthDateStr = exifData.GetValueOrDefault("birthdate") ??
                              exifData.GetValueOrDefault("dateofbirth");

            if (!string.IsNullOrWhiteSpace(birthDateStr))
            {
                // Try various date formats
                var formats = new[] {
                    "yyyy-MM-dd",
                    "yyyyMMdd",
                    "dd.MM.yyyy",
                    "dd/MM/yyyy",
                    "MM/dd/yyyy"
                };

                foreach (var format in formats)
                {
                    if (DateTime.TryParseExact(birthDateStr, format, null,
                        System.Globalization.DateTimeStyles.None, out var date))
                    {
                        birthDate = date;
                        break;
                    }
                }
            }

            // Extract gender
            var genderStr = exifData.GetValueOrDefault("gender") ??
                           exifData.GetValueOrDefault("sex") ??
                           "O";

            var gender = ParseGender(genderStr);

            return new PatientInfo(patientId, name, birthDate, gender);
        }

        private static Gender ParseGender(string? genderStr)
        {
            if (string.IsNullOrWhiteSpace(genderStr))
                return Gender.Other;

            return genderStr.ToUpperInvariant() switch
            {
                "M" or "MALE" or "MANN" or "MÃ„NNLICH" => Gender.Male,
                "F" or "FEMALE" or "FRAU" or "WEIBLICH" => Gender.Female,
                _ => Gender.Other
            };
        }
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }
}
