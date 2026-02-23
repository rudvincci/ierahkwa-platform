using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AITutors.Domain.Entities;
using Pupitre.AITutors.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AITutors.Infrastructure.PostgreSQL.Repositories;

internal class TutorPostgresRepository : EFRepository<Tutor, TutorId>, ITutorRepository, IDisposable
{
    private readonly AITutorsDbContext _serviceNameDbContext;
    public TutorPostgresRepository(AITutorsDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<Tutor>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Tutor> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.Tutors.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Tutor> GetAsync(TutorId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.Tutors
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Tutor entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.Tutors.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(TutorId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.Tutors.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(Tutor entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.Tutors.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Tutor entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(TutorId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.Tutors.Single(c => c.Id == id.Value), cancellationToken);
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