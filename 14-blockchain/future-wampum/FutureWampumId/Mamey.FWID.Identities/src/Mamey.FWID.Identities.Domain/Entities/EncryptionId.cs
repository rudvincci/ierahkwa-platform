using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Strongly-typed identifier for Encryption entities.
/// </summary>
internal class EncryptionId : AggregateId<Guid>, IEquatable<EncryptionId>
{
    public EncryptionId()
        : this(Guid.NewGuid())
    {
    }

    public EncryptionId(Guid encryptionId) : base(encryptionId)
    {
        Value = encryptionId;
    }

    public override Guid Value { get; }

    public static implicit operator Guid(EncryptionId id) => id.Value;

    public static implicit operator EncryptionId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(EncryptionId? other)
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
        return Equals((EncryptionId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out EncryptionId result)
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
