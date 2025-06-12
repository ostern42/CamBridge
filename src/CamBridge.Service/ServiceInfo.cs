// src/CamBridge.Service/ServiceInfo.cs
// Version: 0.7.9
// Description: Central service version and metadata information
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System.Diagnostics;
using System.Reflection;

namespace CamBridge.Service
{
    /// <summary>
    /// Central location for all service version and metadata information
    /// This is the SINGLE SOURCE OF TRUTH for version numbers!
    /// </summary>
    public static class ServiceInfo
    {
        /// <summary>
        /// Current version of the service - UPDATE THIS for new releases!
        /// </summary>
        public const string Version = "0.7.9";

        /// <summary>
        /// Service name as registered in Windows
        /// </summary>
        public const string ServiceName = "CamBridgeService";

        /// <summary>
        /// Display name shown in Services Manager
        /// </summary>
        public const string DisplayName = "CamBridge Medical Image Converter";

        /// <summary>
        /// Service description
        /// </summary>
        public const string Description = "Converts JPEG images from Ricoh cameras to DICOM format for medical imaging systems";

        /// <summary>
        /// Copyright notice
        /// </summary>
        public const string Copyright = "© 2025 Claude's Improbably Reliable Software Solutions";

        /// <summary>
        /// Company name from version information
        /// </summary>
        public static string Company
        {
            get
            {
                try
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                    return fileVersionInfo.CompanyName ?? "Claude's Improbably Reliable Software Solutions";
                }
                catch
                {
                    return "Claude's Improbably Reliable Software Solutions";
                }
            }
        }

        /// <summary>
        /// API Port for HTTP endpoints
        /// </summary>
        public const int ApiPort = 5050;

        /// <summary>
        /// Gets the full version string with product name
        /// </summary>
        public static string GetFullVersionString()
        {
            return $"{DisplayName} v{Version}";
        }

        /// <summary>
        /// Gets build configuration (Debug/Release)
        /// </summary>
        public static string BuildConfiguration
        {
            get
            {
#if DEBUG
                return "Debug";
#else
                return "Release";
#endif
            }
        }
    }
}
