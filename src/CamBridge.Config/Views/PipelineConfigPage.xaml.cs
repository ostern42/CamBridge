// src\CamBridge.Config\Views\PipelineConfigPage.xaml.cs
// Version: 0.6.7
// Description: Pipeline Configuration page code-behind

using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// Pipeline Configuration page - Zero Global Settings!
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class PipelineConfigPage : Page
    {
        public PipelineConfigPage()
        {
            InitializeComponent();
        }

        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // Get the ViewModel from DI container
            var app = Application.Current as App;
            if (app?.Host?.Services != null)
            {
                var viewModel = app.Host.Services.GetRequiredService<PipelineConfigViewModel>();
                DataContext = viewModel;

                // Initialize the ViewModel
                await viewModel.InitializeAsync();
            }
        }

        // Browse button handlers
        private void BrowseWatchFolder_Click(object sender, RoutedEventArgs e)
        {
            BrowseFolder("Select Watch Folder", path =>
            {
                if (DataContext is PipelineConfigViewModel vm && vm.SelectedPipeline != null)
                {
                    vm.SelectedPipeline.WatchSettings.Path = path;
                }
            });
        }

        private void BrowseOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            BrowseFolder("Select Output Folder", path =>
            {
                if (DataContext is PipelineConfigViewModel vm && vm.SelectedPipeline != null)
                {
                    vm.SelectedPipeline.ProcessingOptions.ArchiveFolder = path;
                }
            });
        }

        private void BrowseErrorFolder_Click(object sender, RoutedEventArgs e)
        {
            BrowseFolder("Select Error Folder", path =>
            {
                if (DataContext is PipelineConfigViewModel vm && vm.SelectedPipeline != null)
                {
                    vm.SelectedPipeline.ProcessingOptions.ErrorFolder = path;
                }
            });
        }

        private void BrowseDeadLetterFolder_Click(object sender, RoutedEventArgs e)
        {
            BrowseFolder("Select Dead Letter Folder", path =>
            {
                if (DataContext is PipelineConfigViewModel vm && vm.SelectedPipeline != null)
                {
                    vm.SelectedPipeline.ProcessingOptions.DeadLetterFolder = path;
                }
            });
        }

        private void BrowseLogFolder_Click(object sender, RoutedEventArgs e)
        {
            BrowseFolder("Select Log Folder", path =>
            {
                if (DataContext is PipelineConfigViewModel vm)
                {
                    vm.PipelineLogFolder = path;
                }
            });
        }

        private void BrowseFolder(string description, Action<string> setPath)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = description,
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                setPath(dialog.SelectedPath);
            }
        }
    }
}


