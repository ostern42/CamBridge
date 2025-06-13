// src\CamBridge.Core\ConfigurationPaths.cs
// Version: 0.7.10
// Description: Centralized configuration path management with V2 format support
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
        /// Initializes primary config with default settings - V2 FORMAT!
        /// CRITICAL FIX: Must create config with "CamBridge" wrapper section!
        /// </summary>
        /// <returns>True if config was created, false if it already existed</returns>
        public static bool InitializePrimaryConfig()
        {
            var configPath = GetPrimaryConfigPath();

            if (!File.Exists(configPath))
            {
                // First, check if there's a local appsettings.json to copy
                var localConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (File.Exists(localConfig))
                {
                    try
                    {
                        // Read the local config to check if it has proper V2 format
                        var localJson = File.ReadAllText(localConfig);
                        using var doc = JsonDocument.Parse(localJson);

                        // If it has a CamBridge section, it's V2 format - copy it!
                        if (doc.RootElement.TryGetProperty("CamBridge", out _))
                        {
                            File.Copy(localConfig, configPath);
                            return true; // Config was created from template
                        }
                    }
                    catch
                    {
                        // If anything goes wrong, fall through to create default
                    }
                }

                // Create proper V2 format config with CamBridge wrapper!
                var defaultConfig = new
                {
                    CamBridge = new
                    {
                        Version = "2.0",
                        Service = new
                        {
                            ServiceName = "CamBridgeService",
                            DisplayName = "CamBridge JPEG to DICOM Converter",
                            Description = "Monitors folders for JPEG files from Ricoh cameras and converts them to DICOM format",
                            ApiPort = 5111,  // CRITICAL: Correct port!
                            EnableHealthChecks = true,
                            HealthCheckInterval = "00:01:00",
                            StartupDelaySeconds = 5
                        },
                        Pipelines = new[]
                        {
                            new
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "Default Pipeline",
                                Description = "Default pipeline for JPEG to DICOM conversion",
                                Enabled = true,
                                WatchSettings = new
                                {
                                    Path = @"C:\CamBridge\Watch",
                                    FilePattern = "*.jpg;*.jpeg",
                                    IncludeSubdirectories = false,
                                    MinimumFileAgeSeconds = 2
                                },
                                ProcessingOptions = new
                                {
                                    SuccessAction = "Archive",
                                    FailureAction = "MoveToError",
                                    DeleteSourceAfterSuccess = false,
                                    ProcessExistingOnStartup = true,
                                    MaxRetryAttempts = 3,
                                    RetryDelaySeconds = 30,
                                    ErrorFolder = @"C:\CamBridge\Errors",
                                    ArchiveFolder = @"C:\CamBridge\Output",
                                    BackupFolder = @"C:\CamBridge\Archive",
                                    CreateBackup = true,
                                    MaxConcurrentProcessing = 5,
                                    OutputOrganization = "ByPatientAndDate",
                                    OutputFilePattern = "{PatientId}_{StudyDate}_{Counter:0000}.dcm"
                                },
                                MappingSetId = "00000000-0000-0000-0000-000000000001"
                            }
                        },
                        MappingSets = new[]
                        {
                            new
                            {
                                Id = "00000000-0000-0000-0000-000000000001",
                                Name = "Ricoh Default",
                                Description = "Standard mapping for Ricoh G900 II cameras",
                                IsSystemDefault = true,
                                Rules = new[]
                                {
                                    new
                                    {
                                        Name = "PatientName",
                                        Description = "Patient's full name",
                                        SourceType = "QRBridge",
                                        SourceField = "name",
                                        DicomTag = "(0010,0010)",
                                        Transform = "None",
                                        Required = true,
                                        ValueRepresentation = "PN"
                                    },
                                    new
                                    {
                                        Name = "PatientID",
                                        Description = "Patient identifier",
                                        SourceType = "QRBridge",
                                        SourceField = "examid",
                                        DicomTag = "(0010,0020)",
                                        Transform = "None",
                                        Required = true,
                                        ValueRepresentation = "LO"
                                    }
                                }
                            }
                        },
                        GlobalDicomSettings = new
                        {
                            ImplementationClassUid = "1.2.276.0.7230010.3.0.3.6.4",
                            ImplementationVersionName = "CAMBRIDGE_0710",
                            SourceApplicationEntityTitle = "CAMBRIDGE",
                            Modality = "OT",
                            InstitutionName = "CamBridge Medical Imaging",
                            ValidateAfterCreation = true
                        },
                        DefaultProcessingOptions = new
                        {
                            SuccessAction = "Archive",
                            FailureAction = "MoveToError",
                            ArchiveFolder = @"C:\CamBridge\Output",
                            ErrorFolder = @"C:\CamBridge\Errors",
                            BackupFolder = @"C:\CamBridge\Archive",
                            CreateBackup = true,
                            MaxConcurrentProcessing = 5,
                            RetryOnFailure = true,
                            MaxRetryAttempts = 3,
                            RetryDelaySeconds = 30,
                            OutputOrganization = "ByPatientAndDate",
                            ProcessExistingOnStartup = true,
                            OutputFilePattern = "{PatientId}_{StudyDate}_{Counter:0000}.dcm"
                        },
                        Logging = new
                        {
                            LogLevel = "Information",
                            LogFolder = @"C:\ProgramData\CamBridge\Logs",
                            EnableFileLogging = true,
                            EnableEventLog = true,
                            MaxLogFileSizeMB = 10,
                            MaxLogFiles = 10
                        },
                        Notifications = new
                        {
                            Enabled = true,
                            DeadLetterThreshold = 100,
                            LogNotifications = true,
                            EventLog = new
                            {
                                Enabled = true,
                                LogName = "Application",
                                SourceName = "CamBridge"
                            }
                        },
                        ExifToolPath = "Tools\\exiftool.exe"
                    },
                    Logging = new
                    {
                        LogLevel = new
                        {
                            Default = "Information",
                            Microsoft = "Warning",
                            CamBridge = "Information"
                        }
                    }
                };

                var json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions
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
