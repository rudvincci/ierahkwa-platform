using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AIAssessments.Domain.Entities;

[Serializable]
public class AIAssessmentId : AggregateId<Guid>, IEquatable<AIAssessmentId>
{
    public AIAssessmentId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public AIAssessmentId(Guid aiassessmentId) : base(aiassessmentId)
    {
        Value = aiassessmentId;
    }

    [JsonPropertyName("aiassessmentId")]
    public override Guid Value { get; }

    public static implicit operator Guid(AIAssessmentId id) => id.Value;

    public static implicit operator AIAssessmentId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(AIAssessmentId? other)
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
        return Equals((AIAssessmentId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out AIAssessmentId result)
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

