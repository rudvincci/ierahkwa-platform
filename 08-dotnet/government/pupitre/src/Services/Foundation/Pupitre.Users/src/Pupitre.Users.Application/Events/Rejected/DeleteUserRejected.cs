using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Users.Application.Events.Rejected;

[Contract]
internal record DeleteUserRejected(Guid UserId, string Reason, string Code) : IRejectedEvent;