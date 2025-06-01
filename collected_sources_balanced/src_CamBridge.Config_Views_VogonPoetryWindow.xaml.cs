using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Media.Animation;

namespace CamBridge.Config.Views
{
    [SupportedOSPlatform("windows")]
    public partial class VogonPoetryWindow : Window
    {
        private readonly Storyboard _boingBallAnimation;
        private readonly Storyboard _scrollTextAnimation;
        private readonly Storyboard _rainbowAnimation;

        public VogonPoetryWindow()
        {
            InitializeComponent();

            // Start all the retro animations
            _boingBallAnimation = (Storyboard)FindResource("BoingBallAnimation");
            _scrollTextAnimation = (Storyboard)FindResource("ScrollTextAnimation");
            _rainbowAnimation = (Storyboard)FindResource("RainbowAnimation");

            _boingBallAnimation.Begin();
            _scrollTextAnimation.Begin();
            _rainbowAnimation.Begin();

            // Add some retro computer sound simulation here if you want
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
            _boingBallAnimation?.Stop();
            _scrollTextAnimation?.Stop();
            _rainbowAnimation?.Stop();
            base.OnClosed(e);
        }
    }
}
