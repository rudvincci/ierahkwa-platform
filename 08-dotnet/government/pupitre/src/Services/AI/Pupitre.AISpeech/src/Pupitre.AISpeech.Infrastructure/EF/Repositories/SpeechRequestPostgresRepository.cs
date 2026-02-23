using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AISpeech.Domain.Entities;
using Pupitre.AISpeech.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AISpeech.Infrastructure.EF.Repositories;

internal class SpeechRequestPostgresRepository : EFRepository<SpeechRequest, SpeechRequestId>, ISpeechRequestRepository, IDisposable
{
    private readonly SpeechRequestDbContext _entityNameDbContext;
    public SpeechRequestPostgresRepository(SpeechRequestDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<SpeechRequest>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<SpeechRequest> entityNames = ImmutableList.CreateRange(_entityNameDbContext.SpeechRequests.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<SpeechRequest> GetAsync(SpeechRequestId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.SpeechRequests
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(SpeechRequest entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.SpeechRequests.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(SpeechRequestId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.SpeechRequests.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(SpeechRequest entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.SpeechRequests.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(SpeechRequest entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(SpeechRequestId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.SpeechRequests.Single(c => c.Id == id.Value), cancellationToken);
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