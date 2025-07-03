// src\CamBridge.Config\Views\Pages\LogViewerPage.xaml.cs
// Version: 0.8.16 - Enhanced with TreeView support
// Description: Log viewer page with hierarchical correlation display
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Config.Models;
using CamBridge.Config.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// Interaction logic for LogViewerPage.xaml - Now with TreeView!
    /// </summary>
    public partial class LogViewerPage : Page
    {
        public LogViewerPage()
        {
            InitializeComponent();

            // Initialize ViewModel when page loads
            this.Loaded += async (sender, e) =>
            {
                if (DataContext is LogViewerViewModel viewModel && !viewModel.IsLoading)
                {
                    await viewModel.InitializeAsync();
                }
            };
        }

        /// <summary>
        /// Expand all TreeView items
        /// </summary>
        private void ExpandAll_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LogViewerViewModel viewModel)
            {
                foreach (var group in viewModel.CorrelationGroups)
                {
                    group.IsExpanded = true;
                    foreach (var stage in group.Stages)
                    {
                        stage.IsExpanded = true;
                    }
                }
            }
        }

        /// <summary>
        /// Collapse all TreeView items
        /// </summary>
        private void CollapseAll_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LogViewerViewModel viewModel)
            {
                foreach (var group in viewModel.CorrelationGroups)
                {
                    group.IsExpanded = false;
                    foreach (var stage in group.Stages)
                    {
                        stage.IsExpanded = false;
                    }
                }
            }
        }
    }
}
