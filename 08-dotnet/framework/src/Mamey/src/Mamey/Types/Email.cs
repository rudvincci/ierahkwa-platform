using System.Text.RegularExpressions;
using Mamey.Exceptions;

namespace Mamey.Types
{
    public class Email : IEquatable<Email>
    {
        private const int MaxEmailLength = 100;

        // Simplified and readable regex pattern for email validation.
        private static readonly Regex EmailRegex = new(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Email()
        {
            
        }
        /// <summary>
        /// Constructs an Email instance.
        /// </summary>
        /// <param name="value">The email address.</param>
        /// <exception cref="InvalidEmailException">Thrown when the email is null, exceeds length, or is invalid.</exception>
        public Email(string value)
        {
            Value = Validate(value);
        }

        /// <summary>
        /// The validated email value.
        /// </summary>
        public string Value { get; }

        

        /// <summary>
        /// Validates an email address.
        /// </summary>
        /// <param name="value">The email to validate.</param>
        /// <returns>The validated email address in lowercase.</returns>
        /// <exception cref="InvalidEmailException">Thrown for invalid email addresses.</exception>
        private static string Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidEmailException("Email cannot be null, empty, or whitespace.");
            }

            if (value.Length > MaxEmailLength)
            {
                throw new InvalidEmailException($"Email exceeds the maximum allowed length of {MaxEmailLength} characters.");
            }

            value = value.ToLowerInvariant();

            if (!EmailRegex.IsMatch(value))
            {
                throw new InvalidEmailException($"The provided email '{value}' is invalid.");
            }

            return value;
        }

        /// <summary>
        /// Checks if a given email is valid.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        /// <returns>True if the email is valid, false otherwise.</returns>
        public static bool IsValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email), "Email cannot be null or empty.");
            }

            return EmailRegex.IsMatch(email);
        }

        /// <summary>
        /// Implicitly converts an Email to its string representation.
        /// </summary>
        public static implicit operator string(Email email) => email.Value;

        /// <summary>
        /// Implicitly converts a string to an Email instance.
        /// </summary>
        public static implicit operator Email(string email) => new(email);

        /// <summary>
        /// Checks equality with another Email instance.
        /// </summary>
        public bool Equals(Email? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks equality with another object.
        /// </summary>
        public override bool Equals(object? obj)
        {
            return obj is Email other && Equals(other);
        }

        /// <summary>
        /// Returns the hash code for the email address.
        /// </summary>
        public override int GetHashCode() => Value?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0;

        /// <summary>
        /// Returns the email address as a string.
        /// </summary>
        public override string ToString() => Value;
    }
}
