using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Lessons.Domain.Entities;
using Pupitre.Lessons.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Lessons.Infrastructure.PostgreSQL.Repositories;

internal class LessonPostgresRepository : EFRepository<Lesson, LessonId>, ILessonRepository, IDisposable
{
    private readonly LessonsDbContext _serviceNameDbContext;
    public LessonPostgresRepository(LessonsDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<Lesson>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Lesson> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.Lessons.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Lesson> GetAsync(LessonId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.Lessons
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Lesson entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.Lessons.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(LessonId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.Lessons.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(Lesson entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.Lessons.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Lesson entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(LessonId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.Lessons.Single(c => c.Id == id.Value), cancellationToken);
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