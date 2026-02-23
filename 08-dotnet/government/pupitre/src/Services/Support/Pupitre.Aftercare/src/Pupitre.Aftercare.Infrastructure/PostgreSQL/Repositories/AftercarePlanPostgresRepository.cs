using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Aftercare.Domain.Entities;
using Pupitre.Aftercare.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Aftercare.Infrastructure.PostgreSQL.Repositories;

internal class AftercarePlanPostgresRepository : EFRepository<AftercarePlan, AftercarePlanId>, IAftercarePlanRepository, IDisposable
{
    private readonly AftercareDbContext _serviceNameDbContext;
    public AftercarePlanPostgresRepository(AftercareDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<AftercarePlan>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<AftercarePlan> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.AftercarePlans.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<AftercarePlan> GetAsync(AftercarePlanId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.AftercarePlans
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(AftercarePlan entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.AftercarePlans.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(AftercarePlanId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.AftercarePlans.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(AftercarePlan entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.AftercarePlans.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(AftercarePlan entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(AftercarePlanId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.AftercarePlans.Single(c => c.Id == id.Value), cancellationToken);
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