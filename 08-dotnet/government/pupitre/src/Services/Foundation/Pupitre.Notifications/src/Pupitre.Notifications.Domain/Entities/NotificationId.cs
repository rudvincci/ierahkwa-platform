using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Notifications.Domain.Entities;

[Serializable]
public class NotificationId : AggregateId<Guid>, IEquatable<NotificationId>
{
    public NotificationId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public NotificationId(Guid notificationId) : base(notificationId)
    {
        Value = notificationId;
    }

    [JsonPropertyName("notificationId")]
    public override Guid Value { get; }

    public static implicit operator Guid(NotificationId id) => id.Value;

    public static implicit operator NotificationId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(NotificationId? other)
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
        return Equals((NotificationId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out NotificationId result)
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

