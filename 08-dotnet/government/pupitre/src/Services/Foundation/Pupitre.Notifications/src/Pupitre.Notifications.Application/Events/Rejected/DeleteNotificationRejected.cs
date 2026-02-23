using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Notifications.Application.Events.Rejected;

[Contract]
internal record DeleteNotificationRejected(Guid NotificationId, string Reason, string Code) : IRejectedEvent;