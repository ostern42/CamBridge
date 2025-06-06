// src\CamBridge.Config\Views\SettingsPage.xaml.cs
// Version: 0.5.36
// Description: Fixed with Ookii.Dialogs for modern folder browsing

using CamBridge.Config.Services;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Ookii.Dialogs.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CamBridge.Config.Views
{
    public partial class SettingsPage : Page
    {
        private SettingsViewModel? _viewModel;

        public SettingsPage()
        {
            InitializeComponent();

            // Get ViewModel from DI container
            try
            {
                var app = Application.Current as App;
                if (app?.Host?.Services != null)
                {
                    // Get ViewModel from DI - this ensures all dependencies are properly injected
                    _viewModel = app.Host.Services.GetRequiredService<SettingsViewModel>();
                    DataContext = _viewModel;

                    System.Diagnostics.Debug.WriteLine("SettingsViewModel loaded from DI container");
                }
                else
                {
                    // Fallback if DI not available
                    var configService = new ConfigurationService();
                    _viewModel = new SettingsViewModel(configService);
                    DataContext = _viewModel;

                    System.Diagnostics.Debug.WriteLine("SettingsViewModel created manually (fallback)");
                }

                // Initialize on load
                Loaded += async (s, e) =>
                {
                    try
                    {
                        await _viewModel.InitializeAsync();

                        // Debug info
                        System.Diagnostics.Debug.WriteLine($"Settings initialized - Folders: {_viewModel.WatchFolders.Count}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading settings: {ex.Message}", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating SettingsViewModel: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BrowseWatchFolder_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel?.SelectedWatchFolder == null)
            {
                MessageBox.Show("Please select a watch folder first.", "Information",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var folderPath = ShowFolderDialog("Select Watch Folder", _viewModel.SelectedWatchFolder.Path);
            if (!string.IsNullOrEmpty(folderPath))
            {
                // WICHTIG: Force PropertyChanged durch Clear/Set
                _viewModel.SelectedWatchFolder.Path = "";  // Clear first
                _viewModel.SelectedWatchFolder.Path = folderPath;  // Then set new value

                System.Diagnostics.Debug.WriteLine($"Watch folder path updated to: {folderPath}");
            }
        }

        private void BrowseOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderPath = ShowFolderDialog("Select Output Folder", _viewModel?.DefaultOutputFolder);
            if (!string.IsNullOrEmpty(folderPath) && _viewModel != null)
            {
                _viewModel.DefaultOutputFolder = folderPath;
                System.Diagnostics.Debug.WriteLine($"Output folder path updated to: {folderPath}");
            }
        }

        private void BrowseLogFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderPath = ShowFolderDialog("Select Log Folder", _viewModel?.LogFolder);
            if (!string.IsNullOrEmpty(folderPath) && _viewModel != null)
            {
                _viewModel.LogFolder = folderPath;
                System.Diagnostics.Debug.WriteLine($"Log folder path updated to: {folderPath}");
            }
        }

        private string? ShowFolderDialog(string title, string? initialPath)
        {
            // Use Ookii.Dialogs.Wpf for modern folder browsing
            var dialog = new VistaFolderBrowserDialog
            {
                Description = title,
                UseDescriptionForTitle = true,
                ShowNewFolderButton = true
            };

            if (!string.IsNullOrEmpty(initialPath) && System.IO.Directory.Exists(initialPath))
            {
                dialog.SelectedPath = initialPath;
            }

            // Get the window that owns this page
            var owner = Window.GetWindow(this);

            if (dialog.ShowDialog(owner) == true)
            {
                return dialog.SelectedPath;
            }

            return null;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Only allow numeric input
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}
