using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Notifications.Application.Events;

[Contract]
internal record NotificationDeleted(Guid NotificationId) : IEvent;


