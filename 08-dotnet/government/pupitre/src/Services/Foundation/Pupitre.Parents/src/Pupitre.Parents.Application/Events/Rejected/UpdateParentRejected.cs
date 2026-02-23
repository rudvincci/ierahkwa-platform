using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Parents.Application.Events.Rejected;

[Contract]
internal record UpdateParentRejected(Guid ParentId, string Reason, string Code) : IRejectedEvent;
