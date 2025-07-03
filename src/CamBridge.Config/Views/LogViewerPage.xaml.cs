// src\CamBridge.Config\Views\Pages\LogViewerPage.xaml.cs
// Version: 0.8.19 - Fixed event handler names
// Description: Log viewer page with hierarchical correlation display
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Config.Models;
using CamBridge.Config.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// Interaction logic for LogViewerPage.xaml - Now with correct event handlers!
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
        /// Handle Enter key in search boxes (name matches XAML)
        /// </summary>
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var comboBox = sender as ComboBox;
                if (comboBox != null && DataContext is LogViewerViewModel vm)
                {
                    // Save to history on Enter
                    var tag = comboBox.Tag?.ToString();
                    switch (tag)
                    {
                        case "1":
                            vm.AddToFilter1History();
                            break;
                        case "2":
                            vm.AddToFilter2History();
                            break;
                        case "3":
                            vm.AddToFilter3History();
                            break;
                    }

                    // Move focus away
                    comboBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Handle lost focus for Filter1 (specific handler as expected by XAML)
        /// </summary>
        private void Filter1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is LogViewerViewModel vm)
            {
                vm.AddToFilter1History();
            }
        }

        /// <summary>
        /// Handle lost focus for Filter2 (specific handler as expected by XAML)
        /// </summary>
        private void Filter2_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is LogViewerViewModel vm)
            {
                vm.AddToFilter2History();
            }
        }

        /// <summary>
        /// Handle lost focus for Filter3 (specific handler as expected by XAML)
        /// </summary>
        private void Filter3_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is LogViewerViewModel vm)
            {
                vm.AddToFilter3History();
            }
        }

        // Keep the old methods for compatibility if needed elsewhere
        private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            SearchBox_KeyDown(sender, e);
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && DataContext is LogViewerViewModel vm)
            {
                // Save to history when leaving field
                var tag = comboBox.Tag?.ToString();
                switch (tag)
                {
                    case "1":
                        vm.AddToFilter1History();
                        break;
                    case "2":
                        vm.AddToFilter2History();
                        break;
                    case "3":
                        vm.AddToFilter3History();
                        break;
                }
            }
        }

        /// <summary>
        /// Expand all TreeView items
        /// </summary>
        private void ExpandAll_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LogViewerViewModel vm)
            {
                foreach (var group in vm.CorrelationGroups)
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
            if (DataContext is LogViewerViewModel vm)
            {
                foreach (var group in vm.CorrelationGroups)
                {
                    group.IsExpanded = false;
                }
            }
        }
    }
}
