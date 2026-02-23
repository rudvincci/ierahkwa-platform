using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AIAdaptive.Domain.Entities;
using Pupitre.AIAdaptive.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AIAdaptive.Infrastructure.PostgreSQL.Repositories;

internal class AdaptiveLearningPostgresRepository : EFRepository<AdaptiveLearning, AdaptiveLearningId>, IAdaptiveLearningRepository, IDisposable
{
    private readonly AIAdaptiveDbContext _serviceNameDbContext;
    public AdaptiveLearningPostgresRepository(AIAdaptiveDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<AdaptiveLearning>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<AdaptiveLearning> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.AdaptiveLearnings.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<AdaptiveLearning> GetAsync(AdaptiveLearningId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.AdaptiveLearnings
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(AdaptiveLearning entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.AdaptiveLearnings.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(AdaptiveLearningId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.AdaptiveLearnings.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(AdaptiveLearning entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.AdaptiveLearnings.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(AdaptiveLearning entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(AdaptiveLearningId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.AdaptiveLearnings.Single(c => c.Id == id.Value), cancellationToken);
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