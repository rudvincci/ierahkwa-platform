using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.ServiceName.Application.Events;

[Contract]
internal record EntityNameDeleted(Guid EntityNameId) : IEvent;


