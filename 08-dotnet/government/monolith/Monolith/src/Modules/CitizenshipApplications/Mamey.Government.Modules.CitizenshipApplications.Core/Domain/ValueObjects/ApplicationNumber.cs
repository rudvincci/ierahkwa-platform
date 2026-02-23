namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;

/// <summary>
/// Application number value object (e.g., CIT-2024-001234)
/// </summary>
public class ApplicationNumber
{
    public string Value { get; }

    public ApplicationNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Application number cannot be empty", nameof(value));
        
        Value = value;
    }

    public static implicit operator string(ApplicationNumber number) => number.Value;
    public static implicit operator ApplicationNumber(string value) => new(value);

    public override string ToString() => Value;
    public override bool Equals(object? obj) => obj is ApplicationNumber other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}
