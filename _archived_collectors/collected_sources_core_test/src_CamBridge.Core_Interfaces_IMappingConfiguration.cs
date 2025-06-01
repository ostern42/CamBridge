using System.Collections.Generic;
using CamBridge.Core.ValueObjects;

namespace CamBridge.Core.Interfaces
{
    /// <summary>
    /// Interface for configuring EXIF to DICOM tag mappings
    /// </summary>
    public interface IMappingConfiguration
    {
        /// <summary>
        /// Gets all configured mapping rules
        /// </summary>
        IReadOnlyList<MappingRule> GetMappingRules();

        /// <summary>
        /// Gets mapping rules for a specific source type
        /// </summary>
        IReadOnlyList<MappingRule> GetRulesForSource(string sourceType);

        /// <summary>
        /// Adds a new mapping rule
        /// </summary>
        void AddRule(MappingRule rule);

        /// <summary>
        /// Removes a mapping rule
        /// </summary>
        bool RemoveRule(string ruleName);

        /// <summary>
        /// Gets the default mapping configuration
        /// </summary>
        static IMappingConfiguration GetDefault() => new DefaultMappingConfiguration();
    }

    /// <summary>
    /// Default implementation with standard mappings
    /// </summary>
    internal class DefaultMappingConfiguration : IMappingConfiguration
    {
        private readonly List<MappingRule> _rules = new();

        public DefaultMappingConfiguration()
        {
            InitializeDefaultRules();
        }

        public IReadOnlyList<MappingRule> GetMappingRules() => _rules.AsReadOnly();

        public IReadOnlyList<MappingRule> GetRulesForSource(string sourceType)
            => _rules.Where(r => r.SourceType == sourceType).ToList().AsReadOnly();

        public void AddRule(MappingRule rule)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            _rules.Add(rule);
        }

        public bool RemoveRule(string ruleName)
            => _rules.RemoveAll(r => r.Name == ruleName) > 0;

        private void InitializeDefaultRules()
        {
            // Patient data mappings
            _rules.Add(new MappingRule(
                "PatientName",
                "QRBridge",
                "name",
                DicomTag.PatientModule.PatientName,
                ValueTransform.None
            ));

            _rules.Add(new MappingRule(
                "PatientID",
                "QRBridge",
                "patientid",
                DicomTag.PatientModule.PatientID,
                ValueTransform.None
            ));

            _rules.Add(new MappingRule(
                "PatientBirthDate",
                "QRBridge",
                "birthdate",
                DicomTag.PatientModule.PatientBirthDate,
                ValueTransform.DateToDicom
            ));

            _rules.Add(new MappingRule(
                "PatientSex",
                "QRBridge",
                "gender",
                DicomTag.PatientModule.PatientSex,
                ValueTransform.GenderToDicom
            ));

            // Study data mappings
            _rules.Add(new MappingRule(
                "StudyDescription",
                "QRBridge",
                "comment",
                DicomTag.StudyModule.StudyDescription,
                ValueTransform.None
            ));

            _rules.Add(new MappingRule(
                "StudyID",
                "QRBridge",
                "examid",
                DicomTag.StudyModule.StudyID,
                ValueTransform.TruncateTo16
            ));

            // Equipment mappings
            _rules.Add(new MappingRule(
                "Manufacturer",
                "EXIF",
                "Make",
                DicomTag.EquipmentModule.Manufacturer,
                ValueTransform.None
            ));

            _rules.Add(new MappingRule(
                "Model",
                "EXIF",
                "Model",
                DicomTag.EquipmentModule.ManufacturerModelName,
                ValueTransform.None
            ));
        }
    }
}