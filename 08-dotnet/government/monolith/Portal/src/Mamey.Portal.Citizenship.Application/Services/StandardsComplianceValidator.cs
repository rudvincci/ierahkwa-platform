using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Mamey.AmvvaStandards;
using Mamey.Portal.Citizenship.Application.Models;

namespace Mamey.Portal.Citizenship.Application.Services;

public sealed class StandardsComplianceValidator : IStandardsComplianceValidator
{
    private const int AAMVA_NAME_MAX_LENGTH = 35;
    private const int AAMVA_ADDRESS_MAX_LENGTH = 35;
    private const int AAMVA_POSTAL_CODE_MAX_LENGTH = 11;
    private const int AAMVA_LICENSE_NUMBER_MIN_LENGTH = 5;
    private const int AAMVA_LICENSE_NUMBER_MAX_LENGTH = 16;
    private const int MRZ_LINE_LENGTH = 30;
    private const int MRZ_DOCUMENT_NUMBER_LENGTH = 9;
    private const int MRZ_OPTIONAL_DATA_LENGTH = 11;
    private const int MRZ_SURNAME_MIN_PRESERVE = 3;

    public bool ValidateAamvaFields(BaseAamvaCard card, out List<ValidationError> errors)
    {
        errors = new List<ValidationError>();

        // Validate required fields
        if (string.IsNullOrWhiteSpace(card.FamilyName))
        {
            errors.Add(new ValidationError
            {
                FieldName = nameof(card.FamilyName),
                ErrorCode = "REQUIRED_FIELD_MISSING",
                Message = "Family name is required.",
                Severity = ValidationSeverity.Error
            });
        }

        if (string.IsNullOrWhiteSpace(card.GivenName))
        {
            errors.Add(new ValidationError
            {
                FieldName = nameof(card.GivenName),
                ErrorCode = "REQUIRED_FIELD_MISSING",
                Message = "Given name is required.",
                Severity = ValidationSeverity.Error
            });
        }

        if (string.IsNullOrWhiteSpace(card.LicenseOrIdNumber))
        {
            errors.Add(new ValidationError
            {
                FieldName = nameof(card.LicenseOrIdNumber),
                ErrorCode = "REQUIRED_FIELD_MISSING",
                Message = "License/ID number is required.",
                Severity = ValidationSeverity.Error
            });
        }

        // Validate field lengths and apply truncation
        ValidateAndTruncateField(
            card.FamilyName,
            AAMVA_NAME_MAX_LENGTH,
            FieldType.Name,
            nameof(card.FamilyName),
            errors,
            out var truncatedFamilyName);

        ValidateAndTruncateField(
            card.GivenName,
            AAMVA_NAME_MAX_LENGTH,
            FieldType.Name,
            nameof(card.GivenName),
            errors,
            out var truncatedGivenName);

        if (!string.IsNullOrWhiteSpace(card.MiddleNames))
        {
            ValidateAndTruncateField(
                card.MiddleNames,
                AAMVA_NAME_MAX_LENGTH,
                FieldType.Name,
                nameof(card.MiddleNames),
                errors,
                out _);
        }

        if (!string.IsNullOrWhiteSpace(card.StreetAddress))
        {
            ValidateAndTruncateField(
                card.StreetAddress,
                AAMVA_ADDRESS_MAX_LENGTH,
                FieldType.Address,
                nameof(card.StreetAddress),
                errors,
                out _);
        }

        if (!string.IsNullOrWhiteSpace(card.City))
        {
            ValidateAndTruncateField(
                card.City,
                AAMVA_ADDRESS_MAX_LENGTH,
                FieldType.Address,
                nameof(card.City),
                errors,
                out _);
        }

        if (!string.IsNullOrWhiteSpace(card.PostalCode))
        {
            ValidateAndTruncateField(
                card.PostalCode,
                AAMVA_POSTAL_CODE_MAX_LENGTH,
                FieldType.PostalCode,
                nameof(card.PostalCode),
                errors,
                out _);
        }

        // Validate license number length
        if (!string.IsNullOrWhiteSpace(card.LicenseOrIdNumber))
        {
            var licenseNumber = card.LicenseOrIdNumber.Trim();
            if (licenseNumber.Length < AAMVA_LICENSE_NUMBER_MIN_LENGTH ||
                licenseNumber.Length > AAMVA_LICENSE_NUMBER_MAX_LENGTH)
            {
                errors.Add(new ValidationError
                {
                    FieldName = nameof(card.LicenseOrIdNumber),
                    ErrorCode = "INVALID_LICENSE_NUMBER_LENGTH",
                    Message = $"License number is invalid or out of range ({AAMVA_LICENSE_NUMBER_MIN_LENGTH}-{AAMVA_LICENSE_NUMBER_MAX_LENGTH} chars).",
                    Severity = ValidationSeverity.Error,
                    OriginalValue = licenseNumber
                });
            }
        }

        // Validate dates
        if (card.DateOfBirth > card.IssueDate)
        {
            errors.Add(new ValidationError
            {
                FieldName = nameof(card.DateOfBirth),
                ErrorCode = "INVALID_DATE_ORDER",
                Message = "Date of birth must be before issue date.",
                Severity = ValidationSeverity.Error
            });
        }

        if (card.IssueDate >= card.ExpirationDate)
        {
            errors.Add(new ValidationError
            {
                FieldName = nameof(card.ExpirationDate),
                ErrorCode = "INVALID_DATE_ORDER",
                Message = "Issue date must be before expiration date.",
                Severity = ValidationSeverity.Error
            });
        }

        // Validate numeric ranges
        if (card.HeightInches < 48 || card.HeightInches > 96)
        {
            errors.Add(new ValidationError
            {
                FieldName = nameof(card.HeightInches),
                ErrorCode = "INVALID_HEIGHT_RANGE",
                Message = "Height must be between 48-96 inches (4-8 feet).",
                Severity = ValidationSeverity.Error,
                OriginalValue = card.HeightInches
            });
        }

        // Use existing AAMVA validators
        if (card is DriverLicenseCard dlCard)
        {
            if (!AamvaValidators.ValidateDriverLicenseCard(dlCard, out var aamvaError))
            {
                errors.Add(new ValidationError
                {
                    FieldName = "Card",
                    ErrorCode = "AAMVA_VALIDATION_FAILED",
                    Message = aamvaError ?? "AAMVA validation failed.",
                    Severity = ValidationSeverity.Error
                });
            }
        }
        else if (card is IdentificationCard idCard)
        {
            if (!AamvaValidators.ValidateIdentificationCard(idCard, out var aamvaError))
            {
                errors.Add(new ValidationError
                {
                    FieldName = "Card",
                    ErrorCode = "AAMVA_VALIDATION_FAILED",
                    Message = aamvaError ?? "AAMVA validation failed.",
                    Severity = ValidationSeverity.Error
                });
            }
        }

        return errors.All(e => e.Severity != ValidationSeverity.Error);
    }

    public bool ValidateMrzFields(
        string surname,
        string givenNames,
        string documentNumber,
        DateTime birthDate,
        DateTime expDate,
        out List<ValidationError> errors)
    {
        errors = new List<ValidationError>();

        // Validate document number
        if (string.IsNullOrWhiteSpace(documentNumber))
        {
            errors.Add(new ValidationError
            {
                FieldName = nameof(documentNumber),
                ErrorCode = "REQUIRED_FIELD_MISSING",
                Message = "Document number is required for MRZ.",
                Severity = ValidationSeverity.Error
            });
        }
        else
        {
            // Remove prefixes and validate numeric
            var cleanDocNumber = RemoveDocumentPrefixes(documentNumber);
            if (!Regex.IsMatch(cleanDocNumber, @"^\d+$"))
            {
                errors.Add(new ValidationError
                {
                    FieldName = nameof(documentNumber),
                    ErrorCode = "INVALID_DOCUMENT_NUMBER_FORMAT",
                    Message = "Document number must be numeric (after removing prefixes) for MRZ.",
                    Severity = ValidationSeverity.Error,
                    OriginalValue = documentNumber
                });
            }
            else if (cleanDocNumber.Length > MRZ_DOCUMENT_NUMBER_LENGTH)
            {
                errors.Add(new ValidationError
                {
                    FieldName = nameof(documentNumber),
                    ErrorCode = "DOCUMENT_NUMBER_TOO_LONG",
                    Message = $"Document number exceeds {MRZ_DOCUMENT_NUMBER_LENGTH} digits for MRZ.",
                    Severity = ValidationSeverity.Warning,
                    OriginalValue = documentNumber
                });
            }
        }

        // Validate names
        if (string.IsNullOrWhiteSpace(surname))
        {
            errors.Add(new ValidationError
            {
                FieldName = nameof(surname),
                ErrorCode = "REQUIRED_FIELD_MISSING",
                Message = "Surname is required for MRZ.",
                Severity = ValidationSeverity.Error
            });
        }

        if (string.IsNullOrWhiteSpace(givenNames))
        {
            errors.Add(new ValidationError
            {
                FieldName = nameof(givenNames),
                ErrorCode = "REQUIRED_FIELD_MISSING",
                Message = "Given names are required for MRZ.",
                Severity = ValidationSeverity.Error
            });
        }

        // Validate dates
        if (birthDate > DateTime.UtcNow)
        {
            errors.Add(new ValidationError
            {
                FieldName = nameof(birthDate),
                ErrorCode = "INVALID_BIRTH_DATE",
                Message = "Birth date cannot be in the future.",
                Severity = ValidationSeverity.Error
            });
        }

        if (expDate <= birthDate)
        {
            errors.Add(new ValidationError
            {
                FieldName = nameof(expDate),
                ErrorCode = "INVALID_EXPIRATION_DATE",
                Message = "Expiration date must be after birth date.",
                Severity = ValidationSeverity.Error
            });
        }

        // Check if names fit in MRZ Line 3 (30 chars total: surname + "<<" + givenNames + "<<")
        var sanitizedSurname = SanitizeMrzField(surname ?? string.Empty);
        var sanitizedGivenNames = SanitizeMrzField(givenNames ?? string.Empty);
        var totalNameLength = sanitizedSurname.Length + sanitizedGivenNames.Length + 4; // "<<", "<<"

        if (totalNameLength > MRZ_LINE_LENGTH)
        {
            errors.Add(new ValidationError
            {
                FieldName = "MRZ_Line3",
                ErrorCode = "MRZ_NAME_TOO_LONG",
                Message = $"Combined surname and given names exceed {MRZ_LINE_LENGTH} characters for MRZ Line 3. Truncation will be applied.",
                Severity = ValidationSeverity.Warning,
                OriginalValue = $"{surname} {givenNames}"
            });
        }

        return errors.All(e => e.Severity != ValidationSeverity.Error);
    }

    public string TruncateAamvaField(string field, int maxLength, FieldType fieldType)
    {
        if (string.IsNullOrEmpty(field) || field.Length <= maxLength)
        {
            return field;
        }

        return fieldType switch
        {
            FieldType.Name => TruncateFromRight(field, maxLength),
            FieldType.Address => TruncateAddress(field, maxLength),
            FieldType.PostalCode => TruncatePostalCode(field, maxLength),
            FieldType.LicenseNumber => throw new InvalidOperationException("License numbers should not be truncated."),
            FieldType.Text => TruncateFromRight(field, maxLength),
            _ => TruncateFromRight(field, maxLength)
        };
    }

    public void TruncateMrzName(
        string surname,
        string givenNames,
        out string truncatedSurname,
        out string truncatedGivenNames)
    {
        var sanitizedSurname = SanitizeMrzField(surname ?? string.Empty);
        var sanitizedGivenNames = SanitizeMrzField(givenNames ?? string.Empty);

        // Available space: 30 - 2 ("<<") - 2 ("<<") = 26 characters
        const int availableSpace = MRZ_LINE_LENGTH - 4;
        var totalLength = sanitizedSurname.Length + sanitizedGivenNames.Length;

        if (totalLength <= availableSpace)
        {
            truncatedSurname = sanitizedSurname;
            truncatedGivenNames = sanitizedGivenNames;
            return;
        }

        // Priority: Preserve surname over given names
        // Strategy: Truncate given names first, then surname if needed
        if (sanitizedSurname.Length <= availableSpace - 1) // At least 1 char for given names
        {
            // Surname fits, truncate given names
            var givenNamesSpace = availableSpace - sanitizedSurname.Length;
            truncatedSurname = sanitizedSurname;
            truncatedGivenNames = TruncateFromRight(sanitizedGivenNames, givenNamesSpace);
        }
        else
        {
            // Surname alone exceeds space, truncate both but preserve at least first 3 chars of surname
            var minSurnameLength = Math.Min(MRZ_SURNAME_MIN_PRESERVE, sanitizedSurname.Length);
            var surnameSpace = Math.Max(minSurnameLength, availableSpace - 1);
            truncatedSurname = TruncateFromRight(sanitizedSurname, surnameSpace);
            truncatedGivenNames = sanitizedGivenNames.Length > 0 ? sanitizedGivenNames.Substring(0, 1) : string.Empty;
        }
    }

    public string SanitizeAamvaField(string field)
    {
        if (string.IsNullOrEmpty(field))
        {
            return field;
        }

        var sb = new StringBuilder(field.Length);
        foreach (var c in field)
        {
            // Remove all control characters.
            if (char.IsControl(c))
            {
                continue;
            }

            // Allow: Letters, Numbers, Spaces, Hyphens, Apostrophes, Periods
            if (char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '\'' || c == '.')
            {
                // Convert accented characters to plain equivalents
                var normalized = c.ToString().Normalize(NormalizationForm.FormD);
                var plain = new string(normalized.Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark).ToArray());
                sb.Append(plain.Length > 0 ? plain[0] : c);
            }
            // Replace other special characters with space
            else if (!char.IsControl(c))
            {
                sb.Append(' ');
            }
        }

        var sanitized = sb.ToString();
        if (sanitized.Length == 0)
        {
            return sanitized;
        }

        var filtered = new string(sanitized.Where(ch => !char.IsControl(ch)).ToArray());
        return filtered.Trim();
    }

    public string SanitizeMrzField(string field)
    {
        if (string.IsNullOrEmpty(field))
        {
            return field;
        }

        var sb = new StringBuilder(field.Length);
        foreach (var c in field)
        {
            // Valid characters: A-Z, 0-9, <
            if (char.IsUpper(c) || char.IsDigit(c) || c == '<')
            {
                sb.Append(c);
            }
            // Convert lowercase to uppercase
            else if (char.IsLower(c))
            {
                sb.Append(char.ToUpperInvariant(c));
            }
            // Replace spaces, hyphens with <
            else if (c == ' ' || c == '-')
            {
                sb.Append('<');
            }
            // Convert accented characters to plain equivalents
            else if (char.IsLetter(c))
            {
                var normalized = c.ToString().Normalize(NormalizationForm.FormD);
                var plain = new string(normalized.Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark).ToArray());
                if (plain.Length > 0 && char.IsLetter(plain[0]))
                {
                    sb.Append(char.ToUpperInvariant(plain[0]));
                }
            }
            // Remove apostrophes, periods, and other special characters
            // (they are removed, not replaced)
        }

        return sb.ToString();
    }

    private void ValidateAndTruncateField(
        string field,
        int maxLength,
        FieldType fieldType,
        string fieldName,
        List<ValidationError> errors,
        out string truncated)
    {
        truncated = field ?? string.Empty;

        if (string.IsNullOrEmpty(truncated))
        {
            return;
        }

        var original = truncated;
        var sanitized = SanitizeAamvaField(truncated);
        var wasSanitized = sanitized != original;

        if (sanitized.Length > maxLength)
        {
            truncated = TruncateAamvaField(sanitized, maxLength, fieldType);
            errors.Add(new ValidationError
            {
                FieldName = fieldName,
                ErrorCode = "FIELD_EXCEEDS_LENGTH",
                Message = $"{fieldName} exceeds maximum length of {maxLength}. Truncated from {sanitized.Length} to {truncated.Length} characters.",
                Severity = ValidationSeverity.Warning,
                OriginalValue = original,
                CorrectedValue = truncated,
                WasTruncated = true,
                WasSanitized = wasSanitized
            });
        }
        else if (wasSanitized)
        {
            truncated = sanitized;
            errors.Add(new ValidationError
            {
                FieldName = fieldName,
                ErrorCode = "FIELD_SANITIZED",
                Message = $"{fieldName} contained invalid characters and was sanitized.",
                Severity = ValidationSeverity.Warning,
                OriginalValue = original,
                CorrectedValue = truncated,
                WasSanitized = true
            });
        }
    }

    private static string TruncateFromRight(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
        {
            return value;
        }

        return value.Substring(0, maxLength);
    }

    private static string TruncateAddress(string address, int maxLength)
    {
        if (string.IsNullOrEmpty(address) || address.Length <= maxLength)
        {
            return address;
        }

        // Try to preserve street number (digits at the beginning)
        var match = Regex.Match(address, @"^(\d+\s+)");
        if (match.Success)
        {
            var streetNumber = match.Value;
            var remainingSpace = maxLength - streetNumber.Length;
            if (remainingSpace > 0)
            {
                var rest = address.Substring(streetNumber.Length);
                return streetNumber + TruncateFromRight(rest, remainingSpace);
            }
        }

        return TruncateFromRight(address, maxLength);
    }

    private static string TruncatePostalCode(string postalCode, int maxLength)
    {
        if (string.IsNullOrEmpty(postalCode) || postalCode.Length <= maxLength)
        {
            return postalCode;
        }

        // Remove hyphens if needed
        var withoutHyphens = postalCode.Replace("-", string.Empty);
        if (withoutHyphens.Length <= maxLength)
        {
            return withoutHyphens;
        }

        return TruncateFromRight(withoutHyphens, maxLength);
    }

    private static string RemoveDocumentPrefixes(string documentNumber)
    {
        if (string.IsNullOrEmpty(documentNumber))
        {
            return documentNumber;
        }

        // Remove common prefixes: INKMIA-BC-, INKMIA-CA-, INKMIA-CPRO-, INKMIA-CRES-
        var prefixes = new[] { "INKMIA-BC-", "INKMIA-CA-", "INKMIA-CPRO-", "INKMIA-CRES-" };
        var result = documentNumber;
        foreach (var prefix in prefixes)
        {
            if (result.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                result = result.Substring(prefix.Length);
                break;
            }
        }

        return result;
    }
}
