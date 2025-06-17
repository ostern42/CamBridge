// src\CamBridge.Config\Views\DashboardPage.xaml.cs
// Version: 0.7.21
// Description: MINIMAL Dashboard page code-behind
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System.Windows.Controls;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// MINIMAL Dashboard - No complex initialization!
    /// </summary>
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            // That's it! ViewModel comes from NavigationService
        }
    }
}
