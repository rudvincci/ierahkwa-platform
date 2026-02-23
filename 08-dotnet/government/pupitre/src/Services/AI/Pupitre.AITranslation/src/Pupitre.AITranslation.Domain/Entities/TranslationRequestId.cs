using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AITranslation.Domain.Entities;

[Serializable]
public class TranslationRequestId : AggregateId<Guid>, IEquatable<TranslationRequestId>
{
    public TranslationRequestId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public TranslationRequestId(Guid translationrequestId) : base(translationrequestId)
    {
        Value = translationrequestId;
    }

    [JsonPropertyName("translationrequestId")]
    public override Guid Value { get; }

    public static implicit operator Guid(TranslationRequestId id) => id.Value;

    public static implicit operator TranslationRequestId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(TranslationRequestId? other)
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
        return Equals((TranslationRequestId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out TranslationRequestId result)
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

