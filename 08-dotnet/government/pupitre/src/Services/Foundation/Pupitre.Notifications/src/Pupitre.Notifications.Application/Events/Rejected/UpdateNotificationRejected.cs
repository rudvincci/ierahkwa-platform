using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Notifications.Application.Events.Rejected;

[Contract]
internal record UpdateNotificationRejected(Guid NotificationId, string Reason, string Code) : IRejectedEvent;
