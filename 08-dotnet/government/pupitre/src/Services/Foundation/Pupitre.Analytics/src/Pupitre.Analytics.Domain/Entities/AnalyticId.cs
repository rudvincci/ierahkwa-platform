using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Analytics.Domain.Entities;

[Serializable]
public class AnalyticId : AggregateId<Guid>, IEquatable<AnalyticId>
{
    public AnalyticId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public AnalyticId(Guid analyticId) : base(analyticId)
    {
        Value = analyticId;
    }

    [JsonPropertyName("analyticId")]
    public override Guid Value { get; }

    public static implicit operator Guid(AnalyticId id) => id.Value;

    public static implicit operator AnalyticId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(AnalyticId? other)
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
        return Equals((AnalyticId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out AnalyticId result)
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

