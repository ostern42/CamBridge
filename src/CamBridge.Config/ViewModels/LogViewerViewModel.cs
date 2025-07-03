// src/CamBridge.Config/ViewModels/LogViewerViewModel.cs
// Version: 0.8.18
// Description: SLIM log viewer ViewModel with search history!
// Copyright: (C) 2025 Claude's Improbably Reliable Software Solutions

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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CamBridge.Config.ViewModels
{
    /// <summary>
    /// SLIM ViewModel for log viewer - delegates all logic to services
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

        private const int RefreshIntervalMs = 1000;
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
            FilteredCombinedEntries = new ObservableCollection<LogEntry>();
            CombinedLogEntries = new ObservableCollection<LogEntry>();
            CorrelationGroups = new ObservableCollection<CorrelationGroup>();
            PipelineSelections = new ObservableCollection<PipelineSelection>();
            Filter1History = new ObservableCollection<string>();
            Filter2History = new ObservableCollection<string>();
            Filter3History = new ObservableCollection<string>();

            // Initialize commands
            RefreshCommand = new AsyncRelayCommand(RefreshLogsAsync);
            ClearLogCommand = new RelayCommand(() => { CombinedLogEntries.Clear(); ApplyFilters(); });
            ExportLogCommand = new AsyncRelayCommand(ExportLogsAsync);
            OpenLogFolderCommand = new RelayCommand(OpenLogFolder);
            ClearFiltersCommand = new RelayCommand(() => { Filter1 = Filter2 = Filter3 = ""; });
            CopyLineCommand = new RelayCommand<LogEntry>(CopyLine);
            CopyGroupCommand = new RelayCommand<CorrelationGroup>(CopyGroup);

            // Timer for auto-refresh
            _refreshTimer = new Timer(_ => { if (IsAutoScrollEnabled) _ = RefreshLogsAsync(); },
                                    null, Timeout.Infinite, Timeout.Infinite);

            // Set defaults
            ShowInformation = ShowWarning = ShowError = ShowCritical = true;
            IsTreeViewEnabled = true;
        }

        #region Properties

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayedLineCount))]
        private ObservableCollection<LogEntry> filteredCombinedEntries;

        [ObservableProperty]
        private ObservableCollection<LogEntry> combinedLogEntries;

        [ObservableProperty]
        private ObservableCollection<CorrelationGroup> correlationGroups;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedPipelineCount))]
        private ObservableCollection<PipelineSelection> pipelineSelections;

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
        private ObservableCollection<string> filter1History;

        [ObservableProperty]
        private ObservableCollection<string> filter2History;

        [ObservableProperty]
        private ObservableCollection<string> filter3History;

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
        [NotifyPropertyChangedFor(nameof(DisplayedLineCount))]
        private bool isTreeViewEnabled;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string currentLogFiles = "No files selected";

        [ObservableProperty]
        private int totalLineCount;

        public int DisplayedLineCount => IsTreeViewEnabled
            ? CorrelationGroups?.Sum(g => g.TotalEntries) ?? 0
            : FilteredCombinedEntries?.Count ?? 0;

        public int SelectedPipelineCount => PipelineSelections?.Count(p => p.IsSelected) ?? 0;

        #endregion

        #region Commands

        public IAsyncRelayCommand RefreshCommand { get; }
        public IRelayCommand ClearLogCommand { get; }
        public IAsyncRelayCommand ExportLogCommand { get; }
        public IRelayCommand OpenLogFolderCommand { get; }
        public IRelayCommand ClearFiltersCommand { get; }
        public IRelayCommand<LogEntry> CopyLineCommand { get; }
        public IRelayCommand<CorrelationGroup> CopyGroupCommand { get; }

        #endregion

        #region Public Methods

        public async Task InitializeAsync()
        {
            try
            {
                await LoadPipelinesAsync();
                await RefreshLogsAsync();

                if (IsAutoScrollEnabled)
                    _refreshTimer.Change(RefreshIntervalMs, RefreshIntervalMs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize");
            }
        }

        public void Cleanup()
        {
            _refreshTimer?.Dispose();
            _logFileService.StopWatching();
            foreach (var sel in PipelineSelections)
                sel.PropertyChanged -= OnPipelineSelectionChanged;
        }

        #endregion

        #region Private Methods

        private async Task LoadPipelinesAsync()
        {
            PipelineSelections.Clear();

            // Add service log
            PipelineSelections.Add(new PipelineSelection
            {
                Name = "Service (Global)",
                SanitizedName = "service",
                IsSelected = true
            });

            // Add configured pipelines
            var settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettingsV2>();
            if (settings?.Pipelines != null)
            {
                var isFirst = true;
                foreach (var p in settings.Pipelines)
                {
                    PipelineSelections.Add(new PipelineSelection
                    {
                        Name = p.Name,
                        SanitizedName = _logFileService.GetLogFileName(p.Name),
                        IsSelected = isFirst
                    });
                    isFirst = false;
                }
            }

            // Add archived logs
            var archived = await _logFileService.GetAvailableLogFilesAsync();
            foreach (var kvp in archived.Where(a => !PipelineSelections.Any(p => p.Name == a.Key)))
            {
                PipelineSelections.Add(new PipelineSelection
                {
                    Name = kvp.Key,
                    SanitizedName = kvp.Value,
                    IsSelected = false
                });
            }

            // Subscribe to changes
            foreach (var sel in PipelineSelections)
                sel.PropertyChanged += OnPipelineSelectionChanged;

            // Notify initial pipeline count
            OnPropertyChanged(nameof(SelectedPipelineCount));
        }

        private async Task RefreshLogsAsync()
        {
            try
            {
                IsLoading = true;
                var selected = PipelineSelections.Where(p => p.IsSelected).ToList();
                if (!selected.Any()) return;

                CurrentLogFiles = selected.Count == 1 ? selected[0].Name : $"{selected.Count} pipelines";

                // Collect entries
                var allEntries = new List<LogEntry>();
                foreach (var pipeline in selected)
                {
                    var path = Path.Combine(ConfigurationPaths.GetLogsDirectory(),
                                          _logFileService.GetLogFileName(pipeline.Name));
                    if (File.Exists(path))
                    {
                        var entries = await _logFileService.ReadLogFileAsync(path, pipeline.Name);
                        allEntries.AddRange(entries);
                    }
                }

                // Update UI
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    CombinedLogEntries.Clear();
                    foreach (var e in allEntries.OrderBy(x => x.Timestamp).TakeLast(10000))
                        CombinedLogEntries.Add(e);
                });

                TotalLineCount = CombinedLogEntries.Count;
                ApplyFilters();
            }
            finally { IsLoading = false; }
        }

        private void ApplyFilters()
        {
            var criteria = new LogFilterCriteria
            {
                ShowDebug = ShowDebug,
                ShowInformation = ShowInformation,
                ShowWarning = ShowWarning,
                ShowError = ShowError,
                ShowCritical = ShowCritical,
                SearchText = null, // No separate search field anymore
                Filter1 = Filter1,
                Filter2 = Filter2,
                Filter3 = Filter3
            };

            // Apply all filters to get matching entries
            var filtered = _logFilterService.ApplyFilters(CombinedLogEntries, criteria);

            if (IsTreeViewEnabled)
            {
                // For TreeView: Build groups from ALL entries, but only show filtered entries
                var allGroups = _logTreeBuilder.BuildCorrelationGroups(CombinedLogEntries, false);

                // Create a set of filtered entry references for fast lookup
                var filteredSet = new HashSet<LogEntry>(filtered);

                // Now filter the groups to only include those with matching entries
                var groupsWithMatches = new List<CorrelationGroup>();

                foreach (var group in allGroups)
                {
                    // Check if this group has any matching entries
                    var hasMatch = group.AllEntries.Any(e => filteredSet.Contains(e));

                    if (hasMatch)
                    {
                        // Create a filtered version of the group
                        var filteredGroup = new CorrelationGroup
                        {
                            CorrelationId = group.CorrelationId,
                            Pipeline = group.Pipeline,
                            Status = group.Status,
                            IsExpanded = group.IsExpanded
                        };

                        // Add only filtered entries to stages
                        foreach (var stage in group.Stages)
                        {
                            var filteredStage = new StageGroup
                            {
                                Stage = stage.Stage,
                                IsExpanded = stage.IsExpanded
                            };

                            var stageFilteredEntries = stage.Entries.Where(e => filteredSet.Contains(e)).ToList();
                            if (stageFilteredEntries.Any())
                            {
                                foreach (var entry in stageFilteredEntries)
                                {
                                    filteredStage.Entries.Add(entry);
                                }

                                // Update stage times based on filtered entries
                                filteredStage.StartTime = stageFilteredEntries.Min(e => e.Timestamp);
                                filteredStage.EndTime = stageFilteredEntries.Max(e => e.Timestamp);

                                filteredGroup.Stages.Add(filteredStage);
                            }
                        }

                        // Add filtered ungrouped entries
                        foreach (var entry in group.UngroupedEntries.Where(e => filteredSet.Contains(e)))
                        {
                            filteredGroup.UngroupedEntries.Add(entry);
                        }

                        if (filteredGroup.TotalEntries > 0)
                        {
                            // Update group times based on all filtered entries
                            var allFilteredEntries = filteredGroup.AllEntries.ToList();
                            filteredGroup.StartTime = allFilteredEntries.Min(e => e.Timestamp);
                            filteredGroup.EndTime = allFilteredEntries.Max(e => e.Timestamp);

                            groupsWithMatches.Add(filteredGroup);
                        }
                    }
                }

                CorrelationGroups.Clear();
                foreach (var g in groupsWithMatches)
                    CorrelationGroups.Add(g);

                // Update FilteredCombinedEntries to match what's shown in TreeView
                FilteredCombinedEntries.Clear();
                foreach (var e in filtered)
                    FilteredCombinedEntries.Add(e);
            }
            else
            {
                // For flat view: Just show filtered entries
                FilteredCombinedEntries.Clear();
                foreach (var e in filtered)
                    FilteredCombinedEntries.Add(e);
            }

            // Notify that the displayed count has changed
            OnPropertyChanged(nameof(DisplayedLineCount));
        }

        private void AddToHistory(string value, ObservableCollection<string> history)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            // Remove if already exists
            if (history.Contains(value))
                history.Remove(value);

            // Add to beginning
            history.Insert(0, value);

            // Keep only last N items
            while (history.Count > MaxHistoryItems)
                history.RemoveAt(history.Count - 1);
        }

        private async Task ExportLogsAsync()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Log files (*.log)|*.log",
                FileName = $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.log"
            };

            if (dialog.ShowDialog() == true)
            {
                var sb = new StringBuilder();
                if (IsTreeViewEnabled)
                {
                    foreach (var g in CorrelationGroups)
                    {
                        sb.AppendLine($"=== {g.CorrelationId} [{g.DurationText}] ===");
                        foreach (var e in g.AllEntries)
                            sb.AppendLine($"  {e.Timestamp:HH:mm:ss.fff} {e.Message}");
                    }
                }
                else
                {
                    foreach (var e in FilteredCombinedEntries)
                        sb.AppendLine(e.RawLine ?? $"{e.Timestamp:HH:mm:ss.fff} {e.Message}");
                }
                await File.WriteAllTextAsync(dialog.FileName, sb.ToString());
            }
        }

        private void OpenLogFolder()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = ConfigurationPaths.GetLogsDirectory(),
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void CopyLine(LogEntry? entry)
        {
            if (entry != null)
                Clipboard.SetText($"{entry.Timestamp:HH:mm:ss.fff} [{entry.CorrelationId}] {entry.Message}");
        }

        private void CopyGroup(CorrelationGroup? group)
        {
            if (group != null)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"=== {group.CorrelationId} [{group.DurationText}] ===");
                foreach (var e in group.AllEntries)
                    sb.AppendLine($"{e.Timestamp:HH:mm:ss.fff} {e.Message}");
                Clipboard.SetText(sb.ToString());
            }
        }

        private void OnPipelineSelectionChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PipelineSelection.IsSelected))
            {
                // Force update of SelectedPipelineCount
                OnPropertyChanged(nameof(SelectedPipelineCount));
                _ = RefreshLogsAsync();
            }
        }

        #endregion

        #region Property Change Handlers

        partial void OnShowDebugChanged(bool value) => ApplyFilters();
        partial void OnShowInformationChanged(bool value) => ApplyFilters();
        partial void OnShowWarningChanged(bool value) => ApplyFilters();
        partial void OnShowErrorChanged(bool value) => ApplyFilters();
        partial void OnShowCriticalChanged(bool value) => ApplyFilters();
        partial void OnIsTreeViewEnabledChanged(bool value) => ApplyFilters();

        partial void OnFilter1Changed(string value)
        {
            ApplyFilters();
            if (!string.IsNullOrWhiteSpace(value))
                AddToHistory(value, Filter1History);
        }

        partial void OnFilter2Changed(string value)
        {
            ApplyFilters();
            if (!string.IsNullOrWhiteSpace(value))
                AddToHistory(value, Filter2History);
        }

        partial void OnFilter3Changed(string value)
        {
            ApplyFilters();
            if (!string.IsNullOrWhiteSpace(value))
                AddToHistory(value, Filter3History);
        }

        partial void OnIsAutoScrollEnabledChanged(bool value)
        {
            if (value)
                _refreshTimer.Change(RefreshIntervalMs, RefreshIntervalMs);
            else
                _refreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        #endregion
    }
}
