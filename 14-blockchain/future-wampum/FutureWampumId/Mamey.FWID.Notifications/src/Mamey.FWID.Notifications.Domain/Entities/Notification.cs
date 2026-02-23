using System;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Notifications.Domain.Events;
using Mamey.FWID.Notifications.Domain.Exceptions;
using Mamey.Types;

namespace Mamey.FWID.Notifications.Domain.Entities;

/// <summary>
/// Represents a notification aggregate root.
/// </summary>
internal class Notification : AggregateRoot<NotificationId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private Notification()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Notification aggregate root.
    /// </summary>
    public Notification(
        NotificationId id,
        IdentityId identityId,
        string title,
        string description,
        string message,
        NotificationType type,
        NotificationStatus status,
        DateTime createdAt,
        DateTime? sentAt = null,
        DateTime? readAt = null,
        bool isRead = false,
        string? relatedEntityType = null,
        Guid? relatedEntityId = null)
        : base(id)
    {
        IdentityId = identityId ?? throw new ArgumentNullException(nameof(identityId));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Type = type;
        Status = status;
        CreatedAt = createdAt;
        SentAt = sentAt;
        ReadAt = readAt;
        IsRead = isRead;
        RelatedEntityType = relatedEntityType;
        RelatedEntityId = relatedEntityId;
    }

    public IdentityId IdentityId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Message { get; private set; }
    public NotificationType Type { get; private set; }
    public NotificationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public bool IsRead { get; private set; }
    public string? RelatedEntityType { get; private set; }
    public Guid? RelatedEntityId { get; private set; }

    /// <summary>
    /// Creates a new notification.
    /// </summary>
    public static Notification Create(
        IdentityId identityId,
        string title,
        string description,
        string message,
        NotificationType notificationType,
        string? relatedEntityType = null,
        Guid? relatedEntityId = null)
    {
        var notification = new Notification(
            new NotificationId(identityId),
            identityId,
            title,
            description,
            message,
            notificationType,
            NotificationStatus.Pending,
            DateTime.UtcNow,
            relatedEntityType: relatedEntityType,
            relatedEntityId: relatedEntityId);

        notification.AddEvent(new NotificationCreated(notification));

        return notification;
    }

    /// <summary>
    /// Marks the notification as sent.
    /// </summary>
    public void MarkAsSent()
    {
        if (Status == NotificationStatus.Sent)
            return; // Already sent

        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new NotificationSent(Id, Type, SentAt.Value));
    }

    /// <summary>
    /// Marks the notification as read.
    /// </summary>
    public void MarkAsRead()
    {
        if (ReadAt.HasValue)
        {
            throw new NotificationAlreadyReadException(Id);
        }

        ReadAt = DateTime.UtcNow;
        IsRead = true;
        if (Status == NotificationStatus.Sent)
        {
            Status = NotificationStatus.Read;
        }
        IncrementVersion();

        AddEvent(new NotificationRead(Id, ReadAt.Value));
    }

    /// <summary>
    /// Marks the notification as failed.
    /// </summary>
    public void MarkAsFailed(string? reason = null)
    {
        Status = NotificationStatus.Failed;
        IncrementVersion();

        AddEvent(new NotificationFailed(Id, reason ?? "Unknown error"));
    }
}

