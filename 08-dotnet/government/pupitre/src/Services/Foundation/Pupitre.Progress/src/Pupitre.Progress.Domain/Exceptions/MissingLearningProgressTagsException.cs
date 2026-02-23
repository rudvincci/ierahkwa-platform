using Mamey.Exceptions;

namespace Pupitre.Progress.Domain.Exceptions;

internal class MissingLearningProgressTagsException : DomainException
{
    public MissingLearningProgressTagsException()
        : base("LearningProgress tags are missing.")
    {
    }

    public override string Code => "missing_learningprogress_tags";
}