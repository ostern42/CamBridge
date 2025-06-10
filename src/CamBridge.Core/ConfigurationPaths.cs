// src\CamBridge.Core\ConfigurationPaths.cs
// Version: 0.7.1
// Description: Central configuration path management - Simple version
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.IO;

namespace CamBridge.Core
{
    /// <summary>
    /// Central configuration path management for CamBridge
    /// </summary>
    public static class ConfigurationPaths
    {
        private const string CompanyFolder = "CamBridge";

        /// <summary>
        /// Gets the primary configuration file path
        /// </summary>
        public static string GetPrimaryConfigPath()
        {
            var programData = Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData);
            var configDir = Path.Combine(programData, CompanyFolder);

            Directory.CreateDirectory(configDir);

            return Path.Combine(configDir, "appsettings.json");
        }

        /// <summary>
        /// Gets the logs directory
        /// </summary>
        public static string GetLogsDirectory()
        {
            var programData = Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData);
            var logsDir = Path.Combine(programData, CompanyFolder, "Logs");

            Directory.CreateDirectory(logsDir);

            return logsDir;
        }

        /// <summary>
        /// Checks if primary config exists
        /// </summary>
        public static bool PrimaryConfigExists()
        {
            return File.Exists(GetPrimaryConfigPath());
        }

        /// <summary>
        /// Initialize config from local file
        /// </summary>
        public static bool InitializePrimaryConfig(string sourcePath)
        {
            var primaryPath = GetPrimaryConfigPath();

            if (File.Exists(primaryPath))
                return false;

            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, primaryPath, false);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get diagnostic info
        /// </summary>
        public static string GetDiagnosticInfo()
        {
            return $"Primary Config: {GetPrimaryConfigPath()} (Exists: {PrimaryConfigExists()})";
        }
    }
}
