using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Notifications.Application.Events;

[Contract]
internal record NotificationUpdated(Guid NotificationId) : IEvent;


