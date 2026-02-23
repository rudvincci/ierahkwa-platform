using System;
using Pupitre.Progress.Domain.Entities;
using Mamey.Types;

namespace Pupitre.Progress.Domain.Repositories;

internal interface ILearningProgressRepository
{
    Task AddAsync(LearningProgress learningprogress, CancellationToken cancellationToken = default);
    Task UpdateAsync(LearningProgress learningprogress, CancellationToken cancellationToken = default);
    Task DeleteAsync(LearningProgressId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LearningProgress>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<LearningProgress> GetAsync(LearningProgressId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(LearningProgressId id, CancellationToken cancellationToken = default);
}
