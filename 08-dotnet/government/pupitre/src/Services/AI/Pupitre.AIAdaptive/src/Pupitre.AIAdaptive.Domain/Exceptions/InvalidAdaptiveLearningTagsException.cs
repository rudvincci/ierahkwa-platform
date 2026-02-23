using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.AIAdaptive.Domain.Exceptions;

internal class InvalidAdaptiveLearningTagsException : DomainException
{
    public override string Code { get; } = "invalid_adaptivelearning_tags";

    public InvalidAdaptiveLearningTagsException() : base("AdaptiveLearning tags are invalid.")
    {
    }
}
