namespace Mamey.Types;

public class Address : IEquatable<Address>
{
    public Address() { }

    [JsonConstructor]
    public Address(string firmName, string line, string? line2, string? line3,
        string? urbanization, string city, string state, string zip5,
        string? zip4, string? postalCode, string country, string? province,
        bool isDefault = false, AddressType type = AddressType.Main)
    {
        try
        {
            Country = ValidateNonEmpty(country, nameof(country));
            FirmName = firmName;
            Line = ValidateNonEmpty(line, nameof(line));
            Line2 = line2;
            Line3 = line3;
            Urbanization = urbanization;

            if (IsUSAddress)
            {
                State = ValidateNonEmpty(state, nameof(state));
                Zip5 = ValidateZip(zip5, nameof(zip5));
                Zip4 = zip4;
            }
            else
            {
                State = state;
                PostalCode = ValidateNonEmpty(postalCode ?? string.Empty, nameof(postalCode));
                Province = ValidateNonEmpty(province ?? string.Empty, nameof(province));
            }

            City = ValidateNonEmpty(city, nameof(city));
            IsDefault = isDefault;
            Type = type;
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Error initializing Address: {ex.Message}", ex);
        }
    }

    public Address(string line, string city, string state, string zip5, string country, AddressType type) 
        : this(null, line, null, null, null, city, state, zip5, null, null, country, null)
    {
    }

    [Key]
    public Guid Id { get; } = Guid.NewGuid();

    public string Street => $"{Line}{(!string.IsNullOrWhiteSpace(Line2) ? $" {Line2}" : "")}";

    public string FirmName { get; set; } = string.Empty;

    public string Line { get; set; } = string.Empty;

    public string? Line2 { get; set; }

    public string? Line3 { get; set; }

    public string? Urbanization { get; set; }

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    [MaxLength(5)]
    public string Zip5 { get; set; } = string.Empty;

    [MaxLength(4)]
    public string? Zip4 { get; set; }

    public string? PostalCode { get; set; }

    public string Country { get; set; } = string.Empty;

    public string? Province { get; set; }

    public AddressType Type { get; set; }

    public bool IsDefault { get; private set; }

    public bool IsUSAddress => Country.Equals("US", StringComparison.OrdinalIgnoreCase);

    public void SetDefault(bool value) => IsDefault = value;

    /// <summary>
    /// Validate a non-empty string value.
    /// </summary>
    private static string ValidateNonEmpty(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);
        return value.Trim();
    }

    /// <summary>
    /// Validate a ZIP code.
    /// </summary>
    private static string ValidateZip(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != 5 || !int.TryParse(value, out _))
            throw new ArgumentException($"{paramName} must be a 5-digit number.", paramName);
        return value;
    }

    /// <summary>
    /// Print address in a simple format.
    /// </summary>
    public string ToSimpleFormat()
    {
        return $"{Line} {Line2}, {City}, {(IsUSAddress ? State : Province)}, {Country} {(IsUSAddress ? ZipCode : PostalCode)}";
    }

    public (string, string) ToTwoLineFormat
        => ($"{Line} {Line2}",
            $"{City}, {(IsUSAddress ? State : Province)}, {Country} {(IsUSAddress ? ZipCode : PostalCode)}");

    /// <summary>
    /// Print address in a detailed format.
    /// </summary>
    public string ToDetailedFormat()
    {
        return $"Firm Name: {FirmName}\n" +
               $"Address Line 1: {Line}\n" +
               $"Address Line 2: {Line2 ?? "N/A"}\n" +
               $"City: {City}\n" +
               $"State/Province: {(IsUSAddress ? State : Province)}\n" +
               $"ZIP/Postal Code: {(IsUSAddress ? ZipCode : PostalCode)}\n" +
               $"Country: {Country}";
    }

    /// <summary>
    /// Print address in a label format.
    /// </summary>
    public string ToLabelFormat()
    {
        return $"{FirmName}\n" +
               $"{Line} {Line2}\n" +
               $"{City}, {(IsUSAddress ? State : Province)} {(IsUSAddress ? ZipCode : PostalCode)}\n" +
               $"{Country}";
    }

    /// <summary>
    /// Default ToString implementation.
    /// </summary>
    public override string ToString()
    {
        return ToSimpleFormat();
    }

    public override bool Equals(object? obj)
    {
        return obj is Address address && Equals(address);
    }

    public bool Equals(Address? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return FirmName == other.FirmName &&
               Line == other.Line &&
               Line2 == other.Line2 &&
               Line3 == other.Line3 &&
               Urbanization == other.Urbanization &&
               City == other.City &&
               State == other.State &&
               Zip5 == other.Zip5 &&
               Zip4 == other.Zip4 &&
               PostalCode == other.PostalCode &&
               Country == other.Country &&
               Province == other.Province &&
               Type == other.Type &&
               IsDefault == other.IsDefault;
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(FirmName);
        hash.Add(Line);
        hash.Add(Line2);
        hash.Add(Line3);
        hash.Add(Urbanization);
        hash.Add(City);
        hash.Add(State);
        hash.Add(Zip5);
        hash.Add(Zip4);
        hash.Add(PostalCode);
        hash.Add(Country);
        hash.Add(Province);
        hash.Add(Type);

        return hash.ToHashCode();
    }

    public static bool operator ==(Address? a, Address? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    public static bool operator !=(Address? a, Address? b) => !(a == b);
    
    public string ZipCode => !string.IsNullOrEmpty(Zip5)
        ? $"{Zip5}{(string.IsNullOrEmpty(Zip4) ? "" : $"-{Zip4}")}"
        : string.Empty;

    public enum AddressType
    {
        Main,
        Business,
        Home
    }
}