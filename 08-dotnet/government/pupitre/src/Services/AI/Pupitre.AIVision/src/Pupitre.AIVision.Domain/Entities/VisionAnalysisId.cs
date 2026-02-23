using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AIVision.Domain.Entities;

[Serializable]
public class VisionAnalysisId : AggregateId<Guid>, IEquatable<VisionAnalysisId>
{
    public VisionAnalysisId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public VisionAnalysisId(Guid visionanalysisId) : base(visionanalysisId)
    {
        Value = visionanalysisId;
    }

    [JsonPropertyName("visionanalysisId")]
    public override Guid Value { get; }

    public static implicit operator Guid(VisionAnalysisId id) => id.Value;

    public static implicit operator VisionAnalysisId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(VisionAnalysisId? other)
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
        return Equals((VisionAnalysisId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out VisionAnalysisId result)
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

