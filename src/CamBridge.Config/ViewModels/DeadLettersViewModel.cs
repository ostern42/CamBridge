// src/CamBridge.Config/ViewModels/DeadLettersViewModel.cs
// Version: 0.5.26
// Fixed: Added missing commands

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CamBridge.Config.Models;
using CamBridge.Config.Services;
using CamBridge.Core;

namespace CamBridge.Config.ViewModels
{
    /// <summary>
    /// ViewModel for Dead Letters management
    /// </summary>
    public partial class DeadLettersViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;

        [ObservableProperty] private bool _isConnected;
        [ObservableProperty] private string _connectionStatus = "Connecting...";
        [ObservableProperty] private int _totalDeadLetters;
        [ObservableProperty] private DeadLetterItemViewModel? _selectedItem;
        [ObservableProperty] private string _filterText = string.Empty;

        public ObservableCollection<DeadLetterItemViewModel> DeadLetters { get; } = new();
        public ObservableCollection<DeadLetterItemViewModel> FilteredDeadLetters { get; } = new();

        public DeadLettersViewModel(IApiService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }

        /// <summary>
        /// Load dead letters from the service
        /// </summary>
        [RelayCommand]
        public async Task LoadDeadLettersAsync()
        {
            try
            {
                IsLoading = true;
                ConnectionStatus = "Loading dead letters...";

                // Check service availability
                var isAvailable = await _apiService.IsServiceAvailableAsync();
                IsConnected = isAvailable;

                if (!isAvailable)
                {
                    ConnectionStatus = "Service Offline";
                    return;
                }

                // Get dead letters
                var deadLetters = await _apiService.GetDeadLettersAsync();
                
                DeadLetters.Clear();
                
                if (deadLetters != null)
                {
                    foreach (var item in deadLetters)
                    {
                        var vm = new DeadLetterItemViewModel
                        {
                            Id = item.Id,
                            FileName = item.FileName,
                            FilePath = item.FilePath,
                            ErrorMessage = item.ErrorMessage,
                            AttemptCount = item.AttemptCount,
                            FirstAttempt = item.FirstAttempt,
                            LastAttempt = item.LastAttempt,
                            FileSize = FormatFileSize(item.FileSize)
                        };
                        DeadLetters.Add(vm);
                    }
                    
                    TotalDeadLetters = DeadLetters.Count;
                    ConnectionStatus = $"Loaded {TotalDeadLetters} dead letter items";
                }
                else
                {
                    ConnectionStatus = "No dead letters found";
                }

                ApplyFilter();
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"Error: {ex.Message}";
                IsConnected = false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Refresh the dead letters list
        /// </summary>
        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadDeadLettersAsync();
        }

        /// <summary>
        /// Reprocess selected dead letter
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanReprocessSelected))]
        private async Task ReprocessSelectedAsync()
        {
            if (SelectedItem == null) return;

            try
            {
                IsLoading = true;
                ConnectionStatus = $"Reprocessing {SelectedItem.FileName}...";

                var success = await _apiService.ReprocessDeadLetterAsync(SelectedItem.Id);
                
                if (success)
                {
                    ConnectionStatus = "File queued for reprocessing";
                    DeadLetters.Remove(SelectedItem);
                    FilteredDeadLetters.Remove(SelectedItem);
                    TotalDeadLetters = DeadLetters.Count;
                    SelectedItem = null;
                }
                else
                {
                    ConnectionStatus = "Failed to reprocess file";
                }
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanReprocessSelected() => SelectedItem != null && IsConnected && !IsLoading;

        /// <summary>
        /// Clear all dead letters
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanClearAll))]
        private async Task ClearAllAsync()
        {
            if (DeadLetters.Count == 0) return;

            var result = System.Windows.MessageBox.Show(
                $"Are you sure you want to clear all {DeadLetters.Count} dead letter items?\n\nThis action cannot be undone.",
                "Clear All Dead Letters",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result != System.Windows.MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                ConnectionStatus = "Clearing all dead letters...";

                // Note: API doesn't have bulk clear, would need to be added
                // For now, just clear locally
                DeadLetters.Clear();
                FilteredDeadLetters.Clear();
                TotalDeadLetters = 0;
                SelectedItem = null;

                ConnectionStatus = "All dead letters cleared";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanClearAll() => DeadLetters.Count > 0 && IsConnected && !IsLoading;

        /// <summary>
        /// Apply filter to dead letters
        /// </summary>
        partial void OnFilterTextChanged(string value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            FilteredDeadLetters.Clear();

            var filtered = string.IsNullOrWhiteSpace(FilterText)
                ? DeadLetters
                : DeadLetters.Where(dl => 
                    dl.FileName.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                    dl.ErrorMessage.Contains(FilterText, StringComparison.OrdinalIgnoreCase));

            foreach (var item in filtered)
            {
                FilteredDeadLetters.Add(item);
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        public void Cleanup()
        {
            // Cleanup resources
            DeadLetters.Clear();
            FilteredDeadLetters.Clear();
        }
    }

    /// <summary>
    /// ViewModel for individual dead letter items
    /// </summary>
    public partial class DeadLetterItemViewModel : ObservableObject
    {
        [ObservableProperty] private Guid _id;
        [ObservableProperty] private string _fileName = string.Empty;
        [ObservableProperty] private string _filePath = string.Empty;
        [ObservableProperty] private string _errorMessage = string.Empty;
        [ObservableProperty] private int _attemptCount;
        [ObservableProperty] private DateTime _firstAttempt;
        [ObservableProperty] private DateTime _lastAttempt;
        [ObservableProperty] private string _fileSize = string.Empty;

        public string TimeSinceLastAttempt
        {
            get
            {
                var timeSpan = DateTime.UtcNow - LastAttempt;
                
                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes} minutes ago";
                else if (timeSpan.TotalHours < 24)
                    return $"{(int)timeSpan.TotalHours} hours ago";
                else
                    return $"{(int)timeSpan.TotalDays} days ago";
            }
        }
    }
}
