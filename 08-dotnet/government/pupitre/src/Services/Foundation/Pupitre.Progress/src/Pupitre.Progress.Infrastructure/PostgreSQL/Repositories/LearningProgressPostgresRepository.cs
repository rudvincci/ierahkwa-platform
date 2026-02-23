using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Progress.Domain.Entities;
using Pupitre.Progress.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Progress.Infrastructure.PostgreSQL.Repositories;

internal class LearningProgressPostgresRepository : EFRepository<LearningProgress, LearningProgressId>, ILearningProgressRepository, IDisposable
{
    private readonly ProgressDbContext _serviceNameDbContext;
    public LearningProgressPostgresRepository(ProgressDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<LearningProgress>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<LearningProgress> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.LearningProgress.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<LearningProgress> GetAsync(LearningProgressId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.LearningProgress
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(LearningProgress entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.LearningProgress.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(LearningProgressId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.LearningProgress.Any(c => c.Id == id.Value));
    public async Task UpdateAsync(LearningProgress entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.LearningProgress.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(LearningProgress entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(LearningProgressId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.LearningProgress.Single(c => c.Id == id.Value), cancellationToken);
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