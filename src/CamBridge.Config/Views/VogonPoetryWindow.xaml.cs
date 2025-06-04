// src\CamBridge.Config\Views\VogonPoetryWindow.xaml.cs
// Version: 0.5.26
// Fixed: Nullable warnings resolved

using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CamBridge.Config.Views
{
    [SupportedOSPlatform("windows")]
    public partial class VogonPoetryWindow : Window
    {
        private Storyboard? _boingBallAnimation;
        private Storyboard? _scrollTextAnimation;
        private Storyboard? _rainbowAnimation;

        // Sprite animation members
        private WriteableBitmap? _ballBitmap;
        private DispatcherTimer? _spriteTimer;
        private int _currentFrame = 0;
        private const int TOTAL_FRAMES = 24;
        private bool _movingRight = true;
        private double _lastXPosition = 50;
        private byte[]? _pixelBuffer;
        private readonly int _stride = 100 * 4; // Width * BytesPerPixel

        public VogonPoetryWindow()
        {
            InitializeComponent();

            try
            {
                // Initialize sprite bitmap
                InitializeSpriteBitmap();

                // Start all the retro animations
                _boingBallAnimation = FindResource("BoingBallAnimation") as Storyboard;
                _scrollTextAnimation = FindResource("ScrollTextAnimation") as Storyboard;
                _rainbowAnimation = FindResource("RainbowAnimation") as Storyboard;

                if (_boingBallAnimation != null) _boingBallAnimation.Begin();
                if (_scrollTextAnimation != null) _scrollTextAnimation.Begin();
                if (_rainbowAnimation != null) _rainbowAnimation.Begin();

                // Initialize sprite animation timer
                _spriteTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(50) // 20 FPS
                };
                _spriteTimer.Tick += OnSpriteTimerTick;
                _spriteTimer.Start();

                // Track ball movement direction
                CompositionTarget.Rendering += TrackBallDirection;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing Vogon Poetry Window: {ex.Message}\n\n{ex.StackTrace}",
                    "Initialization Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeSpriteBitmap()
        {
            try
            {
                // Create a 100x100 WriteableBitmap
                _ballBitmap = new WriteableBitmap(100, 100, 96, 96, PixelFormats.Bgra32, null);
                _pixelBuffer = new byte[100 * 100 * 4]; // Width * Height * BytesPerPixel

                if (BoingBall == null)
                {
                    MessageBox.Show("BoingBall Image element not found!", "Error");
                    return;
                }

                BoingBall.Source = _ballBitmap;

                // Draw initial frame
                DrawBoingBallFrame(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing sprite: {ex.Message}", "Error");
            }
        }

        private void DrawBoingBallFrame(int frame)
        {
            if (_ballBitmap == null || _pixelBuffer == null) return;

            try
            {
                // Clear buffer to transparent
                Array.Clear(_pixelBuffer, 0, _pixelBuffer.Length);

                // Calculate rotation angle
                double angle = (frame / (double)TOTAL_FRAMES) * 360.0;
                double rotRad = angle * Math.PI / 180.0;

                int centerX = 50;
                int centerY = 50;
                int radius = 45;

                // Draw the ball
                for (int y = 0; y < 100; y++)
                {
                    for (int x = 0; x < 100; x++)
                    {
                        double dx = x - centerX;
                        double dy = y - centerY;
                        double distance = Math.Sqrt(dx * dx + dy * dy);

                        if (distance <= radius)
                        {
                            // Calculate 3D sphere coordinates
                            double z = Math.Sqrt(radius * radius - distance * distance);
                            double nx = dx / radius;
                            double ny = dy / radius;
                            double nz = z / radius;

                            // Map to texture coordinates with rotation
                            double u = Math.Atan2(ny, nx) + rotRad;
                            double v = Math.Acos(nz);

                            // Create checkerboard pattern
                            int checkerSize = 8;
                            int checkerU = (int)(u * radius / checkerSize) % 2;
                            int checkerV = (int)(v * radius / checkerSize) % 2;

                            // Add shading
                            double lightIntensity = 0.3 + 0.7 * Math.Max(0, nz);

                            int pixelIndex = (y * 100 + x) * 4;

                            if ((checkerU + checkerV) % 2 == 0)
                            {
                                // Red squares
                                _pixelBuffer[pixelIndex + 0] = (byte)(204 * lightIntensity); // B
                                _pixelBuffer[pixelIndex + 1] = 0; // G
                                _pixelBuffer[pixelIndex + 2] = 0; // R
                                _pixelBuffer[pixelIndex + 3] = 255; // A
                            }
                            else
                            {
                                // White squares
                                byte white = (byte)(255 * lightIntensity);
                                _pixelBuffer[pixelIndex + 0] = white; // B
                                _pixelBuffer[pixelIndex + 1] = white; // G
                                _pixelBuffer[pixelIndex + 2] = white; // R
                                _pixelBuffer[pixelIndex + 3] = 255; // A
                            }
                        }
                    }
                }

                // Write pixels to bitmap
                _ballBitmap.WritePixels(
                    new Int32Rect(0, 0, 100, 100),
                    _pixelBuffer,
                    _stride,
                    0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error drawing frame: {ex.Message}");
            }
        }

        private void TrackBallDirection(object? sender, EventArgs e)
        {
            try
            {
                if (BoingBall == null) return;

                double currentX = Canvas.GetLeft(BoingBall);

                if (double.IsNaN(currentX))
                    return;

                if (currentX > _lastXPosition)
                {
                    _movingRight = true;
                }
                else if (currentX < _lastXPosition)
                {
                    _movingRight = false;
                }

                _lastXPosition = currentX;
            }
            catch
            {
                // Ignore errors
            }
        }

        private void OnSpriteTimerTick(object? sender, EventArgs e)
        {
            if (_movingRight)
            {
                _currentFrame = (_currentFrame + 1) % TOTAL_FRAMES;
            }
            else
            {
                _currentFrame = (_currentFrame - 1 + TOTAL_FRAMES) % TOTAL_FRAMES;
            }

            DrawBoingBallFrame(_currentFrame);
        }

        private void AppreciateButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "GURU MEDITATION #00000042.00000042\n\n" +
                "Your appreciation has been noted in sector 42.\n" +
                "Please insert disk 2 to continue.\n\n" +
                "Software Failure. Press left mouse button to continue.\n" +
                "Guru Meditation #DEADBEEF.CAFEBABE",
                "AMIGA SYSTEM ERROR",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            DialogResult = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            try
            {
                _spriteTimer?.Stop();
                CompositionTarget.Rendering -= TrackBallDirection;

                _boingBallAnimation?.Stop();
                _scrollTextAnimation?.Stop();
                _rainbowAnimation?.Stop();
            }
            catch
            {
                // Ignore cleanup errors
            }

            base.OnClosed(e);
        }
    }
}
