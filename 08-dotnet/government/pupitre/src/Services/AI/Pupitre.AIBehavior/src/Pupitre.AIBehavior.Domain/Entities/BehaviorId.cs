using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AIBehavior.Domain.Entities;

[Serializable]
public class BehaviorId : AggregateId<Guid>, IEquatable<BehaviorId>
{
    public BehaviorId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public BehaviorId(Guid behaviorId) : base(behaviorId)
    {
        Value = behaviorId;
    }

    [JsonPropertyName("behaviorId")]
    public override Guid Value { get; }

    public static implicit operator Guid(BehaviorId id) => id.Value;

    public static implicit operator BehaviorId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(BehaviorId? other)
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
        return Equals((BehaviorId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out BehaviorId result)
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

