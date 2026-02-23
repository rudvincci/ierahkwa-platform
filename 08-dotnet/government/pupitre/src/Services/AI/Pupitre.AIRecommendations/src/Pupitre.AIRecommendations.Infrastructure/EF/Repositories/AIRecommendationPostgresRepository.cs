using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AIRecommendations.Domain.Entities;
using Pupitre.AIRecommendations.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AIRecommendations.Infrastructure.EF.Repositories;

internal class AIRecommendationPostgresRepository : EFRepository<AIRecommendation, AIRecommendationId>, IAIRecommendationRepository, IDisposable
{
    private readonly AIRecommendationDbContext _entityNameDbContext;
    public AIRecommendationPostgresRepository(AIRecommendationDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<AIRecommendation>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<AIRecommendation> entityNames = ImmutableList.CreateRange(_entityNameDbContext.AIRecommendations.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<AIRecommendation> GetAsync(AIRecommendationId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.AIRecommendations
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(AIRecommendation entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.AIRecommendations.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(AIRecommendationId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.AIRecommendations.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(AIRecommendation entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.AIRecommendations.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(AIRecommendation entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(AIRecommendationId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.AIRecommendations.Single(c => c.Id == id.Value), cancellationToken);
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