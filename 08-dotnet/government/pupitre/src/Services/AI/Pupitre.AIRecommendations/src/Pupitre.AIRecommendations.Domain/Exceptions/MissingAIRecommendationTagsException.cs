using Mamey.Exceptions;

namespace Pupitre.AIRecommendations.Domain.Exceptions;

internal class MissingAIRecommendationTagsException : DomainException
{
    public MissingAIRecommendationTagsException()
        : base("AIRecommendation tags are missing.")
    {
    }

    public override string Code => "missing_airecommendation_tags";
}