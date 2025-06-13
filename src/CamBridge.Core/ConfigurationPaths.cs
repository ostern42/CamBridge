/**************************************************************************
*  ConfigurationPaths.cs                                                  *
*  PATH: src\CamBridge.Core\Infrastructure\ConfigurationPaths.cs         *
*  VERSION: 0.7.11 | SIZE: ~6KB | MODIFIED: 2025-06-13                   *
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
                            ServicePort = 5111,
                            EnableSwagger = true,
                            EnableHealthChecks = true,
                            LogLevel = "Information",
                            AutoStartPipelines = true,
                            MaxConcurrentProcessing = 4,
                            ProcessingTimeoutMinutes = 30,
                            RetryCount = 3,
                            RetryDelaySeconds = 5,
                            CleanupIntervalMinutes = 60,
                            KeepCompletedDays = 7,
                            EnableMetrics = true,
                            MetricsRetentionDays = 30,
                            DefaultPipelineMode = "Watching",
                            WatcherPollingIntervalSeconds = 5,
                            EnableDetailedLogging = false,
                            MaxLogFileSizeMB = 100,
                            MaxLogFiles = 10,
                            ErrorRetentionDays = 30,
                            EnableNotifications = false,
                            SmtpServer = "",
                            SmtpPort = 587,
                            SmtpUser = "",
                            SmtpPassword = "",
                            NotificationRecipients = "",
                            EnableApiAuthentication = false,
                            ApiKey = "",
                            AllowedOrigins = "http://localhost:*",
                            EnableCors = true,
                            MaxUploadSizeMB = 100,
                            TempFileCleanupHours = 24,
                            EnableAutoUpdate = false,
                            UpdateCheckIntervalHours = 24,
                            UpdateUrl = "",

                            // Pipeline configurations (empty by default)
                            Pipelines = new object[] { }
                        }
                    };

                    var json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    File.WriteAllText(configPath, json);
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
        /// Gets the path for a specific pipeline configuration
        /// </summary>
        public static string GetPipelineConfigPath(string pipelineName)
        {
            var safeName = string.Join("_", pipelineName.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(GetPipelineConfigDirectory(), $"{safeName}.json");
        }

        /// <summary>
        /// Gets the path for a specific mapping rule set
        /// </summary>
        public static string GetMappingRulePath(string mappingName)
        {
            var safeName = string.Join("_", mappingName.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(GetMappingRulesDirectory(), $"{safeName}.json");
        }

        /// <summary>
        /// Checks if running in development mode (useful for testing)
        /// </summary>
        public static bool IsDevMode()
        {
            return Environment.GetEnvironmentVariable("CAMBRIDGE_DEV_MODE") == "true";
        }

        /// <summary>
        /// Gets the base path, considering dev mode
        /// </summary>
        public static string GetBasePath()
        {
            if (IsDevMode())
            {
                return Path.Combine(Environment.CurrentDirectory, "DevData");
            }
            return ProgramDataBase;
        }
    }
}
