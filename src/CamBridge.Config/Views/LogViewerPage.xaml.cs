// src\CamBridge.Config\Views\LogViewerPage.xaml.cs
// Version: 0.7.28
// Description: Enhanced code-behind with multi-line selection support

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace CamBridge.Config.Views
{
    public partial class LogViewerPage : Page
    {
        private LogViewerViewModel? _viewModel;
        private bool _autoScrollPending;

        public LogViewerPage()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }

            _viewModel = DataContext as LogViewerViewModel;

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Get ViewModel from DI if not already set
            if (_viewModel == null)
            {
                var app = Application.Current as App;
                if (app?.Host?.Services != null)
                {
                    try
                    {
                        _viewModel = app.Host.Services.GetRequiredService<LogViewerViewModel>();
                        DataContext = _viewModel;
                    }
                    catch (Exception ex)
                    {
                        // Fallback: Create with minimal dependencies
                        var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<LogViewerViewModel>();
                        var configService = new Services.ConfigurationService();
                        _viewModel = new LogViewerViewModel(logger, configService);
                        DataContext = _viewModel;
                    }
                }
            }

            if (_viewModel != null)
            {
                await _viewModel.InitializeAsync();

                // Initial scroll to bottom after logs are loaded
                await Task.Delay(500); // Give UI time to render
                AutoScrollToBottom();
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                _viewModel.Cleanup();
            }
        }

        private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LogViewerViewModel.FilteredCombinedEntries) &&
                _viewModel?.IsAutoScrollEnabled == true &&
                _viewModel.FilteredCombinedEntries.Count > 0)
            {
                // Debounce auto-scroll to avoid performance issues
                if (!_autoScrollPending)
                {
                    _autoScrollPending = true;
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, () =>
                    {
                        AutoScrollToBottom();
                        _autoScrollPending = false;
                    });
                }
            }
        }

        private void AutoScrollToBottom()
        {
            if (LogListBox.Items.Count > 0)
            {
                var lastItem = LogListBox.Items[LogListBox.Items.Count - 1];
                LogListBox.ScrollIntoView(lastItem);
            }
        }

        // Multi-selection copy support
        private void LogListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                CopySelectedLines();
                e.Handled = true;
            }
        }

        private void CopySelectedLines_Click(object sender, RoutedEventArgs e)
        {
            CopySelectedLines();
        }

        private void CopySelectedMessages_Click(object sender, RoutedEventArgs e)
        {
            CopySelectedMessages();
        }

        private void CopySelectedAsCSV_Click(object sender, RoutedEventArgs e)
        {
            CopySelectedAsCSV();
        }

        private void ExportSelected_Click(object sender, RoutedEventArgs e)
        {
            ExportSelectedLines();
        }

        private void CopySelectedLines()
        {
            if (LogListBox.SelectedItems.Count == 0) return;

            var sb = new StringBuilder();
            var selectedEntries = LogListBox.SelectedItems.Cast<LogEntry>()
                .OrderBy(e => e.Timestamp)
                .ToList();

            foreach (var entry in selectedEntries)
            {
                sb.AppendLine($"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] [{entry.LevelText}] {entry.Message}");
            }

            try
            {
                Clipboard.SetText(sb.ToString());

                // Optional: Show toast/status that copy was successful
                if (_viewModel != null && selectedEntries.Count > 1)
                {
                    _viewModel.CurrentLogFile = $"Copied {selectedEntries.Count} lines to clipboard";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to copy to clipboard: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CopySelectedMessages()
        {
            if (LogListBox.SelectedItems.Count == 0) return;

            var sb = new StringBuilder();
            var selectedEntries = LogListBox.SelectedItems.Cast<LogEntry>()
                .OrderBy(e => e.Timestamp)
                .ToList();

            foreach (var entry in selectedEntries)
            {
                sb.AppendLine(entry.Message);
            }

            try
            {
                Clipboard.SetText(sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to copy to clipboard: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CopySelectedAsCSV()
        {
            if (LogListBox.SelectedItems.Count == 0) return;

            var sb = new StringBuilder();
            sb.AppendLine("Timestamp,Level,Source,Message");

            var selectedEntries = LogListBox.SelectedItems.Cast<LogEntry>()
                .OrderBy(e => e.Timestamp)
                .ToList();

            foreach (var entry in selectedEntries)
            {
                // Escape CSV fields
                var message = entry.Message.Replace("\"", "\"\"");
                sb.AppendLine($"\"{entry.Timestamp:yyyy-MM-dd HH:mm:ss}\",\"{entry.LevelText}\",\"{entry.Source}\",\"{message}\"");
            }

            try
            {
                Clipboard.SetText(sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to copy to clipboard: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExportSelectedLines()
        {
            if (LogListBox.SelectedItems.Count == 0) return;

            var dialog = new SaveFileDialog
            {
                Filter = "Log files (*.log)|*.log|Text files (*.txt)|*.txt|CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                DefaultExt = ".log",
                FileName = $"CamBridge_Selected_{DateTime.Now:yyyyMMdd_HHmmss}.log"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var selectedEntries = LogListBox.SelectedItems.Cast<LogEntry>()
                        .OrderBy(e => e.Timestamp)
                        .ToList();

                    var lines = new List<string>();

                    if (dialog.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                    {
                        // CSV format
                        lines.Add("Timestamp,Level,Source,Message");
                        foreach (var entry in selectedEntries)
                        {
                            var message = entry.Message.Replace("\"", "\"\"");
                            lines.Add($"\"{entry.Timestamp:yyyy-MM-dd HH:mm:ss}\",\"{entry.LevelText}\",\"{entry.Source}\",\"{message}\"");
                        }
                    }
                    else
                    {
                        // Standard log format
                        foreach (var entry in selectedEntries)
                        {
                            lines.Add($"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] [{entry.LevelText}] {entry.Message}");
                        }
                    }

                    System.IO.File.WriteAllLines(dialog.FileName, lines, Encoding.UTF8);

                    if (_viewModel != null)
                    {
                        _viewModel.CurrentLogFile = $"Exported {selectedEntries.Count} entries to {System.IO.Path.GetFileName(dialog.FileName)}";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to export: {ex.Message}", "Export Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
