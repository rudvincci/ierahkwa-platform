using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

[Serializable]
public class SmsConfirmationId : AggregateId<Guid>, IEquatable<SmsConfirmationId>
{
    public SmsConfirmationId()
        : this(Guid.NewGuid())
    {
    }

    [JsonConstructor]
    public SmsConfirmationId(Guid smsConfirmationId) : base(smsConfirmationId)
    {
        Value = smsConfirmationId;
    }

    [JsonPropertyName("smsConfirmationId")]
    public override Guid Value { get; }

    public static implicit operator Guid(SmsConfirmationId id) => id.Value;

    public static implicit operator SmsConfirmationId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(SmsConfirmationId? other)
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
        return Equals((SmsConfirmationId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}

