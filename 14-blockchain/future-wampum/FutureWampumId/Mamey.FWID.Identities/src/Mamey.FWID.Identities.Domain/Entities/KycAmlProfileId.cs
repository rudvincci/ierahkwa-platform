using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Strongly-typed identifier for KycAmlProfile entities.
/// </summary>
internal class KycAmlProfileId : AggregateId<Guid>, IEquatable<KycAmlProfileId>
{
    public KycAmlProfileId()
        : this(Guid.NewGuid())
    {
    }

    public KycAmlProfileId(Guid kycAmlProfileId) : base(kycAmlProfileId)
    {
        Value = kycAmlProfileId;
    }

    public override Guid Value { get; }

    public static implicit operator Guid(KycAmlProfileId id) => id.Value;

    public static implicit operator KycAmlProfileId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(KycAmlProfileId? other)
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
        return Equals((KycAmlProfileId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out KycAmlProfileId result)
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
