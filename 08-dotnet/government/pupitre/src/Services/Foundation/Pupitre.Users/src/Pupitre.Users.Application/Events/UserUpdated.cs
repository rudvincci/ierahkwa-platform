using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Users.Application.Events;

[Contract]
internal record UserUpdated(Guid UserId) : IEvent;


