using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Operations.Domain.Entities;
using Pupitre.Operations.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Operations.Infrastructure.PostgreSQL.Repositories;

internal class OperationMetricPostgresRepository : EFRepository<OperationMetric, OperationMetricId>, IOperationMetricRepository, IDisposable
{
    private readonly OperationsDbContext _serviceNameDbContext;
    public OperationMetricPostgresRepository(OperationsDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<OperationMetric>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<OperationMetric> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.OperationMetrics.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<OperationMetric> GetAsync(OperationMetricId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.OperationMetrics
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(OperationMetric entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.OperationMetrics.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(OperationMetricId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.OperationMetrics.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(OperationMetric entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.OperationMetrics.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(OperationMetric entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(OperationMetricId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.OperationMetrics.Single(c => c.Id == id.Value), cancellationToken);
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