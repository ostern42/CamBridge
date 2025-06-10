// src\CamBridge.Core\ConfigurationPaths.cs
// Version: 0.7.3
// Description: Centralized configuration path management
// © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace CamBridge.Core
{
    /// <summary>
    /// Provides centralized path management for all configuration files
    /// Following the 3-layer architecture:
    /// - System settings: ProgramData (shared)
    /// - Pipeline configs: ProgramData/Pipelines (shared)
    /// - User preferences: AppData/Roaming (per-user)
    /// </summary>
    public static class ConfigurationPaths
    {
        private const string AppName = "CamBridge";
        private const string CompanyName = "ClaudesImprobablyReliableSoftwareSolutions";

        // File names
        private const string SystemSettingsFile = "appsettings.json";
        private const string UserPreferencesFile = "preferences.json";
        private const string MappingsFile = "mappings.json";

        // Folder names
        private const string PipelinesFolder = "Pipelines";
        private const string LogsFolder = "Logs";
        private const string CacheFolder = "Cache";
        private const string BackupFolder = "Backup";

        /// <summary>
        /// Gets the ProgramData folder for system-wide settings
        /// Typically: C:\ProgramData\CamBridge
        /// </summary>
        public static string GetProgramDataFolder()
        {
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var path = Path.Combine(programData, AppName);
            EnsureDirectoryExists(path);
            return path;
        }

        /// <summary>
        /// Gets the AppData\Roaming folder for user-specific preferences
        /// Typically: C:\Users\{User}\AppData\Roaming\CamBridge
        /// </summary>
        public static string GetAppDataFolder()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var path = Path.Combine(appData, AppName);
            EnsureDirectoryExists(path);
            return path;
        }

        /// <summary>
        /// Gets the AppData\Local folder for user-specific cache
        /// Typically: C:\Users\{User}\AppData\Local\CamBridge
        /// </summary>
        public static string GetLocalAppDataFolder()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var path = Path.Combine(localAppData, AppName);
            EnsureDirectoryExists(path);
            return path;
        }

        // === SYSTEM-WIDE SETTINGS (ProgramData) ===

        /// <summary>
        /// Gets the path to system-wide settings file
        /// Used by both Service and Config Tool
        /// </summary>
        public static string GetSystemSettingsPath()
        {
            return Path.Combine(GetProgramDataFolder(), SystemSettingsFile);
        }

        /// <summary>
        /// Gets the folder for pipeline configurations
        /// </summary>
        public static string GetPipelinesFolder()
        {
            var path = Path.Combine(GetProgramDataFolder(), PipelinesFolder);
            EnsureDirectoryExists(path);
            return path;
        }

        /// <summary>
        /// Gets the path to a specific pipeline configuration
        /// </summary>
        public static string GetPipelineConfigPath(Guid pipelineId)
        {
            return Path.Combine(GetPipelinesFolder(), $"{pipelineId}.json");
        }

        /// <summary>
        /// Gets the default mappings file path (legacy support)
        /// </summary>
        public static string GetDefaultMappingsPath()
        {
            return Path.Combine(GetProgramDataFolder(), MappingsFile);
        }

        // === USER-SPECIFIC SETTINGS (AppData) ===

        /// <summary>
        /// Gets the path to user preferences file
        /// </summary>
        public static string GetUserPreferencesPath()
        {
            return Path.Combine(GetAppDataFolder(), UserPreferencesFile);
        }

        /// <summary>
        /// Gets the user-specific cache folder
        /// </summary>
        public static string GetUserCachePath()
        {
            var path = Path.Combine(GetLocalAppDataFolder(), CacheFolder);
            EnsureDirectoryExists(path);
            return path;
        }

        // === SERVICE PATHS ===

        /// <summary>
        /// Gets the logs folder path
        /// </summary>
        public static string GetLogsFolder()
        {
            // Check if custom log path is specified in environment
            var customLogPath = Environment.GetEnvironmentVariable("CAMBRIDGE_LOG_PATH");
            if (!string.IsNullOrEmpty(customLogPath))
            {
                EnsureDirectoryExists(customLogPath);
                return customLogPath;
            }

            // Default to C:\CamBridge\Logs
            var path = Path.Combine(@"C:\CamBridge", LogsFolder);
            EnsureDirectoryExists(path);
            return path;
        }

        /// <summary>
        /// Gets the backup folder path
        /// </summary>
        public static string GetBackupFolder()
        {
            var path = Path.Combine(GetProgramDataFolder(), BackupFolder);
            EnsureDirectoryExists(path);
            return path;
        }

        // === COMPATIBILITY METHODS ===

        /// <summary>
        /// Gets the primary config path (legacy compatibility)
        /// Now points to system settings
        /// </summary>
        public static string GetPrimaryConfigPath()
        {
            return GetSystemSettingsPath();
        }

        /// <summary>
        /// Gets all config search paths (legacy compatibility)
        /// </summary>
        public static string[] GetConfigSearchPaths()
        {
            return new[]
            {
                GetSystemSettingsPath(),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SystemSettingsFile),
                Path.Combine(Environment.CurrentDirectory, SystemSettingsFile)
            };
        }

        // === UTILITY METHODS ===

        /// <summary>
        /// Ensures a directory exists, creating it if necessary
        /// </summary>
        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Creates a backup of a configuration file
        /// </summary>
        public static string CreateBackup(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var fileName = Path.GetFileName(filePath);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupName = $"{Path.GetFileNameWithoutExtension(fileName)}_{timestamp}{Path.GetExtension(fileName)}";
            var backupPath = Path.Combine(GetBackupFolder(), backupName);

            File.Copy(filePath, backupPath);
            return backupPath;
        }

        /// <summary>
        /// Creates a backup of the current configuration (legacy support)
        /// </summary>
        public static string BackupConfig(string configPath)
        {
            return CreateBackup(configPath);
        }

        /// <summary>
        /// Cleans up old backup files (keeps last N backups)
        /// </summary>
        public static void CleanupBackups(int keepCount = 10)
        {
            var backupDir = GetBackupFolder();
            var files = Directory.GetFiles(backupDir, "*.json")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTime)
                .Skip(keepCount);

            foreach (var file in files)
            {
                file.Delete();
            }
        }

        // === LEGACY COMPATIBILITY METHODS ===

        /// <summary>
        /// Checks if primary config exists (legacy compatibility)
        /// </summary>
        public static bool PrimaryConfigExists()
        {
            return File.Exists(GetPrimaryConfigPath());
        }

        /// <summary>
        /// Gets the logs directory (legacy compatibility)
        /// </summary>
        public static string GetLogsDirectory()
        {
            return GetLogsFolder();
        }

        /// <summary>
        /// Gets diagnostic information about configuration paths
        /// </summary>
        public static string GetDiagnosticInfo()
        {
            var info = new System.Text.StringBuilder();
            info.AppendLine("=== Configuration Paths Diagnostic Info ===");
            info.AppendLine($"ProgramData: {GetProgramDataFolder()}");
            info.AppendLine($"AppData: {GetAppDataFolder()}");
            info.AppendLine($"LocalAppData: {GetLocalAppDataFolder()}");
            info.AppendLine($"System Settings: {GetSystemSettingsPath()}");
            info.AppendLine($"User Preferences: {GetUserPreferencesPath()}");
            info.AppendLine($"Pipelines Folder: {GetPipelinesFolder()}");
            info.AppendLine($"Logs Folder: {GetLogsFolder()}");
            info.AppendLine($"Backup Folder: {GetBackupFolder()}");
            info.AppendLine($"Primary Config Exists: {PrimaryConfigExists()}");
            info.AppendLine($"Environment: {Environment.MachineName}");
            info.AppendLine($"User: {Environment.UserName}");
            return info.ToString();
        }

        /// <summary>
        /// Initializes primary config with default settings
        /// </summary>
        /// <returns>True if config was created, false if it already existed</returns>
        public static bool InitializePrimaryConfig()
        {
            var configPath = GetPrimaryConfigPath();
            if (!File.Exists(configPath))
            {
                // Create default config
                var defaultConfig = new
                {
                    version = "0.7.3",
                    created = DateTime.UtcNow,
                    defaultOutputFolder = @"C:\CamBridge\Output",
                    service = new
                    {
                        serviceName = "CamBridgeService",
                        displayName = "CamBridge Service",
                        description = "JPEG to DICOM medical imaging converter"
                    }
                };

                var json = System.Text.Json.JsonSerializer.Serialize(defaultConfig, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(configPath, json);
                return true; // Config was created
            }
            return false; // Config already existed
        }
    }
}
