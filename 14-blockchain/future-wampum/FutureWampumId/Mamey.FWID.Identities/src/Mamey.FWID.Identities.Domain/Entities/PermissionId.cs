using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

[Serializable]
public class PermissionId : AggregateId<Guid>, IEquatable<PermissionId>
{
    public PermissionId()
        : this(Guid.NewGuid())
    {
    }

    [JsonConstructor]
    public PermissionId(Guid permissionId) : base(permissionId)
    {
        Value = permissionId;
    }

    [JsonPropertyName("permissionId")]
    public override Guid Value { get; }

    public static implicit operator Guid(PermissionId id) => id.Value;

    public static implicit operator PermissionId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(PermissionId? other)
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
        return Equals((PermissionId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}

