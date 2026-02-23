using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Ministries.Domain.Entities;

[Serializable]
public class MinistryDataId : AggregateId<Guid>, IEquatable<MinistryDataId>
{
    public MinistryDataId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public MinistryDataId(Guid ministrydataId) : base(ministrydataId)
    {
        Value = ministrydataId;
    }

    [JsonPropertyName("ministrydataId")]
    public override Guid Value { get; }

    public static implicit operator Guid(MinistryDataId id) => id.Value;

    public static implicit operator MinistryDataId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(MinistryDataId? other)
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
        return Equals((MinistryDataId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out MinistryDataId result)
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

