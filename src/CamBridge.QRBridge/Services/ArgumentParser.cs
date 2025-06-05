// src/CamBridge.QRBridge/Services/ArgumentParser.cs
// Version: 0.5.33
// © 2025 Claude's Improbably Reliable Software Solutions

using System.Globalization;
using CamBridge.Core.Entities;
using CamBridge.Core.ValueObjects;
using CamBridge.QRBridge.Constants;
using Microsoft.Extensions.Logging;

namespace CamBridge.QRBridge.Services;

/// <summary>
/// Parses command line arguments into a QRCodeRequest
/// </summary>
public class ArgumentParser
{
    private readonly ILogger<ArgumentParser> _logger;

    public ArgumentParser(ILogger<ArgumentParser> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Parses command line arguments into a QRCodeRequest
    /// </summary>
    public (bool success, QRCodeRequest? request, string? error) Parse(string[] args)
    {
        _logger.LogDebug("Parsing {Count} arguments", args.Length);

        if (args.Length == 0 || args.Any(a => a == "-" + QRBridgeConstants.Arguments.Help || a == "--" + QRBridgeConstants.Arguments.Help))
        {
            return (false, null, GetHelpText());
        }

        try
        {
            var arguments = ParseArguments(args);

            // Validate required fields
            if (!arguments.ContainsKey(QRBridgeConstants.Arguments.ExamId))
            {
                return (false, null, "ERROR: -examid is required");
            }

            if (!arguments.ContainsKey(QRBridgeConstants.Arguments.Name))
            {
                return (false, null, "ERROR: -name is required");
            }

            // Parse values
            var name = arguments[QRBridgeConstants.Arguments.Name];
            var birthDate = ParseBirthDate(arguments);
            var gender = ParseGender(arguments);
            var examId = arguments[QRBridgeConstants.Arguments.ExamId];

            // Create patient info with PatientId
            var patientId = new PatientId($"QR-{DateTime.Now:yyyyMMddHHmmss}");
            var patient = new PatientInfo(patientId, name, birthDate, gender);

            // Create study info with StudyId
            var studyId = StudyId.CreateFromExamId(examId);
            var study = new StudyInfo(
                studyId,                                                           // studyId
                examId,                                                            // examId
                arguments.GetValueOrDefault(QRBridgeConstants.Arguments.Comment), // description
                "OT"                                                               // modality
            );

            // Parse optional fields
            var comment = arguments.GetValueOrDefault(QRBridgeConstants.Arguments.Comment);
            var timeout = ParseTimeout(arguments);

            var request = new QRCodeRequest(patient, study, comment, timeout);

            _logger.LogInformation("Successfully parsed arguments for patient: {Name}", patient.Name);

            return (true, request, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse arguments");
            return (false, null, $"ERROR: {ex.Message}");
        }
    }

    private Dictionary<string, string> ParseArguments(string[] args)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith("-"))
            {
                var key = args[i].TrimStart('-').ToLower();
                var value = (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    ? args[++i]
                    : string.Empty;

                result[key] = value;
                _logger.LogDebug("Parsed argument: {Key} = {Value}", key, value);
            }
        }

        return result;
    }

    private DateTime? ParseBirthDate(Dictionary<string, string> arguments)
    {
        if (!arguments.TryGetValue(QRBridgeConstants.Arguments.BirthDate, out var dateStr))
            return null;

        if (DateTime.TryParseExact(dateStr, QRBridgeConstants.Defaults.DateFormat,
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }

        // Try other common formats
        if (DateTime.TryParse(dateStr, out date))
        {
            return date;
        }

        _logger.LogWarning("Failed to parse birth date: {DateString}", dateStr);
        return null;
    }

    private Gender ParseGender(Dictionary<string, string> arguments)
    {
        if (!arguments.TryGetValue(QRBridgeConstants.Arguments.Gender, out var genderStr))
            return Gender.Other;

        var firstChar = genderStr.ToUpper().FirstOrDefault();

        return firstChar switch
        {
            'M' => Gender.Male,
            'F' => Gender.Female,
            'W' => Gender.Female, // German: Weiblich
            _ => Gender.Other
        };
    }

    private int ParseTimeout(Dictionary<string, string> arguments)
    {
        if (!arguments.TryGetValue(QRBridgeConstants.Arguments.Timeout, out var timeoutStr))
            return QRBridgeConstants.Defaults.TimeoutSeconds;

        if (int.TryParse(timeoutStr, out var timeout) && timeout > 0)
            return timeout;

        _logger.LogWarning("Invalid timeout value: {Value}, using default", timeoutStr);
        return QRBridgeConstants.Defaults.TimeoutSeconds;
    }

    private string GetHelpText()
    {
        return $@"
CamBridge QRBridge v{QRBridgeConstants.Application.Version}
{QRBridgeConstants.Application.Description}
© 2025 Claude's Improbably Reliable Software Solutions

Usage: CamBridge.QRBridge.exe -examid <ID> -name <NAME> [options]

Required Arguments:
  -examid <ID>        Examination/Study ID
  -name <NAME>        Patient name (Last, First)

Optional Arguments:
  -birthdate <DATE>   Birth date (yyyy-MM-dd format)
  -gender <M/F/O>     Gender (M=Male, F=Female, O=Other)
  -comment <TEXT>     Additional comment
  -timeout <SECONDS>  Window timeout (default: {QRBridgeConstants.Defaults.TimeoutSeconds})
  -help               Show this help

Example:
  CamBridge.QRBridge.exe -examid ""EX001"" -name ""Müller, Hans"" -birthdate ""1985-03-15"" -gender ""M"" -comment ""Thorax PA""

Note: The QR code will be displayed for the specified timeout period.
      UTF-8 encoding is used for all text to support international characters.
";
    }
}
