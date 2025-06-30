// src\CamBridge.Config\Views\LogViewerPage.xaml.cs
// Version: 0.8.9
// Description: Log viewer page with auto-initialization
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Config.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// Interaction logic for LogViewerPage.xaml
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

        private void CorrelationHeader_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is CorrelationGroup group)
            {
                group.IsExpanded = !group.IsExpanded;
            }
        }

        private void StageHeader_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is StageGroup stage)
            {
                stage.IsExpanded = !stage.IsExpanded;
            }
        }
    }
}
