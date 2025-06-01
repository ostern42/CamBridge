using System;

namespace CamBridge.Core
{
    /// <summary>
    /// Options for file processing behavior
    /// </summary>
    public class ProcessingOptions
    {
        /// <summary>
        /// What to do with source files after successful conversion
        /// </summary>
        public PostProcessingAction SuccessAction { get; set; } = PostProcessingAction.Archive;

        /// <summary>
        /// What to do with source files after failed conversion
        /// </summary>
        public PostProcessingAction FailureAction { get; set; } = PostProcessingAction.Leave;

        /// <summary>
        /// Archive folder for processed files
        /// </summary>
        public string ArchiveFolder { get; set; } = @"C:\CamBridge\Archive";

        /// <summary>
        /// Error folder for failed files
        /// </summary>
        public string ErrorFolder { get; set; } = @"C:\CamBridge\Errors";

        /// <summary>
        /// Whether to create a backup before processing
        /// </summary>
        public bool CreateBackup { get; set; } = true;

        /// <summary>
        /// Backup folder location
        /// </summary>
        public string BackupFolder { get; set; } = @"C:\CamBridge\Backup";

        /// <summary>
        /// Maximum concurrent file processing tasks
        /// </summary>
        public int MaxConcurrentProcessing { get; set; } = 2;

        /// <summary>
        /// Retry failed conversions
        /// </summary>
        public bool RetryOnFailure { get; set; } = true;

        /// <summary>
        /// Number of retry attempts
        /// </summary>
        public int MaxRetryAttempts { get; set; } = 3;

        /// <summary>
        /// Delay between retry attempts in seconds
        /// </summary>
        public int RetryDelaySeconds { get; set; } = 5;

        /// <summary>
        /// Whether to process existing files on startup
        /// </summary>
        public bool ProcessExistingOnStartup { get; set; } = true;

        /// <summary>
        /// File age threshold - don't process files older than this
        /// </summary>
        public TimeSpan? MaxFileAge { get; set; } = TimeSpan.FromDays(30);

        /// <summary>
        /// Minimum file size in bytes
        /// </summary>
        public long MinimumFileSizeBytes { get; set; } = 1024; // 1KB

        /// <summary>
        /// Maximum file size in bytes
        /// </summary>
        public long MaximumFileSizeBytes { get; set; } = 100 * 1024 * 1024; // 100MB

        /// <summary>
        /// Output filename pattern
        /// </summary>
        public string OutputFilePattern { get; set; } = "{PatientID}_{StudyDate}_{InstanceNumber}.dcm";

        /// <summary>
        /// Whether to preserve original folder structure
        /// </summary>
        public bool PreserveFolderStructure { get; set; } = false;

        /// <summary>
        /// Organization of output files
        /// </summary>
        public OutputOrganization OutputOrganization { get; set; } = OutputOrganization.ByPatient;
    }

    public enum PostProcessingAction
    {
        /// <summary>
        /// Leave the file in place
        /// </summary>
        Leave,

        /// <summary>
        /// Move to archive folder
        /// </summary>
        Archive,

        /// <summary>
        /// Delete the file (use with caution!)
        /// </summary>
        Delete,

        /// <summary>
        /// Move to error folder
        /// </summary>
        MoveToError
    }

    public enum OutputOrganization
    {
        /// <summary>
        /// No organization, all files in output folder
        /// </summary>
        None,

        /// <summary>
        /// Organize by patient: OutputFolder/PatientID/files
        /// </summary>
        ByPatient,

        /// <summary>
        /// Organize by date: OutputFolder/YYYY-MM-DD/files
        /// </summary>
        ByDate,

        /// <summary>
        /// Organize by patient and date: OutputFolder/PatientID/YYYY-MM-DD/files
        /// </summary>
        ByPatientAndDate
    }
}