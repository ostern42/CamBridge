namespace CamBridge.Core
{
    /// <summary>
    /// Enum for value transform types used in mapping rules
    /// </summary>
    public enum ValueTransform
    {
        /// <summary>
        /// No transformation applied
        /// </summary>
        None,

        /// <summary>
        /// Convert date to DICOM format (YYYYMMDD)
        /// </summary>
        DateToDicom,

        /// <summary>
        /// Convert time to DICOM format (HHMMSS)
        /// </summary>
        TimeToDicom,

        /// <summary>
        /// Convert datetime to DICOM format
        /// </summary>
        DateTimeToDicom,

        /// <summary>
        /// Map gender codes (M/F/O to Male/Female/Other)
        /// </summary>
        MapGender,

        /// <summary>
        /// Remove prefix from value
        /// </summary>
        RemovePrefix,

        /// <summary>
        /// Extract date part from datetime
        /// </summary>
        ExtractDate,

        /// <summary>
        /// Extract time part from datetime
        /// </summary>
        ExtractTime,

        /// <summary>
        /// Convert to uppercase
        /// </summary>
        ToUpperCase,

        /// <summary>
        /// Convert to lowercase
        /// </summary>
        ToLowerCase,

        /// <summary>
        /// Trim whitespace
        /// </summary>
        Trim
    }
}
