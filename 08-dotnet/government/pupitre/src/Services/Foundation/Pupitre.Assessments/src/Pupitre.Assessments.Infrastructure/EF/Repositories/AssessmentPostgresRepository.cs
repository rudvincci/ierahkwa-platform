using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Assessments.Domain.Entities;
using Pupitre.Assessments.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Assessments.Infrastructure.EF.Repositories;

internal class AssessmentPostgresRepository : EFRepository<Assessment, AssessmentId>, IAssessmentRepository, IDisposable
{
    private readonly AssessmentDbContext _entityNameDbContext;
    public AssessmentPostgresRepository(AssessmentDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<Assessment>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Assessment> entityNames = ImmutableList.CreateRange(_entityNameDbContext.Assessments.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Assessment> GetAsync(AssessmentId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.Assessments
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Assessment entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.Assessments.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(AssessmentId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.Assessments.Any(c => c.Id == id.Value));
    public async Task UpdateAsync(Assessment entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.Assessments.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Assessment entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(AssessmentId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.Assessments.Single(c => c.Id == id.Value), cancellationToken);
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