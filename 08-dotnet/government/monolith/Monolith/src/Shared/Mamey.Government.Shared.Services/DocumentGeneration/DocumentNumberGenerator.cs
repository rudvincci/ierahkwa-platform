using System.Security.Cryptography;
using System.Text;

namespace Mamey.Government.Shared.Services.DocumentGeneration;

/// <summary>
/// Generates various document numbers (passport, travel identity, etc.)
/// </summary>
public sealed class DocumentNumberGenerator : IDocumentNumberGenerator
{
    private const string ALPHANUMERIC_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const string LETTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const int PASSPORT_NUMBER_LENGTH = 9;
    private const int PASSPORT_SEQUENCE_LENGTH = 5;
    private const int TRAVEL_ID_NUMBER_LENGTH = 8;

    public string GeneratePassportNumber(string lastName, DateTime birthDate)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name is required.", nameof(lastName));
        }

        // Extract first letter of last name (uppercase)
        var firstLetter = char.ToUpperInvariant(lastName.Trim()[0]);
        if (!char.IsLetter(firstLetter))
        {
            throw new ArgumentException("Last name must start with a letter.", nameof(lastName));
        }

        // Extract last 2 digits of birth year (YY)
        var birthYear = birthDate.Year % 100;
        var yearDigits = birthYear.ToString("D2");

        // Generate 5-character alphanumeric sequence
        var sequence = GenerateRandomAlphanumeric(PASSPORT_SEQUENCE_LENGTH);

        // Combine: [Letter][YY][5-char-sequence] = 8 characters
        var withoutCheckDigit = $"{firstLetter}{yearDigits}{sequence}";

        // Calculate Luhn check digit
        var checkDigit = CalculateLuhnCheckDigit(withoutCheckDigit);

        // Replace last character with check digit
        var result = withoutCheckDigit.Substring(0, withoutCheckDigit.Length - 1) + checkDigit;

        // Validate format: [A-Z][0-9]{2}[A-Z0-9]{5}
        if (result.Length != PASSPORT_NUMBER_LENGTH)
        {
            throw new InvalidOperationException($"Generated passport number has invalid length: {result.Length}");
        }

        return result;
    }

    public string GenerateTravelIdentityNumber(string lastName, DateTime birthDate)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name is required.", nameof(lastName));
        }

        // Extract first letter of last name (uppercase)
        var firstLetter = char.ToUpperInvariant(lastName.Trim()[0]);
        if (!char.IsLetter(firstLetter))
        {
            throw new ArgumentException("Last name must start with a letter.", nameof(lastName));
        }

        // Extract last 2 digits of birth year (YY)
        var birthYear = birthDate.Year % 100;
        var yearDigits = birthYear.ToString("D2");

        // Generate 4-character alphanumeric sequence
        var sequence = GenerateRandomAlphanumeric(4);

        // Combine: [Letter][YY][4-char-sequence] = 7 characters
        var withoutCheckDigit = $"{firstLetter}{yearDigits}{sequence}";

        // Calculate Luhn check digit
        var checkDigit = CalculateLuhnCheckDigit(withoutCheckDigit);

        // Append check digit
        var result = $"{withoutCheckDigit}{checkDigit}";

        if (result.Length != TRAVEL_ID_NUMBER_LENGTH)
        {
            throw new InvalidOperationException($"Generated travel ID number has invalid length: {result.Length}");
        }

        return result;
    }

    public string GenerateMedicalCardNumber()
    {
        // Generate 4 uppercase letters
        var prefix = GenerateRandomLetters(4);

        // Generate alphanumeric sequence (6 chars)
        var sequence = GenerateRandomAlphanumeric(6);

        return $"{prefix}-{sequence}";
    }

    public string RemoveDocumentPrefixes(string documentNumber)
    {
        if (string.IsNullOrEmpty(documentNumber))
        {
            return documentNumber;
        }

        // Remove common prefixes
        var prefixes = new[] { "INKMIA-BC-", "INKMIA-CA-", "INKMIA-CPRO-", "INKMIA-CRES-", "GOV-PP-", "GOV-TI-" };
        var result = documentNumber;

        foreach (var prefix in prefixes)
        {
            if (result.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                result = result.Substring(prefix.Length);
                break;
            }
        }

        // Remove any remaining hyphens
        result = result.Replace("-", string.Empty);

        return result;
    }

    public int CalculateLuhnCheckDigit(string numberString)
    {
        if (string.IsNullOrEmpty(numberString))
        {
            throw new ArgumentException("Number string cannot be null or empty.", nameof(numberString));
        }

        // Convert alphanumeric string to numeric values
        // Letters A-Z: A=10, B=11, ..., Z=35
        // Numbers 0-9: Use numeric value
        var digits = new List<int>();
        foreach (var c in numberString)
        {
            if (char.IsDigit(c))
            {
                digits.Add(c - '0');
            }
            else if (char.IsLetter(c))
            {
                var upper = char.ToUpperInvariant(c);
                digits.Add(upper - 'A' + 10);
            }
            else
            {
                throw new ArgumentException($"Invalid character in number string: {c}", nameof(numberString));
            }
        }

        // Luhn algorithm: Starting from right, double every second digit
        var sum = 0;
        var shouldDouble = false;

        // Process from right to left
        for (int i = digits.Count - 1; i >= 0; i--)
        {
            var digit = digits[i];

            if (shouldDouble)
            {
                digit *= 2;
                // If doubling results in two-digit number, subtract 9
                if (digit > 9)
                {
                    digit -= 9;
                }
            }

            sum += digit;
            shouldDouble = !shouldDouble;
        }

        // Check digit = (10 - (sum mod 10)) mod 10
        var checkDigit = (10 - (sum % 10)) % 10;
        return checkDigit;
    }

    private static string GenerateRandomAlphanumeric(int length)
    {
        var sb = new StringBuilder(length);
        using (var rng = RandomNumberGenerator.Create())
        {
            var bytes = new byte[length];
            rng.GetBytes(bytes);

            for (int i = 0; i < length; i++)
            {
                var index = bytes[i] % ALPHANUMERIC_CHARS.Length;
                sb.Append(ALPHANUMERIC_CHARS[index]);
            }
        }

        return sb.ToString();
    }

    private static string GenerateRandomLetters(int length)
    {
        var sb = new StringBuilder(length);
        using (var rng = RandomNumberGenerator.Create())
        {
            var bytes = new byte[length];
            rng.GetBytes(bytes);

            for (int i = 0; i < length; i++)
            {
                var index = bytes[i] % LETTERS.Length;
                sb.Append(LETTERS[index]);
            }
        }

        return sb.ToString();
    }
}
