// src/CamBridge.Config/Views/SettingsPage.xaml.cs
using System;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace CamBridge.Config.Views
{
    [SupportedOSPlatform("windows")]
    public partial class SettingsPage : Page
    {
        private SettingsViewModel? _viewModel;

        public SettingsPage()
        {
            InitializeComponent();

            // Defer ViewModel initialization to Loaded event
            Loaded += SettingsPage_Loaded;
        }

        private async void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Only initialize once
            if (_viewModel != null) return;

            try
            {
                // Get ViewModel from DI with null safety
                var app = Application.Current as App;
                if (app?.Host != null)
                {
                    _viewModel = app.Host.Services.GetRequiredService<SettingsViewModel>();
                    DataContext = _viewModel;

                    // Initialize the view model after setting DataContext
                    await _viewModel.InitializeAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("App.Host is null - DI not available");
                    ShowErrorMessage("Configuration service not available");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading SettingsViewModel: {ex.Message}");
                ShowErrorMessage($"Failed to load settings: {ex.Message}");
            }
        }

        private void ShowErrorMessage(string message)
        {
            // Create a simple error display
            var errorText = new TextBlock
            {
                Text = message,
                Margin = new Thickness(20),
                FontSize = 16,
                Foreground = System.Windows.Media.Brushes.Red
            };

            Content = new Grid { Children = { errorText } };
        }

        // Number validation for TextBox inputs
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // Browse folder dialogs with better error handling
        private void BrowseWatchFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = "Select Watch Folder",
                    CheckFileExists = false,
                    CheckPathExists = true,
                    FileName = "Select Folder",
                    Filter = "Folder|*.none",
                    ValidateNames = false
                };

                if (dialog.ShowDialog() == true)
                {
                    string? folderPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                    if (_viewModel?.SelectedWatchFolder != null && !string.IsNullOrEmpty(folderPath))
                    {
                        _viewModel.SelectedWatchFolder.Path = folderPath;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting folder: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BrowseOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = "Select Output Folder",
                    CheckFileExists = false,
                    CheckPathExists = true,
                    FileName = "Select Folder",
                    Filter = "Folder|*.none",
                    ValidateNames = false
                };

                if (dialog.ShowDialog() == true)
                {
                    string? folderPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                    if (_viewModel != null && !string.IsNullOrEmpty(folderPath))
                    {
                        _viewModel.DefaultOutputFolder = folderPath;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting folder: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BrowseLogFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = "Select Log Folder",
                    CheckFileExists = false,
                    CheckPathExists = true,
                    FileName = "Select Folder",
                    Filter = "Folder|*.none",
                    ValidateNames = false
                };

                if (dialog.ShowDialog() == true)
                {
                    string? folderPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                    if (_viewModel != null && !string.IsNullOrEmpty(folderPath))
                    {
                        _viewModel.LogFolder = folderPath;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting folder: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
