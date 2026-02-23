using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AITranslation.Domain.Entities;
using Pupitre.AITranslation.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AITranslation.Infrastructure.PostgreSQL.Repositories;

internal class TranslationRequestPostgresRepository : EFRepository<TranslationRequest, TranslationRequestId>, ITranslationRequestRepository, IDisposable
{
    private readonly AITranslationDbContext _serviceNameDbContext;
    public TranslationRequestPostgresRepository(AITranslationDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<TranslationRequest>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<TranslationRequest> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.TranslationRequests.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<TranslationRequest> GetAsync(TranslationRequestId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.TranslationRequests
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(TranslationRequest entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.TranslationRequests.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(TranslationRequestId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.TranslationRequests.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(TranslationRequest entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.TranslationRequests.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(TranslationRequest entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(TranslationRequestId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.TranslationRequests.Single(c => c.Id == id.Value), cancellationToken);
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