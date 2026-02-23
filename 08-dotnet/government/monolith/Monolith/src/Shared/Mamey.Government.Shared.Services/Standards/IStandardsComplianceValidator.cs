namespace Mamey.Government.Shared.Services.Standards;

/// <summary>
/// Validates compliance with ICAO 9303 and AAMVA standards for travel documents.
/// </summary>
public interface IStandardsComplianceValidator
{
    /// <summary>
    /// Sanitizes input for use in MRZ fields.
    /// Converts to uppercase, removes invalid characters, replaces spaces with &lt;.
    /// </summary>
    string SanitizeMrzField(string input);

    /// <summary>
    /// Truncates names according to MRZ rules.
    /// Combined surname and given names must fit in available space.
    /// </summary>
    void TruncateMrzName(string surname, string givenNames, out string truncatedSurname, out string truncatedGivenNames);

    /// <summary>
    /// Validates a passport number format.
    /// </summary>
    bool ValidatePassportNumber(string passportNumber);

    /// <summary>
    /// Validates an MRZ line for correct format and check digits.
    /// </summary>
    bool ValidateMrzLine(string line, int expectedLength);
}
