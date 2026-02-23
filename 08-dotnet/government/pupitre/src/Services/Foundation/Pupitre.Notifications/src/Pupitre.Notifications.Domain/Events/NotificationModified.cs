using Mamey.CQRS;
using Pupitre.Notifications.Domain.Entities;

namespace Pupitre.Notifications.Domain.Events;

internal record NotificationModified(Notification Notification): IDomainEvent;

