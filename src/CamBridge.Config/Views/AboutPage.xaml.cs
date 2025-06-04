// src\CamBridge.Config\Views\AboutPage.xaml.cs
// Version: 0.5.26
// Complete about page implementation with all event handlers

using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// About page showing application information and credits
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
            LoadVersionInfo();
            LoadSystemInfo();
        }

        /// <summary>
        /// Loads version information from the assembly
        /// </summary>
        private void LoadVersionInfo()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;

                // Set version text
                if (FindName("VersionText") is TextBlock versionText)
                {
                    versionText.Text = $"Version {version?.ToString() ?? "0.5.26"}";
                }

                // Set build date
                if (FindName("BuildDateText") is TextBlock buildDateText)
                {
                    var buildDate = System.IO.File.GetLastWriteTime(assembly.Location);
                    buildDateText.Text = $"Built on {buildDate:yyyy-MM-dd HH:mm}";
                }

                // Set copyright
                if (FindName("CopyrightText") is TextBlock copyrightText)
                {
                    copyrightText.Text = "© 2025 Claude's Improbably Reliable Software Solutions";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading version info: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads system information
        /// </summary>
        private void LoadSystemInfo()
        {
            try
            {
                if (FindName("SystemInfoText") is TextBlock systemInfo)
                {
                    systemInfo.Text = $"Running on {Environment.OSVersion} " +
                                    $"({Environment.ProcessorCount} cores) " +
                                    $"with .NET {Environment.Version}";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading system info: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles hyperlink navigation requests
        /// </summary>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                // Open URL in default browser
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
                {
                    UseShellExecute = true
                });
                e.Handled = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening URL: {ex.Message}");
                MessageBox.Show(
                    $"Could not open URL: {e.Uri.AbsoluteUri}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Shows Vogon Poetry easter egg window
        /// </summary>
        private void VogonPoetry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var vogonWindow = new VogonPoetryWindow
                {
                    Owner = Window.GetWindow(this),
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                vogonWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to show Vogon Poetry: {ex.Message}");

                // Fallback: Show poetry in message box
                MessageBox.Show(
                    "Oh freddled gruntbuggly,\n" +
                    "Thy micturations are to me\n" +
                    "As plurdled gabbleblotchits on a lurgid bee.\n\n" +
                    "- Prostetnic Vogon Jeltz",
                    "Vogon Poetry",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Handles exit button click - closes the application
        /// </summary>
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to exit CamBridge Configuration?",
                "Exit Application",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Graceful shutdown
                try
                {
                    // Save any pending settings
                    Application.Current.MainWindow?.Close();
                    Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error during shutdown: {ex.Message}");
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// Handles keyboard shortcuts
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Ctrl+W or Escape to close
            if ((e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control) ||
                e.Key == Key.Escape)
            {
                var mainWindow = Window.GetWindow(this);
                if (mainWindow != null)
                {
                    // Navigate back or close
                    var navigationService = NavigationService.GetNavigationService(this);
                    if (navigationService?.CanGoBack == true)
                    {
                        navigationService.GoBack();
                    }
                }
                e.Handled = true;
            }
        }
    }
}
