namespace Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;

/// <summary>
/// Passport number value object (e.g., P12345678)
/// </summary>
public class PassportNumber
{
    public string Value { get; }

    public PassportNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Passport number cannot be empty", nameof(value));
        
        Value = value;
    }

    public static implicit operator string(PassportNumber number) => number.Value;
    public static implicit operator PassportNumber(string value) => new(value);

    public override string ToString() => Value;
    public override bool Equals(object? obj) => obj is PassportNumber other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}
