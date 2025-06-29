// src/CamBridge.Core/Enums/ProcessingStage.cs
// Version: 0.8.6
// Created: Session 97 - Fixing hierarchical logging
// Purpose: Define processing stages for correlation tracking

namespace CamBridge.Core.Enums
{
    /// <summary>
    /// Processing stages for hierarchical logging and correlation tracking
    /// </summary>
    public enum ProcessingStage
    {
        /// <summary>
        /// File received from FTP Server (future feature)
        /// </summary>
        FtpReceived,

        /// <summary>
        /// File detected in watch folder
        /// </summary>
        FileDetected,

        /// <summary>
        /// Extracting metadata from JPEG using ExifTool
        /// </summary>
        ExifExtraction,

        /// <summary>
        /// Applying mapping rules to transform data
        /// </summary>
        TagMapping,

        /// <summary>
        /// Creating DICOM file from JPEG + metadata
        /// </summary>
        DicomConversion,

        /// <summary>
        /// Archive/Delete operations after conversion
        /// </summary>
        PostProcessing,

        /// <summary>
        /// Uploading to PACS server
        /// </summary>
        PacsUpload,

        /// <summary>
        /// Processing completed successfully
        /// </summary>
        Complete,

        /// <summary>
        /// Error occurred during processing
        /// </summary>
        Error
    }
}
