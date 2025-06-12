// src/CamBridge.Service/ServiceInfo.cs
// Version: 0.7.8
// Description: Central service version and metadata
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System.Reflection;

namespace CamBridge.Service
{
    /// <summary>
    /// Central location for service version and metadata
    /// </summary>
    public static class ServiceInfo
    {
        /// <summary>
        /// Current service version
        /// </summary>
        public const string Version = "0.7.8";

        /// <summary>
        /// Copyright notice
        /// </summary>
        public const string Copyright = "© 2025 Claude's Improbably Reliable Software Solutions";

        /// <summary>
        /// Company name (from FileVersionInfo)
        /// </summary>
        public static string Company => GetFileVersionInfo()?.CompanyName ?? "Claude's Improbably Reliable Software Solutions";

        /// <summary>
        /// Service name for Windows Service registration
        /// </summary>
        public const string ServiceName = "CamBridgeService";

        /// <summary>
        /// Display name for service management
        /// </summary>
        public const string DisplayName = "CamBridge Medical Image Converter";

        /// <summary>
        /// Service description
        /// </summary>
        public const string Description = "Converts JPEG images from Ricoh cameras to DICOM format with patient data integration";

        /// <summary>
        /// API port
        /// </summary>
        public const int ApiPort = 5050;

        /// <summary>
        /// Get version from assembly if available, fallback to constant
        /// </summary>
        public static string GetVersion()
        {
            try
            {
                var assembly = typeof(ServiceInfo).Assembly;
                var version = assembly.GetName().Version?.ToString();
                if (!string.IsNullOrEmpty(version) && version != "0.0.0.0")
                {
                    // Remove trailing .0 if present (e.g., "0.7.8.0" -> "0.7.8")
                    if (version.EndsWith(".0"))
                    {
                        version = version.Substring(0, version.LastIndexOf(".0"));
                    }
                    return version;
                }
            }
            catch
            {
                // Fallback to constant
            }
            return Version;
        }

        /// <summary>
        /// Get FileVersionInfo for additional metadata
        /// </summary>
        private static System.Diagnostics.FileVersionInfo? GetFileVersionInfo()
        {
            try
            {
                var assembly = typeof(ServiceInfo).Assembly;
                return System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get full version string with product name
        /// </summary>
        public static string GetFullVersionString()
        {
            return $"CamBridge Service v{GetVersion()}";
        }
    }
}
