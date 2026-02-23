using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIBehavior.Application.Events;

[Contract]
internal record BehaviorUpdated(Guid BehaviorId) : IEvent;


