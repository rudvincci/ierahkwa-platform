using System;
using Mamey.CQRS.Events;
using Mamey.MessageBrokers;

namespace Mamey.FWID.Notifications.Application.Events.Integration.Notifications;

/// <summary>
/// Integration event raised when a notification is read.
/// </summary>
[Message("notifications")]
public record NotificationReadIntegrationEvent(Guid NotificationId, Guid IdentityId, DateTime ReadAt) : IEvent;







