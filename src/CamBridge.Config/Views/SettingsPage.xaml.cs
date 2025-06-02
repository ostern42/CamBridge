using CamBridge.Config.ViewModels;
using Microsoft.Win32;
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

            // CRITICAL FIX: Initialize ViewModel immediately!
            Loaded += async (s, e) =>
            {
                if (DataContext is SettingsViewModel vm)
                {
                    _viewModel = vm;

                    // Initialize the ViewModel to load settings
                    await vm.InitializeAsync();
                }
            };
        }

        private void BrowseWatchFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select Watch Folder"
            };

            if (dialog.ShowDialog() == true && _viewModel?.SelectedWatchFolder != null)
            {
                _viewModel.SelectedWatchFolder.Path = dialog.FolderName;
            }
        }

        private void BrowseOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select Output Folder"
            };

            if (dialog.ShowDialog() == true && _viewModel != null)
            {
                _viewModel.DefaultOutputFolder = dialog.FolderName;
            }
        }

        private void BrowseLogFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select Log Folder"
            };

            if (dialog.ShowDialog() == true && _viewModel != null)
            {
                _viewModel.LogFolder = dialog.FolderName;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Only allow numeric input
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}
