// src/CamBridge.Core/Logging/LogContext.cs
// Version: 0.8.6
// Description: Context for hierarchical logging with correlation IDs and timing
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using CamBridge.Core.Enums; // Use LogVerbosity and ProcessingStage from Enums!

namespace CamBridge.Core.Logging
{
    /// <summary>
    /// Provides context for hierarchical logging with correlation IDs
    /// </summary>
    public class LogContext
    {
        private readonly ILogger _logger;
        private readonly Stack<StageContext> _stageStack = new();

        public string CorrelationId { get; }
        public string PipelineName { get; }
        public ProcessingStage CurrentStage { get; private set; }
        public DateTime StartTime { get; }
        public LogVerbosity Verbosity { get; }

        public LogContext(
            ILogger logger,
            string correlationId,
            string pipelineName,
            LogVerbosity verbosity = LogVerbosity.Detailed)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
            PipelineName = pipelineName ?? throw new ArgumentNullException(nameof(pipelineName));
            Verbosity = verbosity;
            StartTime = DateTime.UtcNow;
            CurrentStage = ProcessingStage.FileDetected;
        }

        /// <summary>
        /// Begins a new processing stage and returns a disposable that logs completion
        /// </summary>
        public IDisposable BeginStage(ProcessingStage stage, string? message = null)
        {
            CurrentStage = stage;
            var stageContext = new StageContext(this, stage, message);
            _stageStack.Push(stageContext);
            return stageContext;
        }

        /// <summary>
        /// Check if should log at this verbosity level
        /// </summary>
        public bool ShouldLog(LogVerbosity requiredLevel)
        {
            return Verbosity >= requiredLevel;
        }

        /// <summary>
        /// Log with correlation context
        /// </summary>
        public void Log(LogLevel level, string message, params object[] args)
        {
            var formattedArgs = new List<object> { CorrelationId, CurrentStage, message, PipelineName };
            if (args != null && args.Length > 0)
            {
                formattedArgs.AddRange(args);
            }

            _logger.Log(level, "[{CorrelationId}] [{Stage}] " + message + " [{Pipeline}]", formattedArgs.ToArray());
        }

        /// <summary>
        /// Log information with correlation context
        /// </summary>
        public void LogInformation(string message, params object[] args)
        {
            if (ShouldLog(LogVerbosity.Minimal))
            {
                Log(LogLevel.Information, message, args);
            }
        }

        /// <summary>
        /// Log debug with correlation context
        /// </summary>
        public void LogDebug(string message, params object[] args)
        {
            if (ShouldLog(LogVerbosity.Debug))
            {
                Log(LogLevel.Debug, message, args);
            }
        }

        /// <summary>
        /// Log warning with correlation context
        /// </summary>
        public void LogWarning(string message, params object[] args)
        {
            if (ShouldLog(LogVerbosity.Normal))
            {
                Log(LogLevel.Warning, message, args);
            }
        }

        /// <summary>
        /// Log error with correlation context
        /// </summary>
        public void LogError(Exception ex, string message, params object[] args)
        {
            // Always log errors
            var formattedArgs = new List<object> { CorrelationId, CurrentStage, message, PipelineName };
            if (args != null && args.Length > 0)
            {
                formattedArgs.AddRange(args);
            }

            _logger.LogError(ex, "[{CorrelationId}] [{Stage}] " + message + " [{Pipeline}]", formattedArgs.ToArray());
        }

        /// <summary>
        /// Context for a processing stage with timing
        /// </summary>
        private class StageContext : IDisposable
        {
            private readonly LogContext _logContext;
            private readonly ProcessingStage _stage;
            private readonly Stopwatch _stopwatch;
            private readonly string? _message;

            public StageContext(LogContext logContext, ProcessingStage stage, string? message)
            {
                _logContext = logContext;
                _stage = stage;
                _message = message;
                _stopwatch = Stopwatch.StartNew();

                // Log stage start if detailed logging
                if (_logContext.ShouldLog(LogVerbosity.Detailed))
                {
                    _logContext.LogInformation(_message ?? $"{stage} started");
                }
            }

            public void Dispose()
            {
                _stopwatch.Stop();

                // Log stage completion with timing
                if (_logContext.ShouldLog(LogVerbosity.Normal))
                {
                    var completionMessage = _message != null
                        ? $"{_message} completed"
                        : $"{_stage} completed";

                    _logContext.LogInformation($"{completionMessage} [{{Duration}}ms]", _stopwatch.ElapsedMilliseconds);
                }

                // Pop from stack
                if (_logContext._stageStack.Count > 0)
                {
                    _logContext._stageStack.Pop();
                }
            }
        }
    }

    // REMOVED: ProcessingStage enum - now in CamBridge.Core.Enums
    // REMOVED: LogVerbosity enum - now in CamBridge.Core.Enums

    /// <summary>
    /// Extension methods for clean logging
    /// </summary>
    public static class LogContextExtensions
    {
        /// <summary>
        /// Create a log context from a logger
        /// </summary>
        public static LogContext CreateContext(
            this ILogger logger,
            string correlationId,
            string pipelineName,
            LogVerbosity verbosity = LogVerbosity.Detailed)
        {
            return new LogContext(logger, correlationId, pipelineName, verbosity);
        }

        /// <summary>
        /// Log a processing stage with timing
        /// </summary>
        public static IDisposable LogStage(
            this LogContext context,
            ProcessingStage stage,
            string? message = null)
        {
            return context.BeginStage(stage, message);
        }
    }
}
