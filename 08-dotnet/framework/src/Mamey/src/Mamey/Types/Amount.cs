using System.Globalization;
using Mamey.Exceptions;

namespace Mamey.Types
{
    public class Amount : IEquatable<Amount>
    {
        /// <summary>
        /// The numeric value of the amount.
        /// </summary>
        public decimal Value { get; }

        /// <summary>
        /// Constructs an Amount with a specified value, precision, and optional maximum amount.
        /// </summary>
        /// <param name="value">The monetary value.</param>
        /// <param name="decimalPlaces">The allowed decimal precision.</param>
        /// <param name="maxAmount">The optional maximum value. Defaults to <see cref="decimal.MaxValue"/>.</param>
        /// <exception cref="InvalidAmountException">Thrown if value exceeds precision or range constraints.</exception>
        public Amount(decimal value, int decimalPlaces = 2, decimal? maxAmount = null)
        {
            if (decimalPlaces < 0 || decimalPlaces > 28) // Decimal max precision is 28
            {
                throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Decimal places must be between 0 and 28.");
            }

            decimal roundedValue = decimal.Round(value, decimalPlaces);

            if (roundedValue != value)
            {
                throw new InvalidAmountException($"Amount exceeds allowed precision of {decimalPlaces} decimal places.", value);
            }

            decimal effectiveMaxAmount = maxAmount ?? decimal.MaxValue;

            if (roundedValue < 0 || roundedValue > effectiveMaxAmount)
            {
                throw new InvalidAmountException($"Amount must be between 0 and {effectiveMaxAmount}.", value);
            }

            Value = roundedValue;
        }

        /// <summary>
        /// Represents a zero amount.
        /// </summary>
        public static Amount Zero => new(0);

        /// <summary>
        /// Implicitly converts a decimal to an Amount.
        /// </summary>
        /// <param name="value">The decimal value.</param>
        public static implicit operator Amount(decimal value) => new(value);

        /// <summary>
        /// Implicitly converts an Amount to a decimal.
        /// </summary>
        /// <param name="value">The Amount instance.</param>
        public static implicit operator decimal(Amount value) => value.Value;

        /// <summary>
        /// Equality operator for Amount.
        /// </summary>
        public static bool operator ==(Amount a, Amount b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Value == b.Value;
        }

        /// <summary>
        /// Inequality operator for Amount.
        /// </summary>
        public static bool operator !=(Amount a, Amount b) => !(a == b);

        /// <summary>
        /// Greater than operator for Amount.
        /// </summary>
        public static bool operator >(Amount a, Amount b) => a.Value > b.Value;

        /// <summary>
        /// Less than operator for Amount.
        /// </summary>
        public static bool operator <(Amount a, Amount b) => a.Value < b.Value;

        /// <summary>
        /// Greater than or equal to operator for Amount.
        /// </summary>
        public static bool operator >=(Amount a, Amount b) => a.Value >= b.Value;

        /// <summary>
        /// Less than or equal to operator for Amount.
        /// </summary>
        public static bool operator <=(Amount a, Amount b) => a.Value <= b.Value;

        /// <summary>
        /// Addition operator for Amount.
        /// </summary>
        public static Amount operator +(Amount a, Amount b) => new(a.Value + b.Value);

        /// <summary>
        /// Subtraction operator for Amount.
        /// </summary>
        public static Amount operator -(Amount a, Amount b) => new(a.Value - b.Value);

        /// <summary>
        /// Checks equality with another Amount.
        /// </summary>
        public bool Equals(Amount? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        /// <summary>
        /// Checks equality with another object.
        /// </summary>
        public override bool Equals(object? obj)
        {
            return obj is Amount other && Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Converts the amount to its string representation.
        /// </summary>
        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
    }
}
