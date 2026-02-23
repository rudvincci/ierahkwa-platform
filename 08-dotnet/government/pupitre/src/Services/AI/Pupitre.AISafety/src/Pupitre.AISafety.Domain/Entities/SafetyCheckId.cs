using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AISafety.Domain.Entities;

[Serializable]
public class SafetyCheckId : AggregateId<Guid>, IEquatable<SafetyCheckId>
{
    public SafetyCheckId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public SafetyCheckId(Guid safetycheckId) : base(safetycheckId)
    {
        Value = safetycheckId;
    }

    [JsonPropertyName("safetycheckId")]
    public override Guid Value { get; }

    public static implicit operator Guid(SafetyCheckId id) => id.Value;

    public static implicit operator SafetyCheckId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(SafetyCheckId? other)
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
        return Equals((SafetyCheckId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out SafetyCheckId result)
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

