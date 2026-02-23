using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.ServiceName.Application.Events.Rejected;

[Contract]
internal record DeleteEntityNameRejected(Guid EntityNameId, string Reason, string Code) : IRejectedEvent;