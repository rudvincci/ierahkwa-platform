using System;
using Pupitre.AIAdaptive.Domain.Entities;
using Mamey.Types;

namespace Pupitre.AIAdaptive.Domain.Repositories;

internal interface IAdaptiveLearningRepository
{
    Task AddAsync(AdaptiveLearning adaptivelearning, CancellationToken cancellationToken = default);
    Task UpdateAsync(AdaptiveLearning adaptivelearning, CancellationToken cancellationToken = default);
    Task DeleteAsync(AdaptiveLearningId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdaptiveLearning>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<AdaptiveLearning> GetAsync(AdaptiveLearningId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(AdaptiveLearningId id, CancellationToken cancellationToken = default);
}
