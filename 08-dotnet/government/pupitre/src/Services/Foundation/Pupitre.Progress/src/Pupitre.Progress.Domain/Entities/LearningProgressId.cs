using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Progress.Domain.Entities;

[Serializable]
public class LearningProgressId : AggregateId<Guid>, IEquatable<LearningProgressId>
{
    public LearningProgressId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public LearningProgressId(Guid learningprogressId) : base(learningprogressId)
    {
        Value = learningprogressId;
    }

    [JsonPropertyName("learningprogressId")]
    public override Guid Value { get; }

    public static implicit operator Guid(LearningProgressId id) => id.Value;

    public static implicit operator LearningProgressId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(LearningProgressId? other)
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
        return Equals((LearningProgressId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out LearningProgressId result)
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

