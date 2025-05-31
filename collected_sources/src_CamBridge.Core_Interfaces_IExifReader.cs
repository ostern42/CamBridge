using System.Collections.Generic;
using System.Threading.Tasks;

namespace CamBridge.Core.Interfaces
{
    /// <summary>
    /// Interface for reading EXIF data from image files
    /// </summary>
    public interface IExifReader
    {
        /// <summary>
        /// Reads all EXIF data from an image file
        /// </summary>
        Task<Dictionary<string, string>> ReadExifDataAsync(string filePath);

        /// <summary>
        /// Extracts the User Comment field from EXIF data
        /// </summary>
        Task<string?> GetUserCommentAsync(string filePath);

        /// <summary>
        /// Parses QRBridge formatted string from User Comment
        /// Example: -examid "EX002" -name "Schmidt, Maria" -birthdate "1985-03-15"
        /// </summary>
        Dictionary<string, string> ParseQRBridgeData(string userComment);

        /// <summary>
        /// Checks if file has valid EXIF data
        /// </summary>
        Task<bool> HasExifDataAsync(string filePath);
    }
}