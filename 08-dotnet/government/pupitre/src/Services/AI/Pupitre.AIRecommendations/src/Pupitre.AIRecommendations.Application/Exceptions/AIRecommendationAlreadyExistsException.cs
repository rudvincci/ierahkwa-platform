using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AIRecommendations.Domain.Entities;

namespace Pupitre.AIRecommendations.Application.Exceptions;

internal class AIRecommendationAlreadyExistsException : MameyException
{
    public AIRecommendationAlreadyExistsException(AIRecommendationId airecommendationId)
        : base($"AIRecommendation with ID: '{airecommendationId.Value}' already exists.")
        => AIRecommendationId = airecommendationId;

    public AIRecommendationId AIRecommendationId { get; }
}
