using System;
using Mamey.FWID.Notifications.Domain.Entities;

namespace Mamey.FWID.Notifications.Domain.Events;

/// <summary>
/// Domain event raised when a notification is read.
/// </summary>
internal record NotificationRead(NotificationId NotificationId, DateTime ReadAt) : IDomainEvent;







