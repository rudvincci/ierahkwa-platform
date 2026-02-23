using Mamey.CQRS;
using Pupitre.AIBehavior.Domain.Entities;

namespace Pupitre.AIBehavior.Domain.Events;

internal record BehaviorModified(Behavior Behavior): IDomainEvent;

