using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Mamey.ISO.ISO13616
{
    public static class IbanGenerator
    {
        internal static readonly Dictionary<string, (int Length, string Pattern)> IbanMetadata = GetIbanMetadata;
        private const int Modulus = 97;
        public static string GenerateIban(string countryCode, string bankCode, string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(countryCode) || string.IsNullOrWhiteSpace(bankCode) ||
                string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Country code, bank code, and account number cannot be null or empty.");

            countryCode = countryCode.ToUpper();
            bankCode = bankCode.ToUpper();
            accountNumber = accountNumber.ToUpper();

            if (!IbanMetadata.TryGetValue(countryCode, out var metadata))
                throw new ArgumentException($"Unsupported country code: {countryCode}");

            // Check IBAN length and pattern
            var expectedLength = metadata.Length;
            var ibanPattern = metadata.Pattern;

            // Construct IBAN without checksum
            var ibanWithoutChecksum = $"{countryCode}00{bankCode}{accountNumber}";

            // Validate against country-specific regex
            if (!Regex.IsMatch(ibanWithoutChecksum, ibanPattern.Replace(@"00", ""))) // Temporarily exclude checksum
                throw new ArgumentException($"Invalid IBAN structure for {countryCode}.");

            // Calculate checksum
            var checksum = CalculateChecksum(ibanWithoutChecksum);
            var ibanWithChecksum = $"{countryCode}{checksum:D2}{bankCode}{accountNumber}";

            // Ensure final IBAN matches expected length
            if (ibanWithChecksum.Length != expectedLength)
                throw new ArgumentException(
                    $"Generated IBAN does not match expected length for {countryCode}. Expected: {expectedLength}, Actual: {ibanWithChecksum.Length}");

            return ibanWithChecksum;
        }

        private static int CalculateChecksum(string iban)
        {
            // Move the first four characters to the end
            var reformattedIban = iban.Substring(4) + iban.Substring(0, 4);

            // Convert letters to numbers (A=10, B=11, ..., Z=35)
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
                var remainder = (int)(ibanNumber % Modulus);
                return Modulus - remainder == Modulus ? 0 : Modulus - remainder; // Handle edge case for remainder 0
            }

            throw new InvalidOperationException("Failed to parse numeric IBAN for checksum calculation.");
        }

        private static readonly Dictionary<string, (int Length, string Pattern)> GetIbanMetadata = new()
        {
            { "AL", (28, @"^[A-Z]{2}[0-9]{2}[0-9]{8}[0-9]{16}$") },
            { "AD", (24, @"^[A-Z]{2}[0-9]{2}[0-9]{4}[0-9]{4}[0-9]{12}$") },
            { "AT", (20, @"^[A-Z]{2}[0-9]{2}[0-9]{5}[0-9]{11}$") },
            { "AZ", (28, @"^[A-Z]{2}[0-9]{2}[A-Z0-9]{4}[0-9]{20}$") },
            { "BH", (22, @"^[A-Z]{2}[0-9]{2}[A-Z0-9]{4}[0-9]{14}$") },
            { "BY", (28, @"^[A-Z]{2}[0-9]{2}[A-Z0-9]{4}[0-9]{20}$") },
            { "BE", (16, @"^[A-Z]{2}[0-9]{2}[0-9]{12}$") },
            { "BA", (20, @"^[A-Z]{2}[0-9]{2}[0-9]{6}[0-9]{10}$") },
            { "BR", (29, @"^[A-Z]{2}[0-9]{2}[0-9]{8}[0-9]{17}[A-Z0-9]{1}$") },
            { "BG", (22, @"^[A-Z]{2}[0-9]{2}[A-Z0-9]{4}[0-9]{6}[0-9]{8}$") },
            { "CR", (22, @"^[A-Z]{2}[0-9]{2}[0-9]{18}$") },
            { "CY", (28, @"^[A-Z]{2}[0-9]{2}[0-9]{8}[0-9]{16}$") },
            { "CZ", (24, @"^[A-Z]{2}[0-9]{2}[0-9]{20}$") },
            { "DE", (22, @"^[A-Z]{2}[0-9]{2}[0-9]{18}$") },
            { "DK", (18, @"^[A-Z]{2}[0-9]{2}[0-9]{14}$") },
            { "DO", (28, @"^[A-Z]{2}[0-9]{2}[A-Z0-9]{4}[0-9]{20}$") },
            { "EE", (20, @"^[A-Z]{2}[0-9]{2}[0-9]{18}$") },
            { "ES", (24, @"^[A-Z]{2}[0-9]{2}[0-9]{20}$") },
            { "FI", (18, @"^[A-Z]{2}[0-9]{2}[0-9]{14}$") },
            { "FR", (27, @"^[A-Z]{2}[0-9]{2}[0-9]{10}[0-9]{11}[0-9]{2}$") },
            { "GB", (22, @"^[A-Z]{2}[0-9]{2}[A-Z]{4}[0-9]{14}$") },
            { "GE", (22, @"^[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{16}$") },
            { "GI", (23, @"^[A-Z]{2}[0-9]{2}[A-Z]{4}[0-9]{15}$") },
            { "GR", (27, @"^[A-Z]{2}[0-9]{2}[0-9]{7}[0-9]{16}$") },
            { "HR", (21, @"^[A-Z]{2}[0-9]{2}[0-9]{19}$") },
            { "HU", (28, @"^[A-Z]{2}[0-9]{2}[0-9]{26}$") },
            { "IE", (22, @"^[A-Z]{2}[0-9]{2}[A-Z]{4}[0-9]{14}$") },
            { "IL", (23, @"^[A-Z]{2}[0-9]{2}[0-9]{19}$") },
            { "IT", (27, @"^[A-Z]{2}[0-9]{2}[A-Z]{1}[0-9]{10}[0-9]{12}$") },
            { "JO", (30, @"^[A-Z]{2}[0-9]{2}[A-Z0-9]{4}[0-9]{22}$") },
            { "KW", (30, @"^[A-Z]{2}[0-9]{2}[A-Z]{4}[0-9]{22}$") },
            { "KZ", (20, @"^[A-Z]{2}[0-9]{2}[0-9]{16}$") },
            { "LB", (28, @"^[A-Z]{2}[0-9]{2}[0-9]{22}$") },
            { "LI", (21, @"^[A-Z]{2}[0-9]{2}[0-9]{17}$") },
            { "LT", (20, @"^[A-Z]{2}[0-9]{2}[0-9]{18}$") },
            { "LU", (20, @"^[A-Z]{2}[0-9]{2}[0-9]{16}$") },
            { "LV", (21, @"^[A-Z]{2}[0-9]{2}[A-Z]{4}[0-9]{13}$") },
            { "MC", (27, @"^[A-Z]{2}[0-9]{2}[0-9]{10}[0-9]{11}[0-9]{2}$") },
            { "MD", (24, @"^[A-Z]{2}[0-9]{2}[A-Z0-9]{2}[0-9]{18}$") },
            { "ME", (22, @"^[A-Z]{2}[0-9]{2}[0-9]{18}$") },
            { "MK", (19, @"^[A-Z]{2}[0-9]{2}[0-9]{7}[0-9]{10}$") },
            { "MT", (31, @"^[A-Z]{2}[0-9]{2}[A-Z]{4}[0-9]{27}$") },
            { "NL", (18, @"^[A-Z]{2}[0-9]{2}[A-Z]{4}[0-9]{10}$") },
            { "NO", (15, @"^[A-Z]{2}[0-9]{2}[0-9]{11}$") },
            { "PL", (28, @"^[A-Z]{2}[0-9]{2}[0-9]{24}$") },
            { "PT", (25, @"^[A-Z]{2}[0-9]{2}[0-9]{21}$") },
            { "QA", (29, @"^[A-Z]{2}[0-9]{2}[A-Z]{4}[0-9]{23}$") },
            { "RO", (24, @"^[A-Z]{2}[0-9]{2}[A-Z]{4}[0-9]{16}$") },
            { "RS", (22, @"^[A-Z]{2}[0-9]{2}[0-9]{18}$") },
            { "SA", (24, @"^[A-Z]{2}[0-9]{2}[0-9]{20}$") },
            { "SE", (24, @"^[A-Z]{2}[0-9]{2}[0-9]{22}$") },
            { "SI", (19, @"^[A-Z]{2}[0-9]{2}[0-9]{15}$") },
            { "SK", (24, @"^[A-Z]{2}[0-9]{2}[0-9]{20}$") },
            { "SM", (27, @"^[A-Z]{2}[0-9]{2}[A-Z]{1}[0-9]{10}[0-9]{12}$") },
            { "TR", (26, @"^[A-Z]{2}[0-9]{2}[0-9]{24}$") },
            { "VG", (24, @"^[A-Z]{2}[0-9]{2}[A-Z0-9]{4}[0-9]{16}$") },
            // Add more countries if needed.
        };
    }
}