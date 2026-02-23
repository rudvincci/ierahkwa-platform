using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

[Serializable]
public class IdentityId : AggregateId<Guid>, IEquatable<IdentityId>
{
    public IdentityId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public IdentityId(Guid identityId) : base(identityId)
    {
        Value = identityId;
    }

    [JsonPropertyName("identityId")]
    public override Guid Value { get; }

    public static implicit operator Guid(IdentityId id) => id.Value;

    public static implicit operator IdentityId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(IdentityId? other)
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
        return Equals((IdentityId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out IdentityId result)
    {

        Guid result2;
        if (Guid.TryParse(input, out result2))
        {
            result = result2;
            return true;
        }
        result = default(Guid);
        return false;
    }
}

