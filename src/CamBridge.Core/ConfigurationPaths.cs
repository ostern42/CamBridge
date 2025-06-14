/**************************************************************************
*  ConfigurationPaths.cs                                                  *
*  PATH: src\CamBridge.Core\Infrastructure\ConfigurationPaths.cs         *
*  VERSION: 0.7.13 | SIZE: ~12KB | MODIFIED: 2025-06-14                  *
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
        /// FIXED v0.7.13: Now creates COMPLETE V2 format with VALID enum values!
        /// </summary>
        public static bool InitializePrimaryConfig()
        {
            try
            {
                var configPath = GetPrimaryConfigPath();
                if (!File.Exists(configPath))
                {
                    EnsureDirectoriesExist();

                    // Create COMPLETE default V2 configuration with CamBridge wrapper
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
                                ListenPort = 5111,  // CRITICAL: Must be ListenPort, not ApiPort!
                                EnableApi = true,
                                EnableHealthChecks = true,
                                MaxConcurrentPipelines = 4,
                                StartupDelaySeconds = 5,
                                ShutdownTimeoutSeconds = 30,
                                HealthCheckIntervalSeconds = 30
                            },
                            Core = new
                            {
                                ExifToolPath = "Tools\\exiftool.exe",
                                MaxGlobalConcurrency = 4,
                                DefaultCulture = "de-DE",
                                InstanceName = "CamBridge",
                                EnableDiagnostics = false,
                                TempFolder = @"C:\CamBridge\Temp",
                                CleanupTempOnStartup = true
                            },
                            Logging = new
                            {
                                LogLevel = new
                                {
                                    Default = "Information",
                                    Microsoft = "Warning",
                                    System = "Warning",
                                    CamBridge = "Information"
                                },
                                Console = new
                                {
                                    Enabled = true,
                                    IncludeScopes = false
                                },
                                File = new
                                {
                                    Enabled = true,
                                    Path = "logs/cambridge_{Date}.log",
                                    RetainedFileCountLimit = 30,
                                    FileSizeLimitBytes = 10485760,
                                    RollOnFileSizeLimit = true
                                }
                            },
                            DicomDefaults = new
                            {
                                InstitutionName = "CamBridge Medical Center",
                                InstitutionDepartment = "Radiology",
                                StationName = "CAMB001",
                                Manufacturer = "Claude's Improbably Reliable Software Solutions",
                                ManufacturerModelName = "CamBridge",
                                ImplementationClassUID = "1.2.276.0.7230010.3.0.3.6.4",
                                ImplementationVersionName = "CAMBRIDGE_001"
                            },
                            // CRITICAL: Must have at least one pipeline with VALID OutputOrganization!
                            Pipelines = new[]
                            {
                                new
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Name = "Default Pipeline",
                                    Description = "Default processing pipeline",
                                    Enabled = true,
                                    Priority = 0,
                                    WatchSettings = new
                                    {
                                        Path = @"C:\CamBridge\Watch",
                                        FilePattern = "*.jpg;*.jpeg",
                                        IncludeSubdirectories = false,
                                        OutputPath = @"C:\CamBridge\Output",
                                        MinimumFileAgeSeconds = 5
                                    },
                                    ProcessingOptions = new
                                    {
                                        SuccessAction = "Archive",
                                        FailureAction = "MoveToError",
                                        ArchiveFolder = @"C:\CamBridge\Archive",
                                        ErrorFolder = @"C:\CamBridge\Errors",
                                        BackupFolder = @"C:\CamBridge\Backup",
                                        CreateBackup = false,
                                        MaxConcurrentProcessing = 2,
                                        RetryOnFailure = true,
                                        MaxRetryAttempts = 3,
                                        // FIX: MUST use valid enum value as string!
                                        // Valid values: None, ByPatient, ByDate, ByPatientAndDate
                                        OutputOrganization = "ByPatientAndDate",  // NOT "PatientName"!
                                        ProcessExistingOnStartup = true,
                                        RetryDelaySeconds = 5,
                                        DeadLetterFolder = @"C:\CamBridge\DeadLetter",
                                        OutputFilePattern = "{PatientID}_{StudyDate}_{InstanceNumber}"
                                    },
                                    DicomOverrides = new
                                    {
                                        InstitutionName = (string?)null,
                                        InstitutionDepartment = (string?)null,
                                        StationName = (string?)null
                                    },
                                    MappingSetId = "default",
                                    CreatedAt = DateTime.UtcNow.ToString("O"),
                                    UpdatedAt = DateTime.UtcNow.ToString("O")
                                }
                            },
                            // Default mapping set matching the expected structure
                            MappingSets = new[]
                            {
                                new
                                {
                                    Id = "default",
                                    Name = "Default Mappings",
                                    Description = "Standard QRBridge to DICOM mappings",
                                    IsSystemDefault = true,
                                    Rules = new[]
                                    {
                                        new
                                        {
                                            name = "PatientID",
                                            description = "Patient ID from QRBridge",
                                            sourceType = "QRBridge",
                                            sourceField = "patientid",
                                            targetTag = "(0010,0020)",  // Use targetTag, not dicomTag
                                            dicomTag = "(0010,0020)",    // Keep both for compatibility
                                            transform = "None",
                                            required = true,
                                            defaultValue = (string?)null,
                                            valueRepresentation = "LO"
                                        },
                                        new
                                        {
                                            name = "PatientName",
                                            description = "Patient's Name from QRBridge",
                                            sourceType = "QRBridge",
                                            sourceField = "name",
                                            targetTag = "(0010,0010)",
                                            dicomTag = "(0010,0010)",
                                            transform = "None",
                                            required = true,
                                            defaultValue = (string?)null,
                                            valueRepresentation = "PN"
                                        },
                                        new
                                        {
                                            name = "StudyID",
                                            description = "Study ID from exam ID",
                                            sourceType = "QRBridge",
                                            sourceField = "examid",
                                            targetTag = "(0020,0010)",
                                            dicomTag = "(0020,0010)",
                                            transform = "None",
                                            required = false,
                                            defaultValue = (string?)null,
                                            valueRepresentation = "SH"
                                        },
                                        new
                                        {
                                            name = "PatientBirthDate",
                                            description = "Patient's Birth Date",
                                            sourceType = "QRBridge",
                                            sourceField = "birthdate",
                                            targetTag = "(0010,0030)",
                                            dicomTag = "(0010,0030)",
                                            transform = "DateFormat",
                                            required = false,
                                            defaultValue = (string?)null,
                                            valueRepresentation = "DA"
                                        },
                                        new
                                        {
                                            name = "PatientSex",
                                            description = "Patient's Sex",
                                            sourceType = "QRBridge",
                                            sourceField = "gender",
                                            targetTag = "(0010,0040)",
                                            dicomTag = "(0010,0040)",
                                            transform = "None",
                                            required = false,
                                            defaultValue = (string?)null,
                                            valueRepresentation = "CS"
                                        }
                                    },
                                    CreatedAt = DateTime.UtcNow.ToString("O"),
                                    UpdatedAt = DateTime.UtcNow.ToString("O")
                                }
                            },
                            Notifications = new
                            {
                                Enabled = false,
                                SmtpServer = "smtp.example.com",
                                SmtpPort = 587,
                                UseSsl = true,
                                RequiresAuthentication = true,
                                SenderEmail = "cambridge@example.com",
                                SenderName = "CamBridge Service",
                                RecipientEmails = new[] { "admin@example.com" },
                                NotificationLevel = 2,  // Error level
                                DailySummary = new
                                {
                                    Enabled = false,
                                    SendTime = "08:00:00",
                                    IncludeStatistics = true,
                                    IncludeErrors = true
                                }
                            },
                            // V2 format should NOT have these at root level
                            // They belong in Pipelines[].ProcessingOptions
                            DefaultOutputFolder = @"C:\CamBridge\Output",
                            ExifToolPath = "Tools\\exiftool.exe"
                        }
                    };

                    var json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    File.WriteAllText(configPath, json);
                    System.Diagnostics.Debug.WriteLine($"Created COMPLETE V2 config at: {configPath}");
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
