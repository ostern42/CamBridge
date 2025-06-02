namespace CamBridge.Core.ValueObjects
{
    /// <summary>
    /// Represents a DICOM tag with group and element numbers
    /// </summary>
    public record DicomTag
    {
        public ushort Group { get; }
        public ushort Element { get; }

        public DicomTag(ushort group, ushort element)
        {
            Group = group;
            Element = element;
        }

        /// <summary>
        /// Creates a DicomTag from string format "(0010,0010)"
        /// </summary>
        public static DicomTag Parse(string tagString)
        {
            if (string.IsNullOrWhiteSpace(tagString))
                throw new ArgumentException("Tag string cannot be empty");

            // Remove parentheses and spaces
            var cleaned = tagString.Trim().Trim('(', ')').Replace(" ", "");
            var parts = cleaned.Split(',');

            if (parts.Length != 2)
                throw new ArgumentException($"Invalid tag format: {tagString}");

            var group = Convert.ToUInt16(parts[0], 16);
            var element = Convert.ToUInt16(parts[1], 16);

            return new DicomTag(group, element);
        }

        public override string ToString() => $"({Group:X4},{Element:X4})";

        // Common DICOM Tags as constants
        public static class PatientModule
        {
            public static readonly DicomTag PatientName = new(0x0010, 0x0010);
            public static readonly DicomTag PatientID = new(0x0010, 0x0020);
            public static readonly DicomTag PatientBirthDate = new(0x0010, 0x0030);
            public static readonly DicomTag PatientSex = new(0x0010, 0x0040);
            public static readonly DicomTag OtherPatientIDs = new(0x0010, 0x1000);
            public static readonly DicomTag PatientComments = new(0x0010, 0x4000);
        }

        public static class StudyModule
        {
            public static readonly DicomTag StudyInstanceUID = new(0x0020, 0x000D);
            public static readonly DicomTag StudyDate = new(0x0008, 0x0020);
            public static readonly DicomTag StudyTime = new(0x0008, 0x0030);
            public static readonly DicomTag AccessionNumber = new(0x0008, 0x0050);
            public static readonly DicomTag ReferringPhysicianName = new(0x0008, 0x0090);
            public static readonly DicomTag StudyID = new(0x0020, 0x0010);
            public static readonly DicomTag StudyDescription = new(0x0008, 0x1030);
        }

        public static class SeriesModule
        {
            public static readonly DicomTag Modality = new(0x0008, 0x0060);
            public static readonly DicomTag SeriesInstanceUID = new(0x0020, 0x000E);
            public static readonly DicomTag SeriesNumber = new(0x0020, 0x0011);
            public static readonly DicomTag SeriesDate = new(0x0008, 0x0021);
            public static readonly DicomTag SeriesTime = new(0x0008, 0x0031);
            public static readonly DicomTag SeriesDescription = new(0x0008, 0x103E);
            public static readonly DicomTag PerformingPhysicianName = new(0x0008, 0x1050);
        }

        public static class InstanceModule
        {
            public static readonly DicomTag SOPClassUID = new(0x0008, 0x0016);
            public static readonly DicomTag SOPInstanceUID = new(0x0008, 0x0018);
            public static readonly DicomTag InstanceNumber = new(0x0020, 0x0013);
            public static readonly DicomTag ContentDate = new(0x0008, 0x0023);
            public static readonly DicomTag ContentTime = new(0x0008, 0x0033);
            public static readonly DicomTag AcquisitionDateTime = new(0x0008, 0x002A);
        }

        public static class EquipmentModule
        {
            public static readonly DicomTag Manufacturer = new(0x0008, 0x0070);
            public static readonly DicomTag InstitutionName = new(0x0008, 0x0080);
            public static readonly DicomTag ManufacturerModelName = new(0x0008, 0x1090);
            public static readonly DicomTag StationName = new(0x0008, 0x1010);
            public static readonly DicomTag SoftwareVersions = new(0x0018, 0x1020);
        }

        // Legacy alias for compatibility
        public static class ImageModule
        {
            public static readonly DicomTag SOPInstanceUID = InstanceModule.SOPInstanceUID;
            public static readonly DicomTag InstanceNumber = InstanceModule.InstanceNumber;
            public static readonly DicomTag ContentDate = InstanceModule.ContentDate;
            public static readonly DicomTag ContentTime = InstanceModule.ContentTime;
            public static readonly DicomTag AcquisitionDateTime = InstanceModule.AcquisitionDateTime;
        }
    }
}
