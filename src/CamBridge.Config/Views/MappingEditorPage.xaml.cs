// src/CamBridge.Config/Views/MappingEditorPage.xaml.cs
// Version: 0.6.2
// Description: Code-behind for Mapping Editor with Expand/Collapse support

using CamBridge.Config.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// Interaction logic for MappingEditorPage.xaml
    /// </summary>
    public partial class MappingEditorPage : Page
    {
        public MappingEditorPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MappingEditorViewModel viewModel)
            {
                await viewModel.InitializeAsync();
            }
        }

        private void SourceField_MouseMove(object sender, MouseEventArgs e)
        {
            // Drag & Drop functionality disabled for now
            // We're using the "glorified list" approach instead
        }

        private void MappingArea_DragOver(object sender, DragEventArgs e)
        {
            // Drag & Drop functionality disabled for now
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void MappingArea_Drop(object sender, DragEventArgs e)
        {
            // Drag & Drop functionality disabled for now
            e.Handled = true;
        }

        /// <summary>
        /// Handle click on expander header to toggle expansion
        /// </summary>
        private void ExpanderHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                // Find the parent Expander
                var expander = FindParent<Expander>(border);
                if (expander != null)
                {
                    expander.IsExpanded = !expander.IsExpanded;
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Handle Edit Details button click
        /// </summary>
        private void EditRuleDetails_Click(object sender, RoutedEventArgs e)
        {
            // Get the rule from the button's DataContext
            if (sender is Button button && button.DataContext is MappingRuleViewModel rule)
            {
                // Select the rule to show it in the properties panel
                if (DataContext is MappingEditorViewModel vm)
                {
                    vm.SelectedRule = rule;
                }

                // Scroll the properties panel into view
                PropertiesPanel?.BringIntoView();
            }
        }

        /// <summary>
        /// Helper method to find parent of specific type
        /// </summary>
        private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = System.Windows.Media.VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            if (parentObject is T parent)
                return parent;

            return FindParent<T>(parentObject);
        }
    }
}
