using System;
using Pupitre.AIBehavior.Domain.Entities;
using Mamey.Types;

namespace Pupitre.AIBehavior.Domain.Repositories;

internal interface IBehaviorRepository
{
    Task AddAsync(Behavior behavior, CancellationToken cancellationToken = default);
    Task UpdateAsync(Behavior behavior, CancellationToken cancellationToken = default);
    Task DeleteAsync(BehaviorId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Behavior>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<Behavior> GetAsync(BehaviorId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(BehaviorId id, CancellationToken cancellationToken = default);
}
