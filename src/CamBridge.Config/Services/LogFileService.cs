// src/CamBridge.Config/Services/LogFileService.cs
// Version: 0.8.16
// Description: Service for log file operations and monitoring
// Copyright: (C) 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Config.Models;
using CamBridge.Config.Services.Interfaces;
using CamBridge.Core;
using CamBridge.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Service implementation for file operations
    /// </summary>
    public class LogFileService : ILogFileService
    {
        private readonly ILogger<LogFileService> _logger;
        private readonly ILogParsingService _parsingService;
        private FileSystemWatcher? _logWatcher;
        private readonly Dictionary<string, long> _filePositions;

        public LogFileService(
            ILogger<LogFileService> logger,
            ILogParsingService parsingService)
        {
            _logger = logger;
            _parsingService = parsingService;
            _filePositions = new Dictionary<string, long>();
        }

        public async Task<List<LogEntry>> ReadLogFileAsync(string logPath, string sourcePipeline)
        {
            var entries = new List<LogEntry>();

            try
            {
                _logger.LogInformation("Reading log file: {Path} for pipeline: {Pipeline}", logPath, sourcePipeline);

                using (var fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fs, Encoding.UTF8))
                {
                    string? line;
                    int lineCount = 0;
                    int parsedCount = 0;

                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        lineCount++;
                        var entry = _parsingService.ParseLogLine(line);
                        if (entry != null)
                        {
                            entry.Source = sourcePipeline;
                            entries.Add(entry);
                            parsedCount++;
                        }
                    }

                    _logger.LogInformation("Read {Lines} lines, parsed {Parsed} entries from {File}",
                        lineCount, parsedCount, Path.GetFileName(logPath));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read log file: {LogPath}", logPath);
            }

            return entries;
        }

        public string GetLogFileName(string pipelineSelection)
        {
            var today = DateTime.Now.ToString("yyyyMMdd");

            if (pipelineSelection == "Service (Global)")
            {
                return $"service_{today}.log";
            }

            if (pipelineSelection == "All Pipelines (Legacy)")
            {
                return $"pipelines_{today}.log";
            }

            // Handle archived pipelines
            if (pipelineSelection.EndsWith(" (Archived)"))
            {
                pipelineSelection = pipelineSelection.Replace(" (Archived)", "");
            }

            // Sanitize the pipeline name
            var safeName = SanitizeForFileName(pipelineSelection);
            return $"pipeline_{safeName}_{today}.log";
        }

        public async Task<Dictionary<string, string>> GetAvailableLogFilesAsync()
        {
            var result = new Dictionary<string, string>();

            try
            {
                var logPath = ConfigurationPaths.GetLogsDirectory();
                if (Directory.Exists(logPath))
                {
                    var pipelineLogFiles = Directory.GetFiles(logPath, "pipeline_*.log")
                        .Select(Path.GetFileNameWithoutExtension)
                        .Where(f => f != null && f.StartsWith("pipeline_"))
                        .Select(f => f!.Substring("pipeline_".Length))
                        .Distinct();

                    foreach (var logName in pipelineLogFiles)
                    {
                        // Extract the pipeline name (remove date suffix)
                        var parts = logName.Split('_');
                        if (parts.Length > 1 && parts[parts.Length - 1].Length == 8)
                        {
                            var pipelineName = string.Join("_", parts.Take(parts.Length - 1));
                            var displayName = $"{pipelineName} (Archived)";
                            result[displayName] = logName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get available log files");
            }

            return await Task.FromResult(result);
        }

        public void StartWatching(IEnumerable<string> logFiles, Action<string> onFileChanged)
        {
            StopWatching();

            try
            {
                var logPath = ConfigurationPaths.GetLogsDirectory();
                if (!Directory.Exists(logPath))
                    return;

                _logWatcher = new FileSystemWatcher(logPath)
                {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                    EnableRaisingEvents = true
                };

                // Watch for changes in any log file
                foreach (var file in logFiles)
                {
                    _logWatcher.Filter = Path.GetFileName(file);
                }

                _logWatcher.Changed += (sender, e) =>
                {
                    if (logFiles.Contains(e.FullPath))
                    {
                        onFileChanged(e.FullPath);
                    }
                };

                _logger.LogInformation("Started watching {Count} log files", logFiles.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start file watching");
            }
        }

        public void StopWatching()
        {
            _logWatcher?.Dispose();
            _logWatcher = null;
        }

        private string SanitizeForFileName(string pipelineName)
        {
            var invalid = Path.GetInvalidFileNameChars()
                .Concat(new[] { ' ', '.', ',', '/', '\\', ':', '-' })
                .Distinct()
                .ToArray();

            var sanitized = string.Join("_", pipelineName.Split(invalid, StringSplitOptions.RemoveEmptyEntries));

            if (sanitized.Length > 100)
            {
                sanitized = sanitized.Substring(0, 97) + "...";
            }

            return sanitized;
        }
    }
}
