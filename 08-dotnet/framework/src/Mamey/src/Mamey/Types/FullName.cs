using Mamey.Exceptions;

namespace Mamey.Types
{
    public class FullName : IEquatable<FullName>
    {
        private const int DefaultMaxLength = 200;
        private const int MinLength = 2;

        /// <summary>
        /// The validated full name value.
        /// </summary>
        public string Value { get; }

        public FullName()
        {
            Value = string.Empty;
        }
        /// <summary>
        /// Constructs a FullName instance with a configurable maximum length.
        /// </summary>
        /// <param name="value">The full name.</param>
        /// <param name="maxLength">The maximum allowed length for the full name. Defaults to 200.</param>
        /// <exception cref="InvalidFullNameException">
        /// Thrown when the name is null, too short, or exceeds the maximum length.
        /// </exception>
        public FullName(string value, int maxLength = DefaultMaxLength)
        {
            if (maxLength < MinLength)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength), $"Maximum length must be at least {MinLength} characters.");
            }

            Value = Validate(value, maxLength);
        }

        /// <summary>
        /// Validates the full name.
        /// </summary>
        /// <param name="value">The full name to validate.</param>
        /// <param name="maxLength">The maximum allowed length for the full name.</param>
        /// <returns>The validated full name.</returns>
        /// <exception cref="InvalidFullNameException">Thrown for invalid names.</exception>
        private static string Validate(string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidFullNameException("Full name cannot be null, empty, or whitespace.");
            }

            if (value.Length < MinLength)
            {
                throw new InvalidFullNameException($"Full name must be at least {MinLength} characters.");
            }

            if (value.Length > maxLength)
            {
                throw new InvalidFullNameException($"Full name exceeds the maximum allowed length of {maxLength} characters.");
            }

            return value;
        }

        /// <summary>
        /// Implicitly converts a string to a FullName instance.
        /// </summary>
        public static implicit operator FullName(string value) => value is null ? null : new FullName(value);

        /// <summary>
        /// Implicitly converts a FullName instance to its string representation.
        /// </summary>
        public static implicit operator string(FullName value) => value?.Value;

        /// <summary>
        /// Checks equality with another FullName instance.
        /// </summary>
        public bool Equals(FullName? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value.Equals(other.Value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Checks equality with another object.
        /// </summary>
        public override bool Equals(object? obj)
        {
            return obj is FullName other && Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this FullName instance.
        /// </summary>
        public override int GetHashCode() => Value?.GetHashCode(StringComparison.Ordinal) ?? 0;

        /// <summary>
        /// Returns the full name as a string.
        /// </summary>
        public override string ToString() => Value;
    }
}
