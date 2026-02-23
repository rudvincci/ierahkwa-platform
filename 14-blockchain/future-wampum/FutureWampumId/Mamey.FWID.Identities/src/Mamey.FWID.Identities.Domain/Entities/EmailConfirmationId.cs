using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

[Serializable]
public class EmailConfirmationId : AggregateId<Guid>, IEquatable<EmailConfirmationId>
{
    public EmailConfirmationId()
        : this(Guid.NewGuid())
    {
    }

    [JsonConstructor]
    public EmailConfirmationId(Guid emailConfirmationId) : base(emailConfirmationId)
    {
        Value = emailConfirmationId;
    }

    [JsonPropertyName("emailConfirmationId")]
    public override Guid Value { get; }

    public static implicit operator Guid(EmailConfirmationId id) => id.Value;

    public static implicit operator EmailConfirmationId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(EmailConfirmationId? other)
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
        return Equals((EmailConfirmationId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}

