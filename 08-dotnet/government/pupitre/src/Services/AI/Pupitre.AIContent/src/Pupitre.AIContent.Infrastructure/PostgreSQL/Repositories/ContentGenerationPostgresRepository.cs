using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AIContent.Domain.Entities;
using Pupitre.AIContent.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AIContent.Infrastructure.PostgreSQL.Repositories;

internal class ContentGenerationPostgresRepository : EFRepository<ContentGeneration, ContentGenerationId>, IContentGenerationRepository, IDisposable
{
    private readonly AIContentDbContext _serviceNameDbContext;
    public ContentGenerationPostgresRepository(AIContentDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<ContentGeneration>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<ContentGeneration> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.ContentGenerations.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<ContentGeneration> GetAsync(ContentGenerationId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.ContentGenerations
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(ContentGeneration entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.ContentGenerations.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(ContentGenerationId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.ContentGenerations.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(ContentGeneration entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.ContentGenerations.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(ContentGeneration entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(ContentGenerationId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.ContentGenerations.Single(c => c.Id == id.Value), cancellationToken);
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