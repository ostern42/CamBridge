// src/CamBridge.Config/ViewModels/LogViewerViewModel.cs
// Version: 0.8.22
// Description: FIXED LogViewerViewModel - Pipeline filter now works in FlatView!
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Config.Models;
using CamBridge.Config.Services;
using CamBridge.Config.Services.Interfaces;
using CamBridge.Core;
using CamBridge.Core.Enums;
using CamBridge.Core.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CamBridge.Config.ViewModels
{
    /// <summary>
    /// FIXED ViewModel for log viewer - Pipeline filter works in both views!
    /// </summary>
    public partial class LogViewerViewModel : ViewModelBase
    {
        private readonly ILogger<LogViewerViewModel> _logger;
        private readonly IConfigurationService _configurationService;
        private readonly ILogFileService _logFileService;
        private readonly ILogParsingService _logParsingService;
        private readonly ILogFilterService _logFilterService;
        private readonly ILogTreeBuilder _logTreeBuilder;
        private readonly Timer _refreshTimer;

        // Track file positions separately since model doesn't have them
        private readonly Dictionary<string, long> _filePositions = new();
        private readonly Dictionary<string, string> _filePaths = new();

        private const int RefreshIntervalMs = 10000; // 10 seconds instead of 1
        private const int MaxHistoryItems = 10;

        public LogViewerViewModel(
            ILogger<LogViewerViewModel> logger,
            IConfigurationService configurationService,
            ILogFileService logFileService,
            ILogParsingService logParsingService,
            ILogFilterService logFilterService,
            ILogTreeBuilder logTreeBuilder)
        {
            _logger = logger;
            _configurationService = configurationService;
            _logFileService = logFileService;
            _logParsingService = logParsingService;
            _logFilterService = logFilterService;
            _logTreeBuilder = logTreeBuilder;

            // Initialize collections
            LogEntries = new ObservableCollection<LogEntry>();
            CombinedLogEntries = new ObservableCollection<LogEntry>();
            FilteredCombinedEntries = new ObservableCollection<LogEntry>();
            CorrelationGroups = new ObservableCollection<CorrelationGroup>();
            PipelineSelections = new ObservableCollection<PipelineSelection>();

            // Initialize search history
            Filter1History = new ObservableCollection<string>();
            Filter2History = new ObservableCollection<string>();
            Filter3History = new ObservableCollection<string>();

            // Set defaults
            ShowDebug = true;
            ShowInformation = true;
            ShowWarning = true;
            ShowError = true;
            ShowCritical = true;
            IsTreeViewEnabled = true;

            // FIX: Set default date to today!
            StartDate = DateTime.Today;
            EndDate = DateTime.Today.AddDays(1).AddSeconds(-1);

            // Setup auto-refresh timer
            _refreshTimer = new Timer(OnRefreshTimerTick, null, Timeout.Infinite, Timeout.Infinite);

            // Setup property change handler - renamed to avoid conflict!
            PropertyChanged += OnViewModelPropertyChanged;
        }

        #region Observable Properties

        [ObservableProperty]
        private ObservableCollection<LogEntry> logEntries;

        [ObservableProperty]
        private ObservableCollection<LogEntry> combinedLogEntries;

        [ObservableProperty]
        private ObservableCollection<LogEntry> filteredCombinedEntries;

        [ObservableProperty]
        private ObservableCollection<CorrelationGroup> correlationGroups;

        [ObservableProperty]
        private ObservableCollection<PipelineSelection> pipelineSelections;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isTreeViewEnabled;

        [ObservableProperty]
        private bool isAutoScrollEnabled;

        // Search history collections
        [ObservableProperty]
        private ObservableCollection<string> filter1History;

        [ObservableProperty]
        private ObservableCollection<string> filter2History;

        [ObservableProperty]
        private ObservableCollection<string> filter3History;

        // Filter properties - NO history spam on PropertyChanged!
        private string _filter1 = string.Empty;
        public string Filter1
        {
            get => _filter1;
            set
            {
                if (SetProperty(ref _filter1, value))
                {
                    ApplyFilters();
                    // Do NOT add to history here!
                }
            }
        }

        private string _filter2 = string.Empty;
        public string Filter2
        {
            get => _filter2;
            set
            {
                if (SetProperty(ref _filter2, value))
                {
                    ApplyFilters();
                    // Do NOT add to history here!
                }
            }
        }

        private string _filter3 = string.Empty;
        public string Filter3
        {
            get => _filter3;
            set
            {
                if (SetProperty(ref _filter3, value))
                {
                    ApplyFilters();
                    // Do NOT add to history here!
                }
            }
        }

        public string PipelineSelectionText
        {
            get
            {
                var count = PipelineSelections?.Count(p => p.IsSelected) ?? 0;
                if (count == 0)
                    return "Select pipelines...";
                else if (count == 1)
                    return PipelineSelections.First(p => p.IsSelected).Name;
                else
                    return $"{count} pipelines selected";
            }
        }

        public string LevelSelectionText
        {
            get
            {
                var levels = new List<string>();
                if (ShowDebug) levels.Add("Debug");
                if (ShowInformation) levels.Add("Info");
                if (ShowWarning) levels.Add("Warning");
                if (ShowError) levels.Add("Error");
                if (ShowCritical) levels.Add("Critical");

                if (levels.Count == 0)
                    return "No levels";
                else if (levels.Count == 5)
                    return "All levels";
                else if (levels.Count == 1)
                    return levels[0];
                else
                    return $"{levels.Count} levels";
            }
        }

        // Add property to show loaded files in status bar
        public string LoadedFilesText
        {
            get
            {
                var loadedCount = PipelineSelections?.Count(p => p.IsSelected) ?? 0;
                if (loadedCount == 0)
                    return "No files loaded";
                else if (loadedCount == 1)
                    return "1 file loaded";
                else
                    return $"{loadedCount} files loaded";
            }
        }

        [ObservableProperty]
        private bool showDebug;

        [ObservableProperty]
        private bool showInformation;

        [ObservableProperty]
        private bool showWarning;

        [ObservableProperty]
        private bool showError;

        [ObservableProperty]
        private bool showCritical;

        #endregion

        #region Computed Properties

        public int TotalLineCount => CombinedLogEntries?.Count ?? 0;
        public int DisplayedLineCount => FilteredCombinedEntries?.Count ?? 0;
        public int SelectedPipelineCount => PipelineSelections?.Count(p => p.IsSelected) ?? 0;

        // Add this property to show/hide "Filters active" in status bar
        public bool HasActiveFilters =>
            !string.IsNullOrWhiteSpace(Filter1) ||
            !string.IsNullOrWhiteSpace(Filter2) ||
            !string.IsNullOrWhiteSpace(Filter3) ||
            StartDate.HasValue ||
            EndDate.HasValue ||
            !ShowDebug || !ShowInformation || !ShowWarning || !ShowError || !ShowCritical;

        #endregion

        #region Date Range Properties

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                {
                    ApplyFilters();
                    // Reload available pipelines for new date range
                    _ = LoadAvailablePipelinesAsync();
                }
            }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value))
                {
                    ApplyFilters();
                    // Reload available pipelines for new date range
                    _ = LoadAvailablePipelinesAsync();
                }
            }
        }

        #endregion

        #region Commands

        // Tree View Commands
        [RelayCommand]
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
            _logger.LogDebug("Expanded all tree nodes");
        }

        [RelayCommand]
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
            _logger.LogDebug("Collapsed all tree nodes");
        }

        // Date Range Commands
        [RelayCommand]
        private void SetToday()
        {
            StartDate = DateTime.Today;
            EndDate = DateTime.Today.AddDays(1).AddSeconds(-1); // End of today
        }

        [RelayCommand]
        private void SetLastWeek()
        {
            EndDate = DateTime.Today.AddDays(1).AddSeconds(-1);
            StartDate = DateTime.Today.AddDays(-7);
        }

        [RelayCommand]
        private void SetLastMonth()
        {
            EndDate = DateTime.Today.AddDays(1).AddSeconds(-1);
            StartDate = DateTime.Today.AddDays(-30);
        }

        [RelayCommand]
        private void ClearDateRange()
        {
            StartDate = null;
            EndDate = null;
        }

        // Main Commands
        [RelayCommand]
        private async Task LoadLogFileAsync()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Log files (*.log)|*.log|All files (*.*)|*.*",
                Title = "Select Log File"
            };

            if (dialog.ShowDialog() == true)
            {
                await LoadSpecificLogFileAsync(dialog.FileName);
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            var selectedPipelines = PipelineSelections.Where(p => p.IsSelected).ToList();
            if (!selectedPipelines.Any())
            {
                _logger.LogInformation("No pipelines selected for refresh");
                return;
            }

            try
            {
                // Don't show loading indicator for auto-refresh to reduce flicker
                if (!IsAutoScrollEnabled)
                    IsLoading = true;

                var allEntries = new List<LogEntry>();

                foreach (var pipeline in selectedPipelines)
                {
                    // Get file path from our tracking dictionary
                    if (_filePaths.TryGetValue(pipeline.Name, out var filePath) && File.Exists(filePath))
                    {
                        // Get stored position
                        var startPosition = _filePositions.TryGetValue(pipeline.Name, out var pos) ? pos : 0;

                        // Read all entries from file
                        var entries = await _logFileService.ReadLogFileAsync(filePath, pipeline.Name);

                        // For incremental reading, skip already read entries
                        if (startPosition > 0 && entries.Count > 0)
                        {
                            // Simple approach: store count instead of file position
                            var previousCount = (int)startPosition;
                            if (entries.Count > previousCount)
                            {
                                entries = entries.Skip(previousCount).ToList();
                            }
                            else
                            {
                                entries.Clear(); // No new entries
                            }
                        }

                        // Update position (using count as proxy)
                        _filePositions[pipeline.Name] = _filePositions.TryGetValue(pipeline.Name, out var oldPos)
                            ? oldPos + entries.Count
                            : entries.Count;

                        allEntries.AddRange(entries);
                    }
                }

                if (allEntries.Any())
                {
                    UpdateCombinedEntries(allEntries);
                    _logger.LogDebug("Refreshed {Count} new entries from {Files} files",
                        allEntries.Count, selectedPipelines.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing logs");
            }
            finally
            {
                if (!IsAutoScrollEnabled)
                    IsLoading = false;
            }
        }

        [RelayCommand]
        private void ClearLog()
        {
            LogEntries.Clear();
            CombinedLogEntries.Clear();
            FilteredCombinedEntries.Clear();
            CorrelationGroups.Clear();
            _filePositions.Clear();
            _logger.LogInformation("Cleared all log entries");
        }

        [RelayCommand]
        private async Task ExportLogAsync()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Log files (*.log)|*.log|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Export Log",
                FileName = $"cambridge_export_{DateTime.Now:yyyyMMdd_HHmmss}.log"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var entries = IsTreeViewEnabled ?
                        GetAllEntriesFromGroups() :
                        FilteredCombinedEntries.ToList();

                    // Simple export implementation
                    var lines = entries.Select(e => e.RawLine ??
                        $"[{e.Timestamp:HH:mm:ss.fff}] [{e.LevelText}] {e.Message}");

                    await File.WriteAllLinesAsync(dialog.FileName, lines);
                    _logger.LogInformation("Exported {Count} log entries to {File}", entries.Count, dialog.FileName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error exporting logs");
                    MessageBox.Show($"Error exporting logs: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void OpenLogFolder()
        {
            var logPath = ConfigurationPaths.GetLogsDirectory();
            if (Directory.Exists(logPath))
            {
                Process.Start("explorer.exe", logPath);
            }
        }

        [RelayCommand]
        private void ClearFilters()
        {
            Filter1 = string.Empty;
            Filter2 = string.Empty;
            Filter3 = string.Empty;
        }

        /// <summary>
        /// Generic command to copy any text to clipboard
        /// </summary>
        [RelayCommand]
        private void CopyText(string? text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    Clipboard.SetText(text);
                    _logger.LogDebug("Copied text to clipboard: {Length} characters", text.Length);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to copy text to clipboard");
                }
            }
        }

        /// <summary>
        /// Export entire correlation group to clipboard
        /// </summary>
        [RelayCommand]
        private void ExportGroup(CorrelationGroup? group)
        {
            if (group == null) return;

            try
            {
                var sb = new StringBuilder();
                sb.AppendLine($"=== Correlation Group Export ===");
                sb.AppendLine($"Correlation ID: {group.CorrelationId}");
                sb.AppendLine($"Pipeline: {group.Pipeline}");
                sb.AppendLine($"Start Time: {group.StartTime:yyyy-MM-dd HH:mm:ss.fff}");
                sb.AppendLine($"Duration: {group.DurationText}");
                sb.AppendLine($"Status: {group.Status}");
                sb.AppendLine();

                foreach (var stage in group.Stages)
                {
                    sb.AppendLine($"[{stage.Stage}] - {stage.Entries.Count} entries - {stage.DurationText}");
                    foreach (var entry in stage.Entries.OrderBy(e => e.Timestamp))
                    {
                        sb.AppendLine($"  {entry.Timestamp:HH:mm:ss.fff} [{entry.LevelText}] {entry.Message}");
                    }
                    sb.AppendLine();
                }

                Clipboard.SetText(sb.ToString());
                _logger.LogInformation("Exported correlation group {CorrelationId} to clipboard", group.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export correlation group");
            }
        }

        /// <summary>
        /// Export all entries from a stage group to clipboard
        /// </summary>
        [RelayCommand]
        private void ExportStage(StageGroup? stage)
        {
            if (stage == null) return;

            try
            {
                var sb = new StringBuilder();
                sb.AppendLine($"=== Stage Export: {stage.Stage} ===");
                sb.AppendLine($"Entries: {stage.Entries.Count}");
                sb.AppendLine($"Duration: {stage.DurationText}");
                sb.AppendLine();

                foreach (var entry in stage.Entries.OrderBy(e => e.Timestamp))
                {
                    sb.AppendLine($"{entry.Timestamp:HH:mm:ss.fff} [{entry.LevelText}] {entry.Message}");
                }

                Clipboard.SetText(sb.ToString());
                _logger.LogInformation("Exported stage {Stage} to clipboard", stage.Stage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export stage");
            }
        }

        #endregion

        #region Public Methods for UI

        /// <summary>
        /// Add current Filter1 to history (called from UI on Enter/LostFocus)
        /// </summary>
        public void AddToFilter1History()
        {
            AddToHistory(Filter1, Filter1History);
        }

        /// <summary>
        /// Add current Filter2 to history
        /// </summary>
        public void AddToFilter2History()
        {
            AddToHistory(Filter2, Filter2History);
        }

        /// <summary>
        /// Add current Filter3 to history
        /// </summary>
        public void AddToFilter3History()
        {
            AddToHistory(Filter3, Filter3History);
        }

        #endregion

        #region Initialization

        public async Task InitializeAsync()
        {
            _logger.LogInformation("Initializing LogViewerViewModel");

            // Load available pipelines
            await LoadAvailablePipelinesAsync();

            // NEW: Auto-select all pipelines
            if (PipelineSelections.Any())
            {
                foreach (var pipeline in PipelineSelections)
                {
                    pipeline.IsSelected = true;
                }

                // Auto-refresh to load initial data
                await RefreshAsync();
            }

            // NEW: Start auto-refresh timer (10 second interval)
            _refreshTimer.Change(RefreshIntervalMs, RefreshIntervalMs);

            // Auto-scroll is now effectively auto-refresh
            IsAutoScrollEnabled = true;
        }

        #endregion

        #region Private Methods

        // MERGED and renamed to avoid conflict!
        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ShowDebug):
                case nameof(ShowInformation):
                case nameof(ShowWarning):
                case nameof(ShowError):
                case nameof(ShowCritical):
                case nameof(IsTreeViewEnabled):
                    ApplyFilters();
                    OnPropertyChanged(nameof(HasActiveFilters));
                    OnPropertyChanged(nameof(LevelSelectionText));  // FIX: Update level text
                    break;
                case nameof(IsAutoScrollEnabled):
                    if (IsAutoScrollEnabled)
                        _refreshTimer.Change(RefreshIntervalMs, RefreshIntervalMs);
                    else
                        _refreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    break;
                case nameof(Filter1):
                case nameof(Filter2):
                case nameof(Filter3):
                case nameof(StartDate):
                case nameof(EndDate):
                    OnPropertyChanged(nameof(HasActiveFilters));
                    break;
            }

            // Update pipeline count when selection changes
            if (e.PropertyName == nameof(PipelineSelections))
            {
                foreach (var pipeline in PipelineSelections)
                {
                    pipeline.PropertyChanged -= OnPipelineSelectionChanged;
                    pipeline.PropertyChanged += OnPipelineSelectionChanged;
                }
            }
        }

        private void OnPipelineSelectionChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PipelineSelection.IsSelected))
            {
                OnPropertyChanged(nameof(SelectedPipelineCount));
                OnPropertyChanged(nameof(PipelineSelectionText));  // FIX: This was missing!
                OnPropertyChanged(nameof(LoadedFilesText));        // FIX: Update status bar

                // FIX: Apply filters when pipeline selection changes!
                ApplyFilters();
            }
        }

        private async Task LoadAvailablePipelinesAsync()
        {
            try
            {
                // Use the generic method from IConfigurationService
                var settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettingsV2>();
                var logPath = ConfigurationPaths.GetLogsDirectory();

                // Remember selected pipelines before clearing
                var selectedNames = PipelineSelections
                    .Where(p => p.IsSelected)
                    .Select(p => p.SanitizedName)
                    .ToHashSet();

                PipelineSelections.Clear();
                _filePaths.Clear();

                // If date range is selected, check multiple days
                var searchDays = new List<DateTime>();

                if (StartDate.HasValue && EndDate.HasValue)
                {
                    var current = StartDate.Value.Date;
                    while (current <= EndDate.Value.Date)
                    {
                        searchDays.Add(current);
                        current = current.AddDays(1);
                    }
                }
                else
                {
                    // Default to today only
                    searchDays.Add(DateTime.Today);
                }

                // Track which pipelines we've already added (to avoid duplicates)
                var addedPipelines = new HashSet<string>();

                // Add service logs for each day
                foreach (var date in searchDays)
                {
                    // FIX: Changed from "cambridgeservice_" to "service_"
                    var servicePath = Path.Combine(logPath, $"service_{date:yyyyMMdd}.log");
                    if (File.Exists(servicePath))
                    {
                        var servicePipeline = new PipelineSelection
                        {
                            Name = $"Service ({date:yyyy-MM-dd})",
                            SanitizedName = "Service",
                            IsSelected = selectedNames.Contains("Service")  // Restore selection
                        };
                        PipelineSelections.Add(servicePipeline);
                        _filePaths[$"Service ({date:yyyy-MM-dd})"] = servicePath;
                    }
                }

                // FIRST: Add pipelines from current config
                if (settings?.Pipelines != null)
                {
                    foreach (var pipeline in settings.Pipelines)
                    {
                        foreach (var date in searchDays)
                        {
                            var sanitizedName = SanitizeForFileName(pipeline.Name);
                            var pipelinePath = Path.Combine(logPath, $"pipeline_{sanitizedName}_{date:yyyyMMdd}.log");
                            if (File.Exists(pipelinePath))
                            {
                                var key = $"{sanitizedName}_{date:yyyy-MM-dd}";
                                if (!addedPipelines.Contains(key))
                                {
                                    var pipelineSelection = new PipelineSelection
                                    {
                                        Name = $"{pipeline.Name} ({date:yyyy-MM-dd})",
                                        SanitizedName = sanitizedName,
                                        IsSelected = selectedNames.Contains(sanitizedName)  // Restore selection
                                    };
                                    PipelineSelections.Add(pipelineSelection);
                                    _filePaths[$"{pipeline.Name} ({date:yyyy-MM-dd})"] = pipelinePath;
                                    addedPipelines.Add(key);
                                }
                            }
                        }
                    }
                }

                // OPTION 1: Find ALL pipeline log files (including renamed/deleted pipelines)
                foreach (var date in searchDays)
                {
                    var pattern = $"pipeline_*_{date:yyyyMMdd}.log";
                    var allPipelineFiles = Directory.GetFiles(logPath, pattern);

                    foreach (var filePath in allPipelineFiles)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(filePath);

                        // Extract pipeline name from filename: pipeline_{name}_{date}
                        var match = Regex.Match(fileName, @"^pipeline_(.+)_(\d{8})$");
                        if (match.Success)
                        {
                            var pipelineName = match.Groups[1].Value;
                            var key = $"{pipelineName}_{date:yyyy-MM-dd}";

                            // Skip if already added from config
                            if (!addedPipelines.Contains(key))
                            {
                                // This is an archived/renamed pipeline!
                                var displayName = UnSanitizeFileName(pipelineName);
                                var pipelineSelection = new PipelineSelection
                                {
                                    Name = $"{displayName} ({date:yyyy-MM-dd}) [Archived]",
                                    SanitizedName = pipelineName,
                                    IsSelected = selectedNames.Contains(pipelineName)  // Restore selection
                                };
                                PipelineSelections.Add(pipelineSelection);
                                _filePaths[$"{displayName} ({date:yyyy-MM-dd}) [Archived]"] = filePath;
                                addedPipelines.Add(key);

                                _logger.LogDebug("Found archived pipeline log: {Pipeline} for {Date}",
                                    displayName, date);
                            }
                        }
                    }
                }

                // Wire up property change handlers
                foreach (var pipeline in PipelineSelections)
                {
                    pipeline.PropertyChanged += OnPipelineSelectionChanged;
                }

                _logger.LogInformation("Found {Count} log files across {Days} days ({Archived} archived)",
                    PipelineSelections.Count, searchDays.Count,
                    PipelineSelections.Count(p => p.Name.Contains("[Archived]")));

                // Update UI
                OnPropertyChanged(nameof(PipelineSelectionText));
                OnPropertyChanged(nameof(LoadedFilesText));

                // Auto-refresh if files were selected
                if (PipelineSelections.Any(p => p.IsSelected))
                {
                    await RefreshAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading available pipelines");
            }
        }

        private string SanitizeForFileName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars()
                .Concat(new[] { ' ', '.', ',', '/', '\\', ':', '-' })
                .Distinct()
                .ToArray();
            return string.Join("_", name.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Try to reverse sanitization for display (best effort)
        /// </summary>
        private string UnSanitizeFileName(string sanitizedName)
        {
            // Common replacements (best guess)
            return sanitizedName
                .Replace("_", " ")
                .Replace("ae", "ä")  // German umlauts
                .Replace("oe", "ö")
                .Replace("ue", "ü")
                .Replace("Ae", "Ä")
                .Replace("Oe", "Ö")
                .Replace("Ue", "Ü");
        }

        private async Task LoadSpecificLogFileAsync(string filePath)
        {
            try
            {
                IsLoading = true;
                _logger.LogInformation("Loading log file: {FilePath}", filePath);

                var sourceName = Path.GetFileNameWithoutExtension(filePath);
                var entries = await _logFileService.ReadLogFileAsync(filePath, sourceName);

                LogEntries.Clear();
                foreach (var entry in entries)
                {
                    LogEntries.Add(entry);
                }

                UpdateCombinedEntries(entries);

                _logger.LogInformation("Loaded {Count} log entries", entries.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading log file");
                MessageBox.Show($"Error loading log file: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateCombinedEntries(List<LogEntry> newEntries)
        {
            // Add to combined collection
            foreach (var entry in newEntries)
            {
                CombinedLogEntries.Add(entry);
            }

            // Sort by timestamp! (FIX for multi-file)
            var sorted = CombinedLogEntries.OrderBy(e => e.Timestamp).ToList();
            CombinedLogEntries.Clear();
            foreach (var entry in sorted)
            {
                CombinedLogEntries.Add(entry);
            }

            // Keep only last 10000 entries
            while (CombinedLogEntries.Count > 10000)
            {
                CombinedLogEntries.RemoveAt(0);
            }

            // Apply filters
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            // Create criteria using ACTUAL property names from LogFilterCriteria
            var criteria = new LogFilterCriteria
            {
                ShowDebug = ShowDebug,
                ShowInformation = ShowInformation,
                ShowWarning = ShowWarning,
                ShowError = ShowError,
                ShowCritical = ShowCritical,
                Filter1 = Filter1,
                Filter2 = Filter2,
                Filter3 = Filter3
            };

            // First apply level and text filters
            var filtered = _logFilterService.ApplyFilters(CombinedLogEntries, criteria);

            // Then apply date range filter if set
            if (StartDate.HasValue || EndDate.HasValue)
            {
                var startTime = StartDate ?? DateTime.MinValue;
                var endTime = EndDate?.AddDays(1) ?? DateTime.MaxValue; // Include entire end day

                filtered = filtered.Where(e => e.Timestamp >= startTime && e.Timestamp < endTime).ToList();
            }

            // FIX: Apply pipeline filter!
            var selectedPipelines = PipelineSelections.Where(p => p.IsSelected).Select(p => p.SanitizedName).ToHashSet();
            if (selectedPipelines.Any())
            {
                filtered = filtered.Where(e =>
                {
                    // Match against the source field (which contains the sanitized name)
                    // OR against Pipeline field for newer entries
                    // Service logs might have "Service (2025-01-04)" as source but no Pipeline
                    return (e.Source != null && selectedPipelines.Any(sp => e.Source.Contains(sp))) ||
                           (e.Pipeline != null && selectedPipelines.Contains(e.Pipeline));
                }).ToList();
            }

            FilteredCombinedEntries.Clear();
            foreach (var entry in filtered)
            {
                FilteredCombinedEntries.Add(entry);
            }

            // Update tree view if enabled
            if (IsTreeViewEnabled)
            {
                // Remember expanded states before update
                var expandedStates = new Dictionary<string, bool>();
                foreach (var group in CorrelationGroups)
                {
                    expandedStates[group.CorrelationId] = group.IsExpanded;
                    foreach (var stage in group.Stages)
                    {
                        expandedStates[$"{group.CorrelationId}_{stage.Stage}"] = stage.IsExpanded;
                    }
                }

                var groups = _logTreeBuilder.BuildCorrelationGroups(filtered);

                // SORT groups by StartTime ASCENDING (oldest first, newest at bottom)
                var sortedGroups = groups.OrderBy(g => g.StartTime).ToList();

                // Update collection without clearing (reduces flicker)
                // First, remove groups that no longer exist
                var existingIds = new HashSet<string>(CorrelationGroups.Select(g => g.CorrelationId));
                var newIds = new HashSet<string>(sortedGroups.Select(g => g.CorrelationId));
                var toRemove = existingIds.Except(newIds).ToList();

                foreach (var id in toRemove)
                {
                    var group = CorrelationGroups.FirstOrDefault(g => g.CorrelationId == id);
                    if (group != null)
                        CorrelationGroups.Remove(group);
                }

                // Then add new groups or update existing
                for (int i = 0; i < sortedGroups.Count; i++)
                {
                    var newGroup = sortedGroups[i];
                    var existingGroup = CorrelationGroups.FirstOrDefault(g => g.CorrelationId == newGroup.CorrelationId);

                    if (existingGroup == null)
                    {
                        // New group - default to collapsed unless it was previously expanded
                        newGroup.IsExpanded = expandedStates.ContainsKey(newGroup.CorrelationId) && expandedStates[newGroup.CorrelationId];

                        // Default all stages to collapsed unless previously expanded
                        foreach (var stage in newGroup.Stages)
                        {
                            var stageKey = $"{newGroup.CorrelationId}_{stage.Stage}";
                            stage.IsExpanded = expandedStates.ContainsKey(stageKey) && expandedStates[stageKey];
                        }

                        // Insert at correct position
                        if (i < CorrelationGroups.Count)
                            CorrelationGroups.Insert(i, newGroup);
                        else
                            CorrelationGroups.Add(newGroup);
                    }
                    else
                    {
                        // Update existing group while preserving expansion state
                        existingGroup.Update(newGroup);

                        // Move to correct position if needed
                        var currentIndex = CorrelationGroups.IndexOf(existingGroup);
                        if (currentIndex != i && i < CorrelationGroups.Count - 1)
                        {
                            CorrelationGroups.Move(currentIndex, i);
                        }
                    }
                }
            }

            OnPropertyChanged(nameof(TotalLineCount));
            OnPropertyChanged(nameof(DisplayedLineCount));
        }

        private async void OnRefreshTimerTick(object? state)
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                if (!IsLoading)
                {
                    await RefreshAsync();
                }
            });
        }

        private List<LogEntry> GetAllEntriesFromGroups()
        {
            var entries = new List<LogEntry>();
            foreach (var group in CorrelationGroups)
            {
                foreach (var stage in group.Stages)
                {
                    entries.AddRange(stage.Entries);
                }
            }
            return entries.OrderBy(e => e.Timestamp).ToList();
        }

        /// <summary>
        /// Generic method to add filter to history
        /// </summary>
        private void AddToHistory(string filter, ObservableCollection<string> history)
        {
            if (string.IsNullOrWhiteSpace(filter)) return;

            // Remove if already exists (to move to top)
            if (history.Contains(filter))
                history.Remove(filter);

            // Add to top
            history.Insert(0, filter);

            // Keep only MaxHistoryItems
            while (history.Count > MaxHistoryItems)
                history.RemoveAt(history.Count - 1);

            _logger.LogDebug("Added to search history: {Filter}", filter);
        }

        #endregion

        public void Cleanup()
        {
            _refreshTimer?.Dispose();
            _logFileService?.StopWatching();
        }
    }
}
