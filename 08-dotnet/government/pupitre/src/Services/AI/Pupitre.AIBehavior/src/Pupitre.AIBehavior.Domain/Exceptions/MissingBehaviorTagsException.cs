using Mamey.Exceptions;

namespace Pupitre.AIBehavior.Domain.Exceptions;

internal class MissingBehaviorTagsException : DomainException
{
    public MissingBehaviorTagsException()
        : base("Behavior tags are missing.")
    {
    }

    public override string Code => "missing_behavior_tags";
}