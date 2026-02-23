using System;
using Pupitre.AIRecommendations.Domain.Entities;
using Mamey.Types;

namespace Pupitre.AIRecommendations.Domain.Repositories;

internal interface IAIRecommendationRepository
{
    Task AddAsync(AIRecommendation airecommendation, CancellationToken cancellationToken = default);
    Task UpdateAsync(AIRecommendation airecommendation, CancellationToken cancellationToken = default);
    Task DeleteAsync(AIRecommendationId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIRecommendation>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<AIRecommendation> GetAsync(AIRecommendationId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(AIRecommendationId id, CancellationToken cancellationToken = default);
}
