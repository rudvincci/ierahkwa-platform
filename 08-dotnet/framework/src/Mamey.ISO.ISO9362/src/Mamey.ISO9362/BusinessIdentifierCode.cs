using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Mamey.ISO.ISO9362
{
    /// <summary>
    /// Represents a Business Identifier Code (BIC), also known as a SWIFT code.
    /// A BIC consists of:
    /// - 4 characters: Business Party Prefix (institution code)
    /// - 2 characters: Country Code (ISO3166-1 alpha-2)
    /// - 2 characters: Location Code (suffix)
    /// - 3 characters (optional): Branch Code
    /// The total length can be either 8 or 11 characters.
    /// </summary>
    public class BusinessIdentifierCode
    {
        private const string BicPattern = @"^[A-Z]{4}[A-Z]{2}[A-Z0-9]{2}([A-Z0-9]{3})?$";

        public BusinessIdentifierCode(string businessPartyPrefix, string countryCode, string suffix, string? branchCode = null)
        {
            if (string.IsNullOrWhiteSpace(businessPartyPrefix) || businessPartyPrefix.Length != 4)
                throw new ArgumentException("BusinessPartyPrefix must be 4 uppercase letters.", nameof(businessPartyPrefix));

            if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Length != 2)
                throw new ArgumentException("CountryCode must be 2 uppercase letters.", nameof(countryCode));

            if (string.IsNullOrWhiteSpace(suffix) || suffix.Length != 2)
                throw new ArgumentException("Suffix must be 2 uppercase letters or digits.", nameof(suffix));

            if (branchCode != null && branchCode.Length != 3)
                throw new ArgumentException("BranchCode must be 3 uppercase letters or digits, or null.", nameof(branchCode));

            BusinessPartyPrefix = businessPartyPrefix.ToUpper();
            CountryCode = countryCode.ToUpper();
            Suffix = suffix.ToUpper();
            BranchCode = branchCode?.ToUpper();
        }

        [Required, MinLength(4), MaxLength(4)]
        public string BusinessPartyPrefix { get; }

        [Required, MinLength(2), MaxLength(2)]
        public string CountryCode { get; }

        [Required, MinLength(2), MaxLength(2)]
        public string Suffix { get; }

        [MinLength(3), MaxLength(3)]
        public string? BranchCode { get; }

        /// <summary>
        /// Returns the full BIC string.
        /// </summary>
        public override string ToString() => $"{BusinessPartyPrefix}{CountryCode}{Suffix}{BranchCode ?? string.Empty}";

        /// <summary>
        /// Validates the BIC format using the standard ISO9362 pattern.
        /// </summary>
        public static bool IsValid(string bic)
        {
            return !string.IsNullOrWhiteSpace(bic) && Regex.IsMatch(bic, BicPattern);
        }
    }

    /// <summary>
    /// Extensions for parsing and validation of BIC strings.
    /// </summary>
    public static class BusinessIdentifierCodeExtensions
    {
        /// <summary>
        /// Parses a string into a BusinessIdentifierCode object.
        /// </summary>
        /// <param name="bic">The full BIC string to parse.</param>
        /// <returns>A <see cref="BusinessIdentifierCode"/> object.</returns>
        public static BusinessIdentifierCode Parse(this string bic)
        {
            if (string.IsNullOrWhiteSpace(bic))
                throw new ArgumentException("BIC string cannot be null or empty.", nameof(bic));

            if (!BusinessIdentifierCode.IsValid(bic))
                throw new FormatException("Invalid BIC format.");

            var businessPartyPrefix = bic.Substring(0, 4);
            var countryCode = bic.Substring(4, 2);
            var suffix = bic.Substring(6, 2);
            var branchCode = bic.Length > 8 ? bic.Substring(8, 3) : null;

            return new BusinessIdentifierCode(businessPartyPrefix, countryCode, suffix, branchCode);
        }
    }
}
