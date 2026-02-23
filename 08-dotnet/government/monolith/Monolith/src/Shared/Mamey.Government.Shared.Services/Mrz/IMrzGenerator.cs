namespace Mamey.Government.Shared.Services.Mrz;

/// <summary>
/// Generates MRZ (Machine Readable Zone) lines according to ICAO 9303 TD1 format.
/// Used for travel documents like national ID cards.
/// </summary>
public interface IMrzGenerator
{
    /// <summary>
    /// Generates TD1 Line 1 from document number.
    /// Format: [9 digits][check digit][21 &lt; characters]
    /// </summary>
    string GenerateTD1Line1(string documentNumber);

    /// <summary>
    /// Generates TD1 Line 2 from birth date, expiration date, and optional data.
    /// Format: [YYMMDD][check][YYMMDD][check][optional 11 chars][check]
    /// </summary>
    string GenerateTD1Line2(DateTime birthDate, DateTime expDate, string optionalData = "");

    /// <summary>
    /// Generates TD1 Line 3 from surname and given names.
    /// Format: SURNAME&lt;&lt;GIVENNAMES&lt;&lt;
    /// </summary>
    string GenerateTD1Line3(string surname, string givenNames);

    /// <summary>
    /// Calculates ICAO check digit for an MRZ string.
    /// Uses weights 7, 3, 1 repeating from right to left.
    /// </summary>
    int CalculateIcaoCheckDigit(string mrzString);

    /// <summary>
    /// Sanitizes input for use in MRZ fields.
    /// </summary>
    string SanitizeMrzFields(string input);
}
