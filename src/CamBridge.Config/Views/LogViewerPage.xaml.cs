// src\CamBridge.Config\Views\LogViewerPage.xaml.cs
// Version: 0.8.11 - MINIMAL VERSION
// Description: Log viewer page - simplified without TreeView
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Config.ViewModels;
using System.Windows.Controls;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// Interaction logic for LogViewerPage.xaml - MINIMAL VERSION
    /// </summary>
    public partial class LogViewerPage : Page
    {
        public LogViewerPage()
        {
            InitializeComponent();

            // IMPORTANT: Initialize ViewModel when page loads
            this.Loaded += async (sender, e) =>
            {
                if (DataContext is LogViewerViewModel viewModel && !viewModel.IsLoading)
                {
                    await viewModel.InitializeAsync();
                }
            };
        }

        // REMOVED: All TreeView-related event handlers
        // - CorrelationHeader_Click
        // - StageHeader_Click
        // - TreeScrollViewer references
    }
}
