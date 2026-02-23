using System;
using Pupitre.Rewards.Domain.Entities;
using Mamey.Types;

namespace Pupitre.Rewards.Domain.Repositories;

internal interface IRewardRepository
{
    Task AddAsync(Reward reward, CancellationToken cancellationToken = default);
    Task UpdateAsync(Reward reward, CancellationToken cancellationToken = default);
    Task DeleteAsync(RewardId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Reward>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<Reward> GetAsync(RewardId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(RewardId id, CancellationToken cancellationToken = default);
}
