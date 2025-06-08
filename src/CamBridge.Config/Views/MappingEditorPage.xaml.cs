// src/CamBridge.Config/Views/MappingEditorPage.xaml.cs
// Version: 0.6.7
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

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

        public MappingEditorPage()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            Loaded += OnLoaded;
        }

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
    }
}
