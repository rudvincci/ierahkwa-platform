using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Aftercare.Domain.Entities;
using Pupitre.Aftercare.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Aftercare.Infrastructure.EF.Repositories;

internal class AftercarePlanPostgresRepository : EFRepository<AftercarePlan, AftercarePlanId>, IAftercarePlanRepository, IDisposable
{
    private readonly AftercarePlanDbContext _entityNameDbContext;
    public AftercarePlanPostgresRepository(AftercarePlanDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<AftercarePlan>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<AftercarePlan> entityNames = ImmutableList.CreateRange(_entityNameDbContext.AftercarePlans.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<AftercarePlan> GetAsync(AftercarePlanId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.AftercarePlans
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(AftercarePlan entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.AftercarePlans.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(AftercarePlanId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.AftercarePlans.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(AftercarePlan entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.AftercarePlans.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(AftercarePlan entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(AftercarePlanId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.AftercarePlans.Single(c => c.Id == id.Value), cancellationToken);
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