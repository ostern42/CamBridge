using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CamBridge.Config.Views
{
    [SupportedOSPlatform("windows")]
    public partial class DeadLettersPage : Page
    {
        private DeadLettersViewModel? _viewModel;

        public DeadLettersPage()
        {
            InitializeComponent();

            // Get ViewModel from DI with null check
            try
            {
                var app = Application.Current as App;
                if (app?.Host != null)
                {
                    _viewModel = app.Host.Services.GetRequiredService<DeadLettersViewModel>();
                    DataContext = _viewModel;
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading DeadLettersViewModel: {ex.Message}");
                // Create a basic viewmodel if DI fails
                _viewModel = new DeadLettersViewModel(null!);
                DataContext = _viewModel;
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel?.Cleanup();
        }
    }
}
