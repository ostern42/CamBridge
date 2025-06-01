using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CamBridge.Config.Views
{
    [SupportedOSPlatform("windows")]
    public partial class AboutPage : Page
    {
        // The Answer to Life, the Universe, and Everything
        private readonly List<Key> _theAnswer = new() { Key.D4, Key.D2 };
        private readonly List<Key> _inputHistory = new();
        private DateTime _lastKeyPress = DateTime.MinValue;

        public AboutPage()
        {
            InitializeComponent();

            // Make sure the page can receive keyboard input
            Loaded += (s, e) =>
            {
                Keyboard.Focus(this);
            };
        }

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Reset if more than 1 second since last key
            if (DateTime.Now - _lastKeyPress > TimeSpan.FromSeconds(1))
            {
                _inputHistory.Clear();
            }

            _lastKeyPress = DateTime.Now;
            _inputHistory.Add(e.Key);

            // Keep only the last 2 keys
            if (_inputHistory.Count > 2)
            {
                _inputHistory.RemoveAt(0);
            }

            // Check if the current input is "42"
            if (_inputHistory.Count >= _theAnswer.Count)
            {
                var lastKeys = _inputHistory.Skip(_inputHistory.Count - _theAnswer.Count).ToList();
                if (lastKeys.SequenceEqual(_theAnswer))
                {
                    // THE ANSWER ACTIVATED!
                    _inputHistory.Clear();
                    ShowVogonPoetry();
                }
            }
        }

        private void ShowVogonPoetry()
        {
            try
            {
                var vogonWindow = new VogonPoetryWindow
                {
                    Owner = Window.GetWindow(this)
                };
                vogonWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing Vogon Poetry: {ex.Message}", "Easter Egg Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
