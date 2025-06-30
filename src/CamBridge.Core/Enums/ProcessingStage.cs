// src/CamBridge.Core/Enums/ProcessingStage.cs
// Version: 0.8.9
// Created: Session 97 - Fixing hierarchical logging
// Updated: Session 105 - Added pipeline lifecycle stages
// Purpose: Define processing stages for correlation tracking
namespace CamBridge.Core.Enums
{
    /// <summary>
    /// Processing stages for hierarchical logging and correlation tracking
    /// </summary>
    public enum ProcessingStage
    {
        // Service-level stages (new in v0.8.7)
        /// <summary>
        /// Service is starting up
        /// </summary>
        ServiceStartup,

        /// <summary>
        /// Loading service configuration
        /// </summary>
        ConfigurationLoading,

        /// <summary>
        /// Initializing processing pipelines
        /// </summary>
        PipelineInitialization,

        /// <summary>
        /// Service is shutting down
        /// </summary>
        ServiceShutdown,

        // NEW in Session 105:
        /// <summary>
        /// Pipeline is being stopped
        /// </summary>
        PipelineShutdown,

        /// <summary>
        /// Pipeline recovery attempt after error
        /// </summary>
        PipelineRecovery,

        /// <summary>
        /// File watcher encountered an error
        /// </summary>
        WatcherError,

        /// <summary>
        /// Health check operation
        /// </summary>
        HealthCheck,

        // File processing stages
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
