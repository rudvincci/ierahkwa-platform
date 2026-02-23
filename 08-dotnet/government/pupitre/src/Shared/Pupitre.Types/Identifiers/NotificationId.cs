namespace Pupitre.Types.Identifiers;

/// <summary>
/// Strongly-typed identifier for Notification entities.
/// </summary>
public sealed record NotificationId
{
    public Guid Value { get; }

    public NotificationId() : this(Guid.NewGuid()) { }
    public NotificationId(Guid value) => Value = value;

    public static NotificationId New() => new(Guid.NewGuid());
    public static NotificationId Empty => new(Guid.Empty);

    public static implicit operator NotificationId(Guid value) => new(value);
    public static implicit operator Guid(NotificationId id) => id.Value;

    public override string ToString() => Value.ToString();
}
