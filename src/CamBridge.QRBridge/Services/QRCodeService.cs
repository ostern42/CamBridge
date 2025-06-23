// src/CamBridge.QRBridge/Services/QRCodeService.cs
// Version: 0.5.33
// Â© 2025 Claude's Improbably Reliable Software Solutions

using System.Text;
using CamBridge.Core.Entities;
using Microsoft.Extensions.Logging;
using QRCoder;

namespace CamBridge.QRBridge.Services;

/// <summary>
/// Interface for QR code generation service
/// </summary>
public interface IQRCodeService
{
    /// <summary>
    /// Generates a QR code bitmap from the request
    /// </summary>
    Bitmap GenerateQRCode(QRCodeRequest request);

    /// <summary>
    /// Generates QR code as PNG byte array
    /// </summary>
    byte[] GenerateQRCodeBytes(QRCodeRequest request);
}

/// <summary>
/// Service for generating QR codes with explicit UTF-8 encoding
/// </summary>
public class QRCodeService : IQRCodeService, IDisposable
{
    private readonly ILogger<QRCodeService> _logger;
    private readonly QRCodeGenerator _qrGenerator;

    public QRCodeService(ILogger<QRCodeService> logger)
    {
        _logger = logger;
        _qrGenerator = new QRCodeGenerator();
    }

    /// <summary>
    /// Generates a QR code bitmap from the request
    /// </summary>
    public Bitmap GenerateQRCode(QRCodeRequest request)
    {
        try
        {
            _logger.LogInformation("Generating QR code for patient: {PatientName}", request.Patient.Name);

            // Format the data for QR code
            var qrData = request.FormatForQRCode();
            _logger.LogDebug("QR data formatted: {Data}", qrData);

            // EXPLICIT UTF-8 ENCODING - This is the key!
            var utf8Bytes = Encoding.UTF8.GetBytes(qrData);
            _logger.LogDebug("UTF-8 byte count: {Count}", utf8Bytes.Length);

            // Generate QR code from UTF-8 bytes
            using var qrCodeData = _qrGenerator.CreateQrCode(utf8Bytes, QRCodeGenerator.ECCLevel.M);
            using var qrCode = new QRCode(qrCodeData);

            // Create bitmap with proper size
            var bitmap = qrCode.GetGraphic(
                pixelsPerModule: 10,
                darkColor: Color.Black,
                lightColor: Color.White,
                drawQuietZones: true
            );

            _logger.LogInformation("QR code generated successfully. Size: {Width}x{Height}",
                bitmap.Width, bitmap.Height);

            return bitmap;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate QR code");
            throw new InvalidOperationException("QR code generation failed", ex);
        }
    }

    /// <summary>
    /// Generates QR code as PNG byte array for API usage
    /// </summary>
    public byte[] GenerateQRCodeBytes(QRCodeRequest request)
    {
        try
        {
            _logger.LogInformation("Generating QR code bytes for patient: {PatientName}", request.Patient.Name);

            var qrData = request.FormatForQRCode();

            // EXPLICIT UTF-8 ENCODING
            var utf8Bytes = Encoding.UTF8.GetBytes(qrData);

            using var qrCodeData = _qrGenerator.CreateQrCode(utf8Bytes, QRCodeGenerator.ECCLevel.M);
            using var qrCode = new PngByteQRCode(qrCodeData);

            var pngBytes = qrCode.GetGraphic(
                pixelsPerModule: 10,
                darkColorRgba: new byte[] { 0, 0, 0, 255 },     // Black
                lightColorRgba: new byte[] { 255, 255, 255, 255 } // White
            );

            _logger.LogInformation("QR code PNG generated. Size: {Size} bytes", pngBytes.Length);

            return pngBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate QR code bytes");
            throw new InvalidOperationException("QR code generation failed", ex);
        }
    }

    public void Dispose()
    {
        _qrGenerator?.Dispose();
    }
}
