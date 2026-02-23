namespace Mamey.Portal.Citizenship.Application.Services;

/// <summary>
/// Service for generating ICAO Doc 9303-compliant MRZ (Machine Readable Zone) for ID cards
/// </summary>
public interface IMrzGenerator
{
    /// <summary>
    /// Generates TD1 MRZ Line 1 (30 characters exactly): Document number + check digit + padding
    /// </summary>
    /// <param name="documentNumber">Document number (may include prefixes, will be cleaned)</param>
    /// <returns>30-character MRZ Line 1</returns>
    string GenerateTD1Line1(string documentNumber);

    /// <summary>
    /// Generates TD1 MRZ Line 2 (30 characters exactly): Dates + check digits + optional data
    /// </summary>
    /// <param name="birthDate">Date of birth</param>
    /// <param name="expDate">Expiration date</param>
    /// <param name="optionalData">Optional 11-character data field</param>
    /// <returns>30-character MRZ Line 2</returns>
    string GenerateTD1Line2(DateTime birthDate, DateTime expDate, string optionalData = "");

    /// <summary>
    /// Generates TD1 MRZ Line 3 (30 characters exactly): Surname + given names
    /// </summary>
    /// <param name="surname">Surname</param>
    /// <param name="givenNames">Given names</param>
    /// <returns>30-character MRZ Line 3</returns>
    string GenerateTD1Line3(string surname, string givenNames);

    /// <summary>
    /// Calculates ICAO check digit for a string (weights 7, 3, 1 repeating)
    /// </summary>
    /// <param name="mrzString">String to calculate check digit for</param>
    /// <returns>Check digit (0-9)</returns>
    int CalculateIcaoCheckDigit(string mrzString);

    /// <summary>
    /// Sanitizes text for MRZ compliance (A-Z, 0-9, &lt; only)
    /// </summary>
    /// <param name="input">Input text</param>
    /// <returns>Sanitized MRZ-compliant string</returns>
    string SanitizeMrzFields(string input);
}


