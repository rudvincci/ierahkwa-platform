using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Mamey.ServiceName.Domain.Entities;

[Serializable]
public class EntityNameId : AggregateId<Guid>, IEquatable<EntityNameId>
{
    public EntityNameId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public EntityNameId(Guid entitynameId) : base(entitynameId)
    {
        Value = entitynameId;
    }

    [JsonPropertyName("entitynameId")]
    public override Guid Value { get; }

    public static implicit operator Guid(EntityNameId id) => id.Value;

    public static implicit operator EntityNameId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(EntityNameId? other)
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
        return Equals((EntityNameId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out EntityNameId result)
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

