// src/CamBridge.QRBridge/Constants/QRBridgeConstants.cs
// Version: 0.7.8
// © 2025 Claude's Improbably Reliable Software Solutions

namespace CamBridge.QRBridge.Constants;

/// <summary>
/// Constants for QRBridge application
/// </summary>
public static class QRBridgeConstants
{
    /// <summary>
    /// Application information
    /// </summary>
    public static class Application
    {
        public const string Name = "CamBridge QRBridge";
        public const string Version = "0.7.8";
        public const string Description = "Modern QR Code Generator for Ricoh Cameras";
    }

    /// <summary>
    /// UI related constants
    /// </summary>
    public static class UI
    {
        public const int QRCodeSize = 400;
        public const int FormWidth = 450;
        public const int FormHeight = 520;
        public const int CountdownInterval = 1000; // 1 second
        public const string CountdownFormat = "Fenster schließt in {0} Sekunden...";
        public const string WindowTitle = "CamBridge QRBridge - QR Code";
    }

    /// <summary>
    /// Colors (using CamBridge standard colors)
    /// </summary>
    public static class Colors
    {
        public static readonly Color Background = Color.FromArgb(45, 45, 48);
        public static readonly Color Foreground = Color.White;
        public static readonly Color Accent = Color.FromArgb(0, 122, 204);
        public static readonly Color Success = Color.FromArgb(76, 175, 80);
        public static readonly Color Error = Color.FromArgb(244, 67, 54);
    }

    /// <summary>
    /// Command line argument names
    /// </summary>
    public static class Arguments
    {
        public const string ExamId = "examid";
        public const string Name = "name";
        public const string BirthDate = "birthdate";
        public const string Gender = "gender";
        public const string Comment = "comment";
        public const string Timeout = "timeout";
        public const string Help = "help";
    }

    /// <summary>
    /// Default values
    /// </summary>
    public static class Defaults
    {
        public const int TimeoutSeconds = 10;
        public const string DateFormat = "yyyy-MM-dd";
    }

    /// <summary>
    /// Exit codes
    /// </summary>
    public static class ExitCodes
    {
        public const int Success = 0;
        public const int InvalidArguments = 1;
        public const int GenerationError = 2;
        public const int UnhandledException = 99;
    }
}
