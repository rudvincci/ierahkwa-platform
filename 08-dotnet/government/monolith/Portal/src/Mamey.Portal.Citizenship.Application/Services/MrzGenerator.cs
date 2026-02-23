using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Mamey.Portal.Citizenship.Application.Models;

namespace Mamey.Portal.Citizenship.Application.Services;

public sealed class MrzGenerator : IMrzGenerator
{
    private const int MRZ_LINE_LENGTH = 30;
    private const int MRZ_DOCUMENT_NUMBER_LENGTH = 9;
    private const int MRZ_OPTIONAL_DATA_LENGTH = 11;
    private readonly IDocumentNumberGenerator _documentNumberGenerator;
    private readonly IStandardsComplianceValidator _validator;

    public MrzGenerator(
        IDocumentNumberGenerator documentNumberGenerator,
        IStandardsComplianceValidator validator)
    {
        _documentNumberGenerator = documentNumberGenerator;
        _validator = validator;
    }

    public string GenerateTD1Line1(string documentNumber)
    {
        // Remove prefixes and hyphens
        var cleanNumber = _documentNumberGenerator.RemoveDocumentPrefixes(documentNumber ?? string.Empty);
        
        // Validate numeric format
        if (!Regex.IsMatch(cleanNumber, @"^\d+$"))
        {
            throw new ArgumentException($"Document number must be numeric after removing prefixes: {documentNumber}", nameof(documentNumber));
        }

        // Pad to 9 characters with leading zeros if needed
        if (cleanNumber.Length < MRZ_DOCUMENT_NUMBER_LENGTH)
        {
            cleanNumber = cleanNumber.PadLeft(MRZ_DOCUMENT_NUMBER_LENGTH, '0');
        }
        else if (cleanNumber.Length > MRZ_DOCUMENT_NUMBER_LENGTH)
        {
            // Truncate from right (preserve leftmost 9 digits)
            cleanNumber = cleanNumber.Substring(0, MRZ_DOCUMENT_NUMBER_LENGTH);
        }

        // Calculate ICAO check digit
        var checkDigit = CalculateIcaoCheckDigit(cleanNumber);

        // Format: [9 digits][check digit][21 < characters]
        var line = $"{cleanNumber}{checkDigit}";
        line = line.PadRight(MRZ_LINE_LENGTH, '<');

        if (line.Length != MRZ_LINE_LENGTH)
        {
            throw new InvalidOperationException($"Generated MRZ Line 1 has invalid length: {line.Length}, expected {MRZ_LINE_LENGTH}");
        }

        return line;
    }

    public string GenerateTD1Line2(DateTime birthDate, DateTime expDate, string optionalData = "")
    {
        // Format birth date as YYMMDD
        var birthDateStr = FormatDateYYMMDD(birthDate);
        var birthCheckDigit = CalculateIcaoCheckDigit(birthDateStr);

        // Format expiration date as YYMMDD
        var expDateStr = FormatDateYYMMDD(expDate);
        var expCheckDigit = CalculateIcaoCheckDigit(expDateStr);

        // Format optional data (11 characters, pad with < if needed, truncate if longer)
        var optional = SanitizeMrzFields(optionalData ?? string.Empty);
        if (optional.Length < MRZ_OPTIONAL_DATA_LENGTH)
        {
            optional = optional.PadRight(MRZ_OPTIONAL_DATA_LENGTH, '<');
        }
        else if (optional.Length > MRZ_OPTIONAL_DATA_LENGTH)
        {
            optional = optional.Substring(0, MRZ_OPTIONAL_DATA_LENGTH);
        }

        // Build line: [YYMMDD][check][YYMMDD][check][optional 11 chars]
        var lineWithoutFinalCheck = $"{birthDateStr}{birthCheckDigit}{expDateStr}{expCheckDigit}{optional}";

        // Calculate final check digit for entire line
        var finalCheckDigit = CalculateIcaoCheckDigit(lineWithoutFinalCheck);

        // Complete line: [YYMMDD][check][YYMMDD][check][optional 11 chars][check]
        var line = $"{lineWithoutFinalCheck}{finalCheckDigit}";

        // Pad to 30 characters if needed (should be exactly 30, but pad for safety)
        line = line.PadRight(MRZ_LINE_LENGTH, '<');

        if (line.Length != MRZ_LINE_LENGTH)
        {
            throw new InvalidOperationException($"Generated MRZ Line 2 has invalid length: {line.Length}, expected {MRZ_LINE_LENGTH}");
        }

        return line;
    }

    public string GenerateTD1Line3(string surname, string givenNames)
    {
        // Use validator to truncate names according to MRZ rules
        _validator.TruncateMrzName(surname ?? string.Empty, givenNames ?? string.Empty, out var truncatedSurname, out var truncatedGivenNames);

        // Format: SURNAME<<GIVENNAMES<<
        var line = $"{truncatedSurname}<<{truncatedGivenNames}<<";

        // Pad to exactly 30 characters
        line = line.PadRight(MRZ_LINE_LENGTH, '<');

        if (line.Length != MRZ_LINE_LENGTH)
        {
            throw new InvalidOperationException($"Generated MRZ Line 3 has invalid length: {line.Length}, expected {MRZ_LINE_LENGTH}");
        }

        return line;
    }

    public int CalculateIcaoCheckDigit(string mrzString)
    {
        if (string.IsNullOrEmpty(mrzString))
        {
            throw new ArgumentException("MRZ string cannot be null or empty.", nameof(mrzString));
        }

        // ICAO Algorithm: Weights 7, 3, 1 repeating from right to left
        var weights = new[] { 7, 3, 1 };
        var sum = 0;

        // Process from right to left
        for (int i = mrzString.Length - 1, weightIndex = 0; i >= 0; i--, weightIndex++)
        {
            var c = mrzString[i];
            var weight = weights[weightIndex % weights.Length];

            // Character values:
            // 0-9: Use numeric value (0-9)
            // A-Z: A=10, B=11, ..., Z=35
            // <: Value = 0
            int charValue;
            if (char.IsDigit(c))
            {
                charValue = c - '0';
            }
            else if (char.IsLetter(c))
            {
                var upper = char.ToUpperInvariant(c);
                charValue = upper - 'A' + 10;
            }
            else if (c == '<')
            {
                charValue = 0;
            }
            else
            {
                throw new ArgumentException($"Invalid character in MRZ string: {c}", nameof(mrzString));
            }

            sum += charValue * weight;
        }

        // Check digit = sum mod 10
        return sum % 10;
    }

    public string SanitizeMrzFields(string input)
    {
        return _validator.SanitizeMrzField(input);
    }

    private static string FormatDateYYMMDD(DateTime date)
    {
        // Format as YYMMDD (2-digit year, 2-digit month, 2-digit day)
        var year = date.Year % 100;
        return $"{year:D2}{date.Month:D2}{date.Day:D2}";
    }
}


