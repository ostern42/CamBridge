// src/CamBridge.QRBridge/Forms/QRDisplayForm.cs
// Version: 0.5.33
// © 2025 Claude's Improbably Reliable Software Solutions

using System.Drawing.Drawing2D;
using CamBridge.Core.Entities;
using CamBridge.QRBridge.Constants;
using CamBridge.QRBridge.Services;
using Microsoft.Extensions.Logging;

namespace CamBridge.QRBridge.Forms;

/// <summary>
/// Windows Form for displaying QR codes with countdown timer
/// </summary>
public partial class QRDisplayForm : Form
{
    private readonly ILogger<QRDisplayForm> _logger;
    private readonly IQRCodeService _qrCodeService;
    private readonly QRCodeRequest _request;
    private readonly System.Windows.Forms.Timer _countdownTimer;
    private int _remainingSeconds;
    private Bitmap? _qrCodeBitmap;

    // UI Controls
    private PictureBox _qrCodePictureBox;
    private Label _patientLabel;
    private Label _examIdLabel;
    private Label _commentLabel;
    private Label _countdownLabel;
    private Panel _infoPanel;

    public QRDisplayForm(
        ILogger<QRDisplayForm> logger,
        IQRCodeService qrCodeService,
        QRCodeRequest request)
    {
        _logger = logger;
        _qrCodeService = qrCodeService;
        _request = request;
        _remainingSeconds = request.TimeoutSeconds;

        _countdownTimer = new System.Windows.Forms.Timer();
        _countdownTimer.Interval = QRBridgeConstants.UI.CountdownInterval;
        _countdownTimer.Tick += CountdownTimer_Tick;

        InitializeComponent();
        GenerateAndDisplayQRCode();
    }

    private void InitializeComponent()
    {
        // Form settings
        Text = QRBridgeConstants.UI.WindowTitle;
        Size = new Size(QRBridgeConstants.UI.FormWidth, QRBridgeConstants.UI.FormHeight);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = QRBridgeConstants.Colors.Background;

        // Create controls
        _qrCodePictureBox = new PictureBox
        {
            Size = new Size(QRBridgeConstants.UI.QRCodeSize, QRBridgeConstants.UI.QRCodeSize),
            Location = new Point(25, 10),
            BackColor = Color.White,
            SizeMode = PictureBoxSizeMode.Zoom
        };

        _infoPanel = new Panel
        {
            Location = new Point(25, 420),
            Size = new Size(400, 60),
            BackColor = Color.FromArgb(60, 60, 60)
        };

        _patientLabel = CreateLabel(10, 5, 380);
        _examIdLabel = CreateLabel(10, 23, 380);
        _commentLabel = CreateLabel(10, 41, 380);

        _countdownLabel = new Label
        {
            Location = new Point(25, 485),
            Size = new Size(400, 20),
            ForeColor = QRBridgeConstants.Colors.Accent,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        };

        // Add controls
        _infoPanel.Controls.AddRange(new Control[] { _patientLabel, _examIdLabel, _commentLabel });
        Controls.AddRange(new Control[] { _qrCodePictureBox, _infoPanel, _countdownLabel });

        // Set initial text
        UpdateLabels();
    }

    private Label CreateLabel(int x, int y, int width)
    {
        return new Label
        {
            Location = new Point(x, y),
            Size = new Size(width, 16),
            ForeColor = QRBridgeConstants.Colors.Foreground,
            BackColor = Color.Transparent,
            Font = new Font("Segoe UI", 9F),
            AutoEllipsis = true
        };
    }

    private void UpdateLabels()
    {
        _patientLabel.Text = $"Patient: {_request.Patient.Name}";
        _examIdLabel.Text = $"Untersuchung: {_request.Study.ExamId}";
        _commentLabel.Text = $"Kommentar: {_request.Comment ?? "-"}";
        UpdateCountdown();
    }

    private void UpdateCountdown()
    {
        _countdownLabel.Text = string.Format(
            QRBridgeConstants.UI.CountdownFormat,
            _remainingSeconds);
    }

    private void GenerateAndDisplayQRCode()
    {
        try
        {
            _logger.LogInformation("Generating QR code for display");

            _qrCodeBitmap = _qrCodeService.GenerateQRCode(_request);
            _qrCodePictureBox.Image = _qrCodeBitmap;

            _logger.LogInformation("QR code displayed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate QR code");

            // Show error message
            _qrCodePictureBox.BackColor = QRBridgeConstants.Colors.Error;
            var errorLabel = new Label
            {
                Text = "Fehler beim Generieren des QR-Codes!",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            _qrCodePictureBox.Controls.Add(errorLabel);
        }
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        // Start countdown timer
        _countdownTimer.Start();

        // Bring to front
        TopMost = true;
        TopMost = false;
        Focus();
    }

    private void CountdownTimer_Tick(object? sender, EventArgs e)
    {
        _remainingSeconds--;
        UpdateCountdown();

        if (_remainingSeconds <= 0)
        {
            _countdownTimer.Stop();
            _logger.LogInformation("Countdown completed, closing form");
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);

        _countdownTimer?.Dispose();
        _qrCodeBitmap?.Dispose();

        _logger.LogInformation("QR display form closed");
    }

    // Allow closing with ESC key
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Escape)
        {
            _logger.LogInformation("User pressed ESC, closing form");
            Close();
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }
}
