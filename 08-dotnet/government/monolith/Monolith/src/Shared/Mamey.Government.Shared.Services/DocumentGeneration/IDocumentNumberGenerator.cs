namespace Mamey.Government.Shared.Services.DocumentGeneration;

/// <summary>
/// Generates various document numbers (passport, medical card, etc.)
/// </summary>
public interface IDocumentNumberGenerator
{
    /// <summary>
    /// Generates a passport number based on last name and birth date.
    /// Format: [Letter][YY][5-char-alphanumeric][Luhn-check-digit]
    /// </summary>
    string GeneratePassportNumber(string lastName, DateTime birthDate);

    /// <summary>
    /// Generates a medical card number.
    /// Format: [4-letter-prefix]-[6-char-alphanumeric]
    /// </summary>
    string GenerateMedicalCardNumber();

    /// <summary>
    /// Generates a travel identity card number.
    /// </summary>
    string GenerateTravelIdentityNumber(string lastName, DateTime birthDate);

    /// <summary>
    /// Removes document prefixes from a document number.
    /// </summary>
    string RemoveDocumentPrefixes(string documentNumber);

    /// <summary>
    /// Calculates Luhn check digit for alphanumeric string.
    /// </summary>
    int CalculateLuhnCheckDigit(string numberString);
}
