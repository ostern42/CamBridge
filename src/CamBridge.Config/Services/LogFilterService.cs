// src/CamBridge.Config/Services/LogFilterService.cs
// Version: 0.8.16
// Description: Service for filtering log entries
// Copyright: (C) 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CamBridge.Config.Models;
using CamBridge.Config.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Service implementation for log filtering
    /// </summary>
    public class LogFilterService : ILogFilterService
    {
        private readonly ILogger<LogFilterService> _logger;

        public LogFilterService(ILogger<LogFilterService> logger)
        {
            _logger = logger;
        }

        public List<LogEntry> ApplyFilters(IEnumerable<LogEntry> entries, LogFilterCriteria criteria)
        {
            _logger.LogDebug("ApplyFilters: Starting with {Count} entries", entries.Count());

            var filtered = entries.AsEnumerable();

            // Apply level filters
            var anyLevelSelected = criteria.ShowDebug || criteria.ShowInformation ||
                                 criteria.ShowWarning || criteria.ShowError || criteria.ShowCritical;

            if (anyLevelSelected)
            {
                filtered = filtered.Where(e =>
                    (criteria.ShowDebug && e.Level == LogLevel.Debug) ||
                    (criteria.ShowInformation && e.Level == LogLevel.Information) ||
                    (criteria.ShowWarning && e.Level == LogLevel.Warning) ||
                    (criteria.ShowError && e.Level == LogLevel.Error) ||
                    (criteria.ShowCritical && e.Level == LogLevel.Critical));
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(criteria.SearchText))
            {
                var searchLower = criteria.SearchText.ToLowerInvariant();
                filtered = filtered.Where(e =>
                    e.Message.ToLowerInvariant().Contains(searchLower) ||
                    e.LevelText.ToLowerInvariant().Contains(searchLower) ||
                    e.Source.ToLowerInvariant().Contains(searchLower) ||
                    (e.CorrelationId?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                    (e.Stage?.ToString().ToLowerInvariant().Contains(searchLower) ?? false));
            }

            // Apply wildcard filters
            if (!string.IsNullOrWhiteSpace(criteria.Filter1) ||
                !string.IsNullOrWhiteSpace(criteria.Filter2) ||
                !string.IsNullOrWhiteSpace(criteria.Filter3))
            {
                filtered = filtered.Where(e =>
                {
                    var fullText = $"{e.Timestamp:HH:mm:ss.fff} {e.LevelText} {e.CorrelationId} {e.Stage} {e.Message} {e.Pipeline}".ToLowerInvariant();

                    return MatchesWildcard(fullText, criteria.Filter1) &&
                           MatchesWildcard(fullText, criteria.Filter2) &&
                           MatchesWildcard(fullText, criteria.Filter3);
                });
            }

            var result = filtered.ToList();
            _logger.LogDebug("After filtering: {Count} entries remain", result.Count);

            return result;
        }

        public bool MatchesWildcard(string text, string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return true;

            // Convert wildcard pattern to regex
            var regexPattern = "^" + Regex.Escape(pattern)
                .Replace("\\*", ".*")    // * = any number of chars
                .Replace("\\?", ".")     // ? = exactly one char
                + "$";

            return Regex.IsMatch(text, regexPattern, RegexOptions.IgnoreCase);
        }
    }
}
