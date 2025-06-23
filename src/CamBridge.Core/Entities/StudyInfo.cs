using System;
using CamBridge.Core.ValueObjects;

namespace CamBridge.Core.Entities
{
    /// <summary>
    /// Information about a medical imaging study
    /// </summary>
    public class StudyInfo
    {
        public StudyId StudyId { get; }
        public string? ExamId { get; }
        public string? Description { get; }
        public string? Modality { get; }
        public DateTime StudyDate { get; }
        public string? AccessionNumber { get; }
        public string? ReferringPhysician { get; }
        public string? Comment { get; }

        public StudyInfo(
            StudyId studyId,
            string? examId = null,
            string? description = null,
            string? modality = null,
            DateTime? studyDate = null,
            string? accessionNumber = null,
            string? referringPhysician = null,
            string? comment = null)
        {
            StudyId = studyId ?? throw new ArgumentNullException(nameof(studyId));
            ExamId = examId;
            Description = description;
            Modality = modality ?? "OT"; // Other modality as default
            StudyDate = studyDate ?? DateTime.Now;
            AccessionNumber = accessionNumber;
            ReferringPhysician = referringPhysician;
            Comment = comment;
        }
    }
}
