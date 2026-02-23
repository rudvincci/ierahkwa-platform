using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Strongly-typed identifier for ZeroTrust entities.
/// </summary>
internal class ZeroTrustId : AggregateId<Guid>, IEquatable<ZeroTrustId>
{
    public ZeroTrustId()
        : this(Guid.NewGuid())
    {
    }

    public ZeroTrustId(Guid zeroTrustId) : base(zeroTrustId)
    {
        Value = zeroTrustId;
    }

    public override Guid Value { get; }

    public static implicit operator Guid(ZeroTrustId id) => id.Value;

    public static implicit operator ZeroTrustId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(ZeroTrustId? other)
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
        return Equals((ZeroTrustId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out ZeroTrustId result)
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
