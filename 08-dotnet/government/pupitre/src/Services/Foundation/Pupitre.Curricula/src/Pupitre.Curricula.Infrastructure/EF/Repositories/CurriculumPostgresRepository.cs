using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Curricula.Domain.Entities;
using Pupitre.Curricula.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Curricula.Infrastructure.EF.Repositories;

internal class CurriculumPostgresRepository : EFRepository<Curriculum, CurriculumId>, ICurriculumRepository, IDisposable
{
    private readonly CurriculumDbContext _entityNameDbContext;
    public CurriculumPostgresRepository(CurriculumDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<Curriculum>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Curriculum> entityNames = ImmutableList.CreateRange(_entityNameDbContext.Curriculums.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Curriculum> GetAsync(CurriculumId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.Curriculums
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Curriculum entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.Curriculums.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(CurriculumId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.Curriculums.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(Curriculum entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.Curriculums.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Curriculum entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(CurriculumId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.Curriculums.Single(c => c.Id == id.Value), cancellationToken);
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