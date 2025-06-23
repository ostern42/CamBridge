// src/CamBridge.Config/Views/MappingEditorPage.xaml.cs
// Version: 0.7.25
// Copyright: Â© 2025 Claude's Improbably Reliable Software Solutions

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// Interaction logic for MappingEditorPage.xaml
    /// </summary>
    public partial class MappingEditorPage : Page
    {
        private MappingEditorViewModel? _viewModel;
        private Point _startPoint;
        private bool _isDragging;

        public MappingEditorPage()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            Loaded += OnLoaded;
        }

        #region Rule Selection Event Handlers

        private void RuleItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // When double-clicking a rule, open the DICOM tag browser
            if (_viewModel?.SelectedRule != null && _viewModel.CanEditCurrentSet)
            {
                _viewModel.BrowseDicomTagsCommand.Execute(null);
            }
        }

        #endregion

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = DataContext as MappingEditorViewModel;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Get the ViewModel from DI container if not already set
            if (_viewModel == null)
            {
                var app = Application.Current as App;
                if (app?.Host?.Services != null)
                {
                    _viewModel = app.Host.Services.GetRequiredService<MappingEditorViewModel>();
                    DataContext = _viewModel;
                }
                else
                {
                    // Fallback: Create manually with required services
                    var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<MappingEditorViewModel>();
                    var configService = new Services.ConfigurationService();
                    _viewModel = new MappingEditorViewModel(logger, configService);
                    DataContext = _viewModel;
                }
            }

            // Initialize the ViewModel
            if (_viewModel != null)
            {
                await _viewModel.InitializeAsync();
            }
        }

        #region Drag & Drop Event Handlers

        private void SourceField_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private void SourceField_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
            {
                Point position = e.GetPosition(null);

                if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    _isDragging = true;

                    // Get the source field data
                    if (sender is FrameworkElement element && element.DataContext is MappingEditorViewModel.SourceFieldInfo fieldInfo)
                    {
                        // Create drag data
                        DataObject dragData = new DataObject("SourceField", fieldInfo);
                        DragDrop.DoDragDrop(element, dragData, DragDropEffects.Copy);
                    }

                    _isDragging = false;
                }
            }
        }

        private void MappingArea_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("SourceField"))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void MappingArea_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("SourceField") && _viewModel != null)
            {
                var fieldInfo = e.Data.GetData("SourceField") as MappingEditorViewModel.SourceFieldInfo;
                if (fieldInfo != null && _viewModel.CanEditCurrentSet)
                {
                    // Add new mapping rule with the dropped field
                    _viewModel.AddRuleFromField(fieldInfo);
                }
            }
        }

        #endregion
    }
}
