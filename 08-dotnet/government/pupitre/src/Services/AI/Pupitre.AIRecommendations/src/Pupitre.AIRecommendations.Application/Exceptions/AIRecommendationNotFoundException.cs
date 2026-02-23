using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AIRecommendations.Domain.Entities;

namespace Pupitre.AIRecommendations.Application.Exceptions;

internal class AIRecommendationNotFoundException : MameyException
{
    public AIRecommendationNotFoundException(AIRecommendationId airecommendationId)
        : base($"AIRecommendation with ID: '{airecommendationId.Value}' was not found.")
        => AIRecommendationId = airecommendationId;

    public AIRecommendationId AIRecommendationId { get; }
}

