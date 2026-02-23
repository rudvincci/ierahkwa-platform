using System.Text;
using System.Text.RegularExpressions;

namespace Mamey.Government.Shared.Services.Standards;

/// <summary>
/// Validates compliance with ICAO 9303 and AAMVA standards for travel documents.
/// </summary>
public sealed class StandardsComplianceValidator : IStandardsComplianceValidator
{
    private const int MRZ_NAME_MAX_LENGTH = 30; // TD1 format
    private const int MRZ_SEPARATOR_LENGTH = 2; // << between surname and given names

    public string SanitizeMrzField(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(input.Length);
        
        foreach (var c in input.ToUpperInvariant())
        {
            if (char.IsLetterOrDigit(c))
            {
                // Only A-Z and 0-9 are valid
                if ((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                }
                else
                {
                    // Convert accented characters to base letters
                    sb.Append(RemoveDiacritics(c));
                }
            }
            else if (c == ' ' || c == '-' || c == '\'')
            {
                // Spaces, hyphens, and apostrophes become <
                sb.Append('<');
            }
            // Other characters are ignored
        }

        return sb.ToString();
    }

    public void TruncateMrzName(string surname, string givenNames, out string truncatedSurname, out string truncatedGivenNames)
    {
        // Sanitize both names
        var cleanSurname = SanitizeMrzField(surname);
        var cleanGivenNames = SanitizeMrzField(givenNames);

        // Calculate available space: 30 - 2 (for <<) = 28 characters for names
        var availableSpace = MRZ_NAME_MAX_LENGTH - MRZ_SEPARATOR_LENGTH;

        // Combined length check
        var totalLength = cleanSurname.Length + cleanGivenNames.Length;

        if (totalLength <= availableSpace)
        {
            // Both names fit
            truncatedSurname = cleanSurname;
            truncatedGivenNames = cleanGivenNames;
            return;
        }

        // Need to truncate - prioritize surname, then truncate given names
        // Minimum surname length: 1 character
        var minSurnameLength = Math.Min(cleanSurname.Length, 15);
        var maxGivenNamesLength = availableSpace - minSurnameLength;

        if (cleanGivenNames.Length > maxGivenNamesLength)
        {
            truncatedSurname = cleanSurname.Substring(0, minSurnameLength);
            truncatedGivenNames = cleanGivenNames.Substring(0, maxGivenNamesLength);
        }
        else
        {
            // Given names fit, truncate surname
            var remainingForSurname = availableSpace - cleanGivenNames.Length;
            truncatedSurname = cleanSurname.Length > remainingForSurname 
                ? cleanSurname.Substring(0, remainingForSurname) 
                : cleanSurname;
            truncatedGivenNames = cleanGivenNames;
        }
    }

    public bool ValidatePassportNumber(string passportNumber)
    {
        if (string.IsNullOrEmpty(passportNumber))
        {
            return false;
        }

        // Passport number format: [A-Z][0-9]{2}[A-Z0-9]{5}[0-9] (9 characters)
        return Regex.IsMatch(passportNumber, @"^[A-Z][0-9]{2}[A-Z0-9]{5}[0-9]$");
    }

    public bool ValidateMrzLine(string line, int expectedLength)
    {
        if (string.IsNullOrEmpty(line))
        {
            return false;
        }

        if (line.Length != expectedLength)
        {
            return false;
        }

        // MRZ lines can only contain A-Z, 0-9, and <
        return Regex.IsMatch(line, @"^[A-Z0-9<]+$");
    }

    private static char RemoveDiacritics(char c)
    {
        // Map common accented characters to their base letters
        return c switch
        {
            'À' or 'Á' or 'Â' or 'Ã' or 'Ä' or 'Å' => 'A',
            'Ç' => 'C',
            'È' or 'É' or 'Ê' or 'Ë' => 'E',
            'Ì' or 'Í' or 'Î' or 'Ï' => 'I',
            'Ñ' => 'N',
            'Ò' or 'Ó' or 'Ô' or 'Õ' or 'Ö' => 'O',
            'Ù' or 'Ú' or 'Û' or 'Ü' => 'U',
            'Ý' => 'Y',
            'Ÿ' => 'Y',
            'Æ' => 'A',
            'Ø' => 'O',
            'ß' => 'S',
            _ => c >= 'A' && c <= 'Z' ? c : 'X' // Default for unhandled characters
        };
    }
}
