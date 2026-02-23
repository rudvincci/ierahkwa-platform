using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Strongly-typed identifier for Guardian entities.
/// </summary>
internal class GuardianId : AggregateId<Guid>, IEquatable<GuardianId>
{
    public GuardianId()
        : this(Guid.NewGuid())
    {
    }

    public GuardianId(Guid guardianId) : base(guardianId)
    {
        Value = guardianId;
    }

    public override Guid Value { get; }

    public static implicit operator Guid(GuardianId id) => id.Value;

    public static implicit operator GuardianId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(GuardianId? other)
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
        return Equals((GuardianId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out GuardianId result)
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

/// <summary>
/// Strongly-typed identifier for Delegation entities.
/// </summary>
internal class DelegationId : AggregateId<Guid>, IEquatable<DelegationId>
{
    public DelegationId()
        : this(Guid.NewGuid())
    {
    }

    public DelegationId(Guid delegationId) : base(delegationId)
    {
        Value = delegationId;
    }

    public override Guid Value { get; }

    public static implicit operator Guid(DelegationId id) => id.Value;

    public static implicit operator DelegationId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(DelegationId? other)
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
        return Equals((DelegationId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out DelegationId result)
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
