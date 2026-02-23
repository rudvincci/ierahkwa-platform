using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AIAdaptive.Domain.Entities;
using Pupitre.AIAdaptive.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AIAdaptive.Infrastructure.EF.Repositories;

internal class AdaptiveLearningPostgresRepository : EFRepository<AdaptiveLearning, AdaptiveLearningId>, IAdaptiveLearningRepository, IDisposable
{
    private readonly AdaptiveLearningDbContext _entityNameDbContext;
    public AdaptiveLearningPostgresRepository(AdaptiveLearningDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<AdaptiveLearning>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<AdaptiveLearning> entityNames = ImmutableList.CreateRange(_entityNameDbContext.AdaptiveLearnings.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<AdaptiveLearning> GetAsync(AdaptiveLearningId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.AdaptiveLearnings
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(AdaptiveLearning entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.AdaptiveLearnings.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(AdaptiveLearningId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.AdaptiveLearnings.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(AdaptiveLearning entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.AdaptiveLearnings.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(AdaptiveLearning entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(AdaptiveLearningId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.AdaptiveLearnings.Single(c => c.Id == id.Value), cancellationToken);
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