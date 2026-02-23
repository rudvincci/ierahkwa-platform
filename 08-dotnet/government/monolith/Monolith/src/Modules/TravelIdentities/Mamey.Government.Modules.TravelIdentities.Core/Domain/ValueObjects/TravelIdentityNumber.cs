namespace Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;

/// <summary>
/// Travel Identity number value object (AAMVA-compliant format)
/// </summary>
public class TravelIdentityNumber
{
    public string Value { get; }

    public TravelIdentityNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Travel Identity number cannot be empty", nameof(value));
        
        Value = value;
    }

    public static implicit operator string(TravelIdentityNumber number) => number.Value;
    public static implicit operator TravelIdentityNumber(string value) => new(value);

    public override string ToString() => Value;
    public override bool Equals(object? obj) => obj is TravelIdentityNumber other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}
