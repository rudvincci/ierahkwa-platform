using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Curricula.Domain.Entities;

[Serializable]
public class CurriculumId : AggregateId<Guid>, IEquatable<CurriculumId>
{
    public CurriculumId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public CurriculumId(Guid curriculumId) : base(curriculumId)
    {
        Value = curriculumId;
    }

    [JsonPropertyName("curriculumId")]
    public override Guid Value { get; }

    public static implicit operator Guid(CurriculumId id) => id.Value;

    public static implicit operator CurriculumId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(CurriculumId? other)
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
        return Equals((CurriculumId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out CurriculumId result)
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

