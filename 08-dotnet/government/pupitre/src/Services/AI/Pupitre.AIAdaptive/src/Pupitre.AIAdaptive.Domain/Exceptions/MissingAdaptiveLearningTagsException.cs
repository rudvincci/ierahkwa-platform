using Mamey.Exceptions;

namespace Pupitre.AIAdaptive.Domain.Exceptions;

internal class MissingAdaptiveLearningTagsException : DomainException
{
    public MissingAdaptiveLearningTagsException()
        : base("AdaptiveLearning tags are missing.")
    {
    }

    public override string Code => "missing_adaptivelearning_tags";
}