using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Curricula.Domain.Entities;
using Pupitre.Curricula.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Curricula.Infrastructure.PostgreSQL.Repositories;

internal class CurriculumPostgresRepository : EFRepository<Curriculum, CurriculumId>, ICurriculumRepository, IDisposable
{
    private readonly CurriculaDbContext _serviceNameDbContext;
    public CurriculumPostgresRepository(CurriculaDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<Curriculum>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Curriculum> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.Curriculums.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Curriculum> GetAsync(CurriculumId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.Curriculums
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Curriculum entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.Curriculums.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(CurriculumId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.Curriculums.Any(c => c.Id == id.Value));
    public async Task UpdateAsync(Curriculum entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.Curriculums.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Curriculum entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(CurriculumId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.Curriculums.Single(c => c.Id == id.Value), cancellationToken);
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