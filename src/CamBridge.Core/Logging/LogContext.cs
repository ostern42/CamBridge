// src/CamBridge.Core/Logging/LogContext.cs
// Version: 0.8.8
// Description: Context for hierarchical logging with correlation IDs and timing
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using CamBridge.Core.Enums;

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
        /// Log with correlation context - FIXED to use proper template!
        /// </summary>
        public void Log(LogLevel level, string messageTemplate, params object[] args)
        {
            // Build the full template with correlation info
            var fullTemplate = $"[{{CorrelationId}}] [{{Stage}}] {messageTemplate} [{{Pipeline}}]";

            // Build args array with correlation values first
            var fullArgs = new List<object> { CorrelationId, CurrentStage };

            // Add the user-provided args
            if (args != null && args.Length > 0)
            {
                fullArgs.AddRange(args);
            }

            // Add pipeline name at the end
            fullArgs.Add(PipelineName);

            // Log with proper template formatting
            _logger.Log(level, fullTemplate, fullArgs.ToArray());
        }

        /// <summary>
        /// Log information with correlation context
        /// </summary>
        public void LogInformation(string messageTemplate, params object[] args)
        {
            if (ShouldLog(LogVerbosity.Minimal))
            {
                Log(LogLevel.Information, messageTemplate, args);
            }
        }

        /// <summary>
        /// Log debug with correlation context
        /// </summary>
        public void LogDebug(string messageTemplate, params object[] args)
        {
            if (ShouldLog(LogVerbosity.Debug))
            {
                Log(LogLevel.Debug, messageTemplate, args);
            }
        }

        /// <summary>
        /// Log warning with correlation context
        /// </summary>
        public void LogWarning(string messageTemplate, params object[] args)
        {
            if (ShouldLog(LogVerbosity.Normal))
            {
                Log(LogLevel.Warning, messageTemplate, args);
            }
        }

        /// <summary>
        /// Log error with correlation context
        /// </summary>
        public void LogError(Exception ex, string messageTemplate, params object[] args)
        {
            // Always log errors
            var fullTemplate = $"[{{CorrelationId}}] [{{Stage}}] {messageTemplate} [{{Pipeline}}]";

            var fullArgs = new List<object> { CorrelationId, CurrentStage };
            if (args != null && args.Length > 0)
            {
                fullArgs.AddRange(args);
            }
            fullArgs.Add(PipelineName);

            _logger.LogError(ex, fullTemplate, fullArgs.ToArray());
        }

        /// <summary>
        /// Context for a processing stage with timing
        /// </summary>
        private class StageContext : IDisposable
        {
            private readonly LogContext _logContext;
            private readonly ProcessingStage _stage;
            private readonly Stopwatch _stopwatch;
            private readonly string? _customMessage;
            private bool _hasLoggedStart = false;

            public StageContext(LogContext logContext, ProcessingStage stage, string? customMessage)
            {
                _logContext = logContext;
                _stage = stage;
                _customMessage = customMessage;
                _stopwatch = Stopwatch.StartNew();

                // ONLY log start if:
                // 1. Detailed logging is enabled
                // 2. AND a custom message was provided (indicating explicit logging intent)
                if (_logContext.ShouldLog(LogVerbosity.Detailed) && !string.IsNullOrEmpty(_customMessage))
                {
                    _logContext.LogInformation(_customMessage);
                    _hasLoggedStart = true;
                }
            }

            public void Dispose()
            {
                _stopwatch.Stop();

                // ONLY log completion if:
                // 1. Normal+ logging is enabled
                // 2. AND we didn't already log a custom message
                // 3. OR it's a significant stage completion (Complete/Error)
                if (_logContext.ShouldLog(LogVerbosity.Normal))
                {
                    // Special handling for key stages
                    if (_stage == ProcessingStage.Complete || _stage == ProcessingStage.Error)
                    {
                        // Always log these important stages
                        _logContext.LogInformation("{Stage} [{Duration}ms]", _stage, _stopwatch.ElapsedMilliseconds);
                    }
                    else if (!_hasLoggedStart && _stopwatch.ElapsedMilliseconds > 100)
                    {
                        // Only log other stages if they took significant time and we didn't log start
                        _logContext.LogInformation("{Stage} completed [{Duration}ms]", _stage, _stopwatch.ElapsedMilliseconds);
                    }
                }

                // Pop from stack
                if (_logContext._stageStack.Count > 0)
                {
                    _logContext._stageStack.Pop();
                }
            }
        }
    }

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
        /// Log a processing stage with timing - SIMPLIFIED!
        /// </summary>
        public static IDisposable LogStage(
            this LogContext context,
            ProcessingStage stage)
        {
            // No message = no duplicate logging!
            return context.BeginStage(stage, null);
        }

        /// <summary>
        /// Log a processing stage with custom message
        /// </summary>
        public static IDisposable LogStageWithMessage(
            this LogContext context,
            ProcessingStage stage,
            string message)
        {
            return context.BeginStage(stage, message);
        }
    }
}
