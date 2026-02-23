using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Strongly-typed identifier for AuditTrail entities.
/// </summary>
internal class AuditTrailId : AggregateId<Guid>, IEquatable<AuditTrailId>
{
    public AuditTrailId()
        : this(Guid.NewGuid())
    {
    }

    public AuditTrailId(Guid auditTrailId) : base(auditTrailId)
    {
        Value = auditTrailId;
    }

    public override Guid Value { get; }

    public static implicit operator Guid(AuditTrailId id) => id.Value;

    public static implicit operator AuditTrailId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(AuditTrailId? other)
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
        return Equals((AuditTrailId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out AuditTrailId result)
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
