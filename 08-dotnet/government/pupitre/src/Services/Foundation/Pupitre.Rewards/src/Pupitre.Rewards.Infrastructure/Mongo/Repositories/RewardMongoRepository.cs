using System;
using Mamey.Persistence.MongoDB;
using Pupitre.Rewards.Domain.Repositories;
using Pupitre.Rewards.Domain.Entities;
using Pupitre.Rewards.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.Rewards.Infrastructure.Mongo.Repositories;

internal class RewardMongoRepository : IRewardRepository
{
    private readonly IMongoRepository<RewardDocument, Guid> _repository;

    public RewardMongoRepository(IMongoRepository<RewardDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Reward reward, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new RewardDocument(reward));

    public async Task UpdateAsync(Reward reward, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new RewardDocument(reward));
    public async Task DeleteAsync(RewardId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<Reward>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<Reward> GetAsync(RewardId id, CancellationToken cancellationToken = default)
    {
        var reward = await _repository.GetAsync(id.Value);
        return reward?.AsEntity();
    }
    public async Task<bool> ExistsAsync(RewardId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



