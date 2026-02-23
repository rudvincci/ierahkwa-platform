using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Analytics.Domain.Entities;
using Pupitre.Analytics.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Analytics.Infrastructure.PostgreSQL.Repositories;

internal class AnalyticPostgresRepository : EFRepository<Analytic, AnalyticId>, IAnalyticRepository, IDisposable
{
    private readonly AnalyticsDbContext _serviceNameDbContext;
    public AnalyticPostgresRepository(AnalyticsDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<Analytic>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Analytic> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.Analytics.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Analytic> GetAsync(AnalyticId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.Analytics
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Analytic entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.Analytics.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(AnalyticId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.Analytics.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(Analytic entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.Analytics.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Analytic entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(AnalyticId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.Analytics.Single(c => c.Id == id.Value), cancellationToken);
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