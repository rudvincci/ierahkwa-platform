using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Strongly-typed identifier for VerificationSession entities.
/// </summary>
internal class VerificationSessionId : AggregateId<Guid>, IEquatable<VerificationSessionId>
{
    public VerificationSessionId()
        : this(Guid.NewGuid())
    {
    }

    public VerificationSessionId(Guid verificationSessionId) : base(verificationSessionId)
    {
        Value = verificationSessionId;
    }

    public override Guid Value { get; }

    public static implicit operator Guid(VerificationSessionId id) => id.Value;

    public static implicit operator VerificationSessionId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(VerificationSessionId? other)
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
        return Equals((VerificationSessionId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out VerificationSessionId result)
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
