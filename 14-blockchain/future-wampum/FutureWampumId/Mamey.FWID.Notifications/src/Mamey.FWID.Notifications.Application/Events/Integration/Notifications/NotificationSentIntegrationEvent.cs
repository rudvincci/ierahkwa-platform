using System;
using Mamey.CQRS.Events;
using Mamey.MessageBrokers;

namespace Mamey.FWID.Notifications.Application.Events.Integration.Notifications;

/// <summary>
/// Integration event raised when a notification is sent.
/// </summary>
[Message("notifications")]
public record NotificationSentIntegrationEvent(Guid NotificationId, Guid IdentityId, string Type, DateTime SentAt) : IEvent;







