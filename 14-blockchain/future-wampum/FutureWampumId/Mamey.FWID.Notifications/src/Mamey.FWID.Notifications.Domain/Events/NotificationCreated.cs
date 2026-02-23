using Mamey.FWID.Notifications.Domain.Entities;

namespace Mamey.FWID.Notifications.Domain.Events;

/// <summary>
/// Domain event raised when a notification is created.
/// </summary>
internal record NotificationCreated(Notification Notification) : IDomainEvent;







