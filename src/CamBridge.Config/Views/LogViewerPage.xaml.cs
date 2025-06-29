// src\CamBridge.Config\Views\LogViewerPage.xaml.cs
// Version: 0.8.6
// Description: Enhanced code-behind for log viewer with tree view support
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Config.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// Interaction logic for enhanced LogViewerPage.xaml
    /// </summary>
    public partial class LogViewerPage : Page
    {
        public LogViewerPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is LogViewerViewModel viewModel)
            {
                await viewModel.InitializeAsync();
            }
        }

        private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is LogViewerViewModel viewModel)
            {
                viewModel.Cleanup();
            }
        }

        /// <summary>
        /// Handle correlation group header click to expand/collapse
        /// </summary>
        private void CorrelationHeader_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.FrameworkElement element &&
                element.Tag is CorrelationGroup group)
            {
                group.IsExpanded = !group.IsExpanded;
            }
        }

        /// <summary>
        /// Handle stage header click to expand/collapse
        /// </summary>
        private void StageHeader_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.FrameworkElement element &&
                element.Tag is StageGroup stage)
            {
                stage.IsExpanded = !stage.IsExpanded;
            }
        }
    }
}
