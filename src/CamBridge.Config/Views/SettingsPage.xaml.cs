// src\CamBridge.Config\Views\SettingsPage.xaml.cs
// Version: 0.5.26 - Mit vollem DI Support

using CamBridge.Config.Services;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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
            var folderPath = ShowFolderDialog("Select Watch Folder", _viewModel?.SelectedWatchFolder?.Path);
            if (!string.IsNullOrEmpty(folderPath) && _viewModel?.SelectedWatchFolder != null)
            {
                _viewModel.SelectedWatchFolder.Path = folderPath;
            }
        }

        private void BrowseOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderPath = ShowFolderDialog("Select Output Folder", _viewModel?.DefaultOutputFolder);
            if (!string.IsNullOrEmpty(folderPath) && _viewModel != null)
            {
                _viewModel.DefaultOutputFolder = folderPath;
            }
        }

        private void BrowseLogFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderPath = ShowFolderDialog("Select Log Folder", _viewModel?.LogFolder);
            if (!string.IsNullOrEmpty(folderPath) && _viewModel != null)
            {
                _viewModel.LogFolder = folderPath;
            }
        }

        private string? ShowFolderDialog(string title, string? initialPath)
        {
            // Try Windows Forms FolderBrowserDialog via reflection
            try
            {
                var assemblyName = "System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                var assembly = System.Reflection.Assembly.Load(assemblyName);
                var dialogType = assembly.GetType("System.Windows.Forms.FolderBrowserDialog");

                if (dialogType != null)
                {
                    dynamic dialog = Activator.CreateInstance(dialogType)!;
                    dialog.Description = title;
                    dialog.ShowNewFolderButton = true;

                    if (!string.IsNullOrEmpty(initialPath) && System.IO.Directory.Exists(initialPath))
                    {
                        dialog.SelectedPath = initialPath;
                    }

                    var dialogResultType = assembly.GetType("System.Windows.Forms.DialogResult");
                    var okValue = Enum.Parse(dialogResultType!, "OK");

                    if (dialog.ShowDialog() == okValue)
                    {
                        return dialog.SelectedPath;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FolderDialog error: {ex.Message}");
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
