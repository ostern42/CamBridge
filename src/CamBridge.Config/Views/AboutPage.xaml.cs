using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CamBridge.Config.Views
{
    [SupportedOSPlatform("windows")]
    public partial class AboutPage : Page
    {
        private int _secretCounter = 0;
        private readonly string _secretCode = "42";
        private string _inputBuffer = "";
        private DateTime _lastKeyPress = DateTime.MinValue;
        private DispatcherTimer? _spriteTimer;
        private WriteableBitmap? _spriteBitmap;
        private int _spriteX = 50;
        private int _spriteY = 50;
        private double _spriteDX = 2;
        private double _spriteDY = 2;
        private readonly Random _random = new Random();

        public AboutPage()
        {
            InitializeComponent();

            // Set focus to enable keyboard input
            Loaded += (s, e) =>
            {
                Focus();
                Focusable = true;
            };

            // Wire up keyboard handler
            PreviewKeyDown += AboutPage_PreviewKeyDown;
        }

        private void AboutPage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Check if more than 2 seconds have passed - reset buffer
            if ((DateTime.Now - _lastKeyPress).TotalSeconds > 2)
            {
                _inputBuffer = "";
            }

            _lastKeyPress = DateTime.Now;

            // Add the key to buffer (only digits)
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                _inputBuffer += (e.Key - Key.D0).ToString();
            }
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                _inputBuffer += (e.Key - Key.NumPad0).ToString();
            }

            // Check if we have the secret code
            if (_inputBuffer.EndsWith(_secretCode))
            {
                _secretCounter++;
                _inputBuffer = ""; // Reset buffer

                if (_secretCounter == 1)
                {
                    // First time - show subtle hint
                    ShowEasterEggHint();
                }
                else if (_secretCounter == 3)
                {
                    // Third time - ACTIVATE VOGON POETRY MODE!
                    ActivateVogonPoetryMode();
                }
            }

            // Keep buffer reasonable size
            if (_inputBuffer.Length > 10)
            {
                _inputBuffer = _inputBuffer.Substring(_inputBuffer.Length - 10);
            }
        }

        private void ShowEasterEggHint()
        {
            // Create a subtle animation on the version text
            var storyboard = new Storyboard();

            var colorAnimation = new ColorAnimation
            {
                From = Colors.White,
                To = Colors.Gold,
                Duration = TimeSpan.FromSeconds(0.5),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(3)
            };

            Storyboard.SetTarget(colorAnimation, VersionText);
            Storyboard.SetTargetProperty(colorAnimation,
                new PropertyPath("(TextBlock.Foreground).(SolidColorBrush.Color)"));

            storyboard.Children.Add(colorAnimation);
            storyboard.Begin();

            // Update the subtitle with a hint
            SubtitleText.Text = "The answer to life, universe, and DICOM...";
        }

        private void ActivateVogonPoetryMode()
        {
            // Hide normal content
            NormalContent.Visibility = Visibility.Collapsed;
            VogonContent.Visibility = Visibility.Visible;

            // Start the Amiga ball animation
            StartAmigaBallAnimation();

            // Start scrolling text
            StartScrollingText();

            // Add random guru meditation messages
            StartGuruMeditation();

            // Play with the background
            StartBackgroundEffects();
        }

        private void StartAmigaBallAnimation()
        {
            // Create a simple sprite for the Amiga ball
            _spriteBitmap = new WriteableBitmap(50, 50, 96, 96, PixelFormats.Bgr32, null);
            SpriteImage.Source = _spriteBitmap;

            // Draw a simple ball (red and white checkerboard pattern)
            DrawAmigaBall();

            // Start the animation timer
            _spriteTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
            };
            _spriteTimer.Tick += AnimateSprite;
            _spriteTimer.Start();
        }

        private void DrawAmigaBall()
        {
            if (_spriteBitmap == null) return;

            try
            {
                _spriteBitmap.Lock();

                unsafe
                {
                    int stride = _spriteBitmap.BackBufferStride;
                    byte* buffer = (byte*)_spriteBitmap.BackBuffer;

                    int centerX = 25;
                    int centerY = 25;
                    int radius = 23;

                    for (int y = 0; y < 50; y++)
                    {
                        for (int x = 0; x < 50; x++)
                        {
                            int dx = x - centerX;
                            int dy = y - centerY;
                            double distance = Math.Sqrt(dx * dx + dy * dy);

                            if (distance <= radius)
                            {
                                // Create checkerboard pattern
                                bool isRed = ((x / 10) + (y / 10)) % 2 == 0;

                                int pixelOffset = y * stride + x * 4;

                                if (isRed)
                                {
                                    buffer[pixelOffset] = 0;      // Blue
                                    buffer[pixelOffset + 1] = 0;  // Green
                                    buffer[pixelOffset + 2] = 255; // Red
                                }
                                else
                                {
                                    buffer[pixelOffset] = 255;     // Blue
                                    buffer[pixelOffset + 1] = 255; // Green
                                    buffer[pixelOffset + 2] = 255; // Red
                                }
                                buffer[pixelOffset + 3] = 255; // Alpha
                            }
                        }
                    }
                }

                _spriteBitmap.AddDirtyRect(new Int32Rect(0, 0, 50, 50));
            }
            finally
            {
                _spriteBitmap.Unlock();
            }
        }

        private void AnimateSprite(object? sender, EventArgs e)
        {
            // Update position
            _spriteX += (int)_spriteDX;
            _spriteY += (int)_spriteDY;

            // Get actual bounds
            var maxX = (int)(SpriteCanvas.ActualWidth - 50);
            var maxY = (int)(SpriteCanvas.ActualHeight - 50);

            // Bounce off walls
            if (_spriteX <= 0 || _spriteX >= maxX)
            {
                _spriteDX = -_spriteDX;
                _spriteX = Math.Max(0, Math.Min(_spriteX, maxX));
            }

            if (_spriteY <= 0 || _spriteY >= maxY)
            {
                _spriteDY = -_spriteDY;
                _spriteY = Math.Max(0, Math.Min(_spriteY, maxY));
            }

            // Update canvas position
            Canvas.SetLeft(SpriteImage, _spriteX);
            Canvas.SetTop(SpriteImage, _spriteY);

            // Randomly change direction occasionally
            if (_random.Next(100) < 2)
            {
                _spriteDX += (_random.NextDouble() - 0.5) * 0.5;
                _spriteDY += (_random.NextDouble() - 0.5) * 0.5;

                // Keep speed reasonable
                _spriteDX = Math.Max(-5, Math.Min(5, _spriteDX));
                _spriteDY = Math.Max(-5, Math.Min(5, _spriteDY));
            }
        }

        private void StartScrollingText()
        {
            var scrollAnimation = new DoubleAnimation
            {
                From = ActualWidth,
                To = -ScrollingText.ActualWidth - 1000,
                Duration = TimeSpan.FromSeconds(30),
                RepeatBehavior = RepeatBehavior.Forever
            };

            ScrollingText.BeginAnimation(Canvas.LeftProperty, scrollAnimation);
        }

        private void StartGuruMeditation()
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };

            timer.Tick += (s, e) =>
            {
                var messages = new[]
                {
                    "Guru Meditation #00000004.0000AAC0",
                    "Guru Meditation #80000003.DEADBEEF",
                    "Guru Meditation #00000009.42424242",
                    "Guru Meditation #DICOM.FATAL.ERROR",
                    "Guru Meditation #RICOH.G900.POETRY"
                };

                GuruText.Text = messages[_random.Next(messages.Length)];

                // Flash effect
                var flashAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(100),
                    AutoReverse = true,
                    RepeatBehavior = new RepeatBehavior(3)
                };

                GuruText.BeginAnimation(OpacityProperty, flashAnimation);
            };

            timer.Start();
        }

        private void StartBackgroundEffects()
        {
            var colorAnimation = new ColorAnimation
            {
                From = Color.FromRgb(0, 0, 0),
                To = Color.FromRgb(10, 0, 20),
                Duration = TimeSpan.FromSeconds(3),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            var brush = new SolidColorBrush(Colors.Black);
            VogonContent.Background = brush;

            brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Stop all animations
            _spriteTimer?.Stop();

            // Show normal content
            VogonContent.Visibility = Visibility.Collapsed;
            NormalContent.Visibility = Visibility.Visible;

            // Reset counter
            _secretCounter = 0;
            SubtitleText.Text = "JPEG to DICOM Converter for Ricoh Cameras";
        }
    }
}
