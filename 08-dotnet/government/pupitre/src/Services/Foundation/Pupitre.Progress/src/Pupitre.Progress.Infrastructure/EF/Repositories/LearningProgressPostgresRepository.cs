using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Progress.Domain.Entities;
using Pupitre.Progress.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Progress.Infrastructure.EF.Repositories;

internal class LearningProgressPostgresRepository : EFRepository<LearningProgress, LearningProgressId>, ILearningProgressRepository, IDisposable
{
    private readonly LearningProgressDbContext _entityNameDbContext;
    public LearningProgressPostgresRepository(LearningProgressDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<LearningProgress>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<LearningProgress> entityNames = ImmutableList.CreateRange(_entityNameDbContext.LearningProgresss.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<LearningProgress> GetAsync(LearningProgressId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.LearningProgresss
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(LearningProgress entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.LearningProgresss.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(LearningProgressId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.LearningProgresss.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(LearningProgress entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.LearningProgresss.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(LearningProgress entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(LearningProgressId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.LearningProgresss.Single(c => c.Id == id.Value), cancellationToken);
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