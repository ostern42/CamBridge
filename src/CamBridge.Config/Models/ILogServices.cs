// src/CamBridge.Config/Services/Interfaces/ILogServices.cs
// Version: 0.8.16
// Description: Service interfaces for log viewer functionality
// Copyright: (C) 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CamBridge.Config.Models;
using CamBridge.Core.Enums;
using Microsoft.Extensions.Logging;

namespace CamBridge.Config.Services.Interfaces
{
    /// <summary>
    /// Service for file operations and monitoring
    /// </summary>
    public interface ILogFileService
    {
        /// <summary>
        /// Read log entries from a file
        /// </summary>
        Task<List<LogEntry>> ReadLogFileAsync(string logPath, string sourcePipeline);

        /// <summary>
        /// Get the log file name for a pipeline
        /// </summary>
        string GetLogFileName(string pipelineSelection);

        /// <summary>
        /// Get available log files for pipelines
        /// </summary>
        Task<Dictionary<string, string>> GetAvailableLogFilesAsync();

        /// <summary>
        /// Start watching log files for changes
        /// </summary>
        void StartWatching(IEnumerable<string> logFiles, Action<string> onFileChanged);

        /// <summary>
        /// Stop watching log files
        /// </summary>
        void StopWatching();
    }

    /// <summary>
    /// Service for parsing log lines into structured entries
    /// </summary>
    public interface ILogParsingService
    {
        /// <summary>
        /// Parse a single log line
        /// </summary>
        LogEntry? ParseLogLine(string line);

        /// <summary>
        /// Parse log level from string
        /// </summary>
        LogLevel ParseLogLevel(string levelStr);

        /// <summary>
        /// Parse processing stage from string
        /// </summary>
        ProcessingStage? ParseStage(string stageStr);

        /// <summary>
        /// Infer stage from message content
        /// </summary>
        ProcessingStage? InferStageFromMessage(string message);
    }

    /// <summary>
    /// Service for filtering log entries
    /// </summary>
    public interface ILogFilterService
    {
        /// <summary>
        /// Apply filters to log entries
        /// </summary>
        List<LogEntry> ApplyFilters(
            IEnumerable<LogEntry> entries,
            LogFilterCriteria criteria);

        /// <summary>
        /// Check if text matches wildcard pattern
        /// </summary>
        bool MatchesWildcard(string text, string pattern);
    }

    /// <summary>
    /// Service for building hierarchical log views
    /// </summary>
    public interface ILogTreeBuilder
    {
        /// <summary>
        /// Build correlation groups from log entries
        /// </summary>
        List<CorrelationGroup> BuildCorrelationGroups(
            IEnumerable<LogEntry> entries,
            bool applyTextFilters = false,
            params string[] textFilters);
    }

    /// <summary>
    /// Filter criteria for log entries
    /// </summary>
    public class LogFilterCriteria
    {
        public bool ShowDebug { get; set; }
        public bool ShowInformation { get; set; }
        public bool ShowWarning { get; set; }
        public bool ShowError { get; set; }
        public bool ShowCritical { get; set; }
        public string? SearchText { get; set; }
        public string? Filter1 { get; set; }
        public string? Filter2 { get; set; }
        public string? Filter3 { get; set; }
    }
}
