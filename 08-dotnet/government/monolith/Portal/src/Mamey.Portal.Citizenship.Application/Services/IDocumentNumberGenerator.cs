namespace Mamey.Portal.Citizenship.Application.Services;

/// <summary>
/// Service for generating AAMVA/MRZ-compliant document numbers
/// </summary>
public interface IDocumentNumberGenerator
{
    /// <summary>
    /// Generates a passport number in format [A-Z][0-9]{2}[A-Z0-9]{5} (9 characters) with Luhn check digit
    /// </summary>
    /// <param name="lastName">Last name to extract first letter</param>
    /// <param name="birthDate">Date of birth to extract YY</param>
    /// <returns>9-character passport number</returns>
    string GeneratePassportNumber(string lastName, DateTime birthDate);

    /// <summary>
    /// Generates a medical card number in format [A-Z]{4}-[A-Z0-9]+
    /// </summary>
    /// <returns>Medical card number with format [A-Z]{4}-[A-Z0-9]+</returns>
    string GenerateMedicalCardNumber();

    /// <summary>
    /// Removes document prefixes (INKMIA-BC-, INKMIA-CA-, etc.) for MRZ compatibility
    /// </summary>
    /// <param name="documentNumber">Document number with optional prefix</param>
    /// <returns>Document number without prefix</returns>
    string RemoveDocumentPrefixes(string documentNumber);

    /// <summary>
    /// Calculates Luhn check digit for a numeric/alphanumeric string
    /// </summary>
    /// <param name="numberString">String to calculate check digit for (letters converted to numbers)</param>
    /// <returns>Check digit (0-9)</returns>
    int CalculateLuhnCheckDigit(string numberString);
}


