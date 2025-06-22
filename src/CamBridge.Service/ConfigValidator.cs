// src/CamBridge.Service/ConfigValidator.cs
// Version: 0.7.28
// Description: Validates configuration and warns about invalid values
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using CamBridge.Core;
using Serilog;

namespace CamBridge.Service
{
    /// <summary>
    /// Validates configuration JSON and warns about invalid enum values
    /// </summary>
    public static class ConfigValidator
    {
        /// <summary>
        /// Validates the settings object (new method for v0.7.28)
        /// </summary>
        public static void ValidateSettings(CamBridgeSettingsV2 settings)
        {
            // Basic validation that was expected by Program.cs
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrEmpty(settings.Version))
                throw new InvalidOperationException("Settings version is required");

            if (settings.Version != "2.0")
                throw new InvalidOperationException($"Unsupported settings version: {settings.Version}. Expected: 2.0");

            // Validate pipelines
            if (settings.Pipelines == null || settings.Pipelines.Count == 0)
                throw new InvalidOperationException("At least one pipeline must be configured");

            foreach (var pipeline in settings.Pipelines)
            {
                if (string.IsNullOrEmpty(pipeline.Name))
                    throw new InvalidOperationException($"Pipeline {pipeline.Id} must have a name");

                if (pipeline.WatchSettings == null)
                    throw new InvalidOperationException($"Pipeline {pipeline.Name} must have watch settings");

                if (string.IsNullOrEmpty(pipeline.WatchSettings.Path))
                    throw new InvalidOperationException($"Pipeline {pipeline.Name} must have a watch path");
            }
        }

        /// <summary>
        /// Validates the configuration file and warns about any issues
        /// </summary>
        public static void ValidateAndWarn(string configPath, Serilog.ILogger logger)
        {
            try
            {
                if (!File.Exists(configPath))
                    return;

                var configJson = File.ReadAllText(configPath);
                var warnings = new List<string>();

                // Check for invalid OutputOrganization values
                var validOutputOrgValues = new[] { "None", "ByPatient", "ByDate", "ByPatientAndDate" };

                // Find all OutputOrganization values in the JSON
                var outputOrgMatches = Regex.Matches(configJson, @"""OutputOrganization""\s*:\s*""([^""]+)""");
                foreach (Match match in outputOrgMatches)
                {
                    var value = match.Groups[1].Value;
                    if (!validOutputOrgValues.Contains(value, StringComparer.OrdinalIgnoreCase))
                    {
                        warnings.Add($"Invalid OutputOrganization value '{value}' found. Valid values are: {string.Join(", ", validOutputOrgValues)}");
                    }
                }

                // Check for invalid PostProcessingAction values
                var validActionValues = new[] { "Leave", "Archive", "Delete", "MoveToError" };

                var actionPatterns = new[]
                {
                    @"""SuccessAction""\s*:\s*""([^""]+)""",
                    @"""FailureAction""\s*:\s*""([^""]+)"""
                };

                foreach (var pattern in actionPatterns)
                {
                    var matches = Regex.Matches(configJson, pattern);
                    foreach (Match match in matches)
                    {
                        var value = match.Groups[1].Value;
                        if (!validActionValues.Contains(value, StringComparer.OrdinalIgnoreCase))
                        {
                            warnings.Add($"Invalid PostProcessingAction value '{value}' found. Valid values are: {string.Join(", ", validActionValues)}");
                        }
                    }
                }

                // Try to parse and count pipelines
                try
                {
                    using var doc = JsonDocument.Parse(configJson);
                    if (doc.RootElement.TryGetProperty("CamBridge", out var camBridge))
                    {
                        if (camBridge.TryGetProperty("Pipelines", out var pipelines))
                        {
                            var totalPipelines = pipelines.GetArrayLength();
                            var validPipelines = 0;

                            foreach (var pipeline in pipelines.EnumerateArray())
                            {
                                var hasValidConfig = true;
                                string? pipelineName = null;

                                if (pipeline.TryGetProperty("Name", out var nameElement))
                                {
                                    pipelineName = nameElement.GetString();
                                }

                                if (pipeline.TryGetProperty("ProcessingOptions", out var options))
                                {
                                    if (options.TryGetProperty("OutputOrganization", out var outputOrg))
                                    {
                                        var value = outputOrg.GetString();
                                        if (!string.IsNullOrEmpty(value) &&
                                            !validOutputOrgValues.Contains(value, StringComparer.OrdinalIgnoreCase))
                                        {
                                            hasValidConfig = false;
                                        }
                                    }
                                }

                                if (hasValidConfig)
                                    validPipelines++;
                                else if (!string.IsNullOrEmpty(pipelineName))
                                    warnings.Add($"Pipeline '{pipelineName}' may not load due to invalid configuration");
                            }

                            if (validPipelines < totalPipelines)
                            {
                                warnings.Add($"Only {validPipelines} of {totalPipelines} pipelines will be loaded due to configuration errors");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Warning("Could not parse config for detailed validation: {Message}", ex.Message);
                }

                // Output all warnings
                if (warnings.Any())
                {
                    logger.Warning("⚠️ CONFIGURATION WARNINGS ⚠️");
                    logger.Warning("============================");
                    foreach (var warning in warnings)
                    {
                        logger.Warning("⚠️ {Warning}", warning);
                    }
                    logger.Warning("============================");
                    logger.Warning("Service will continue but some pipelines may not load correctly!");
                    logger.Warning("Use the Config Tool to fix these issues.");
                }
                else
                {
                    logger.Information("✅ Configuration validation passed - no issues found");
                }
            }
            catch (Exception ex)
            {
                logger.Warning("Could not validate configuration: {Message}", ex.Message);
            }
        }
    }
}
