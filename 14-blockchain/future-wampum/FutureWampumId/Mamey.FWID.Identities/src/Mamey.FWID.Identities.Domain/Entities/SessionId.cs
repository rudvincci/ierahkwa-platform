using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

[Serializable]
public class SessionId : AggregateId<Guid>, IEquatable<SessionId>
{
    public SessionId()
        : this(Guid.NewGuid())
    {
    }

    [JsonConstructor]
    public SessionId(Guid sessionId) : base(sessionId)
    {
        Value = sessionId;
    }

    [JsonPropertyName("sessionId")]
    public override Guid Value { get; }

    public static implicit operator Guid(SessionId id) => id.Value;

    public static implicit operator SessionId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(SessionId? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(Value, other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SessionId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}

