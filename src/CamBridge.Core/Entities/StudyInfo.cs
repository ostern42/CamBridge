using System;
using CamBridge.Core.ValueObjects;

namespace CamBridge.Core.Entities
{
    /// <summary>
    /// Represents study/examination information
    /// </summary>
    public class StudyInfo
    {
        public StudyId Id { get; }
        public string? ExamId { get; }
        public DateTime StudyDate { get; }
        public string? StudyDescription { get; }
        public string? AccessionNumber { get; }
        public string? ReferringPhysician { get; }
        public string? PerformingPhysician { get; }
        public string Modality { get; }
        public string? Comment { get; }

        public StudyInfo(
            StudyId id,
            DateTime studyDate,
            string modality = "XC",
            string? examId = null,
            string? studyDescription = null,
            string? accessionNumber = null,
            string? referringPhysician = null,
            string? performingPhysician = null,
            string? comment = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            StudyDate = studyDate;
            Modality = modality ?? "XC"; // XC = External Camera
            ExamId = examId;
            StudyDescription = studyDescription;
            AccessionNumber = accessionNumber;
            ReferringPhysician = referringPhysician;
            PerformingPhysician = performingPhysician;
            Comment = comment;
        }

        /// <summary>
        /// Creates StudyInfo from parsed EXIF user comment data
        /// </summary>
        public static StudyInfo FromExifData(Dictionary<string, string> exifData, DateTime imageDate)
        {
            var examId = exifData.GetValueOrDefault("examid");
            var comment = exifData.GetValueOrDefault("comment");

            var studyId = new StudyId(examId ?? GenerateStudyId());

            return new StudyInfo(
                id: studyId,
                studyDate: imageDate,
                modality: "XC",
                examId: examId,
                studyDescription: comment,
                comment: comment
            );
        }

        private static string GenerateStudyId()
        {
            // Generate unique study ID based on timestamp
            return $"STUDY{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }
    }
}