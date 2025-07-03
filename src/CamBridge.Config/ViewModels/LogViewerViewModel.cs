// src/CamBridge.Config/ViewModels/LogViewerViewModel.cs
// Version: 0.8.16
// Description: SLIM log viewer ViewModel - only UI logic!
// Copyright: (C) 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Config.Models;
using CamBridge.Config.Services;
using CamBridge.Config.Services.Interfaces;
using CamBridge.Core;
using CamBridge.Core.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
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

            // Initialize commands
            RefreshCommand = new AsyncRelayCommand(RefreshLogsAsync);
            ClearLogCommand = new RelayCommand(() => { CombinedLogEntries.Clear(); ApplyFilters(); });
            ExportLogCommand = new AsyncRelayCommand(ExportLogsAsync);
            OpenLogFolderCommand = new RelayCommand(OpenLogFolder);
            ClearFiltersCommand = new RelayCommand(() => { SearchText = Filter1 = Filter2 = Filter3 = ""; });
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
                SearchText = SearchText,
                Filter1 = Filter1,
                Filter2 = Filter2,
                Filter3 = Filter3
            };

            var filtered = _logFilterService.ApplyFilters(CombinedLogEntries, criteria);

            FilteredCombinedEntries.Clear();
            foreach (var e in filtered)
                FilteredCombinedEntries.Add(e);

            if (IsTreeViewEnabled)
            {
                var groups = _logTreeBuilder.BuildCorrelationGroups(filtered,
                    !string.IsNullOrEmpty(Filter1 + Filter2 + Filter3),
                    Filter1, Filter2, Filter3);

                CorrelationGroups.Clear();
                foreach (var g in groups)
                    CorrelationGroups.Add(g);
            }
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
                OnPropertyChanged(nameof(SelectedPipelineCount));
                _ = RefreshLogsAsync();
            }
        }

        #endregion

        #region Property Change Handlers

        partial void OnSearchTextChanged(string? value) => ApplyFilters();
        partial void OnShowDebugChanged(bool value) => ApplyFilters();
        partial void OnShowInformationChanged(bool value) => ApplyFilters();
        partial void OnShowWarningChanged(bool value) => ApplyFilters();
        partial void OnShowErrorChanged(bool value) => ApplyFilters();
        partial void OnShowCriticalChanged(bool value) => ApplyFilters();
        partial void OnIsTreeViewEnabledChanged(bool value) => ApplyFilters();
        partial void OnFilter1Changed(string value) => ApplyFilters();
        partial void OnFilter2Changed(string value) => ApplyFilters();
        partial void OnFilter3Changed(string value) => ApplyFilters();

        #endregion
    }
}
