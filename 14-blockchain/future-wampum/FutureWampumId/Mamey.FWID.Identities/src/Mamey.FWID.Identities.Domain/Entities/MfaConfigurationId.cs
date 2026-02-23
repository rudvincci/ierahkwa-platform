using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

[Serializable]
public class MfaConfigurationId : AggregateId<Guid>, IEquatable<MfaConfigurationId>
{
    public MfaConfigurationId()
        : this(Guid.NewGuid())
    {
    }

    [JsonConstructor]
    public MfaConfigurationId(Guid mfaConfigurationId) : base(mfaConfigurationId)
    {
        Value = mfaConfigurationId;
    }

    [JsonPropertyName("mfaConfigurationId")]
    public override Guid Value { get; }

    public static implicit operator Guid(MfaConfigurationId id) => id.Value;

    public static implicit operator MfaConfigurationId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(MfaConfigurationId? other)
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
        return Equals((MfaConfigurationId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}

