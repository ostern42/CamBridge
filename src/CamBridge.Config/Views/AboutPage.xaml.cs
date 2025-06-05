// src\CamBridge.Config\Views\AboutPage.xaml.cs
// Version: 0.5.27
// Fixed: Simplified easter egg - shows Vogon poetry on 5 clicks

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// About page showing application information and credits
    /// </summary>
    public partial class AboutPage : Page
    {
        private int _clickCount = 0;
        private System.Windows.Threading.DispatcherTimer? _resetTimer;

        public AboutPage()
        {
            InitializeComponent();
            LoadVersionInfo();
        }

        /// <summary>
        /// Loads version information from the assembly
        /// </summary>
        private void LoadVersionInfo()
        {
            try
            {
                // Set version text (hardcoded to avoid assembly conflicts)
                if (FindName("VersionText") is TextBlock versionText)
                {
                    versionText.Text = "Version 0.5.27";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading version info: {ex.Message}");
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

        /// <summary>
        /// Handles logo clicks for easter egg
        /// </summary>
        private void Logo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _clickCount++;

            if (_clickCount == 5)
            {
                ShowVogonHaiku();
                _clickCount = 0; // Reset counter
            }
        }

        /// <summary>
        /// Shows the Vogon DICOM poetry easter egg
        /// </summary>
        private void ShowVogonHaiku()
        {
            if (FindName("InfoText") is TextBlock infoText)
            {
                // Store original text
                var originalRuns = new List<Run>();
                foreach (var inline in infoText.Inlines.ToList())
                {
                    if (inline is Run run)
                    {
                        originalRuns.Add(new Run(run.Text));
                    }
                }

                // Dramatic fade out first
                var fadeOut = new System.Windows.Media.Animation.DoubleAnimation
                {
                    From = 0.8,
                    To = 0.0,
                    Duration = TimeSpan.FromSeconds(1.5),
                    EasingFunction = new System.Windows.Media.Animation.PowerEase { EasingMode = System.Windows.Media.Animation.EasingMode.EaseIn }
                };

                fadeOut.Completed += (s, args) =>
                {
                    // Clear and show poetry
                    infoText.Inlines.Clear();
                    infoText.FontFamily = new System.Windows.Media.FontFamily("Consolas");
                    infoText.Foreground = System.Windows.Media.Brushes.Green;

                    infoText.Inlines.Add(new Run("Oh freddled gruntbuggly, thy DICOM tags are to me\n"));
                    infoText.Inlines.Add(new Run("As plurdled gabbleblotchits on a lurgid JPEG tree!\n"));
                    infoText.Inlines.Add(new Run("\n"));
                    infoText.Inlines.Add(new Run("See how (0010,0010) PatientName doth slumber!"));

                    // Dramatic fade in
                    var fadeIn = new System.Windows.Media.Animation.DoubleAnimation
                    {
                        From = 0.0,
                        To = 1.0,
                        Duration = TimeSpan.FromSeconds(2.5),
                        EasingFunction = new System.Windows.Media.Animation.QuadraticEase { EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut }
                    };
                    infoText.BeginAnimation(TextBlock.OpacityProperty, fadeIn);

                    // Subtle scale effect
                    var scaleTransform = new System.Windows.Media.ScaleTransform(1.0, 1.0);
                    infoText.RenderTransform = scaleTransform;
                    infoText.RenderTransformOrigin = new Point(0.5, 0.5);

                    var scaleAnimation = new System.Windows.Media.Animation.DoubleAnimation
                    {
                        From = 0.95,
                        To = 1.0,
                        Duration = TimeSpan.FromSeconds(2.5),
                        EasingFunction = new System.Windows.Media.Animation.ElasticEase
                        {
                            EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut,
                            Oscillations = 1,
                            Springiness = 8
                        }
                    };
                    scaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, scaleAnimation);
                    scaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, scaleAnimation);
                };

                infoText.BeginAnimation(TextBlock.OpacityProperty, fadeOut);

                // Reset after 10 seconds
                _resetTimer?.Stop();
                _resetTimer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(10)
                };
                _resetTimer.Tick += (s, args) =>
                {
                    _resetTimer.Stop();

                    // Fade out poetry
                    var fadeOutPoetry = new System.Windows.Media.Animation.DoubleAnimation
                    {
                        From = 1.0,
                        To = 0.0,
                        Duration = TimeSpan.FromSeconds(2.0),
                        EasingFunction = new System.Windows.Media.Animation.PowerEase { EasingMode = System.Windows.Media.Animation.EasingMode.EaseIn }
                    };

                    fadeOutPoetry.Completed += (sender, eventArgs) =>
                    {
                        // Restore original text
                        infoText.Inlines.Clear();
                        infoText.FontFamily = new System.Windows.Media.FontFamily("Segoe UI");
                        infoText.Foreground = System.Windows.Media.Brushes.Black;
                        infoText.ClearValue(TextBlock.FontFamilyProperty);
                        infoText.ClearValue(TextBlock.ForegroundProperty);
                        infoText.ClearValue(TextBlock.RenderTransformProperty);

                        infoText.Inlines.Add(new Run("CamBridge seamlessly converts JPEG images from Ricoh G900 II cameras"));
                        infoText.Inlines.Add(new LineBreak());
                        infoText.Inlines.Add(new Run("to DICOM format, preserving patient data from QRBridge QR codes."));
                        infoText.Inlines.Add(new LineBreak());
                        infoText.Inlines.Add(new LineBreak());
                        infoText.Inlines.Add(new Run("Designed for medical imaging workflows where reliability matters."));

                        // Fade back in
                        var fadeBack = new System.Windows.Media.Animation.DoubleAnimation
                        {
                            From = 0.0,
                            To = 0.8,
                            Duration = TimeSpan.FromSeconds(2.0),
                            EasingFunction = new System.Windows.Media.Animation.QuadraticEase { EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut }
                        };
                        infoText.BeginAnimation(TextBlock.OpacityProperty, fadeBack);
                    };

                    infoText.BeginAnimation(TextBlock.OpacityProperty, fadeOutPoetry);
                };
                _resetTimer.Start();
            }
        }
    }
}
