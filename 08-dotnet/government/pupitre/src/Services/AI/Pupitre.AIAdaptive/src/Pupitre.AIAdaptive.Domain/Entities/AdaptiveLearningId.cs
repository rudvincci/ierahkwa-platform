using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AIAdaptive.Domain.Entities;

[Serializable]
public class AdaptiveLearningId : AggregateId<Guid>, IEquatable<AdaptiveLearningId>
{
    public AdaptiveLearningId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public AdaptiveLearningId(Guid adaptivelearningId) : base(adaptivelearningId)
    {
        Value = adaptivelearningId;
    }

    [JsonPropertyName("adaptivelearningId")]
    public override Guid Value { get; }

    public static implicit operator Guid(AdaptiveLearningId id) => id.Value;

    public static implicit operator AdaptiveLearningId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(AdaptiveLearningId? other)
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
        return Equals((AdaptiveLearningId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out AdaptiveLearningId result)
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

