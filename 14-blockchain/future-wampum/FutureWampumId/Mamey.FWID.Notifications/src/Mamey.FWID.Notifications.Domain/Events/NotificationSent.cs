using System;
using Mamey.FWID.Notifications.Domain.Entities;

namespace Mamey.FWID.Notifications.Domain.Events;

/// <summary>
/// Domain event raised when a notification is sent.
/// </summary>
internal record NotificationSent(NotificationId NotificationId, NotificationType Type, DateTime SentAt) : IDomainEvent;

