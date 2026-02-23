using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AISpeech.Domain.Entities;
using Pupitre.AISpeech.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AISpeech.Infrastructure.PostgreSQL.Repositories;

internal class SpeechRequestPostgresRepository : EFRepository<SpeechRequest, SpeechRequestId>, ISpeechRequestRepository, IDisposable
{
    private readonly AISpeechDbContext _serviceNameDbContext;
    public SpeechRequestPostgresRepository(AISpeechDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<SpeechRequest>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<SpeechRequest> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.SpeechRequests.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<SpeechRequest> GetAsync(SpeechRequestId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.SpeechRequests
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(SpeechRequest entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.SpeechRequests.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(SpeechRequestId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.SpeechRequests.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(SpeechRequest entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.SpeechRequests.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(SpeechRequest entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(SpeechRequestId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.SpeechRequests.Single(c => c.Id == id.Value), cancellationToken);
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