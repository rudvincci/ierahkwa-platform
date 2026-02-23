using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AIBehavior.Domain.Entities;
using Pupitre.AIBehavior.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AIBehavior.Infrastructure.EF.Repositories;

internal class BehaviorPostgresRepository : EFRepository<Behavior, BehaviorId>, IBehaviorRepository, IDisposable
{
    private readonly BehaviorDbContext _entityNameDbContext;
    public BehaviorPostgresRepository(BehaviorDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<Behavior>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Behavior> entityNames = ImmutableList.CreateRange(_entityNameDbContext.Behaviors.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Behavior> GetAsync(BehaviorId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.Behaviors
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Behavior entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.Behaviors.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(BehaviorId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.Behaviors.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(Behavior entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.Behaviors.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Behavior entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(BehaviorId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.Behaviors.Single(c => c.Id == id.Value), cancellationToken);
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