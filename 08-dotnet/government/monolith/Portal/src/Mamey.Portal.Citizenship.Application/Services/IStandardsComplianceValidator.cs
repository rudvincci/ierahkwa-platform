using Mamey.AmvvaStandards;
using Mamey.Portal.Citizenship.Application.Models;

namespace Mamey.Portal.Citizenship.Application.Services;

/// <summary>
/// Service for validating and sanitizing fields according to AAMVA and ICAO MRZ standards
/// </summary>
public interface IStandardsComplianceValidator
{
    /// <summary>
    /// Validates AAMVA card fields and applies truncation/sanitization rules
    /// </summary>
    /// <param name="card">The AAMVA card to validate</param>
    /// <param name="errors">List of validation errors and warnings</param>
    /// <returns>True if validation passes (may have warnings), false if errors found</returns>
    bool ValidateAamvaFields(BaseAamvaCard card, out List<ValidationError> errors);

    /// <summary>
    /// Validates MRZ fields and applies truncation/sanitization rules
    /// </summary>
    /// <param name="surname">Surname for MRZ Line 3</param>
    /// <param name="givenNames">Given names for MRZ Line 3</param>
    /// <param name="documentNumber">Document number for MRZ Line 1</param>
    /// <param name="birthDate">Birth date for MRZ Line 2</param>
    /// <param name="expDate">Expiration date for MRZ Line 2</param>
    /// <param name="errors">List of validation errors and warnings</param>
    /// <returns>True if validation passes (may have warnings), false if errors found</returns>
    bool ValidateMrzFields(
        string surname,
        string givenNames,
        string documentNumber,
        DateTime birthDate,
        DateTime expDate,
        out List<ValidationError> errors);

    /// <summary>
    /// Applies AAMVA truncation rules to a field
    /// </summary>
    /// <param name="field">The field value to truncate</param>
    /// <param name="maxLength">Maximum length allowed</param>
    /// <param name="fieldType">Type of field (determines truncation strategy)</param>
    /// <returns>Truncated field value</returns>
    string TruncateAamvaField(string field, int maxLength, FieldType fieldType);

    /// <summary>
    /// Applies MRZ name truncation rules (surname priority)
    /// </summary>
    /// <param name="surname">Surname to truncate</param>
    /// <param name="givenNames">Given names to truncate</param>
    /// <param name="truncatedSurname">Output truncated surname</param>
    /// <param name="truncatedGivenNames">Output truncated given names</param>
    void TruncateMrzName(
        string surname,
        string givenNames,
        out string truncatedSurname,
        out string truncatedGivenNames);

    /// <summary>
    /// Sanitizes an AAMVA field by removing invalid characters and converting accents
    /// </summary>
    /// <param name="field">Field value to sanitize</param>
    /// <returns>Sanitized field value</returns>
    string SanitizeAamvaField(string field);

    /// <summary>
    /// Sanitizes an MRZ field to contain only A-Z, 0-9, and &lt; characters
    /// </summary>
    /// <param name="field">Field value to sanitize</param>
    /// <returns>Sanitized field value</returns>
    string SanitizeMrzField(string field);
}


