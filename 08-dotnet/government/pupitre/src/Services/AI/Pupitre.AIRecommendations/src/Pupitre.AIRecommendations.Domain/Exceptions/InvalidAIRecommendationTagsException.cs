using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.AIRecommendations.Domain.Exceptions;

internal class InvalidAIRecommendationTagsException : DomainException
{
    public override string Code { get; } = "invalid_airecommendation_tags";

    public InvalidAIRecommendationTagsException() : base("AIRecommendation tags are invalid.")
    {
    }
}
