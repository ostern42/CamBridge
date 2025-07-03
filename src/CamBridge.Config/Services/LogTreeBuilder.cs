// src/CamBridge.Config/Services/LogTreeBuilder.cs
// Version: 0.8.16
// Description: Service for building hierarchical log views
// Copyright: (C) 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.Linq;
using CamBridge.Config.Models;
using CamBridge.Config.Services.Interfaces;
using CamBridge.Core.Enums;
using Microsoft.Extensions.Logging;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Service implementation for building tree views
    /// </summary>
    public class LogTreeBuilder : ILogTreeBuilder
    {
        private readonly ILogger<LogTreeBuilder> _logger;
        private readonly ILogFilterService _filterService;

        public LogTreeBuilder(
            ILogger<LogTreeBuilder> logger,
            ILogFilterService filterService)
        {
            _logger = logger;
            _filterService = filterService;
        }

        public List<CorrelationGroup> BuildCorrelationGroups(
            IEnumerable<LogEntry> entries,
            bool applyTextFilters = false,
            params string[] textFilters)
        {
            var result = new List<CorrelationGroup>();

            // If text filters are provided, filter at the GROUP level
            if (applyTextFilters && textFilters.Any(f => !string.IsNullOrWhiteSpace(f)))
            {
                var groupsToShow = new HashSet<string>();

                foreach (var entry in entries.Where(e => !string.IsNullOrEmpty(e.CorrelationId)))
                {
                    var fullText = $"{entry.Timestamp:HH:mm:ss.fff} {entry.LevelText} {entry.CorrelationId} {entry.Stage} {entry.Message} {entry.Pipeline}".ToLowerInvariant();

                    bool matchesAll = true;
                    foreach (var filter in textFilters.Where(f => !string.IsNullOrWhiteSpace(f)))
                    {
                        if (!_filterService.MatchesWildcard(fullText, filter))
                        {
                            matchesAll = false;
                            break;
                        }
                    }

                    if (matchesAll)
                    {
                        groupsToShow.Add(entry.CorrelationId!);
                    }
                }

                // Filter entries to only those in matching groups
                entries = entries.Where(e => string.IsNullOrEmpty(e.CorrelationId) || groupsToShow.Contains(e.CorrelationId!));
            }

            // Group by correlation ID
            var groups = entries
                .Where(e => !string.IsNullOrEmpty(e.CorrelationId))
                .GroupBy(e => e.CorrelationId!)
                .OrderByDescending(g => g.Max(e => e.Timestamp));

            foreach (var group in groups)
            {
                var correlationGroup = new CorrelationGroup
                {
                    CorrelationId = group.Key,
                    StartTime = group.Min(e => e.Timestamp),
                    EndTime = group.Max(e => e.Timestamp),
                    Pipeline = group.FirstOrDefault(e => !string.IsNullOrEmpty(e.Pipeline))?.Pipeline ?? "Unknown",
                    IsExpanded = true // Default expanded
                };

                // Build stage hierarchy
                var stages = new Dictionary<ProcessingStage, StageGroup>();

                foreach (var entry in group.OrderBy(e => e.Timestamp))
                {
                    if (entry.Stage.HasValue)
                    {
                        if (!stages.ContainsKey(entry.Stage.Value))
                        {
                            stages[entry.Stage.Value] = new StageGroup
                            {
                                Stage = entry.Stage.Value,
                                StartTime = entry.Timestamp,
                                IsExpanded = true
                            };
                        }

                        stages[entry.Stage.Value].Entries.Add(entry);
                        stages[entry.Stage.Value].EndTime = entry.Timestamp;
                    }
                    else
                    {
                        correlationGroup.UngroupedEntries.Add(entry);
                    }
                }

                // Add stages to correlation group
                foreach (var stage in stages.Values.OrderBy(s => s.StartTime))
                {
                    correlationGroup.Stages.Add(stage);
                }

                // Determine overall status
                if (stages.ContainsKey(ProcessingStage.Error))
                {
                    correlationGroup.Status = ProcessingStatus.Failed;
                }
                else if (stages.ContainsKey(ProcessingStage.Complete))
                {
                    correlationGroup.Status = ProcessingStatus.Completed;
                }
                else
                {
                    correlationGroup.Status = ProcessingStatus.InProgress;
                }

                result.Add(correlationGroup);
            }

            // Add entries without correlation ID
            var uncorrelatedEntries = entries.Where(e => string.IsNullOrEmpty(e.CorrelationId)).ToList();

            if (uncorrelatedEntries.Any())
            {
                var uncorrelatedGroup = new CorrelationGroup
                {
                    CorrelationId = "Uncorrelated",
                    StartTime = uncorrelatedEntries.Min(e => e.Timestamp),
                    EndTime = uncorrelatedEntries.Max(e => e.Timestamp),
                    Pipeline = "Various",
                    IsExpanded = true,
                    Status = ProcessingStatus.Unknown
                };

                foreach (var entry in uncorrelatedEntries.OrderBy(e => e.Timestamp))
                {
                    uncorrelatedGroup.UngroupedEntries.Add(entry);
                }

                result.Add(uncorrelatedGroup);
            }

            _logger.LogDebug("Built {Count} correlation groups from {EntryCount} entries",
                result.Count, entries.Count());

            return result;
        }
    }
}
