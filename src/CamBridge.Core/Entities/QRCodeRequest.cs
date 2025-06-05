// src/CamBridge.Core/Entities/QRCodeRequest.cs
// Version: 0.5.33
// © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Core.ValueObjects;

namespace CamBridge.Core.Entities;

/// <summary>
/// Request object for QR code generation containing patient and study information
/// </summary>
public record QRCodeRequest(
    PatientInfo Patient,
    StudyInfo Study,
    string? Comment = null,
    int TimeoutSeconds = 10
)
{
    /// <summary>
    /// Formats the QR code data in the expected format for Ricoh cameras
    /// Format: ExamId|Name|BirthDate|Gender|Comment
    /// </summary>
    public string FormatForQRCode()
    {
        // Format exactly as legacy QRBridge expects
        var parts = new[]
        {
            Study.ExamId ?? string.Empty,
            Patient.Name ?? string.Empty,
            Patient.BirthDate?.ToString("yyyy-MM-dd") ?? string.Empty,
            Patient.Gender.ToString()[0].ToString(), // M/F/O
            Comment ?? string.Empty
        };

        return string.Join("|", parts);
    }

    /// <summary>
    /// Creates an empty request for initialization
    /// </summary>
    public static QRCodeRequest Empty => new(
        new PatientInfo(
            new PatientId("QR-EMPTY"),  // id - POSITIONAL, not named!
            "Unknown",                   // name
            null,                        // birthDate
            Gender.Other                 // gender
        ),
        new StudyInfo(
            StudyId.Generate(),          // studyId
            "UNKNOWN",                   // examId
            null,                        // description
            "OT"                         // modality
        ),
        null,
        10                              // default timeout seconds
    );
}
