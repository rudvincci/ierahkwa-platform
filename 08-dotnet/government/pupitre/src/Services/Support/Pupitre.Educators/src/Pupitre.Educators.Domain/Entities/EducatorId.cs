using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Educators.Domain.Entities;

[Serializable]
public class EducatorId : AggregateId<Guid>, IEquatable<EducatorId>
{
    public EducatorId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public EducatorId(Guid educatorId) : base(educatorId)
    {
        Value = educatorId;
    }

    [JsonPropertyName("educatorId")]
    public override Guid Value { get; }

    public static implicit operator Guid(EducatorId id) => id.Value;

    public static implicit operator EducatorId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(EducatorId? other)
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
        return Equals((EducatorId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out EducatorId result)
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

