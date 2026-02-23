using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Analytics.Domain.Entities;
using Pupitre.Analytics.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Analytics.Infrastructure.EF.Repositories;

internal class AnalyticPostgresRepository : EFRepository<Analytic, AnalyticId>, IAnalyticRepository, IDisposable
{
    private readonly AnalyticDbContext _entityNameDbContext;
    public AnalyticPostgresRepository(AnalyticDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<Analytic>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Analytic> entityNames = ImmutableList.CreateRange(_entityNameDbContext.Analytics.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Analytic> GetAsync(AnalyticId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.Analytics
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Analytic entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.Analytics.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(AnalyticId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.Analytics.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(Analytic entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.Analytics.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Analytic entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(AnalyticId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.Analytics.Single(c => c.Id == id.Value), cancellationToken);
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