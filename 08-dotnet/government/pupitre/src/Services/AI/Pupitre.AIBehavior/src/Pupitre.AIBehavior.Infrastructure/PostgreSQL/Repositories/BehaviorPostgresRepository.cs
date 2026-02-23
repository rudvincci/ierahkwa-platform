using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AIBehavior.Domain.Entities;
using Pupitre.AIBehavior.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AIBehavior.Infrastructure.PostgreSQL.Repositories;

internal class BehaviorPostgresRepository : EFRepository<Behavior, BehaviorId>, IBehaviorRepository, IDisposable
{
    private readonly AIBehaviorDbContext _serviceNameDbContext;
    public BehaviorPostgresRepository(AIBehaviorDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<Behavior>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Behavior> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.Behaviors.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Behavior> GetAsync(BehaviorId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.Behaviors
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Behavior entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.Behaviors.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(BehaviorId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.Behaviors.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(Behavior entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.Behaviors.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Behavior entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(BehaviorId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.Behaviors.Single(c => c.Id == id.Value), cancellationToken);
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