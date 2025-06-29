// src/CamBridge.Core/Enums/LogVerbosity.cs
// Version: 0.8.6
// Created: Session 96 - Making Logs Great Again!
// Purpose: Define global log verbosity levels for the service

namespace CamBridge.Core.Enums
{
    /// <summary>
    /// Defines the verbosity levels for service logging.
    /// Controls how much detail is written to log files.
    /// </summary>
    public enum LogVerbosity
    {
        /// <summary>
        /// Minimal logging - Only start/end events (~150 KB/day)
        /// Logs: File detected, processing complete/failed
        /// </summary>
        Minimal = 0,

        /// <summary>
        /// Normal logging - Key events and compact info (~750 KB/day)
        /// Logs: Minimal + EXIF data summary, mapping rules applied
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Detailed logging - All stages with timing (~1.75 MB/day)
        /// Logs: Normal + all processing stages, timing information
        /// DEFAULT setting for troubleshooting
        /// </summary>
        Detailed = 2,

        /// <summary>
        /// Debug logging - Everything including raw data (~3.5 MB/day)
        /// Logs: Detailed + raw EXIF JSON, DICOM tags, full error stacks
        /// Use only for deep debugging
        /// </summary>
        Debug = 3
    }
}
