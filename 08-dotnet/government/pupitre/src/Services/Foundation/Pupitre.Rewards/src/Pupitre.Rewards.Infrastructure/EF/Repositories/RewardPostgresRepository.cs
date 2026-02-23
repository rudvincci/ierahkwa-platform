using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Rewards.Domain.Entities;
using Pupitre.Rewards.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Rewards.Infrastructure.EF.Repositories;

internal class RewardPostgresRepository : EFRepository<Reward, RewardId>, IRewardRepository, IDisposable
{
    private readonly RewardDbContext _entityNameDbContext;
    public RewardPostgresRepository(RewardDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<Reward>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Reward> entityNames = ImmutableList.CreateRange(_entityNameDbContext.Rewards.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Reward> GetAsync(RewardId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.Rewards
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Reward entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.Rewards.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(RewardId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.Rewards.Any(c => c.Id == id.Value));
    public async Task UpdateAsync(Reward entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.Rewards.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Reward entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(RewardId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.Rewards.Single(c => c.Id == id.Value), cancellationToken);
    }
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _entityNameDbContext.Dispose();
            }
        }
        this.disposed = true;
    }
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}