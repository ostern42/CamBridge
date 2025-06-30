// src\CamBridge.Config\ViewModels\LogViewerViewModel.cs
// Version: 0.8.9
// Description: Enhanced log viewer with Triple Filter and Default Expanded
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Config.Services;
using CamBridge.Core;
using CamBridge.Core.Enums;
using CamBridge.Core.Infrastructure;
using CamBridge.Core.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;


namespace CamBridge.Config.ViewModels
{
    /// <summary>
    /// Tracks file position for incremental reading
    /// </summary>
    public class FilePositionInfo
    {
        public long Position { get; set; }
        public DateTime LastRead { get; set; }
    }

    /// <summary>
    /// Represents a selectable pipeline in the multi-select dropdown
    /// </summary>
    public class PipelineSelection : ViewModelBase
    {
        private bool _isSelected;

        public string Name { get; set; } = string.Empty;
        public string SanitizedName { get; set; } = string.Empty;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }

    /// <summary>
    /// Enhanced ViewModel for the log viewer with correlation ID support
    /// </summary>
    public partial class LogViewerViewModel : ViewModelBase
    {
        private readonly ILogger<LogViewerViewModel> _logger;
        private readonly IConfigurationService _configurationService;
        private readonly Dictionary<string, string> _pipelineNameMapping;
        private readonly Timer _refreshTimer;
        private readonly Dictionary<string, long> _filePositions;
        private CancellationTokenSource? _watcherCancellation;
        private FileSystemWatcher? _logWatcher;

        // Constants
        private const int MaxDisplayedEntries = 10000;
        private const int TailLineCount = 1000;
        private const int RefreshIntervalMs = 1000;

        public LogViewerViewModel(
            ILogger<LogViewerViewModel> logger,
            IConfigurationService configurationService)
        {
            _logger = logger;
            _configurationService = configurationService;
            _pipelineNameMapping = new Dictionary<string, string>();
            _filePositions = new Dictionary<string, long>();

            // Initialize collections
            LogEntries = new ObservableCollection<LogEntry>();
            FilteredCombinedEntries = new ObservableCollection<LogEntry>();
            CombinedLogEntries = new ObservableCollection<LogEntry>();
            CorrelationGroups = new ObservableCollection<CorrelationGroup>();
            AvailablePipelines = new ObservableCollection<string>();
            PipelineSelections = new ObservableCollection<PipelineSelection>();

            // Initialize commands
            RefreshCommand = new AsyncRelayCommand(RefreshLogsAsync);
            ClearLogCommand = new RelayCommand(ClearLogs);
            ExportLogCommand = new AsyncRelayCommand(ExportLogsAsync);
            ToggleTreeViewCommand = new RelayCommand(() => IsTreeViewEnabled = !IsTreeViewEnabled);
            ExpandAllCommand = new RelayCommand(ExpandAll);
            CollapseAllCommand = new RelayCommand(CollapseAll);
            CopySelectedCommand = new RelayCommand(CopySelected);
            OpenLogFolderCommand = new RelayCommand(OpenLogFolder);
            ClearFiltersCommand = new RelayCommand(ClearFilters);
            CopyLineCommand = new RelayCommand<object>(CopyLine);
            CopyGroupCommand = new RelayCommand<object>(CopyGroup);
            ExpandGroupCommand = new RelayCommand<object>(ExpandGroup);

            // Initialize timer for auto-refresh
            _refreshTimer = new Timer(OnRefreshTimer, null, Timeout.Infinite, Timeout.Infinite);

            // Set default filter values
            ShowDebug = false;  // Debug meist zu viel
            ShowInformation = true;
            ShowWarning = true;
            ShowError = true;
            ShowCritical = true;
            IsAutoScrollEnabled = false; // Default OFF to prevent flicker
            IsTreeViewEnabled = true; // Default to tree view for correlation
        }

        #region Properties

        [ObservableProperty]
        private ObservableCollection<LogEntry> logEntries;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayedLineCount))]
        private ObservableCollection<LogEntry> filteredCombinedEntries;

        [ObservableProperty]
        private ObservableCollection<LogEntry> combinedLogEntries;

        [ObservableProperty]
        private ObservableCollection<CorrelationGroup> correlationGroups;

        [ObservableProperty]
        private ObservableCollection<string> availablePipelines;

        [ObservableProperty]
        private ObservableCollection<PipelineSelection> pipelineSelections;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCombinedEntries))]
        [NotifyPropertyChangedFor(nameof(CorrelationGroups))]
        private string? searchText;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCombinedEntries))]
        [NotifyPropertyChangedFor(nameof(CorrelationGroups))]
        private string filter1 = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCombinedEntries))]
        [NotifyPropertyChangedFor(nameof(CorrelationGroups))]
        private string filter2 = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCombinedEntries))]
        [NotifyPropertyChangedFor(nameof(CorrelationGroups))]
        private string filter3 = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCombinedEntries))]
        [NotifyPropertyChangedFor(nameof(CorrelationGroups))]
        private bool showDebug;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCombinedEntries))]
        [NotifyPropertyChangedFor(nameof(CorrelationGroups))]
        private bool showInformation;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCombinedEntries))]
        [NotifyPropertyChangedFor(nameof(CorrelationGroups))]
        private bool showWarning;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCombinedEntries))]
        [NotifyPropertyChangedFor(nameof(CorrelationGroups))]
        private bool showError;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCombinedEntries))]
        [NotifyPropertyChangedFor(nameof(CorrelationGroups))]
        private bool showCritical;

        [ObservableProperty]
        private bool isAutoScrollEnabled;

        [ObservableProperty]
        private bool isTreeViewEnabled;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string currentLogFile = "No log file loaded";

        [ObservableProperty]
        private string currentLogFiles = "No files selected";

        [ObservableProperty]
        private int totalLineCount;

        [ObservableProperty]
        private DateTime lastUpdateTime = DateTime.Now;

        public int DisplayedLineCount =>
            IsTreeViewEnabled
                ? CorrelationGroups?.Sum(g => g.TotalEntries) ?? 0
                : FilteredCombinedEntries?.Count ?? 0;

        public int SelectedPipelineCount => PipelineSelections?.Count(p => p.IsSelected) ?? 0;

        public int SelectedLevelCount
        {
            get
            {
                var count = 0;
                if (ShowDebug) count++;
                if (ShowInformation) count++;
                if (ShowWarning) count++;
                if (ShowError) count++;
                if (ShowCritical) count++;
                return count;
            }
        }

        #endregion

        #region Commands

        public IAsyncRelayCommand RefreshCommand { get; }
        public IRelayCommand ClearLogCommand { get; }
        public IAsyncRelayCommand ExportLogCommand { get; }
        public IRelayCommand ToggleTreeViewCommand { get; }
        public IRelayCommand ExpandAllCommand { get; }
        public IRelayCommand CollapseAllCommand { get; }
        public IRelayCommand CopySelectedCommand { get; }
        public IRelayCommand OpenLogFolderCommand { get; }
        public IRelayCommand ClearFiltersCommand { get; }
        public ICommand CopyLineCommand { get; }
        public ICommand CopyGroupCommand { get; }
        public ICommand ExpandGroupCommand { get; }

        #endregion

        #region Public Methods

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Initializing Enhanced LogViewerViewModel");

                // Load available pipelines
                await LoadAvailablePipelinesAsync();

                // Update count display
                OnPropertyChanged(nameof(SelectedPipelineCount));
                OnPropertyChanged(nameof(SelectedLevelCount));

                // Log initial filter state
                _logger.LogInformation("Initial filter state: Debug={Debug}, Info={Info}, Warn={Warn}, Error={Error}, Critical={Critical}",
                    ShowDebug, ShowInformation, ShowWarning, ShowError, ShowCritical);

                // Log selected pipelines
                var selected = PipelineSelections.Where(p => p.IsSelected).Select(p => p.Name).ToList();
                _logger.LogInformation("Selected pipelines: {Pipelines}", string.Join(", ", selected));

                // Load initial logs
                await RefreshLogsAsync();

                // Start auto-refresh if enabled
                if (IsAutoScrollEnabled)
                {
                    _refreshTimer.Change(RefreshIntervalMs, RefreshIntervalMs);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize LogViewerViewModel");
            }
        }

        public void Cleanup()
        {
            try
            {
                _refreshTimer?.Change(Timeout.Infinite, Timeout.Infinite);
                _refreshTimer?.Dispose();
                _watcherCancellation?.Cancel();
                _watcherCancellation?.Dispose();
                _logWatcher?.Dispose();

                // Unsubscribe from selection changes
                foreach (var selection in PipelineSelections)
                {
                    selection.PropertyChanged -= OnPipelineSelectionChanged;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cleanup");
            }
        }

        #endregion

        #region Copy Command Implementations

        private void CopyLine(object? parameter)
        {
            if (parameter is LogEntry entry)
            {
                var text = $"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{entry.CorrelationId}] [{entry.Stage}] {entry.Message}";
                Clipboard.SetText(text);
            }
        }

        private void CopyGroup(object? parameter)
        {
            if (parameter is CorrelationGroup group)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"=== {group.CorrelationId} - {group.Pipeline} [{group.DurationText}] ===");

                // Get all entries sorted
                var allEntries = new List<LogEntry>();
                foreach (var stage in group.Stages)
                {
                    allEntries.AddRange(stage.Entries);
                }
                allEntries.AddRange(group.UngroupedEntries);

                foreach (var entry in allEntries.OrderBy(e => e.Timestamp))
                {
                    sb.AppendLine($"{entry.Timestamp:HH:mm:ss.fff} {entry.Stage}: {entry.Message}");
                }

                Clipboard.SetText(sb.ToString());
            }
        }

        private void ExpandGroup(object? parameter)
        {
            if (parameter is CorrelationGroup group)
            {
                group.IsExpanded = true;
                // In compact view we don't have nested stages anymore
            }
        }

        #endregion

        private void ApplyFilters()
        {
            // WICHTIG: Muss auf dem UI Thread laufen!
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => ApplyFilters());
                return;
            }

            _logger.LogDebug("ApplyFilters: Starting with {Count} combined entries", CombinedLogEntries.Count);

            var filtered = CombinedLogEntries.AsEnumerable();

            // Apply level filters
            var anyLevelSelected = ShowDebug || ShowInformation || ShowWarning || ShowError || ShowCritical;

            if (anyLevelSelected)
            {
                filtered = filtered.Where(e =>
                    (ShowDebug && e.Level == LogLevel.Debug) ||
                    (ShowInformation && e.Level == LogLevel.Information) ||
                    (ShowWarning && e.Level == LogLevel.Warning) ||
                    (ShowError && e.Level == LogLevel.Error) ||
                    (ShowCritical && e.Level == LogLevel.Critical));
            }

            // Apply search filter (legacy single search)
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchLower = SearchText.ToLowerInvariant();
                filtered = filtered.Where(e =>
                    e.Message.ToLowerInvariant().Contains(searchLower) ||
                    e.LevelText.ToLowerInvariant().Contains(searchLower) ||
                    e.Source.ToLowerInvariant().Contains(searchLower) ||
                    (e.CorrelationId?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                    (e.Stage?.ToString().ToLowerInvariant().Contains(searchLower) ?? false));
            }

            // Update filtered collection
            var filteredList = filtered.ToList();

            // NEW: Apply Triple Text Filter if tree view is enabled
            if (IsTreeViewEnabled && (!string.IsNullOrWhiteSpace(Filter1) || !string.IsNullOrWhiteSpace(Filter2) || !string.IsNullOrWhiteSpace(Filter3)))
            {
                filteredList = ApplyTripleTextFilter(filteredList);
            }

            _logger.LogDebug("After filtering: {Count} entries remain", filteredList.Count);

            FilteredCombinedEntries.Clear();
            foreach (var entry in filteredList)
            {
                FilteredCombinedEntries.Add(entry);
            }

            // Update correlation groups if tree view is enabled
            if (IsTreeViewEnabled)
            {
                UpdateCorrelationGroups(filteredList);
            }
        }

        private List<LogEntry> ApplyTripleTextFilter(List<LogEntry> entries)
        {
            // For tree-aware filtering, we need to check groups
            var groups = entries
                .Where(e => !string.IsNullOrEmpty(e.CorrelationId))
                .GroupBy(e => e.CorrelationId!)
                .ToList();

            var matchingEntries = new List<LogEntry>();

            foreach (var group in groups)
            {
                // Check if ANY entry in the group matches ALL active filters
                bool groupMatches = group.Any(entry =>
                {
                    var fullText = $"{entry.Timestamp:HH:mm:ss.fff} {entry.LevelText} {entry.CorrelationId} {entry.Stage} {entry.Message} {entry.Pipeline}";

                    return MatchesWildcard(fullText, Filter1) &&
                           MatchesWildcard(fullText, Filter2) &&
                           MatchesWildcard(fullText, Filter3);
                });

                if (groupMatches)
                {
                    // Include ALL entries from matching groups (tree-aware!)
                    matchingEntries.AddRange(group);
                }
            }

            // Also check uncorrelated entries individually
            var uncorrelated = entries.Where(e => string.IsNullOrEmpty(e.CorrelationId)).ToList();
            foreach (var entry in uncorrelated)
            {
                var fullText = $"{entry.Timestamp:HH:mm:ss.fff} {entry.LevelText} {entry.Message} {entry.Pipeline}";

                if (MatchesWildcard(fullText, Filter1) &&
                    MatchesWildcard(fullText, Filter2) &&
                    MatchesWildcard(fullText, Filter3))
                {
                    matchingEntries.Add(entry);
                }
            }

            return matchingEntries;
        }

        private bool MatchesWildcard(string text, string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern)) return true;

            // Convert wildcard pattern to regex
            var regexPattern = "^" + Regex.Escape(pattern)
                .Replace("\\*", ".*")    // * = any number of chars
                .Replace("\\?", ".")     // ? = exactly one char
                + "$";

            return Regex.IsMatch(text, regexPattern, RegexOptions.IgnoreCase);
        }

        private void ClearFilters()
        {
            SearchText = string.Empty;
            Filter1 = string.Empty;
            Filter2 = string.Empty;
            Filter3 = string.Empty;
        }

        private void UpdateCorrelationGroups(List<LogEntry> entries)
        {
            // DEBUG OUTPUT
            Console.WriteLine($"[DEBUG] UpdateCorrelationGroups called with {entries.Count} entries");

            CorrelationGroups.Clear();

            // Group by correlation ID
            var groups = entries
                .Where(e => !string.IsNullOrEmpty(e.CorrelationId))
                .GroupBy(e => e.CorrelationId!)
                .OrderByDescending(g => g.Max(e => e.Timestamp));

            Console.WriteLine($"[DEBUG] Found {groups.Count()} correlation groups");

            foreach (var group in groups)
            {
                Console.WriteLine($"[DEBUG] Processing group {group.Key} with {group.Count()} entries");

                var correlationGroup = new CorrelationGroup
                {
                    CorrelationId = group.Key,
                    StartTime = group.Min(e => e.Timestamp),
                    EndTime = group.Max(e => e.Timestamp),
                    Pipeline = group.FirstOrDefault(e => !string.IsNullOrEmpty(e.Pipeline))?.Pipeline ?? "Unknown",
                    IsExpanded = true // Default expanded!
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
                                IsExpanded = true // Default expanded!
                            };
                        }

                        stages[entry.Stage.Value].Entries.Add(entry);
                        stages[entry.Stage.Value].EndTime = entry.Timestamp;
                    }
                    else
                    {
                        // Add entries without stage directly to correlation group
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

                CorrelationGroups.Add(correlationGroup);
            }

            // Add entries without correlation ID
            var uncorrelatedEntries = entries.Where(e => string.IsNullOrEmpty(e.CorrelationId)).ToList();
            Console.WriteLine($"[DEBUG] Found {uncorrelatedEntries.Count} uncorrelated entries");

            if (uncorrelatedEntries.Any())
            {
                var uncorrelatedGroup = new CorrelationGroup
                {
                    CorrelationId = "Uncorrelated",
                    StartTime = uncorrelatedEntries.Min(e => e.Timestamp),
                    EndTime = uncorrelatedEntries.Max(e => e.Timestamp),
                    Pipeline = "Various",
                    IsExpanded = true, // Default expanded!
                    Status = ProcessingStatus.Unknown
                };

                foreach (var entry in uncorrelatedEntries.OrderBy(e => e.Timestamp))
                {
                    uncorrelatedGroup.UngroupedEntries.Add(entry);
                }

                CorrelationGroups.Add(uncorrelatedGroup);
            }

            Console.WriteLine($"[DEBUG] Total CorrelationGroups: {CorrelationGroups.Count}");
        }

        private void ExpandAll()
        {
            foreach (var group in CorrelationGroups)
            {
                group.IsExpanded = true;
                foreach (var stage in group.Stages)
                {
                    stage.IsExpanded = true;
                }
            }
        }

        private void CollapseAll()
        {
            foreach (var group in CorrelationGroups)
            {
                group.IsExpanded = false;
                foreach (var stage in group.Stages)
                {
                    stage.IsExpanded = false;
                }
            }
        }

        private void CopySelected()
        {
            // Copy all visible log entries to clipboard
            var sb = new StringBuilder();

            if (IsTreeViewEnabled)
            {
                // Copy tree structure
                foreach (var group in CorrelationGroups)
                {
                    sb.AppendLine($"=== {group.CorrelationId} - {group.Pipeline} [{group.DurationText}] ===");

                    foreach (var stage in group.Stages)
                    {
                        sb.AppendLine($"  {stage.Stage} [{stage.DurationText}]");
                        foreach (var entry in stage.Entries)
                        {
                            sb.AppendLine($"    {entry.Timestamp:HH:mm:ss.fff} {entry.LevelText} {entry.Message}");
                        }
                    }

                    if (group.UngroupedEntries.Any())
                    {
                        sb.AppendLine("  [Ungrouped]");
                        foreach (var entry in group.UngroupedEntries)
                        {
                            sb.AppendLine($"    {entry.Timestamp:HH:mm:ss.fff} {entry.LevelText} {entry.Message}");
                        }
                    }

                    sb.AppendLine();
                }
            }
            else
            {
                // Copy flat list
                foreach (var entry in FilteredCombinedEntries)
                {
                    sb.AppendLine($"{entry.Timestamp:HH:mm:ss.fff} {entry.LevelText} [{entry.CorrelationId}] [{entry.Stage}] {entry.Message}");
                }
            }

            if (sb.Length > 0)
            {
                Clipboard.SetText(sb.ToString());
                _logger.LogInformation("Copied {Count} entries to clipboard",
                    IsTreeViewEnabled ? CorrelationGroups.Sum(g => g.TotalEntries) : FilteredCombinedEntries.Count);
            }
        }

        private void OpenLogFolder()
        {
            try
            {
                var logPath = ConfigurationPaths.GetLogsDirectory();
                if (Directory.Exists(logPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = logPath,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                    _logger.LogInformation("Opened log folder: {Path}", logPath);
                }
                else
                {
                    _logger.LogWarning("Log folder does not exist: {Path}", logPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to open log folder");
            }
        }
        private async Task LoadAvailablePipelinesAsync()
        {
            try
            {
                AvailablePipelines.Clear();
                PipelineSelections.Clear();
                _pipelineNameMapping.Clear();

                // Always add Service (Global) log
                AvailablePipelines.Add("Service (Global)");
                PipelineSelections.Add(new PipelineSelection
                {
                    Name = "Service (Global)",
                    SanitizedName = "service",
                    IsSelected = true // Default selected
                });

                // Get configured pipelines
                var settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettingsV2>();
                if (settings?.Pipelines != null)
                {
                    foreach (var pipeline in settings.Pipelines)
                    {
                        var displayName = $"{pipeline.Name}";
                        var sanitizedName = SanitizeForFileName(pipeline.Name);

                        AvailablePipelines.Add(displayName);
                        _pipelineNameMapping[displayName] = sanitizedName;

                        PipelineSelections.Add(new PipelineSelection
                        {
                            Name = displayName,
                            SanitizedName = sanitizedName,
                            IsSelected = false
                        });
                    }
                }

                // Also check for existing log files that might not be in config
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
                        if (!_pipelineNameMapping.Values.Contains(logName))
                        {
                            var displayName = $"{logName} (Archived)";
                            AvailablePipelines.Add(displayName);
                            _pipelineNameMapping[displayName] = logName;

                            PipelineSelections.Add(new PipelineSelection
                            {
                                Name = displayName,
                                SanitizedName = logName,
                                IsSelected = false
                            });
                        }
                    }
                }

                // Listen for selection changes
                foreach (var selection in PipelineSelections)
                {
                    selection.PropertyChanged += OnPipelineSelectionChanged;
                }

                // Update initial count
                OnPropertyChanged(nameof(SelectedPipelineCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load available pipelines");
            }
        }

        private async Task RefreshLogsAsync()
        {
            try
            {
                IsLoading = true;

                // Get selected pipelines
                var selectedPipelines = PipelineSelections.Where(p => p.IsSelected).ToList();
                if (!selectedPipelines.Any())
                {
                    CombinedLogEntries.Clear();
                    CorrelationGroups.Clear();
                    CurrentLogFiles = "No pipelines selected";
                    return;
                }

                // Update current files display
                CurrentLogFiles = string.Join(", ", selectedPipelines.Select(p => p.Name));

                // Collect all log entries from selected pipelines
                var allEntries = new List<LogEntry>();

                foreach (var pipeline in selectedPipelines)
                {
                    var logFileName = GetLogFileName(pipeline.Name);
                    var logPath = Path.Combine(ConfigurationPaths.GetLogsDirectory(), logFileName);

                    if (File.Exists(logPath))
                    {
                        // Read entries from this pipeline
                        var entries = await ReadLogFileAsync(logPath, pipeline.Name);
                        allEntries.AddRange(entries);
                    }
                }

                // Sort all entries by timestamp (millisecond precision)
                var sortedEntries = allEntries
                    .OrderBy(e => e.Timestamp)
                    .TakeLast(MaxDisplayedEntries)
                    .ToList();

                // Update collection efficiently (minimize UI updates)
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // For auto-scroll, append only new entries to prevent flicker
                    if (IsAutoScrollEnabled && CombinedLogEntries.Count > 0)
                    {
                        // Find the last timestamp we have
                        var lastTimestamp = CombinedLogEntries.Last().Timestamp;

                        // Add only newer entries
                        var newEntries = sortedEntries
                            .Where(e => e.Timestamp > lastTimestamp)
                            .ToList();

                        foreach (var entry in newEntries)
                        {
                            CombinedLogEntries.Add(entry);

                            // Maintain max entries
                            if (CombinedLogEntries.Count > MaxDisplayedEntries)
                                CombinedLogEntries.RemoveAt(0);
                        }
                    }
                    else
                    {
                        // Full refresh when not auto-scrolling
                        CombinedLogEntries.Clear();
                        foreach (var entry in sortedEntries)
                        {
                            CombinedLogEntries.Add(entry);
                        }
                    }
                });

                TotalLineCount = CombinedLogEntries.Count;
                LastUpdateTime = DateTime.Now;
                ApplyFilters();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh logs");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task<List<LogEntry>> ReadLogFileAsync(string logPath, string sourcePipeline)
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
                        var entry = ParseLogLine(line);
                        if (entry != null)
                        {
                            entry.Source = sourcePipeline; // Tag with source
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

        private LogEntry? ParseLogLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            try
            {
                // DEBUG output for first few lines
                if (TotalLineCount < 10)
                {
                    Console.WriteLine($"[DEBUG] Parsing line: {line}");
                }

                // Try the REAL format from actual logs!
                // Format: [HH:mm:ss LEVEL] [CorrelationId] ["Stage"] Message [Duplicate]
                // Example: [23:56:28 INF] [F23562879-R0010168] ["FileDetected"] Processing file: R0010168.JPG [Processing file: R0010168.JPG]
                var realLogFormatMatch = Regex.Match(line,
                    @"^\[(\d{2}:\d{2}:\d{2})\s+(\w+)\]\s+\[([^\]]+)\]\s+\[""([^""]+)""\]\s+([^[]+)(?:\[.*\])?");

                if (realLogFormatMatch.Success)
                {
                    var timeStr = realLogFormatMatch.Groups[1].Value;
                    var levelStr = realLogFormatMatch.Groups[2].Value;
                    var correlationId = realLogFormatMatch.Groups[3].Value;
                    var stageStr = realLogFormatMatch.Groups[4].Value;
                    var message = realLogFormatMatch.Groups[5].Value.Trim();

                    // Parse timestamp (add today's date)
                    var timestamp = DateTime.ParseExact(timeStr, "HH:mm:ss", CultureInfo.InvariantCulture);
                    timestamp = DateTime.Today.Add(timestamp.TimeOfDay);

                    // Parse log level
                    var level = ParseLogLevel(levelStr);

                    // Parse stage
                    ProcessingStage? stage = null;
                    if (Enum.TryParse<ProcessingStage>(stageStr, true, out var parsedStage))
                    {
                        stage = parsedStage;
                    }

                    // Extract pipeline name if present at end of message
                    string? pipeline = null;
                    var pipelineMatch = Regex.Match(message, @"\[([^\]]+)\]\s*$");
                    if (pipelineMatch.Success)
                    {
                        pipeline = pipelineMatch.Groups[1].Value;
                        message = message.Substring(0, message.Length - pipelineMatch.Value.Length).Trim();
                    }

                    // Extract duration if present
                    int? durationMs = null;
                    var durationMatch = Regex.Match(message, @"\[(\d+)ms\]");
                    if (durationMatch.Success)
                    {
                        durationMs = int.Parse(durationMatch.Groups[1].Value);
                        message = message.Replace(durationMatch.Value, "").Trim();
                    }

                    // DEBUG output
                    if (TotalLineCount < 10)
                    {
                        Console.WriteLine($"[DEBUG] MATCHED REAL FORMAT: CorrelationId={correlationId}, Stage={stage}, Pipeline={pipeline}");
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

                // First try the ACTUAL format from the logs
                // Example: [F00044243-R0010168] ["ExifExtraction"] Extracting EXIF metadata [Extracting EXIF metadata]
                // Note: Last part in brackets might be duplicated message - ignore it
                var actualFormatMatch = Regex.Match(line,
                    @"^\[([^\]]+)\]\s+\[""?([^\]""]+)""?\]\s+([^[]+)(?:\[.*\])?$");

                if (actualFormatMatch.Success)
                {
                    var correlationId = actualFormatMatch.Groups[1].Value;
                    var stageStr = actualFormatMatch.Groups[2].Value;
                    var message = actualFormatMatch.Groups[3].Value.Trim();

                    // Check if this is a real correlation ID (starts with letter + numbers)
                    if (Regex.IsMatch(correlationId, @"^[A-Z]\d{8,}"))
                    {
                        // Use current time since no timestamp in this format
                        var timestamp = DateTime.Now;

                        // Parse stage
                        ProcessingStage? stage = null;
                        if (Enum.TryParse<ProcessingStage>(stageStr, true, out var parsedStage))
                        {
                            stage = parsedStage;
                        }

                        // Determine log level from content
                        var level = DetermineLogLevel(stage, message);

                        // Extract pipeline name if present
                        string? pipeline = null;
                        var pipelineMatch = Regex.Match(message, @"\[([^\]]+)\]$");
                        if (pipelineMatch.Success)
                        {
                            pipeline = pipelineMatch.Groups[1].Value;
                            message = message.Substring(0, message.Length - pipelineMatch.Value.Length).Trim();
                        }

                        // DEBUG output
                        if (TotalLineCount < 10)
                        {
                            Console.WriteLine($"[DEBUG] Parsed NEW FORMAT: CorrelationId={correlationId}, Stage={stage}, Pipeline={pipeline}");
                        }

                        return new LogEntry
                        {
                            Timestamp = timestamp,
                            Level = level,
                            Message = message,
                            RawLine = line,
                            CorrelationId = correlationId,
                            Stage = stage,
                            Pipeline = pipeline
                        };
                    }
                }

                // ALSO try the timestamp format we expected
                // Example: 23:01:37.000 INF [F23013594-R0010168] ["ExifExtraction"] Extracting EXIF metadata completed
                var timestampFormatMatch = Regex.Match(line,
                    @"^(\d{2}:\d{2}:\d{2}\.\d{3})\s+(\w+)\s+\[([^\]]+)\]\s+\[""?([^\]""]+)""?\]\s+(.+)$");

                if (timestampFormatMatch.Success)
                {
                    var timeStr = timestampFormatMatch.Groups[1].Value;
                    var levelStr = timestampFormatMatch.Groups[2].Value;
                    var correlationId = timestampFormatMatch.Groups[3].Value;
                    var stageStr = timestampFormatMatch.Groups[4].Value;
                    var message = timestampFormatMatch.Groups[5].Value;

                    // Parse timestamp (add today's date)
                    var timestamp = DateTime.ParseExact(timeStr, "HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    timestamp = DateTime.Today.Add(timestamp.TimeOfDay);

                    // Parse log level
                    var level = ParseLogLevel(levelStr);

                    // Parse stage
                    ProcessingStage? stage = null;
                    if (Enum.TryParse<ProcessingStage>(stageStr, true, out var parsedStage))
                    {
                        stage = parsedStage;
                    }

                    // Extract pipeline name from message if present
                    string? pipeline = null;
                    var pipelineMatch = Regex.Match(message, @"\[([\w\s]+)\]$");
                    if (pipelineMatch.Success)
                    {
                        pipeline = pipelineMatch.Groups[1].Value;
                        message = message.Substring(0, message.Length - pipelineMatch.Value.Length).Trim();
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

                // Try simpler correlation format without stage
                // Example: 23:01:37.000 DBG [F23013594-R0010168] Found barcode data in Barcode field
                var simpleCorrelationMatch = Regex.Match(line,
                    @"^(\d{2}:\d{2}:\d{2}\.\d{3})\s+(\w+)\s+\[([^\]]+)\]\s+(.+)$");

                if (simpleCorrelationMatch.Success)
                {
                    var timeStr = simpleCorrelationMatch.Groups[1].Value;
                    var levelStr = simpleCorrelationMatch.Groups[2].Value;
                    var correlationIdOrMessage = simpleCorrelationMatch.Groups[3].Value;
                    var remainingMessage = simpleCorrelationMatch.Groups[4].Value;

                    // Check if this looks like a correlation ID (starts with F followed by digits)
                    if (Regex.IsMatch(correlationIdOrMessage, @"^F\d{8,}"))
                    {
                        var timestamp = DateTime.ParseExact(timeStr, "HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        timestamp = DateTime.Today.Add(timestamp.TimeOfDay);

                        return new LogEntry
                        {
                            Timestamp = timestamp,
                            Level = ParseLogLevel(levelStr),
                            Message = remainingMessage,
                            RawLine = line,
                            CorrelationId = correlationIdOrMessage
                        };
                    }
                }

                // Fallback to standard Serilog format WITHOUT correlation
                // Example: [14:23:45 INF] Pipeline Radiology started
                var standardMatch = Regex.Match(line, @"^\[?(\d{2}:\d{2}:\d{2}(?:\.\d{3})?)\s+(\w+)\]?\s+(.+)$");

                if (standardMatch.Success)
                {
                    var timeStr = standardMatch.Groups[1].Value;
                    var levelStr = standardMatch.Groups[2].Value;
                    var message = standardMatch.Groups[3].Value;

                    // Parse timestamp
                    DateTime timestamp;
                    if (timeStr.Contains('.'))
                    {
                        timestamp = DateTime.ParseExact(timeStr, "HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        timestamp = DateTime.ParseExact(timeStr, "HH:mm:ss", CultureInfo.InvariantCulture);
                    }

                    // Add today's date
                    timestamp = DateTime.Today.Add(timestamp.TimeOfDay);

                    var level = ParseLogLevel(levelStr);

                    return new LogEntry
                    {
                        Timestamp = timestamp,
                        Level = level,
                        Message = message,
                        RawLine = line
                    };
                }

                // Last resort - just treat as info message
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
                // Return the line as-is
                return new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Level = LogLevel.Information,
                    Message = line,
                    RawLine = line
                };
            }
        }

        private LogLevel DetermineLogLevel(ProcessingStage? stage, string message)
        {
            // Check stage first
            if (stage == ProcessingStage.Error)
                return LogLevel.Error;

            // Check message content
            var messageLower = message.ToLowerInvariant();
            if (messageLower.Contains("error") || messageLower.Contains("failed"))
                return LogLevel.Error;

            if (messageLower.Contains("warning") || messageLower.Contains("warn") || messageLower.Contains("slow"))
                return LogLevel.Warning;

            if (messageLower.Contains("debug"))
                return LogLevel.Debug;

            // Default to Information
            return LogLevel.Information;
        }

        private LogLevel ParseLogLevel(string levelStr)
        {
            return levelStr.ToUpperInvariant() switch
            {
                "DBG" or "DEB" or "DEBUG" => LogLevel.Debug,
                "INF" or "INFO" or "INFORMATION" => LogLevel.Information,
                "WRN" or "WARN" or "WARNING" => LogLevel.Warning,
                "ERR" or "ERROR" => LogLevel.Error,
                "CRT" or "CRIT" or "CRITICAL" or "FTL" or "FATAL" => LogLevel.Critical,
                _ => LogLevel.Information  // Default to Information instead of failing
            };
        }

        private void ClearLogs()
        {
            LogEntries.Clear();
            CombinedLogEntries.Clear();
            FilteredCombinedEntries.Clear();
            CorrelationGroups.Clear();
            TotalLineCount = 0;
            _filePositions.Clear();
        }

        private async Task ExportLogsAsync()
        {
            try
            {
                // Create export dialog
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Log files (*.log)|*.log|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    DefaultExt = ".log",
                    FileName = $"CamBridge_Export_{DateTime.Now:yyyyMMdd_HHmmss}.log"
                };

                if (dialog.ShowDialog() == true)
                {
                    var sb = new StringBuilder();

                    if (IsTreeViewEnabled)
                    {
                        // Export tree structure
                        foreach (var group in CorrelationGroups)
                        {
                            sb.AppendLine($"=== Correlation: {group.CorrelationId} ===");
                            sb.AppendLine($"Pipeline: {group.Pipeline}");
                            sb.AppendLine($"Duration: {group.DurationText}");
                            sb.AppendLine($"Status: {group.Status}");
                            sb.AppendLine();

                            foreach (var stage in group.Stages)
                            {
                                sb.AppendLine($"  Stage: {stage.Stage} [{stage.DurationText}]");
                                foreach (var entry in stage.Entries)
                                {
                                    sb.AppendLine($"    {entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff} {entry.LevelText} {entry.Message}");
                                }
                                sb.AppendLine();
                            }

                            if (group.UngroupedEntries.Any())
                            {
                                sb.AppendLine("  [Ungrouped Entries]");
                                foreach (var entry in group.UngroupedEntries)
                                {
                                    sb.AppendLine($"    {entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff} {entry.LevelText} {entry.Message}");
                                }
                                sb.AppendLine();
                            }

                            sb.AppendLine(new string('=', 50));
                            sb.AppendLine();
                        }
                    }
                    else
                    {
                        // Export flat list
                        var lines = FilteredCombinedEntries.Select(e => e.RawLine ??
                            $"{e.Timestamp:yyyy-MM-dd HH:mm:ss.fff} {e.LevelText} [{e.CorrelationId}] [{e.Stage}] {e.Message}");
                        foreach (var line in lines)
                        {
                            sb.AppendLine(line);
                        }
                    }

                    await File.WriteAllTextAsync(dialog.FileName, sb.ToString(), Encoding.UTF8);

                    _logger.LogInformation("Exported {Count} log entries to {FileName}",
                        IsTreeViewEnabled ? CorrelationGroups.Sum(g => g.TotalEntries) : FilteredCombinedEntries.Count,
                        dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export logs");
            }
        }

        private string GetLogFileName(string pipelineSelection)
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

            // Get sanitized name from mapping
            if (_pipelineNameMapping.TryGetValue(pipelineSelection, out var sanitizedName))
            {
                return $"pipeline_{sanitizedName}_{today}.log";
            }

            // Fallback - sanitize the selection directly
            var safeName = SanitizeForFileName(pipelineSelection.Replace(" (Archived)", ""));
            return $"pipeline_{safeName}_{today}.log";
        }

        private string SanitizeForFileName(string pipelineName)
        {
            // Replace invalid filename characters and common separators
            var invalid = Path.GetInvalidFileNameChars()
                .Concat(new[] { ' ', '.', ',', '/', '\\', ':', '-' })
                .Distinct()
                .ToArray();

            var sanitized = string.Join("_", pipelineName.Split(invalid, StringSplitOptions.RemoveEmptyEntries));

            // Limit length
            if (sanitized.Length > 100)
            {
                sanitized = sanitized.Substring(0, 97) + "...";
            }

            return sanitized;
        }

        private void OnRefreshTimer(object? state)
        {
            if (IsAutoScrollEnabled && !IsLoading)
            {
                _ = RefreshLogsAsync();
            }
        }

        private void OnPipelineSelectionChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PipelineSelection.IsSelected))
            {
                // Update count
                OnPropertyChanged(nameof(SelectedPipelineCount));

                // Update current files display
                var selectedNames = PipelineSelections.Where(p => p.IsSelected).Select(p => p.Name).ToList();
                CurrentLogFiles = selectedNames.Any()
                    ? string.Join(", ", selectedNames)
                    : "No pipelines selected";

                // Refresh logs with new selection
                _ = RefreshLogsAsync();
            }
        }

        partial void OnSearchTextChanged(string? value)
        {
            // Debounce search to avoid too many updates
            ApplyFilters();
        }

        partial void OnShowDebugChanged(bool value)
        {
            ApplyFilters();
            OnPropertyChanged(nameof(SelectedLevelCount));
        }

        partial void OnShowInformationChanged(bool value)
        {
            ApplyFilters();
            OnPropertyChanged(nameof(SelectedLevelCount));
        }

        partial void OnShowWarningChanged(bool value)
        {
            ApplyFilters();
            OnPropertyChanged(nameof(SelectedLevelCount));
        }

        partial void OnShowErrorChanged(bool value)
        {
            ApplyFilters();
            OnPropertyChanged(nameof(SelectedLevelCount));
        }

        partial void OnShowCriticalChanged(bool value)
        {
            ApplyFilters();
            OnPropertyChanged(nameof(SelectedLevelCount));
        }

        partial void OnIsTreeViewEnabledChanged(bool value)
        {
            ApplyFilters();
        }

        partial void OnFilter1Changed(string value)
        {
            ApplyFilters();
        }

        partial void OnFilter2Changed(string value)
        {
            ApplyFilters();
        }

        partial void OnFilter3Changed(string value)
        {
            ApplyFilters();
        }
    }

    /// <summary>
    /// Enhanced log entry with correlation support
    /// </summary>
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? RawLine { get; set; }
        public string Source { get; set; } = string.Empty; // Which log file this came from

        // Enhanced properties for correlation
        public string? CorrelationId { get; set; }
        public ProcessingStage? Stage { get; set; }
        public string? Pipeline { get; set; }
        public int? DurationMs { get; set; }

        // UI Helper Properties
        public string LevelText => Level switch
        {
            LogLevel.Debug => "DBG",
            LogLevel.Information => "INF",
            LogLevel.Warning => "WRN",
            LogLevel.Error => "ERR",
            LogLevel.Critical => "CRT",
            _ => "???"
        };

        public string LevelColor => Level switch
        {
            LogLevel.Debug => "#808080",      // Gray
            LogLevel.Information => "#0078D4", // Blue
            LogLevel.Warning => "#FFA500",     // Orange
            LogLevel.Error => "#FF0000",       // Red
            LogLevel.Critical => "#8B0000",    // Dark Red
            _ => "#000000"
        };

        // FIXED emoji icons!
        public string StageIcon => Stage switch
        {
            ProcessingStage.ServiceStartup => "🚀",
            ProcessingStage.ConfigurationLoading => "⚙️",
            ProcessingStage.PipelineInitialization => "🔧",
            ProcessingStage.ServiceShutdown => "🛑",
            ProcessingStage.FileDetected => "📄",
            ProcessingStage.ExifExtraction => "📷",
            ProcessingStage.TagMapping => "🔄",
            ProcessingStage.DicomConversion => "🏥",
            ProcessingStage.PostProcessing => "📋",
            ProcessingStage.PacsUpload => "☁️",
            ProcessingStage.Complete => "✅",
            ProcessingStage.Error => "❌",
            ProcessingStage.PipelineShutdown => "🔴",
            ProcessingStage.PipelineRecovery => "🔧",
            ProcessingStage.WatcherError => "👁️",
            ProcessingStage.HealthCheck => "💓",
            _ => "📌"
        };

        public string FormattedDuration => DurationMs.HasValue ? $"{DurationMs}ms" : "";
    }

    /// <summary>
    /// Represents a group of log entries with the same correlation ID
    /// </summary>
    public class CorrelationGroup : ObservableObject
    {
        private bool _isExpanded;

        public string CorrelationId { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Pipeline { get; set; } = string.Empty;
        public ProcessingStatus Status { get; set; }
        public ObservableCollection<StageGroup> Stages { get; } = new();
        public ObservableCollection<LogEntry> UngroupedEntries { get; } = new();

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public TimeSpan Duration => EndTime - StartTime;
        public string DurationText => $"{Duration.TotalMilliseconds:0}ms";
        public int TotalEntries => Stages.Sum(s => s.Entries.Count) + UngroupedEntries.Count;

        // FIXED emoji icons!
        public string StatusIcon => Status switch
        {
            ProcessingStatus.Completed => "✅",
            ProcessingStatus.Failed => "❌",
            ProcessingStatus.InProgress => "⏳",
            _ => "❓"
        };

        public string StatusColor => Status switch
        {
            ProcessingStatus.Completed => "#4CAF50",
            ProcessingStatus.Failed => "#F44336",
            ProcessingStatus.InProgress => "#FFA500",
            _ => "#808080"
        };
    }

    /// <summary>
    /// Represents a group of log entries for a specific processing stage
    /// </summary>
    public class StageGroup : ObservableObject
    {
        private bool _isExpanded;

        public ProcessingStage Stage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ObservableCollection<LogEntry> Entries { get; } = new();

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public TimeSpan Duration => EndTime - StartTime;
        public string DurationText => $"{Duration.TotalMilliseconds:0}ms";

        public string StageIcon => Entries.FirstOrDefault()?.StageIcon ?? "📌";
    }

    /// <summary>
    /// Processing status for correlation groups
    /// </summary>
    public enum ProcessingStatus
    {
        Unknown,
        InProgress,
        Completed,
        Failed
    }
}
