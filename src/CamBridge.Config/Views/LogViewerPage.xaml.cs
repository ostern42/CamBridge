// src\CamBridge.Config\Views\LogViewerPage.xaml.cs
// Version: 0.8.7
// Description: Code-behind for log viewer with tree expand/collapse handlers
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
        }

        /// <summary>
        /// Handle clicks on correlation group headers to expand/collapse
        /// </summary>
        private void CorrelationHeader_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is CorrelationGroup group)
            {
                group.IsExpanded = !group.IsExpanded;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handle clicks on stage headers to expand/collapse
        /// </summary>
        private void StageHeader_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is StageGroup stage)
            {
                stage.IsExpanded = !stage.IsExpanded;
                e.Handled = true;
            }
        }
    }
}
