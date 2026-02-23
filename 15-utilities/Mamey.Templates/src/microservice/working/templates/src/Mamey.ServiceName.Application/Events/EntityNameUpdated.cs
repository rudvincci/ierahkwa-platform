using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.ServiceName.Application.Events;

[Contract]
internal record EntityNameUpdated(Guid EntityNameId) : IEvent;


