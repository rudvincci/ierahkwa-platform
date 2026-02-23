using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Strongly-typed identifier for CredentialRegistry entities.
/// </summary>
internal class CredentialRegistryId : AggregateId<Guid>, IEquatable<CredentialRegistryId>
{
    public CredentialRegistryId()
        : this(Guid.NewGuid())
    {
    }

    public CredentialRegistryId(Guid credentialRegistryId) : base(credentialRegistryId)
    {
        Value = credentialRegistryId;
    }

    public override Guid Value { get; }

    public static implicit operator Guid(CredentialRegistryId id) => id.Value;

    public static implicit operator CredentialRegistryId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(CredentialRegistryId? other)
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
        return Equals((CredentialRegistryId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out CredentialRegistryId result)
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
