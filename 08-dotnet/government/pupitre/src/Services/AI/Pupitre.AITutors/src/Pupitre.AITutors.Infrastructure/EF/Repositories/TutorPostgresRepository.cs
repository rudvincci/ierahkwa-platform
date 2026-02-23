using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AITutors.Domain.Entities;
using Pupitre.AITutors.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AITutors.Infrastructure.EF.Repositories;

internal class TutorPostgresRepository : EFRepository<Tutor, TutorId>, ITutorRepository, IDisposable
{
    private readonly TutorDbContext _entityNameDbContext;
    public TutorPostgresRepository(TutorDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<Tutor>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Tutor> entityNames = ImmutableList.CreateRange(_entityNameDbContext.Tutors.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Tutor> GetAsync(TutorId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.Tutors
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Tutor entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.Tutors.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(TutorId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.Tutors.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(Tutor entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.Tutors.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Tutor entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(TutorId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.Tutors.Single(c => c.Id == id.Value), cancellationToken);
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