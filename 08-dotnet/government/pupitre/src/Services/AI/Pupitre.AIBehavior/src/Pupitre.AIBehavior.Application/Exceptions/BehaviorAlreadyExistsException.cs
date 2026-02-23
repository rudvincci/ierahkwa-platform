using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AIBehavior.Domain.Entities;

namespace Pupitre.AIBehavior.Application.Exceptions;

internal class BehaviorAlreadyExistsException : MameyException
{
    public BehaviorAlreadyExistsException(BehaviorId behaviorId)
        : base($"Behavior with ID: '{behaviorId.Value}' already exists.")
        => BehaviorId = behaviorId;

    public BehaviorId BehaviorId { get; }
}
