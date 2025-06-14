/**************************************************************************
*  ConfigurationPaths.cs                                                  *
*  PATH: src\CamBridge.Core\Infrastructure\ConfigurationPaths.cs         *
*  VERSION: 0.7.12 | SIZE: ~10KB | MODIFIED: 2025-06-14                  *
*                                                                         *
*  DESCRIPTION: Centralized configuration path management with V2 FIX     *
*  Copyright (c) 2025 Claude's Improbably Reliable Software Solutions     *
**************************************************************************/

using System;
using System.IO;
using System.Text.Json;

namespace CamBridge.Core.Infrastructure
{
    /// <summary>
    /// Centralized configuration path management for CamBridge
    /// Single source of truth for all configuration locations
    /// </summary>
    public static class ConfigurationPaths
    {
        /// <summary>
        /// Gets the base directory for all CamBridge data in ProgramData
        /// </summary>
        public static string ProgramDataBase => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "CamBridge");

        /// <summary>
        /// Gets the path to the primary configuration file
        /// </summary>
        public static string GetPrimaryConfigPath()
        {
            return Path.Combine(ProgramDataBase, "appsettings.json");
        }

        /// <summary>
        /// Gets the directory for pipeline configurations
        /// </summary>
        public static string GetPipelineConfigDirectory()
        {
            return Path.Combine(ProgramDataBase, "Pipelines");
        }

        /// <summary>
        /// Gets the directory for mapping rules
        /// </summary>
        public static string GetMappingRulesDirectory()
        {
            return Path.Combine(ProgramDataBase, "Mappings");
        }

        /// <summary>
        /// Gets the directory for error files
        /// </summary>
        public static string GetErrorDirectory()
        {
            return Path.Combine(@"C:\CamBridge", "Errors");
        }

        /// <summary>
        /// Gets the directory for processing temporary files
        /// </summary>
        public static string GetProcessingDirectory()
        {
            return Path.Combine(ProgramDataBase, "Processing");
        }

        /// <summary>
        /// Gets the directory for completed files
        /// </summary>
        public static string GetCompletedDirectory()
        {
            return Path.Combine(ProgramDataBase, "Completed");
        }

        /// <summary>
        /// Gets the directory for logs
        /// </summary>
        public static string GetLogsDirectory()
        {
            return Path.Combine(ProgramDataBase, "Logs");
        }

        /// <summary>
        /// Gets the user preferences file path
        /// </summary>
        public static string GetUserPreferencesPath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "CamBridge", "preferences.json");
        }

        /// <summary>
        /// Ensures all required directories exist
        /// </summary>
        public static void EnsureDirectoriesExist()
        {
            Directory.CreateDirectory(ProgramDataBase);
            Directory.CreateDirectory(GetPipelineConfigDirectory());
            Directory.CreateDirectory(GetMappingRulesDirectory());
            Directory.CreateDirectory(GetErrorDirectory());
            Directory.CreateDirectory(GetProcessingDirectory());
            Directory.CreateDirectory(GetCompletedDirectory());
            Directory.CreateDirectory(GetLogsDirectory());
        }

        /// <summary>
        /// Checks if primary config exists
        /// </summary>
        public static bool PrimaryConfigExists()
        {
            return File.Exists(GetPrimaryConfigPath());
        }

        /// <summary>
        /// Initializes the primary configuration file if it doesn't exist
        /// FIXED: Now creates proper V2 format with CamBridge wrapper!
        /// </summary>
        public static bool InitializePrimaryConfig()
        {
            try
            {
                var configPath = GetPrimaryConfigPath();
                if (!File.Exists(configPath))
                {
                    EnsureDirectoriesExist();

                    // Create default V2 configuration with CamBridge wrapper
                    var defaultConfig = new
                    {
                        // CRITICAL: Must have CamBridge wrapper for V2 format!
                        CamBridge = new
                        {
                            Version = "2.0",
                            Service = new
                            {
                                ServiceName = "CamBridgeService",
                                DisplayName = "CamBridge Image Processing Service",
                                Description = "Monitors folders for JPEG images and converts them to DICOM format",
                                ApiPort = 5111,  // CORRECT PORT!
                                EnableHealthChecks = true,
                                LogLevel = "Information"
                            },
                            Pipelines = new[]
                            {
                                new
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Name = "Default Pipeline",
                                    Description = "Default image processing pipeline",
                                    Enabled = true,
                                    WatchSettings = new
                                    {
                                        Path = "C:\\CamBridge\\Input",
                                        FilePattern = "*.jpg",
                                        IncludeSubdirectories = false,
                                        OutputPath = "C:\\CamBridge\\Output"
                                    },
                                    ProcessingOptions = new
                                    {
                                        OutputOrganization = "PatientName",
                                        OutputFilePattern = "{PatientName}_{StudyDate}_{Modality}_{Counter:D4}.dcm",
                                        CreateBackup = true,
                                        BackupFolder = "C:\\CamBridge\\Backup",
                                        ArchiveFolder = "C:\\CamBridge\\Archive",
                                        ArchiveProcessedFiles = true,
                                        DeleteAfterProcessing = false,
                                        MinimumFileAgeSeconds = 5,
                                        MaxRetryAttempts = 3,
                                        RetryDelaySeconds = 30
                                    },
                                    DicomOverrides = new
                                    {
                                        InstitutionName = "CamBridge Medical Center",
                                        StationName = "CAMBRIDGE01",
                                        AdditionalTags = new { }
                                    },
                                    MappingSetId = (string?)null,
                                    ErrorHandling = new
                                    {
                                        ErrorFolder = "C:\\CamBridge\\Errors",
                                        MoveFailedFiles = true
                                    }
                                }
                            },
                            MappingSets = new[]
                            {
                                new
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Name = "[System] Ricoh G900 II Default",
                                    Description = "Default mapping for Ricoh G900 II camera with QRBridge",
                                    IsSystemDefault = true,
                                    Rules = new object[]
                                    {
                                        new
                                        {
                                            SourceField = "PatientId",
                                            TargetTag = "(0010,0020)",
                                            Description = "Patient ID",
                                            IsRequired = true,
                                            ValueRepresentation = "LO"
                                        },
                                        new
                                        {
                                            SourceField = "PatientName",
                                            TargetTag = "(0010,0010)",
                                            Description = "Patient's Name",
                                            IsRequired = true,
                                            ValueRepresentation = "PN"
                                        },
                                        new
                                        {
                                            SourceField = "PatientBirthDate",
                                            TargetTag = "(0010,0030)",
                                            Description = "Patient's Birth Date",
                                            Transform = "DateFormat",
                                            IsRequired = false,
                                            ValueRepresentation = "DA"
                                        },
                                        new
                                        {
                                            SourceField = "PatientSex",
                                            TargetTag = "(0010,0040)",
                                            Description = "Patient's Sex",
                                            IsRequired = false,
                                            ValueRepresentation = "CS"
                                        },
                                        new
                                        {
                                            SourceField = "StudyDescription",
                                            TargetTag = "(0008,1030)",
                                            Description = "Study Description",
                                            IsRequired = false,
                                            ValueRepresentation = "LO"
                                        }
                                    }
                                }
                            },
                            Logging = new
                            {
                                LogLevel = "Information",
                                LogFolder = GetLogsDirectory(),
                                MaxLogFiles = 30,
                                EnableFileLogging = true,
                                EnableConsoleLogging = true,
                                EnableDebugLogging = false
                            },
                            ExifToolPath = "Tools\\exiftool.exe",
                            GlobalDicomSettings = new
                            {
                                ImplementationClassUID = "1.2.276.0.7230010.3.0.3.6.4",
                                ImplementationVersionName = "CAMBRIDGE_001"
                            },
                            DefaultProcessingOptions = new
                            {
                                OutputOrganization = "PatientName",
                                OutputFilePattern = "{PatientName}_{StudyDate}_{Modality}_{Counter:D4}.dcm",
                                CreateBackup = true,
                                BackupFolder = "C:\\CamBridge\\Backup",
                                ArchiveFolder = "C:\\CamBridge\\Archive",
                                ArchiveProcessedFiles = true,
                                DeleteAfterProcessing = false,
                                MinimumFileAgeSeconds = 5,
                                MaxRetryAttempts = 3,
                                RetryDelaySeconds = 30
                            },
                            Notifications = new
                            {
                                EnableNotifications = false,
                                EnableEmailNotifications = false,
                                EnableDailySummary = false,
                                DailySummaryTime = "18:00"
                            }
                        }
                    };

                    var json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    File.WriteAllText(configPath, json);
                    System.Diagnostics.Debug.WriteLine($"Created default config at: {configPath}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log error but don't throw - let the application continue
                System.Diagnostics.Debug.WriteLine($"Error initializing config: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets diagnostic information about configuration paths
        /// </summary>
        public static string GetDiagnosticInfo()
        {
            return $@"Configuration Paths:
- ProgramData Base: {ProgramDataBase}
- Primary Config: {GetPrimaryConfigPath()}
- Config Exists: {PrimaryConfigExists()}
- Pipelines Dir: {GetPipelineConfigDirectory()}
- Mappings Dir: {GetMappingRulesDirectory()}
- Error Dir: {GetErrorDirectory()}
- Logs Dir: {GetLogsDirectory()}";
        }

        /// <summary>
        /// Copies local config to primary location if needed
        /// </summary>
        public static bool CopyLocalConfigIfNeeded(string localPath)
        {
            try
            {
                if (!File.Exists(localPath)) return false;

                var primaryPath = GetPrimaryConfigPath();
                if (!File.Exists(primaryPath))
                {
                    EnsureDirectoriesExist();
                    File.Copy(localPath, primaryPath, true);
                    System.Diagnostics.Debug.WriteLine($"Copied local config to: {primaryPath}");
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
