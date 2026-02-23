using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Parents.Domain.Entities;

[Serializable]
public class ParentId : AggregateId<Guid>, IEquatable<ParentId>
{
    public ParentId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public ParentId(Guid parentId) : base(parentId)
    {
        Value = parentId;
    }

    [JsonPropertyName("parentId")]
    public override Guid Value { get; }

    public static implicit operator Guid(ParentId id) => id.Value;

    public static implicit operator ParentId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(ParentId? other)
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
        return Equals((ParentId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out ParentId result)
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

