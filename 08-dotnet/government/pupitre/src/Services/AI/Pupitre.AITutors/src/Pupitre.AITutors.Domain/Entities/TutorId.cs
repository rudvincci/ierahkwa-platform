using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AITutors.Domain.Entities;

[Serializable]
public class TutorId : AggregateId<Guid>, IEquatable<TutorId>
{
    public TutorId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public TutorId(Guid tutorId) : base(tutorId)
    {
        Value = tutorId;
    }

    [JsonPropertyName("tutorId")]
    public override Guid Value { get; }

    public static implicit operator Guid(TutorId id) => id.Value;

    public static implicit operator TutorId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(TutorId? other)
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
        return Equals((TutorId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out TutorId result)
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

