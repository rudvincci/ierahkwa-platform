using System.Text.Json.Serialization;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Entities;

[Serializable]
internal class SubjectId : AggregateId<Guid>, IEquatable<SubjectId>
{
    public SubjectId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public SubjectId(Guid subjectId) : base(subjectId)
    {
        Value = subjectId;
    }

    [JsonPropertyName("subjectId")]
    public override Guid Value { get; }

    public static implicit operator Guid(SubjectId id) => id.Value;

    public static implicit operator SubjectId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(SubjectId? other)
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
        return Equals((SubjectId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out SubjectId result)
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