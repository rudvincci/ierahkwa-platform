using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AIAssessments.Domain.Entities;
using Pupitre.AIAssessments.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AIAssessments.Infrastructure.EF.Repositories;

internal class AIAssessmentPostgresRepository : EFRepository<AIAssessment, AIAssessmentId>, IAIAssessmentRepository, IDisposable
{
    private readonly AIAssessmentDbContext _entityNameDbContext;
    public AIAssessmentPostgresRepository(AIAssessmentDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<AIAssessment>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<AIAssessment> entityNames = ImmutableList.CreateRange(_entityNameDbContext.AIAssessments.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<AIAssessment> GetAsync(AIAssessmentId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.AIAssessments
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(AIAssessment entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.AIAssessments.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(AIAssessmentId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.AIAssessments.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(AIAssessment entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.AIAssessments.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(AIAssessment entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(AIAssessmentId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.AIAssessments.Single(c => c.Id == id.Value), cancellationToken);
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