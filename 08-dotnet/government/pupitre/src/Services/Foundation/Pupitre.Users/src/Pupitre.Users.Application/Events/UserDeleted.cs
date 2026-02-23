using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Users.Application.Events;

[Contract]
internal record UserDeleted(Guid UserId) : IEvent;


