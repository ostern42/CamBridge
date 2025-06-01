using System.Windows;
using System.Windows.Controls;
using ModernWpf.Controls;

namespace CamBridge.Config
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Navigate to Dashboard on startup
            if (ContentFrame != null)
            {
                ContentFrame.Navigate(new Views.DashboardPage());
            }
        }

        private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null && ContentFrame != null)
            {
                var tag = args.SelectedItemContainer.Tag?.ToString();

                switch (tag)
                {
                    case "Dashboard":
                        ContentFrame.Navigate(new Views.DashboardPage());
                        break;
                    case "ServiceControl":
                        ContentFrame.Navigate(new Views.ServiceControlPage());
                        break;
                    case "DeadLetters":
                        ContentFrame.Navigate(new Views.DeadLetterPage());
                        break;
                    case "Settings":
                        ContentFrame.Navigate(new Views.SettingsPage());
                        break;
                }
            }
        }
    }
}
