using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.AIBehavior.Domain.Exceptions;

internal class InvalidBehaviorTagsException : DomainException
{
    public override string Code { get; } = "invalid_behavior_tags";

    public InvalidBehaviorTagsException() : base("Behavior tags are invalid.")
    {
    }
}
