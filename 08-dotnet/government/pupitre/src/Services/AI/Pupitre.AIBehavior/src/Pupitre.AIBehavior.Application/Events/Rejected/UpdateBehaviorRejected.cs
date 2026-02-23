using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIBehavior.Application.Events.Rejected;

[Contract]
internal record UpdateBehaviorRejected(Guid BehaviorId, string Reason, string Code) : IRejectedEvent;
