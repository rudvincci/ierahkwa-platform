using Mamey.Exceptions;

namespace Mamey.Types
{
    public class Currency : IEquatable<Currency>
    {
        private static IReadOnlyCollection<string> _allowedValues = new HashSet<string>
        {
            "USD", "EUR", "JPY", "GBP", "PLN"
        };

        /// <summary>
        /// The value of the currency, represented as a 3-character ISO code.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Configures the allowed values for the Currency class.
        /// </summary>
        /// <param name="allowedValues">The collection of allowed currency codes.</param>
        /// <exception cref="ArgumentException">Thrown when the collection is null or empty.</exception>
        public static void ConfigureAllowedValues(IEnumerable<string> allowedValues)
        {
            if (allowedValues == null || !allowedValues.Any())
            {
                throw new ArgumentException("AllowedValues must be a non-empty collection.", nameof(allowedValues));
            }

            _allowedValues = new HashSet<string>(
                allowedValues.Select(v => ValidateCurrencyCode(v, nameof(allowedValues))),
                StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Constructs a Currency instance.
        /// </summary>
        /// <param name="value">The currency code.</param>
        /// <exception cref="InvalidOperationException">Thrown when allowed values are not configured.</exception>
        /// <exception cref="InvalidCurrencyException">Thrown when the value is null, empty, or whitespace.</exception>
        /// <exception cref="UnsupportedCurrencyException">Thrown when the value is not in the allowed values.</exception>
        public Currency(string value)
        {
            if (_allowedValues == null || !_allowedValues.Any())
            {
                throw new InvalidOperationException("AllowedValues must be configured before creating Currency instances.");
            }

            Code = ValidateCurrencyCode(value, nameof(value));

            if (!_allowedValues.Contains(Code))
            {
                throw new UnsupportedCurrencyException(Code);
            }
        }

        /// <summary>
        /// Validates that a currency code is a non-empty 3-character string.
        /// </summary>
        /// <param name="value">The currency code to validate.</param>
        /// <param name="paramName">The name of the parameter being validated.</param>
        /// <returns>The validated currency code.</returns>
        /// <exception cref="InvalidCurrencyException">Thrown if the value is null, empty, or not 3 characters long.</exception>
        private static string ValidateCurrencyCode(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidCurrencyException($"Currency code cannot be null, empty, or whitespace. Parameter: {paramName}", value);
            }

            value = value.ToUpperInvariant();

            if (value.Length != 3)
            {
                throw new InvalidCurrencyException($"Currency code must be exactly 3 characters. Parameter: {paramName}", value);
            }

            return value;
        }

        /// <summary>
        /// Implicitly converts a string to a Currency instance.
        /// </summary>
        public static implicit operator Currency(string value) => new(value);

        /// <summary>
        /// Implicitly converts a Currency instance to its string representation.
        /// </summary>
        public static implicit operator string(Currency value) => value?.Code;

        /// <summary>
        /// Equality operator for Currency.
        /// </summary>
        public static bool operator ==(Currency a, Currency b) => Equals(a, b);

        /// <summary>
        /// Inequality operator for Currency.
        /// </summary>
        public static bool operator !=(Currency a, Currency b) => !Equals(a, b);

        /// <summary>
        /// Checks equality with another Currency instance.
        /// </summary>
        public bool Equals(Currency? other) => other is not null && Code.Equals(other.Code, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Checks equality with another object.
        /// </summary>
        public override bool Equals(object? obj) => obj is Currency other && Equals(other);

        /// <summary>
        /// Returns the hash code for this Currency instance.
        /// </summary>
        public override int GetHashCode() => Code?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0;

        /// <summary>
        /// Returns the string representation of this Currency instance.
        /// </summary>
        public override string ToString() => Code;
    }
}
