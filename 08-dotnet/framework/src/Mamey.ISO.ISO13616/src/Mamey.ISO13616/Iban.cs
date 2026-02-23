using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Mamey.ISO.ISO13616;

/// <summary>
/// GB82 WEST 1234 5698 7654 32
/// </summary>
public class Iban
{
    private Iban() => Value = string.Empty;
    public Iban(string iban)
    {
        ValidateIBAN(iban);
        Value = iban;
    }
    private const int Modulus = 97;
    
    public string Value { get; }

    public static bool ValidateIBAN(string iban)
    {
        if (string.IsNullOrWhiteSpace(iban)) return false;

        // Remove spaces and convert to uppercase
        iban = iban.Replace(" ", "").ToUpper();

        // Validate country-specific length and format
        if (!ValidateCountrySpecificLengthAndFormat(iban)) return false;

        // Perform checksum validation
        return PerformChecksumValidation(iban);
    }
    
    public static implicit operator Iban(string value) => new(value);
        
    public static implicit operator string(Iban iban) => iban.Value;

    private static bool ValidateCountrySpecificLengthAndFormat(string iban)
    {
        var countryCode = iban.Substring(0, 2);
        if (!IbanGenerator.IbanMetadata.TryGetValue(countryCode, out var metadata)) return false;

        if (iban.Length != metadata.Length) return false;

        if (!Regex.IsMatch(iban, metadata.Pattern)) return false;

        return true;
    }

    private static bool PerformChecksumValidation(string iban)
    {
        // Move the first four characters to the end
        var reformattedIban = iban.Substring(4) + iban.Substring(0, 4);

        // Convert letters to numbers
        var numericIban = new StringBuilder();
        foreach (var ch in reformattedIban)
        {
            if (char.IsLetter(ch))
            {
                numericIban.Append((ch - 55).ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                numericIban.Append(ch);
            }
        }

        // Use BigInteger for large number modulus operation
        if (BigInteger.TryParse(numericIban.ToString(), out var ibanNumber))
        {
            return ibanNumber % Modulus == 1;
        }

        return false;
    }

    
}