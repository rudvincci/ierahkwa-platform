using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AITranslation.Domain.Entities;
using Pupitre.AITranslation.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AITranslation.Infrastructure.EF.Repositories;

internal class TranslationRequestPostgresRepository : EFRepository<TranslationRequest, TranslationRequestId>, ITranslationRequestRepository, IDisposable
{
    private readonly TranslationRequestDbContext _entityNameDbContext;
    public TranslationRequestPostgresRepository(TranslationRequestDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<TranslationRequest>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<TranslationRequest> entityNames = ImmutableList.CreateRange(_entityNameDbContext.TranslationRequests.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<TranslationRequest> GetAsync(TranslationRequestId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.TranslationRequests
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(TranslationRequest entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.TranslationRequests.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(TranslationRequestId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.TranslationRequests.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(TranslationRequest entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.TranslationRequests.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(TranslationRequest entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(TranslationRequestId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.TranslationRequests.Single(c => c.Id == id.Value), cancellationToken);
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