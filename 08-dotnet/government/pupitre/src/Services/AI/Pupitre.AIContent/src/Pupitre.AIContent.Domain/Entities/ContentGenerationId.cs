using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AIContent.Domain.Entities;

[Serializable]
public class ContentGenerationId : AggregateId<Guid>, IEquatable<ContentGenerationId>
{
    public ContentGenerationId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public ContentGenerationId(Guid contentgenerationId) : base(contentgenerationId)
    {
        Value = contentgenerationId;
    }

    [JsonPropertyName("contentgenerationId")]
    public override Guid Value { get; }

    public static implicit operator Guid(ContentGenerationId id) => id.Value;

    public static implicit operator ContentGenerationId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(ContentGenerationId? other)
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
        return Equals((ContentGenerationId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out ContentGenerationId result)
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

