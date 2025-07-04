// src/CamBridge.Core/Interfaces/IDicomTagMapper.cs
// Version: 0.8.24 - Cleaned up interface
// Transform logic belongs to MappingRule, not the mapper!

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
        /// Maps source data to a DICOM dataset using mapping rules
        /// </summary>
        /// <param name="dataset">The DICOM dataset to populate</param>
        /// <param name="sourceData">The source data dictionary</param>
        /// <param name="mappingRules">The mapping rules to apply</param>
        void MapToDataset(DicomDataset dataset, Dictionary<string, string> sourceData, IEnumerable<MappingRule> mappingRules);
    }
}
