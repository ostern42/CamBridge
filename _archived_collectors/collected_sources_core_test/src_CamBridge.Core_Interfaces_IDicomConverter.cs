using System.Threading.Tasks;
using CamBridge.Core.Entities;

namespace CamBridge.Core.Interfaces
{
    /// <summary>
    /// Interface for converting JPEG images to DICOM format
    /// </summary>
    public interface IDicomConverter
    {
        /// <summary>
        /// Converts a JPEG file to DICOM using the provided metadata
        /// </summary>
        Task<ConversionResult> ConvertToDicomAsync(
            string sourceJpegPath,
            string destinationDicomPath,
            ImageMetadata metadata);

        /// <summary>
        /// Validates if the generated DICOM file is compliant
        /// </summary>
        Task<ValidationResult> ValidateDicomFileAsync(string dicomFilePath);

        /// <summary>
        /// Gets the SOP Class UID for photographic images
        /// </summary>
        string GetPhotographicSopClassUid();
    }

    public class ConversionResult
    {
        public bool Success { get; init; }
        public string? DicomFilePath { get; init; }
        public string? ErrorMessage { get; init; }
        public string? SopInstanceUid { get; init; }
        public long FileSizeBytes { get; init; }

        public static ConversionResult CreateSuccess(string filePath, string sopInstanceUid, long fileSize)
            => new()
            {
                Success = true,
                DicomFilePath = filePath,
                SopInstanceUid = sopInstanceUid,
                FileSizeBytes = fileSize
            };

        public static ConversionResult CreateFailure(string error)
            => new() { Success = false, ErrorMessage = error };
    }

    public class ValidationResult
    {
        public bool IsValid { get; init; }
        public List<string> Errors { get; init; } = new();
        public List<string> Warnings { get; init; } = new();

        public static ValidationResult Valid()
            => new() { IsValid = true };

        public static ValidationResult Invalid(params string[] errors)
            => new() { IsValid = false, Errors = errors.ToList() };
    }
}