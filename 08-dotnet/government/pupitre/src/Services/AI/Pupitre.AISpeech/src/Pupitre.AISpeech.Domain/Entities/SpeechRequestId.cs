using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AISpeech.Domain.Entities;

[Serializable]
public class SpeechRequestId : AggregateId<Guid>, IEquatable<SpeechRequestId>
{
    public SpeechRequestId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public SpeechRequestId(Guid speechrequestId) : base(speechrequestId)
    {
        Value = speechrequestId;
    }

    [JsonPropertyName("speechrequestId")]
    public override Guid Value { get; }

    public static implicit operator Guid(SpeechRequestId id) => id.Value;

    public static implicit operator SpeechRequestId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(SpeechRequestId? other)
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
        return Equals((SpeechRequestId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out SpeechRequestId result)
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

