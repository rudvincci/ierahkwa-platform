using Mamey;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Strongly-typed identifier for Consent entities.
/// </summary>
internal readonly struct ConsentId : IEquatable<ConsentId>
{
    public Guid Value { get; }

    public ConsentId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ConsentId cannot be empty", nameof(value));

        Value = value;
    }

    public static ConsentId New() => new(Guid.NewGuid());

    public bool Equals(ConsentId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj) => obj is ConsentId other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => $"ConsentId({Value})";

    public static bool operator ==(ConsentId left, ConsentId right) => left.Equals(right);

    public static bool operator !=(ConsentId left, ConsentId right) => !left.Equals(right);

    public static implicit operator Guid(ConsentId id) => id.Value;

    public static explicit operator ConsentId(Guid value) => new(value);
}
