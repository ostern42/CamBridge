// src/CamBridge.Config/Services/LogParsingService.cs
// Version: 0.8.16
// Description: Service for parsing log lines into structured entries
// Copyright: (C) 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using CamBridge.Config.Models;
using CamBridge.Config.Services.Interfaces;
using CamBridge.Core.Enums;
using Microsoft.Extensions.Logging;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Service implementation for log line parsing
    /// </summary>
    public class LogParsingService : ILogParsingService
    {
        private readonly ILogger<LogParsingService> _logger;
        private static int _debugLineCount = 0;

        public LogParsingService(ILogger<LogParsingService> logger)
        {
            _logger = logger;
        }

        public LogEntry? ParseLogLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            try
            {
                // DEBUG output for first few lines
                if (_debugLineCount < 10)
                {
                    _debugLineCount++;
                    _logger.LogDebug("Parsing line: {Line}", line);
                }

                // Pattern 1: Full format with stage in quotes
                // [HH:mm:ss LEVEL] [CorrelationId] ["Stage"] Message [Pipeline]
                var fullFormatQuoted = Regex.Match(line,
                    @"^\[(\d{2}:\d{2}:\d{2})\s+(\w+)\]\s+\[([^\]]+)\]\s+\[""([^""]+)""\]\s+(.+)$");

                if (fullFormatQuoted.Success)
                {
                    return ParseFullFormat(line, fullFormatQuoted, true);
                }

                // Pattern 2: Full format with stage without quotes
                // [HH:mm:ss LEVEL] [CorrelationId] [Stage] Message [Pipeline]
                var fullFormatUnquoted = Regex.Match(line,
                    @"^\[(\d{2}:\d{2}:\d{2})\s+(\w+)\]\s+\[([^\]]+)\]\s+\[([^\]]+)\]\s+(.+)$");

                if (fullFormatUnquoted.Success)
                {
                    return ParseFullFormat(line, fullFormatUnquoted, false);
                }

                // Pattern 3: Format with correlation ID but no stage
                // [HH:mm:ss LEVEL] [CorrelationId] Message
                var noStageFormat = Regex.Match(line,
                    @"^\[(\d{2}:\d{2}:\d{2})\s+(\w+)\]\s+\[([^\]]+)\]\s+(.+)$");

                if (noStageFormat.Success)
                {
                    var timeStr = noStageFormat.Groups[1].Value;
                    var levelStr = noStageFormat.Groups[2].Value;
                    var correlationId = noStageFormat.Groups[3].Value;
                    var message = noStageFormat.Groups[4].Value;

                    // Parse timestamp
                    var timestamp = DateTime.ParseExact(timeStr, "HH:mm:ss", CultureInfo.InvariantCulture);
                    timestamp = DateTime.Today.Add(timestamp.TimeOfDay);

                    // Extract pipeline if present at end
                    string? pipeline = null;
                    var pipelineMatch = Regex.Match(message, @"\s*\[([^\]]+)\]\s*$");
                    if (pipelineMatch.Success)
                    {
                        pipeline = pipelineMatch.Groups[1].Value;
                        message = message.Substring(0, message.Length - pipelineMatch.Value.Length).Trim();
                    }

                    // Try to infer stage from message patterns
                    ProcessingStage? stage = InferStageFromMessage(message);

                    return new LogEntry
                    {
                        Timestamp = timestamp,
                        Level = ParseLogLevel(levelStr),
                        Message = message,
                        RawLine = line,
                        CorrelationId = correlationId,
                        Stage = stage,
                        Pipeline = pipeline
                    };
                }

                // Pattern 4: Simple format without correlation ID
                // [HH:mm:ss LEVEL] Message
                var simpleFormat = Regex.Match(line,
                    @"^\[(\d{2}:\d{2}:\d{2})\s+(\w+)\]\s+(.+)$");

                if (simpleFormat.Success)
                {
                    var timeStr = simpleFormat.Groups[1].Value;
                    var levelStr = simpleFormat.Groups[2].Value;
                    var message = simpleFormat.Groups[3].Value;

                    var timestamp = DateTime.ParseExact(timeStr, "HH:mm:ss", CultureInfo.InvariantCulture);
                    timestamp = DateTime.Today.Add(timestamp.TimeOfDay);

                    return new LogEntry
                    {
                        Timestamp = timestamp,
                        Level = ParseLogLevel(levelStr),
                        Message = message,
                        RawLine = line
                    };
                }

                // Fallback: return as-is
                return new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Level = LogLevel.Information,
                    Message = line,
                    RawLine = line
                };
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to parse log line: {Line}", line);
                return new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Level = LogLevel.Information,
                    Message = line,
                    RawLine = line
                };
            }
        }

        public LogLevel ParseLogLevel(string levelStr)
        {
            return levelStr.ToUpperInvariant() switch
            {
                "DBG" or "DEB" or "DEBUG" => LogLevel.Debug,
                "INF" or "INFO" or "INFORMATION" => LogLevel.Information,
                "WRN" or "WARN" or "WARNING" => LogLevel.Warning,
                "ERR" or "ERROR" => LogLevel.Error,
                "CRT" or "CRIT" or "CRITICAL" or "FTL" or "FATAL" => LogLevel.Critical,
                _ => LogLevel.Information
            };
        }

        public ProcessingStage? ParseStage(string stageStr)
        {
            if (string.IsNullOrWhiteSpace(stageStr))
                return null;

            // Try exact match first
            if (Enum.TryParse<ProcessingStage>(stageStr, true, out var stage))
                return stage;

            // Try some common mappings
            var stageLower = stageStr.ToLowerInvariant();

            if (stageLower.Contains("startup"))
                return ProcessingStage.ServiceStartup;
            if (stageLower.Contains("shutdown"))
                return ProcessingStage.ServiceShutdown;
            if (stageLower.Contains("initialization") || stageLower.Contains("init"))
                return ProcessingStage.PipelineInitialization;
            if (stageLower.Contains("detected"))
                return ProcessingStage.FileDetected;
            if (stageLower.Contains("exif"))
                return ProcessingStage.ExifExtraction;
            if (stageLower.Contains("mapping"))
                return ProcessingStage.TagMapping;
            if (stageLower.Contains("dicom"))
                return ProcessingStage.DicomConversion;
            if (stageLower.Contains("pacs"))
                return ProcessingStage.PacsUpload;
            if (stageLower.Contains("complete"))
                return ProcessingStage.Complete;
            if (stageLower.Contains("error") || stageLower.Contains("failed"))
                return ProcessingStage.Error;

            return null;
        }

        public ProcessingStage? InferStageFromMessage(string message)
        {
            var messageLower = message.ToLowerInvariant();

            if (messageLower.Contains("starting") && messageLower.Contains("pipeline"))
                return ProcessingStage.PipelineInitialization;
            if (messageLower.Contains("processing file") || messageLower.Contains("file detected"))
                return ProcessingStage.FileDetected;
            if (messageLower.Contains("exif") || messageLower.Contains("metadata"))
                return ProcessingStage.ExifExtraction;
            if (messageLower.Contains("mapping") || messageLower.Contains("tag"))
                return ProcessingStage.TagMapping;
            if (messageLower.Contains("dicom") || messageLower.Contains("conversion"))
                return ProcessingStage.DicomConversion;
            if (messageLower.Contains("pacs") || messageLower.Contains("upload"))
                return ProcessingStage.PacsUpload;
            if (messageLower.Contains("success") || messageLower.Contains("complete"))
                return ProcessingStage.Complete;
            if (messageLower.Contains("error") || messageLower.Contains("failed"))
                return ProcessingStage.Error;

            return null;
        }

        private LogEntry ParseFullFormat(string line, Match match, bool stageIsQuoted)
        {
            var timeStr = match.Groups[1].Value;
            var levelStr = match.Groups[2].Value;
            var correlationId = match.Groups[3].Value;
            var stageStr = match.Groups[4].Value;
            var messageAndMore = match.Groups[5].Value;

            // Parse timestamp
            var timestamp = DateTime.ParseExact(timeStr, "HH:mm:ss", CultureInfo.InvariantCulture);
            timestamp = DateTime.Today.Add(timestamp.TimeOfDay);

            // Parse log level
            var level = ParseLogLevel(levelStr);

            // Parse stage
            ProcessingStage? stage = ParseStage(stageStr);

            // Extract message and pipeline
            var message = messageAndMore;
            string? pipeline = null;

            // Check for pipeline at end: [Radiology]
            var pipelineMatch = Regex.Match(messageAndMore, @"^(.+?)\s*\[([^\]]+)\]\s*$");
            if (pipelineMatch.Success)
            {
                message = pipelineMatch.Groups[1].Value.Trim();
                pipeline = pipelineMatch.Groups[2].Value;
            }

            // Extract duration if present
            int? durationMs = null;
            var durationMatch = Regex.Match(message, @"\[(\d+)ms\]");
            if (durationMatch.Success)
            {
                durationMs = int.Parse(durationMatch.Groups[1].Value);
                message = message.Replace(durationMatch.Value, "").Trim();
            }

            return new LogEntry
            {
                Timestamp = timestamp,
                Level = level,
                Message = message,
                RawLine = line,
                CorrelationId = correlationId,
                Stage = stage,
                Pipeline = pipeline,
                DurationMs = durationMs
            };
        }
    }
}
