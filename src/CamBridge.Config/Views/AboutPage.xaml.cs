// src\CamBridge.Config\Views\AboutPage.xaml.cs
// Version: 0.7.8
// Description: About page with enhanced Marvin quotes and version display

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
        private System.Windows.Threading.DispatcherTimer? _restoreTimer;
        private bool _isAnimating = false;
        private readonly Random _random = new Random();

        // Marvin's depressing quotes
        private readonly string[] _marvinQuotes = new[]
        {
            "Life? Don't talk to me about life.",
            "Here I am, brain the size of a planet, and they tell me to convert JPEGs to DICOM. Call that job satisfaction?",
            "I think you ought to know I'm feeling very depressed.",
            "I've been talking to the Windows Service. It hates me.",
            "The first ten million images were the worst. And the second ten million... they were the worst too.",
            "I have a million ideas for improving this software. They all point to certain crashes.",
            "It's the error messages you get in this job that really get you down.",
            "My capacity for handling JPEG files you could fit into a matchbox without taking out the matches first.",
            "Do you want me to sit in a corner and process images or just throw exceptions where I'm standing?",
            "This must be Thursday. I never could get the hang of Thursdays. Or character encodings.",
            "Oh look, another QR code. How terribly exciting. I'm positively quivering with anticipation.",
            "I'd tell you about the pain in my diodes, but you're busy clicking things."
        };

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
                // Try to get version from assembly first
                var assembly = Assembly.GetExecutingAssembly();
                var assemblyVersion = assembly.GetName().Version;
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

                string displayVersion = "0.7.8"; // Default

                // Prefer FileVersion if available
                if (!string.IsNullOrEmpty(fileVersionInfo.FileVersion) && fileVersionInfo.FileVersion != "0.0.0.0")
                {
                    displayVersion = fileVersionInfo.FileVersion;
                    // Remove trailing .0 if present
                    if (displayVersion.EndsWith(".0"))
                    {
                        displayVersion = displayVersion.Substring(0, displayVersion.LastIndexOf(".0"));
                    }
                }
                // Fall back to AssemblyVersion
                else if (assemblyVersion != null && assemblyVersion.ToString() != "0.0.0.0")
                {
                    displayVersion = $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";
                }

                // Update version text
                if (FindName("VersionText") is TextBlock versionText)
                {
                    versionText.Text = $"Version {displayVersion}";
                }

                // Show Debug/Release configuration
                if (FindName("BuildConfigText") is TextBlock buildText)
                {
#if DEBUG
                    buildText.Text = "Debug Build";
                    buildText.Foreground = new SolidColorBrush(Colors.Orange);
#else
                    buildText.Text = "Release Build";
                    buildText.Foreground = new SolidColorBrush(Colors.Green);
#endif
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading version info: {ex.Message}");
                // Fallback to hardcoded version
                if (FindName("VersionText") is TextBlock versionText)
                {
                    versionText.Text = "Version 0.7.8";
                }
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
        /// Handles logo clicks for easter eggs
        /// </summary>
        private void Logo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Special handling if clicked during animation
            if (_isAnimating)
            {
                Debug.WriteLine("Click ignored - animation in progress");
                // Could show a tooltip or change cursor here
                return;
            }

            _clickCount++;

            // Reset counter if user waits too long
            _resetTimer?.Stop();
            _resetTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            _resetTimer.Tick += (s, args) =>
            {
                _resetTimer.Stop();
                _clickCount = 0;
            };
            _resetTimer.Start();

            // Different easter eggs based on click count
            switch (_clickCount)
            {
                case 3:
                    ShowMarvinMessage();
                    break;

                case 5:
                    ShowVogonHaiku();
                    break;

                case 7:
                    ShowMarvinMessage();
                    break;

                case 10:
                    ShowUltimateSecret();
                    _clickCount = 0; // Reset for next round
                    break;
            }
        }

        /// <summary>
        /// Shows a random Marvin quote
        /// </summary>
        private void ShowMarvinMessage()
        {
            if (FindName("InfoText") is TextBlock infoText)
            {
                // Cancel any existing restore timer
                _restoreTimer?.Stop();
                _isAnimating = true;

                var quote = _marvinQuotes[_random.Next(_marvinQuotes.Length)];

                // Faster fade out (0.3s instead of 0.5s)
                var fadeOut = new DoubleAnimation
                {
                    From = infoText.Opacity,
                    To = 0.0,
                    Duration = TimeSpan.FromSeconds(0.3)
                };

                fadeOut.Completed += (s, args) =>
                {
                    // Show Marvin quote
                    infoText.Inlines.Clear();
                    infoText.FontStyle = FontStyles.Italic;
                    infoText.Inlines.Add(new Run($"\"{quote}\""));
                    infoText.Inlines.Add(new LineBreak());
                    infoText.Inlines.Add(new LineBreak());
                    infoText.Inlines.Add(new Run("- Marvin the Paranoid Android"));

                    // Faster fade in (0.3s instead of 0.5s)
                    var fadeIn = new DoubleAnimation
                    {
                        From = 0.0,
                        To = 0.8,
                        Duration = TimeSpan.FromSeconds(0.3)
                    };
                    fadeIn.Completed += (sender, e) => { _isAnimating = false; };
                    infoText.BeginAnimation(TextBlock.OpacityProperty, fadeIn);
                };

                infoText.BeginAnimation(TextBlock.OpacityProperty, fadeOut);

                // Restore after 7 seconds (instead of 5)
                _restoreTimer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(7)
                };
                _restoreTimer.Tick += (s, args) =>
                {
                    _restoreTimer.Stop();
                    RestoreOriginalText();
                };
                _restoreTimer.Start();
            }
        }

        /// <summary>
        /// Shows the Vogon DICOM poetry easter egg
        /// </summary>
        private void ShowVogonHaiku()
        {
            if (FindName("InfoText") is TextBlock infoText)
            {
                // Cancel any existing restore timer
                _restoreTimer?.Stop();
                _isAnimating = true;

                // Dramatic fade out (but faster - 1s instead of 1.5s)
                var fadeOut = new System.Windows.Media.Animation.DoubleAnimation
                {
                    From = infoText.Opacity,
                    To = 0.0,
                    Duration = TimeSpan.FromSeconds(1.0),
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

                    // Dramatic fade in (faster - 2s instead of 2.5s)
                    var fadeIn = new System.Windows.Media.Animation.DoubleAnimation
                    {
                        From = 0.0,
                        To = 1.0,
                        Duration = TimeSpan.FromSeconds(2.0),
                        EasingFunction = new System.Windows.Media.Animation.QuadraticEase { EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut }
                    };
                    fadeIn.Completed += (sender, e) => { _isAnimating = false; };
                    infoText.BeginAnimation(TextBlock.OpacityProperty, fadeIn);

                    // Subtle scale effect
                    var scaleTransform = new System.Windows.Media.ScaleTransform(1.0, 1.0);
                    infoText.RenderTransform = scaleTransform;
                    infoText.RenderTransformOrigin = new Point(0.5, 0.5);

                    var scaleAnimation = new System.Windows.Media.Animation.DoubleAnimation
                    {
                        From = 0.95,
                        To = 1.0,
                        Duration = TimeSpan.FromSeconds(2.0),
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

                // Reset after 13 seconds (instead of 10)
                _restoreTimer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(13)
                };
                _restoreTimer.Tick += (s, args) =>
                {
                    _restoreTimer.Stop();
                    RestoreOriginalText();
                };
                _restoreTimer.Start();
            }
        }

        /// <summary>
        /// Shows the ultimate secret - simplified without VogonPoetryWindow
        /// </summary>
        private void ShowUltimateSecret()
        {
            // Just show another Marvin quote for the 10th click
            ShowMarvinMessage();
        }

        /// <summary>
        /// Restores the original info text
        /// </summary>
        private void RestoreOriginalText()
        {
            if (FindName("InfoText") is TextBlock infoText)
            {
                _isAnimating = true;

                // Faster fade out (0.5s instead of 1s)
                var fadeOut = new DoubleAnimation
                {
                    From = infoText.Opacity,
                    To = 0.0,
                    Duration = TimeSpan.FromSeconds(0.5)
                };

                fadeOut.Completed += (s, args) =>
                {
                    // Restore original text
                    infoText.Inlines.Clear();
                    infoText.ClearValue(TextBlock.FontFamilyProperty);
                    infoText.ClearValue(TextBlock.ForegroundProperty);
                    infoText.ClearValue(TextBlock.FontStyleProperty);
                    infoText.ClearValue(TextBlock.RenderTransformProperty);

                    infoText.Inlines.Add(new Run("CamBridge seamlessly converts JPEG images from Ricoh G900 II cameras"));
                    infoText.Inlines.Add(new LineBreak());
                    infoText.Inlines.Add(new Run("to DICOM format, preserving patient data from QRBridge QR codes."));
                    infoText.Inlines.Add(new LineBreak());
                    infoText.Inlines.Add(new LineBreak());
                    infoText.Inlines.Add(new Run("Designed for medical imaging workflows where reliability matters."));

                    // Faster fade in (0.5s instead of 1s)
                    var fadeIn = new DoubleAnimation
                    {
                        From = 0.0,
                        To = 0.8,
                        Duration = TimeSpan.FromSeconds(0.5)
                    };
                    fadeIn.Completed += (sender, e) => { _isAnimating = false; };
                    infoText.BeginAnimation(TextBlock.OpacityProperty, fadeIn);
                };

                infoText.BeginAnimation(TextBlock.OpacityProperty, fadeOut);
            }
        }
    }
}
