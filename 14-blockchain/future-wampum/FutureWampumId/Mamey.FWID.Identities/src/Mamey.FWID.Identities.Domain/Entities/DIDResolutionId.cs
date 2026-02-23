using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Strongly-typed identifier for DIDResolution entities.
/// </summary>
internal class DIDResolutionId : AggregateId<Guid>, IEquatable<DIDResolutionId>
{
    public DIDResolutionId()
        : this(Guid.NewGuid())
    {
    }

    public DIDResolutionId(Guid didResolutionId) : base(didResolutionId)
    {
        Value = didResolutionId;
    }

    public override Guid Value { get; }

    public static implicit operator Guid(DIDResolutionId id) => id.Value;

    public static implicit operator DIDResolutionId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(DIDResolutionId? other)
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
        return Equals((DIDResolutionId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out DIDResolutionId result)
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
