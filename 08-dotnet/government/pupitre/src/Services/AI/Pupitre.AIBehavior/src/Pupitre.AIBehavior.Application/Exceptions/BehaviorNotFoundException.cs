using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AIBehavior.Domain.Entities;

namespace Pupitre.AIBehavior.Application.Exceptions;

internal class BehaviorNotFoundException : MameyException
{
    public BehaviorNotFoundException(BehaviorId behaviorId)
        : base($"Behavior with ID: '{behaviorId.Value}' was not found.")
        => BehaviorId = behaviorId;

    public BehaviorId BehaviorId { get; }
}

