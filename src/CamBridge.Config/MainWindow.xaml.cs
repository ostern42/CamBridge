using Microsoft.UI.Xaml;

namespace CamBridge.Config;

/// <summary>
/// Main window for CamBridge configuration interface.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();

        // Set window properties
        Title = "CamBridge Configuration";

        // TODO: Implement navigation and view models in Phase 8-10
    }
}