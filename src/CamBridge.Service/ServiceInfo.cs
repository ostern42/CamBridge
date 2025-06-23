// src/CamBridge.Service/ServiceInfo.cs
// Version: 0.7.15
// Description: Central service version and metadata information
// Copyright: Â© 2025 Claude's Improbably Reliable Software Solutions

using System.Diagnostics;
using System.Reflection;

namespace CamBridge.Service
{
    /// <summary>
    /// Central location for all service version and metadata information
    /// Version is now dynamically read from assembly attributes!
    /// </summary>
    public static class ServiceInfo
    {
        /// <summary>
        /// Current version of the service - dynamically read from assembly
        /// This now automatically uses Version.props values!
        /// </summary>
        public static string Version
        {
            get
            {
                try
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

                    // Try FileVersion first (from Version.props FileVersion)
                    if (!string.IsNullOrEmpty(fileVersionInfo.FileVersion))
                    {
                        // Remove trailing .0 if present (e.g., 0.7.15.0 -> 0.7.15)
                        var version = fileVersionInfo.FileVersion;
                        if (version.EndsWith(".0"))
                            version = version.Substring(0, version.LastIndexOf(".0"));
                        return version;
                    }

                    // Fallback to ProductVersion
                    if (!string.IsNullOrEmpty(fileVersionInfo.ProductVersion))
                    {
                        // Handle versions with commit hash (e.g., "0.7.15+abc123")
                        var productVersion = fileVersionInfo.ProductVersion;
                        var plusIndex = productVersion.IndexOf('+');
                        if (plusIndex > 0)
                            return productVersion.Substring(0, plusIndex);
                        return productVersion;
                    }

                    // Last fallback to assembly version
                    var assemblyVersion = assembly.GetName().Version;
                    if (assemblyVersion != null)
                        return $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";

                    return "0.7.15"; // Emergency fallback
                }
                catch
                {
                    return "0.7.15"; // Emergency fallback
                }
            }
        }

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
        public const string Copyright = "Â© 2025 Claude's Improbably Reliable Software Solutions";

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
        public const int ApiPort = 5111;

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
