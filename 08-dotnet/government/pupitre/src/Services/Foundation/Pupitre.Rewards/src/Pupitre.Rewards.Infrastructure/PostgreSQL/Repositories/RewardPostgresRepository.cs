using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Rewards.Domain.Entities;
using Pupitre.Rewards.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Rewards.Infrastructure.PostgreSQL.Repositories;

internal class RewardPostgresRepository : EFRepository<Reward, RewardId>, IRewardRepository, IDisposable
{
    private readonly RewardsDbContext _serviceNameDbContext;
    public RewardPostgresRepository(RewardsDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<Reward>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Reward> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.Rewards.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Reward> GetAsync(RewardId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.Rewards
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Reward entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.Rewards.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(RewardId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.Rewards.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(Reward entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.Rewards.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Reward entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(RewardId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.Rewards.Single(c => c.Id == id.Value), cancellationToken);
    }
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _serviceNameDbContext.Dispose();
            }
        }
        this.disposed = true;
    }
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}