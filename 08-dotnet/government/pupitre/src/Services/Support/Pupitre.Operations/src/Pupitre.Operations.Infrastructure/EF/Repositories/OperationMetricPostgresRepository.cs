using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Operations.Domain.Entities;
using Pupitre.Operations.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Operations.Infrastructure.EF.Repositories;

internal class OperationMetricPostgresRepository : EFRepository<OperationMetric, OperationMetricId>, IOperationMetricRepository, IDisposable
{
    private readonly OperationMetricDbContext _entityNameDbContext;
    public OperationMetricPostgresRepository(OperationMetricDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<OperationMetric>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<OperationMetric> entityNames = ImmutableList.CreateRange(_entityNameDbContext.OperationMetrics.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<OperationMetric> GetAsync(OperationMetricId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.OperationMetrics
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(OperationMetric entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.OperationMetrics.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(OperationMetricId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.OperationMetrics.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(OperationMetric entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.OperationMetrics.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(OperationMetric entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(OperationMetricId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.OperationMetrics.Single(c => c.Id == id.Value), cancellationToken);
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