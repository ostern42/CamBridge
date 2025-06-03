using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CamBridge.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Composite EXIF reader that tries multiple readers in priority order.
    /// Provides automatic fallback when preferred readers are not available.
    /// </summary>
    public class CompositeExifReader : IExifReader
    {
        private readonly ILogger<CompositeExifReader> _logger;
        private readonly List<ReaderInfo> _readers;

        public CompositeExifReader(
            ILogger<CompositeExifReader> logger,
            ExifToolReader exifToolReader,
            RicohExifReader ricohExifReader,
            ExifReader standardReader)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Priority order: ExifTool > Ricoh > Standard
            _readers = new List<ReaderInfo>
            {
                new ReaderInfo("ExifTool", exifToolReader, exifToolReader.IsAvailable()),
                new ReaderInfo("Ricoh", ricohExifReader, true),
                new ReaderInfo("Standard", standardReader, true)
            };

            LogReaderStatus();
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, string>> ReadExifDataAsync(string filePath)
        {
            Exception? lastException = null;

            foreach (var readerInfo in _readers.Where(r => r.IsAvailable))
            {
                try
                {
                    _logger.LogDebug("Attempting to read EXIF data with {Reader}", readerInfo.Name);
                    var result = await readerInfo.Reader.ReadExifDataAsync(filePath);

                    if (result.Count > 0)
                    {
                        _logger.LogInformation("Successfully read {Count} EXIF tags using {Reader}",
                            result.Count, readerInfo.Name);
                        return result;
                    }
                    else
                    {
                        _logger.LogDebug("{Reader} returned no data", readerInfo.Name);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to read EXIF data with {Reader}", readerInfo.Name);
                    lastException = ex;
                }
            }

            if (lastException != null)
            {
                throw new InvalidOperationException(
                    "All EXIF readers failed. See inner exception for last error.", lastException);
            }

            _logger.LogWarning("No EXIF data found by any reader for: {FilePath}", filePath);
            return new Dictionary<string, string>();
        }

        /// <inheritdoc />
        public async Task<string?> GetUserCommentAsync(string filePath)
        {
            Exception? lastException = null;

            foreach (var readerInfo in _readers.Where(r => r.IsAvailable))
            {
                try
                {
                    _logger.LogDebug("Attempting to get UserComment with {Reader}", readerInfo.Name);
                    var result = await readerInfo.Reader.GetUserCommentAsync(filePath);

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        // Validate that we got actual data, not just "GCM_TAG"
                        if (result.Trim() == "GCM_TAG")
                        {
                            _logger.LogDebug("{Reader} returned only GCM_TAG marker, trying next reader",
                                readerInfo.Name);
                            continue;
                        }

                        _logger.LogInformation("Successfully extracted QRBridge data using {Reader}: {Data}",
                            readerInfo.Name, result);
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to get UserComment with {Reader}", readerInfo.Name);
                    lastException = ex;
                }
            }

            if (lastException != null)
            {
                _logger.LogError(lastException, "All readers failed to extract QRBridge data");
            }

            return null;
        }

        /// <inheritdoc />
        public Dictionary<string, string> ParseQRBridgeData(string userComment)
        {
            // All readers should use the same parsing logic
            // Use the first available reader for parsing
            var reader = _readers.FirstOrDefault(r => r.IsAvailable)?.Reader;
            if (reader == null)
            {
                _logger.LogError("No EXIF readers available for parsing");
                return new Dictionary<string, string>();
            }

            return reader.ParseQRBridgeData(userComment);
        }

        /// <inheritdoc />
        public async Task<bool> HasExifDataAsync(string filePath)
        {
            foreach (var readerInfo in _readers.Where(r => r.IsAvailable))
            {
                try
                {
                    if (await readerInfo.Reader.HasExifDataAsync(filePath))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Error checking EXIF data with {Reader}", readerInfo.Name);
                }
            }

            return false;
        }

        /// <summary>
        /// Gets diagnostic information about available readers
        /// </summary>
        public Dictionary<string, bool> GetReaderStatus()
        {
            return _readers.ToDictionary(r => r.Name, r => r.IsAvailable);
        }

        private void LogReaderStatus()
        {
            _logger.LogInformation("EXIF Reader Status:");
            foreach (var reader in _readers)
            {
                _logger.LogInformation("  {Reader}: {Status}",
                    reader.Name,
                    reader.IsAvailable ? "Available" : "Not Available");
            }

            var availableCount = _readers.Count(r => r.IsAvailable);
            if (availableCount == 0)
            {
                _logger.LogError("No EXIF readers are available!");
            }
            else if (!_readers[0].IsAvailable) // ExifTool not available
            {
                _logger.LogWarning("ExifTool not available. Using fallback readers. " +
                    "Some proprietary tags may not be readable.");
            }
        }

        /// <summary>
        /// Information about a reader
        /// </summary>
        private class ReaderInfo
        {
            public string Name { get; }
            public IExifReader Reader { get; }
            public bool IsAvailable { get; }

            public ReaderInfo(string name, IExifReader reader, bool isAvailable)
            {
                Name = name;
                Reader = reader;
                IsAvailable = isAvailable;
            }
        }
    }
}
