// src/CamBridge.Core/ProcessingOptions.cs
// Version: 0.6.0
// Description: Processing options with DeadLetterFolder for pipeline architecture
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;

namespace CamBridge.Core
{
    /// <summary>
    /// Processing options for file handling
    /// </summary>
    public class ProcessingOptions
    {
        public PostProcessingAction SuccessAction { get; set; } = PostProcessingAction.Archive;
        public PostProcessingAction FailureAction { get; set; } = PostProcessingAction.MoveToError;

        public string ArchiveFolder { get; set; } = @"C:\CamBridge\Archive";
        public string ErrorFolder { get; set; } = @"C:\CamBridge\Errors";
        public string BackupFolder { get; set; } = @"C:\CamBridge\Backup";

        /// <summary>
        /// Folder for dead letter items (files that failed processing after all retries)
        /// </summary>
        public string DeadLetterFolder { get; set; } = @"C:\CamBridge\DeadLetter";

        public bool CreateBackup { get; set; } = true;
        public int MaxConcurrentProcessing { get; set; } = 2;
        public bool RetryOnFailure { get; set; } = true;
        public int MaxRetryAttempts { get; set; } = 3;

        public OutputOrganization OutputOrganization { get; set; } = OutputOrganization.ByPatientAndDate;
        public bool ProcessExistingOnStartup { get; set; } = true;
        public TimeSpan? MaxFileAge { get; set; } = TimeSpan.FromDays(30);

        // Additional properties required by Infrastructure
        public long MinimumFileSizeBytes { get; set; } = 1024; // 1 KB minimum
        public long MaximumFileSizeBytes { get; set; } = 104857600; // 100 MB maximum
        public string OutputFilePattern { get; set; } = "{PatientID}_{StudyDate}_{InstanceNumber}.dcm";
        public int RetryDelaySeconds { get; set; } = 2;
    }

    /// <summary>
    /// Actions to take after processing a file
    /// </summary>
    public enum PostProcessingAction
    {
        /// <summary>Leave the file in place</summary>
        Leave,

        /// <summary>Move to archive folder</summary>
        Archive,

        /// <summary>Delete the file</summary>
        Delete,

        /// <summary>Move to error folder</summary>
        MoveToError
    }

    /// <summary>
    /// How to organize output files
    /// </summary>
    public enum OutputOrganization
    {
        /// <summary>No organization, all files in root</summary>
        None,

        /// <summary>Organize by patient ID</summary>
        ByPatient,

        /// <summary>Organize by study date</summary>
        ByDate,

        /// <summary>Organize by patient ID and then date</summary>
        ByPatientAndDate
    }
}
