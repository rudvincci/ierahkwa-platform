using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Accessibility.Domain.Entities;

[Serializable]
public class AccessProfileId : AggregateId<Guid>, IEquatable<AccessProfileId>
{
    public AccessProfileId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public AccessProfileId(Guid accessprofileId) : base(accessprofileId)
    {
        Value = accessprofileId;
    }

    [JsonPropertyName("accessprofileId")]
    public override Guid Value { get; }

    public static implicit operator Guid(AccessProfileId id) => id.Value;

    public static implicit operator AccessProfileId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(AccessProfileId? other)
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
        return Equals((AccessProfileId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out AccessProfileId result)
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

