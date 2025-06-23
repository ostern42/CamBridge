using System.Collections.Generic;
using FellowOakDicom;
using CamBridge.Core;

namespace CamBridge.Core.Interfaces
{
    /// <summary>
    /// Interface for DICOM tag mapping services
    /// </summary>
    public interface IDicomTagMapper
    {
        /// <summary>
        /// Applies a mapping rule to transform a value
        /// </summary>
        /// <param name="value">The value to transform</param>
        /// <param name="transform">The transform to apply</param>
        /// <returns>The transformed value</returns>
        string? ApplyTransform(string? value, string? transform);

        /// <summary>
        /// Maps source data to a DICOM dataset using mapping rules
        /// </summary>
        /// <param name="dataset">The DICOM dataset to populate</param>
        /// <param name="sourceData">The source data dictionary</param>
        /// <param name="mappingRules">The mapping rules to apply</param>
        void MapToDataset(DicomDataset dataset, Dictionary<string, string> sourceData, IEnumerable<MappingRule> mappingRules);
    }
}
