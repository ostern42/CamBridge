// src/CamBridge.Config/Views/MappingEditorPage.xaml.cs
// Version: 0.6.3
// Description: Code-behind for Mapping Editor with Designer mode check
// © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Config.ViewModels;
using CamBridge.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
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
        private MappingEditorViewModel? _viewModel;

        public MappingEditorPage()
        {
            InitializeComponent();

            // Skip ViewModel initialization in design mode
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeViewModel();
            }
        }

        private void InitializeViewModel()
        {
            try
            {
                // Additional null check for safety
                if (Application.Current == null)
                {
                    System.Diagnostics.Debug.WriteLine("Application.Current is null - likely in designer mode");
                    return;
                }

                // Get the App instance to access DI container
                if (Application.Current is App app)
                {
                    if (app?.Host?.Services != null)
                    {
                        // Get ViewModel from DI container - all dependencies are properly injected
                        _viewModel = app.Host.Services.GetRequiredService<MappingEditorViewModel>();
                        DataContext = _viewModel;

                        System.Diagnostics.Debug.WriteLine("MappingEditorViewModel loaded from DI container");

                        // Initialize the ViewModel after setting DataContext
                        Loaded += async (s, e) =>
                        {
                            if (_viewModel != null && !DesignerProperties.GetIsInDesignMode(this))
                            {
                                await _viewModel.InitializeAsync();
                            }
                        };
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Host.Services not available");
                        ShowError("Initialization Error", "Dependency injection container not available");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating MappingEditorViewModel: {ex.Message}");
                ShowError("Failed to initialize Mapping Editor", ex.Message);
            }
        }

        private void ShowError(string title, string message)
        {
            // Show error in UI
            Content = new TextBlock
            {
                Text = $"{title}\n\n{message}",
                Margin = new Thickness(20),
                TextWrapping = TextWrapping.Wrap
            };
        }

        private void SourceField_MouseMove(object sender, MouseEventArgs e)
        {
            // Drag & Drop functionality disabled for now
            // We're using the "glorified list" approach instead

            // If we wanted to implement drag & drop in the future:
            // if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement element)
            // {
            //     DragDrop.DoDragDrop(element, element.Tag, DragDropEffects.Copy);
            // }
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
            // We're using the "glorified list" approach instead
            e.Handled = true;

            // If we wanted to implement drag & drop in the future:
            // if (_viewModel?.CanEditCurrentSet == true && e.Data.GetDataPresent(typeof(SourceFieldInfo)))
            // {
            //     var sourceField = e.Data.GetData(typeof(SourceFieldInfo)) as SourceFieldInfo;
            //     // Create new rule from dropped field
            // }
        }

        private void ExpanderHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // The XAML expects this to toggle expand/collapse for MappingRuleViewModel items
            // The Expander control in the DataTemplate handles this automatically
            // This handler is called but we don't need to do anything special

            // Note: The IsExpanded property would need to be added to MappingRuleViewModel
            // if we want to track expansion state. For now, let WPF handle it.
        }

        private void EditRuleDetails_Click(object sender, RoutedEventArgs e)
        {
            // Get the rule from the button's DataContext
            if (sender is Button button &&
                button.DataContext is MappingRuleViewModel rule &&
                _viewModel != null)
            {
                // Select the rule to show it in the properties panel
                _viewModel.SelectedRule = rule;

                // Optional: Scroll the properties panel into view
                if (FindName("PropertiesPanel") is FrameworkElement propertiesPanel)
                {
                    propertiesPanel.BringIntoView();
                }
            }
        }
    }
}
