using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Lessons.Domain.Entities;
using Pupitre.Lessons.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Lessons.Infrastructure.EF.Repositories;

internal class LessonPostgresRepository : EFRepository<Lesson, LessonId>, ILessonRepository, IDisposable
{
    private readonly LessonDbContext _entityNameDbContext;
    public LessonPostgresRepository(LessonDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<Lesson>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Lesson> entityNames = ImmutableList.CreateRange(_entityNameDbContext.Lessons.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Lesson> GetAsync(LessonId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.Lessons
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Lesson entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.Lessons.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(LessonId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.Lessons.Any(c => c.Id == id.Value));
    public async Task UpdateAsync(Lesson entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.Lessons.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Lesson entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(LessonId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.Lessons.Single(c => c.Id == id.Value), cancellationToken);
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