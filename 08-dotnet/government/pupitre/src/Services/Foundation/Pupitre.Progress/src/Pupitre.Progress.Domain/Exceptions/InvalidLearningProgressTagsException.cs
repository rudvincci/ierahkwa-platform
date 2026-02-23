using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.Progress.Domain.Exceptions;

internal class InvalidLearningProgressTagsException : DomainException
{
    public override string Code { get; } = "invalid_learningprogress_tags";

    public InvalidLearningProgressTagsException() : base("LearningProgress tags are invalid.")
    {
    }
}
