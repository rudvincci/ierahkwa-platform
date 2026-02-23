using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Strongly-typed identifier for Disclosure entities.
/// </summary>
internal class DisclosureId : AggregateId<Guid>, IEquatable<DisclosureId>
{
    public DisclosureId()
        : this(Guid.NewGuid())
    {
    }

    public DisclosureId(Guid disclosureId) : base(disclosureId)
    {
        Value = disclosureId;
    }

    public override Guid Value { get; }

    public static implicit operator Guid(DisclosureId id) => id.Value;

    public static implicit operator DisclosureId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(DisclosureId? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(Value, other.Value);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((DisclosureId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out DisclosureId result)
    {
        Guid result2;
        if (Guid.TryParse(input, out result2))
        {
            result = result2;
            return true;
        }
        result = default!;
        return false;
    }
}
