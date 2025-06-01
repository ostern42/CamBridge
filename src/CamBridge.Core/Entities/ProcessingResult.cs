using System;

namespace CamBridge.Core.Entities
{
    /// <summary>
    /// Result of processing a single image file
    /// </summary>
    public class ProcessingResult
    {
        public string FilePath { get; private set; }
        public string FileName => System.IO.Path.GetFileName(FilePath);
        public bool Success { get; private set; }
        public string? OutputFile { get; private set; }
        public string? ErrorMessage { get; private set; }
        public DateTime ProcessedAt { get; private set; }
        public TimeSpan? ProcessingTime { get; private set; }
        public PatientInfo? PatientInfo { get; set; }

        private ProcessingResult(string filePath, bool success, string? outputFile, string? errorMessage, TimeSpan? processingTime)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Success = success;
            OutputFile = outputFile;
            ErrorMessage = errorMessage;
            ProcessedAt = DateTime.UtcNow;
            ProcessingTime = processingTime;
        }

        public static ProcessingResult CreateSuccess(string filePath, string outputFile, TimeSpan processingTime)
        {
            return new ProcessingResult(filePath, true, outputFile, null, processingTime);
        }

        public static ProcessingResult CreateFailure(string filePath, string errorMessage, TimeSpan? processingTime = null)
        {
            return new ProcessingResult(filePath, false, null, errorMessage, processingTime);
        }
    }
}
