using Mamey.CQRS;
using Pupitre.AIBehavior.Domain.Entities;

namespace Pupitre.AIBehavior.Domain.Events;

internal record BehaviorRemoved(Behavior Behavior) : IDomainEvent;