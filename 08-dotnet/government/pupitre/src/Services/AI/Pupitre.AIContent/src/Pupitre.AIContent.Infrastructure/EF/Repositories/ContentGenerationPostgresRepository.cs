using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AIContent.Domain.Entities;
using Pupitre.AIContent.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AIContent.Infrastructure.EF.Repositories;

internal class ContentGenerationPostgresRepository : EFRepository<ContentGeneration, ContentGenerationId>, IContentGenerationRepository, IDisposable
{
    private readonly ContentGenerationDbContext _entityNameDbContext;
    public ContentGenerationPostgresRepository(ContentGenerationDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<ContentGeneration>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<ContentGeneration> entityNames = ImmutableList.CreateRange(_entityNameDbContext.ContentGenerations.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<ContentGeneration> GetAsync(ContentGenerationId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.ContentGenerations
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(ContentGeneration entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.ContentGenerations.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(ContentGenerationId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.ContentGenerations.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(ContentGeneration entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.ContentGenerations.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(ContentGeneration entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(ContentGenerationId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.ContentGenerations.Single(c => c.Id == id.Value), cancellationToken);
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