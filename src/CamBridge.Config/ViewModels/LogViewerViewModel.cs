// src\CamBridge.Config\ViewModels\LogViewerViewModel.cs
// Version: 0.7.28
// Description: Professional log viewer with multi-pipeline support, Unicode handling, and real-time updates
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Config.Services;
using CamBridge.Core;
using CamBridge.Core.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// ViewModel for the professional log viewer with multi-pipeline support
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
            AvailablePipelines = new ObservableCollection<string>();
            PipelineSelections = new ObservableCollection<PipelineSelection>();

            // Initialize commands
            RefreshCommand = new AsyncRelayCommand(RefreshLogsAsync);
            ClearLogCommand = new RelayCommand(ClearLogs);
            ExportLogCommand = new AsyncRelayCommand(ExportLogsAsync);

            // Initialize timer for auto-refresh
            _refreshTimer = new Timer(OnRefreshTimer, null, Timeout.Infinite, Timeout.Infinite);

            // Set default filter values - ALLE AN für sinnvolle Defaults!
            ShowDebug = false;  // Debug meist zu viel
            ShowInformation = true;
            ShowWarning = true;
            ShowError = true;
            ShowCritical = true;
            IsAutoScrollEnabled = false; // Default OFF to prevent flicker
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
        private ObservableCollection<string> availablePipelines;

        [ObservableProperty]
        private ObservableCollection<PipelineSelection> pipelineSelections;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCombinedEntries))]
        private string? searchText;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCombinedEntries))]
        private bool showDebug;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCombinedEntries))]
        private bool showInformation;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCombinedEntries))]
        private bool showWarning;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCombinedEntries))]
        private bool showError;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCombinedEntries))]
        private bool showCritical;

        [ObservableProperty]
        private bool isAutoScrollEnabled;

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

        public int DisplayedLineCount => FilteredCombinedEntries?.Count ?? 0;

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

        #endregion

        #region Public Methods

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Initializing LogViewerViewModel");

                // Load available pipelines
                await LoadAvailablePipelinesAsync();

                // PipelineSelections sollte bereits "Service (Global)" als ausgewählt haben!
                // (Das passiert in LoadAvailablePipelinesAsync mit IsSelected = true)

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

            // Apply level filters - wenn KEINS ausgewählt, zeige ALLE!
            var anyLevelSelected = ShowDebug || ShowInformation || ShowWarning || ShowError || ShowCritical;

            _logger.LogDebug("Level filter state: Debug={Debug}, Info={Info}, Warn={Warn}, Error={Error}, Critical={Critical}, Any={Any}",
                ShowDebug, ShowInformation, ShowWarning, ShowError, ShowCritical, anyLevelSelected);

            if (anyLevelSelected)
            {
                filtered = filtered.Where(e =>
                    (ShowDebug && e.Level == LogLevel.Debug) ||
                    (ShowInformation && e.Level == LogLevel.Information) ||
                    (ShowWarning && e.Level == LogLevel.Warning) ||
                    (ShowError && e.Level == LogLevel.Error) ||
                    (ShowCritical && e.Level == LogLevel.Critical));
            }
            // Wenn keine Level ausgewählt → zeige alle!

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchLower = SearchText.ToLowerInvariant();
                filtered = filtered.Where(e =>
                    e.Message.ToLowerInvariant().Contains(searchLower) ||
                    e.LevelText.ToLowerInvariant().Contains(searchLower) ||
                    e.Source.ToLowerInvariant().Contains(searchLower));

                _logger.LogDebug("Search filter applied: '{Search}'", SearchText);
            }

            // Update filtered collection
            var filteredList = filtered.ToList();
            _logger.LogDebug("After filtering: {Count} entries remain", filteredList.Count);

            FilteredCombinedEntries.Clear();
            foreach (var entry in filteredList)
            {
                FilteredCombinedEntries.Add(entry);
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
                    // Check for old combined pipelines log
                    if (File.Exists(Path.Combine(logPath, $"pipelines_{DateTime.Now:yyyyMMdd}.log")))
                    {
                        PipelineSelections.Add(new PipelineSelection
                        {
                            Name = "All Pipelines (Legacy)",
                            SanitizedName = "pipelines",
                            IsSelected = false
                        });
                    }

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

        private async Task ReadTailAsync(string logPath, int lineCount)
        {
            try
            {
                var lines = new List<string>();

                using (var fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fs, Encoding.UTF8))
                {
                    // Efficient tail reading
                    var buffer = new Queue<string>(lineCount);
                    string? line;

                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (buffer.Count >= lineCount)
                            buffer.Dequeue();
                        buffer.Enqueue(line);
                    }

                    lines.AddRange(buffer);
                    _filePositions[logPath] = fs.Position;
                }

                // Parse and add entries
                LogEntries.Clear();
                foreach (var line in lines)
                {
                    var entry = ParseLogLine(line);
                    if (entry != null)
                        LogEntries.Add(entry);
                }

                TotalLineCount = LogEntries.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read tail of log file: {LogPath}", logPath);
            }
        }

        private async Task ReadNewLinesAsync(string logPath, long lastPosition)
        {
            try
            {
                using (var fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // If file was truncated, reset
                    if (fs.Length < lastPosition)
                    {
                        await ReadTailAsync(logPath, TailLineCount);
                        return;
                    }

                    fs.Seek(lastPosition, SeekOrigin.Begin);
                    using (var reader = new StreamReader(fs, Encoding.UTF8))
                    {
                        string? line;
                        var newEntries = new List<LogEntry>();

                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            var entry = ParseLogLine(line);
                            if (entry != null)
                                newEntries.Add(entry);
                        }

                        // Add new entries and maintain max count
                        foreach (var entry in newEntries)
                        {
                            LogEntries.Add(entry);
                            if (LogEntries.Count > MaxDisplayedEntries)
                                LogEntries.RemoveAt(0);
                        }

                        _filePositions[logPath] = fs.Position;
                        TotalLineCount = LogEntries.Count;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read new lines from log file: {LogPath}", logPath);
            }
        }

        private LogEntry? ParseLogLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            try
            {
                // Serilog format: [HH:mm:ss LVL] Message
                // Example: [14:23:45 INF] Pipeline Radiology started
                // Flexibleres Pattern für verschiedene Level-Längen (INF, INFO, ERR, ERROR etc.)
                var match = Regex.Match(line, @"^\[(\d{2}:\d{2}:\d{2})\s+(\w+)\]\s+(.+)$");

                if (match.Success)
                {
                    var timeStr = match.Groups[1].Value;
                    var levelStr = match.Groups[2].Value;
                    var message = match.Groups[3].Value;

                    // Parse timestamp - handle with or without milliseconds
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
                else
                {
                    // Try alternative format without brackets
                    // Example: 2025-06-22 14:23:45 [INF] Message
                    match = Regex.Match(line, @"^(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2})\s+\[(\w{3})\]\s+(.+)$");

                    if (match.Success)
                    {
                        var dateTimeStr = match.Groups[1].Value;
                        var levelStr = match.Groups[2].Value;
                        var message = match.Groups[3].Value;

                        var timestamp = DateTime.Parse(dateTimeStr);
                        var level = ParseLogLevel(levelStr);

                        return new LogEntry
                        {
                            Timestamp = timestamp,
                            Level = level,
                            Message = message,
                            RawLine = line
                        };
                    }

                    // Fallback for non-standard format
                    _logger.LogDebug("Could not parse log line format: {Line}", line);
                    return new LogEntry
                    {
                        Timestamp = DateTime.Now,
                        Level = LogLevel.Information,
                        Message = line,
                        RawLine = line
                    };
                }
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
                    var lines = FilteredCombinedEntries.Select(e => e.RawLine ?? e.Message);
                    await File.WriteAllLinesAsync(dialog.FileName, lines, Encoding.UTF8);

                    _logger.LogInformation("Exported {Count} log entries to {FileName}",
                        FilteredCombinedEntries.Count, dialog.FileName);
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
    }

    /// <summary>
    /// Represents a single log entry with metadata
    /// </summary>
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? RawLine { get; set; }
        public string Source { get; set; } = string.Empty; // Which pipeline this came from

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
    }
}
