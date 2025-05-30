using System;
using System.Threading.Tasks;

namespace CamBridge.Core.Interfaces
{
    /// <summary>
    /// Interface for processing image files through the conversion pipeline
    /// </summary>
    public interface IFileProcessor
    {
        /// <summary>
        /// Processes a single JPEG file
        /// </summary>
        Task<ProcessingResult> ProcessFileAsync(string filePath);

        /// <summary>
        /// Checks if a file should be processed
        /// </summary>
        bool ShouldProcessFile(string filePath);

        /// <summary>
        /// Event raised when processing starts
        /// </summary>
        event EventHandler<FileProcessingEventArgs> ProcessingStarted;

        /// <summary>
        /// Event raised when processing completes
        /// </summary>
        event EventHandler<FileProcessingEventArgs> ProcessingCompleted;

        /// <summary>
        /// Event raised when an error occurs
        /// </summary>
        event EventHandler<FileProcessingErrorEventArgs> ProcessingError;
    }

    public class ProcessingResult
    {
        public string SourceFile { get; init; } = string.Empty;
        public string? OutputFile { get; init; }
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }
        public DateTime ProcessedAt { get; init; } = DateTime.UtcNow;
        public TimeSpan ProcessingTime { get; init; }

        public static ProcessingResult CreateSuccess(string source, string output, TimeSpan time)
            => new()
            {
                SourceFile = source,
                OutputFile = output,
                Success = true,
                ProcessingTime = time
            };

        public static ProcessingResult CreateFailure(string source, string error, TimeSpan time)
            => new()
            {
                SourceFile = source,
                Success = false,
                ErrorMessage = error,
                ProcessingTime = time
            };
    }

    public class FileProcessingEventArgs : EventArgs
    {
        public string FilePath { get; }
        public DateTime Timestamp { get; }

        public FileProcessingEventArgs(string filePath)
        {
            FilePath = filePath;
            Timestamp = DateTime.UtcNow;
        }
    }

    public class FileProcessingErrorEventArgs : FileProcessingEventArgs
    {
        public Exception Exception { get; }

        public FileProcessingErrorEventArgs(string filePath, Exception exception)
            : base(filePath)
        {
            Exception = exception;
        }
    }
}