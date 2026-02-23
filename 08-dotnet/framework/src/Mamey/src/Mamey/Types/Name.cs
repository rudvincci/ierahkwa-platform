using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Mamey.Types
{
    /// <summary>
    /// Represents a person's name, including first, middle, last, and nickname.
    /// </summary>
    [Serializable]
    public class Name : IEquatable<Name>, ICloneable, IComparable<Name>//, IEnumerable<string>
    {
        /// <summary>
        /// Default JSON serialization options.
        /// </summary>
        public static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = {
                    new JsonStringEnumConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                },
            WriteIndented = true,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            IncludeFields = true,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { JsonExtensions.AddNestedDerivedTypes }
            }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Name"/> class with default values.
        /// </summary>
        public Name()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Name"/> class with the specified name parts.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="middleName">The middle name.</param>
        /// <param name="nickname">The nickname.</param>
        [JsonConstructor]
        public Name(string firstName, string lastName, string? middleName = null, string? nickname = null)
        {
            ValidateName(firstName, nameof(FirstName));
            ValidateName(lastName, nameof(LastName));

            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            Nickname = nickname;
        }

        [Required] public string FirstName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }= string.Empty;

        public string? Nickname { get; set; }

        /// <summary>
        /// Gets the full name in "First Middle Last" format.
        /// </summary>
        public virtual string FullName => $"{FirstName} {(string.IsNullOrEmpty(MiddleName) ? "" : $"{MiddleName} ")}{LastName}";

        /// <summary>
        /// Gets the given names (first and middle names).
        /// </summary>
        public virtual string GivenNames => $"{FirstName}{(string.IsNullOrEmpty(MiddleName) ? "" : $" {MiddleName}")}";

        /// <summary>
        /// Gets the short form of the full name in "First Last" format.
        /// </summary>
        public virtual string ShortFullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Gets the initials of the name in "F.M.L." format.
        /// </summary>
        public virtual string Initials => $"{FirstName[0]}.{(string.IsNullOrEmpty(MiddleName) ? "" : $"{MiddleName[0]}.")}{LastName[0]}.";

        public bool Equals(Name? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return FirstName == other.FirstName &&
                   MiddleName == other.MiddleName &&
                   LastName == other.LastName &&
                   Nickname == other.Nickname;
        }

        public override bool Equals(object? obj)
        {
            return obj is Name name && Equals(name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (FirstName != null ? FirstName.GetHashCode() : 0);
                hash = hash * 23 + (MiddleName != null ? MiddleName.GetHashCode() : 0);
                hash = hash * 23 + (LastName != null ? LastName.GetHashCode() : 0);
                hash = hash * 23 + (Nickname != null ? Nickname.GetHashCode() : 0);
                return hash;
            }
        }

        public static bool operator ==(Name? a, Name? b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(Name? a, Name? b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return FullName;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this, SerializerOptions);
        }

        public object Clone()
        {
            return new Name(FirstName, LastName, MiddleName, Nickname);
        }

        public int CompareTo(Name? other)
        {
            if (other == null) return 1;
            int lastNameComparison = string.Compare(LastName, other.LastName, StringComparison.Ordinal);
            if (lastNameComparison != 0) return lastNameComparison;

            int firstNameComparison = string.Compare(FirstName, other.FirstName, StringComparison.Ordinal);
            if (firstNameComparison != 0) return firstNameComparison;

            return string.Compare(MiddleName, other.MiddleName, StringComparison.Ordinal);
        }

        private static void ValidateName(string name, string paramName)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{paramName} is required.", paramName);
        }

        private static void AddNestedDerivedTypes(JsonTypeInfo jsonTypeInfo)
        {
            // Implementation of AddNestedDerivedTypes to handle specific customizations.
        }

        ///// <summary>
        ///// Returns an enumerator that iterates through the name components (FirstName, MiddleName, LastName, and Nickname if available).
        ///// </summary>
        ///// <returns>An enumerator that can be used to iterate through the name parts.</returns>
        //public IEnumerator<string> GetEnumerator()
        //{
        //    yield return FirstName;
        //    if (!string.IsNullOrEmpty(MiddleName)) yield return MiddleName!;
        //    yield return LastName;
        //    if (!string.IsNullOrEmpty(Nickname)) yield return Nickname!;
        //}

        ///// <summary>
        ///// Returns a non-generic enumerator that iterates through the name components.
        ///// </summary>
        ///// <returns>A non-generic enumerator that can be used to iterate through the name parts.</returns>
        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}
    }
}
