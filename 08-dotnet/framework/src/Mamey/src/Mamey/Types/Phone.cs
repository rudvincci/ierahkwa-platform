using System.Text.Json;

namespace Mamey.Types
{
    [Serializable]
    public class Phone : IEquatable<Phone>
    {
        public enum PhoneType
        {
            Main,
            Home,
            Mobile,
            Fax,
            Other
        }

        public Phone()
        {
            // Id = Guid.NewGuid();
        }

        public Phone(string countryCode, string number)
            : this(countryCode, number, null, PhoneType.Main, false)
        {
        }

        public Phone(string countryCode, string number, string? extension = null, string? type = null, bool isDefault = false)
            : this(countryCode, number, extension, ParsePhoneType(type), isDefault)
        {
        }

        [JsonConstructor]
        public Phone(string countryCode, string number, string? extension = null, PhoneType type = PhoneType.Main, bool isDefault = false)
        {
            // Id = Guid.NewGuid();
            CountryCode = ValidateNonEmpty(countryCode, nameof(countryCode));
            Number = ValidateNonEmpty(number, nameof(number));
            Extension = extension;
            Type = type;
            IsDefault = isDefault;
        }


        [Required] public string CountryCode { get; set; } = string.Empty;

        [Required]
        public string Number { get; set; } = string.Empty;

        public PhoneType Type { get; set; } = PhoneType.Main;

        public string? Extension { get; set; }

        public bool IsDefault { get; private set; }

        /// <summary>
        /// Sets this phone as the default phone and returns the instance.
        /// </summary>
        public Phone SetDefault(bool isDefault)
        {
            IsDefault = isDefault;
            return this;
        }

        /// <summary>
        /// Parses a string to PhoneType. Defaults to PhoneType.Main if null or invalid.
        /// </summary>
        private static PhoneType ParsePhoneType(string? type)
        {
            return Enum.TryParse<PhoneType>(type, ignoreCase: true, out var parsedType) 
                ? parsedType 
                : PhoneType.Main;
        }

        /// <summary>
        /// Validates that a string is not null or empty.
        /// </summary>
        private static string ValidateNonEmpty(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);
            return value;
        }

        public bool Equals(Phone? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return CountryCode == other.CountryCode &&
                   Number == other.Number &&
                   Extension == other.Extension &&
                   Type == other.Type;
        }

        public override bool Equals(object? obj)
        {
            return obj is Phone other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CountryCode, Number, Extension, Type);
        }

        public static bool operator ==(Phone? a, Phone? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(Phone? a, Phone? b) => !(a == b);

        public override string ToString()
        {
            return $"+{CountryCode} {Number}{(string.IsNullOrEmpty(Extension) ? "" : $" Ext. {Extension}")}";
        }

        /// <summary>
        /// Serializes the phone object as JSON.
        /// </summary>
        public string AsJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }
    }
}
