// src\CamBridge.Core\UserPreferences.cs
// Version: 0.7.3
// Description: Per-user UI preferences
// © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;

namespace CamBridge.Core
{
    /// <summary>
    /// User-specific UI preferences
    /// Stored in: %AppData%\CamBridge\preferences.json
    /// </summary>
    public class UserPreferences
    {
        // UI Theme and Appearance
        public string Theme { get; set; } = "Dark";
        public string Language { get; set; } = "de-DE";
        public string AccentColor { get; set; } = "Blue";
        public double UiScale { get; set; } = 1.0;

        // Window Management
        public string WindowState { get; set; } = "Maximized";
        public WindowPosition? LastWindowPosition { get; set; }
        public bool AlwaysOnTop { get; set; } = false;

        // Navigation and Workflow
        public string DefaultView { get; set; } = "Dashboard";
        public bool ShowAdvancedOptions { get; set; } = false;
        public bool ShowStatusBar { get; set; } = true;
        public bool ShowNavigationPane { get; set; } = true;

        // Recent Items
        public List<Guid> RecentPipelines { get; set; } = new();
        public List<string> RecentFolders { get; set; } = new();
        public int MaxRecentItems { get; set; } = 10;

        // Favorites and Shortcuts
        public Dictionary<string, object> FavoriteSettings { get; set; } = new();
        public List<string> PinnedViews { get; set; } = new() { "Dashboard", "ServiceControl" };

        // Editor Preferences
        public bool EnableSyntaxHighlighting { get; set; } = true;
        public bool EnableAutoComplete { get; set; } = true;
        public int EditorFontSize { get; set; } = 12;
        public string EditorFontFamily { get; set; } = "Consolas";

        // Notification Preferences
        public bool EnableDesktopNotifications { get; set; } = true;
        public bool PlaySoundOnError { get; set; } = true;
        public bool PlaySoundOnSuccess { get; set; } = false;

        // Last session state
        public DateTime LastSessionTime { get; set; } = DateTime.UtcNow;
        public string LastSelectedPipelineId { get; set; } = string.Empty;
        public Dictionary<string, bool> CollapsedSections { get; set; } = new();
    }

    /// <summary>
    /// Window position and size
    /// </summary>
    public class WindowPosition
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; } = 1200;
        public int Height { get; set; } = 800;

        /// <summary>
        /// Monitor identifier for multi-monitor setups
        /// </summary>
        public string MonitorId { get; set; } = string.Empty;

        /// <summary>
        /// Validates if position is reasonable (not off-screen)
        /// </summary>
        public bool IsValid()
        {
            return X >= -Width / 2 &&
                   Y >= -Height / 2 &&
                   Width >= 800 &&
                   Height >= 600;
        }
    }

    /// <summary>
    /// Available UI themes
    /// </summary>
    public static class UiThemes
    {
        public const string Light = "Light";
        public const string Dark = "Dark";
        public const string HighContrast = "HighContrast";
        public const string System = "System"; // Follow Windows theme
    }

    /// <summary>
    /// Available UI languages
    /// </summary>
    public static class UiLanguages
    {
        public const string German = "de-DE";
        public const string English = "en-US";
        public const string French = "fr-FR";
        public const string Spanish = "es-ES";
    }
}
