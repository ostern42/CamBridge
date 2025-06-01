using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CamBridge.Config.Views
{
    [SupportedOSPlatform("windows")]
    public partial class MappingEditorPage : Page
    {
        private MappingEditorViewModel? _viewModel;
        private Point _startPoint;
        private bool _isDragging;

        public MappingEditorPage()
        {
            InitializeComponent();
            Loaded += MappingEditorPage_Loaded;
        }

        private async void MappingEditorPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Only initialize once
            if (_viewModel != null) return;

            try
            {
                var app = Application.Current as App;
                if (app?.Host != null)
                {
                    _viewModel = app.Host.Services.GetRequiredService<MappingEditorViewModel>();
                    DataContext = _viewModel;

                    await _viewModel.InitializeAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading MappingEditorViewModel: {ex.Message}");
                MessageBox.Show($"Failed to load mapping editor: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Drag & Drop Handling
        private void DraggableItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
            _isDragging = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
            {
                var currentPoint = e.GetPosition(null);
                var diff = _startPoint - currentPoint;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    // Find the source element
                    var source = e.OriginalSource as DependencyObject;
                    while (source != null && !(source is Border border && border.Tag != null))
                    {
                        source = VisualTreeHelper.GetParent(source);
                    }

                    if (source is Border draggedBorder && draggedBorder.Tag is string tagData)
                    {
                        _isDragging = true;

                        // Create data object
                        var data = new DataObject("MappingField", tagData);

                        // Start drag operation
                        DragDrop.DoDragDrop(draggedBorder, data, DragDropEffects.Copy);
                        _isDragging = false;
                    }
                }
            }
        }

        private void DropZone_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("MappingField"))
            {
                e.Effects = DragDropEffects.Copy;

                // Visual feedback
                if (sender is Border border)
                {
                    border.BorderBrush = (Brush)FindResource("SystemControlHighlightAccentBrush");
                    border.Background = (Brush)FindResource("SystemControlBackgroundListLowBrush");
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void DropZone_DragLeave(object sender, DragEventArgs e)
        {
            // Reset visual feedback
            if (sender is Border border)
            {
                border.BorderBrush = (Brush)FindResource("SystemControlForegroundBaseMediumLowBrush");
                border.Background = (Brush)FindResource("SystemControlBackgroundChromeMediumLowBrush");
            }
        }

        private void DropZone_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("MappingField"))
            {
                var fieldData = e.Data.GetData("MappingField") as string;
                if (!string.IsNullOrEmpty(fieldData))
                {
                    // Parse field data (format: "SourceType|FieldName")
                    var parts = fieldData.Split('|');
                    if (parts.Length == 2)
                    {
                        var sourceType = parts[0];
                        var fieldName = parts[1];

                        // Show mapping dialog
                        ShowMappingDialog(sourceType, fieldName);
                    }
                }
            }

            // Reset visual feedback
            DropZone_DragLeave(sender, e);
            e.Handled = true;
        }

        private void ShowMappingDialog(string sourceType, string fieldName)
        {
            // For now, just show a message box
            // In a real implementation, this would open a dialog for DICOM tag selection
            MessageBox.Show($"Create mapping for {sourceType}.{fieldName}\n\nThis would open a DICOM tag selector dialog.",
                "Create Mapping",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            // TODO: Implement proper mapping dialog
            // - DICOM tag browser/selector
            // - Transform options
            // - Validation rules
            // - Default values
        }
    }
}
