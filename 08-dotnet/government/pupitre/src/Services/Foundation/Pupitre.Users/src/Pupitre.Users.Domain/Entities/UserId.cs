using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Users.Domain.Entities;

[Serializable]
public class UserId : AggregateId<Guid>, IEquatable<UserId>
{
    public UserId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public UserId(Guid userId) : base(userId)
    {
        Value = userId;
    }

    [JsonPropertyName("userId")]
    public override Guid Value { get; }

    public static implicit operator Guid(UserId id) => id.Value;

    public static implicit operator UserId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(UserId? other)
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
        return Equals((UserId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out UserId result)
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

