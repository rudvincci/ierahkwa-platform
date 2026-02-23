using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AIRecommendations.Domain.Entities;
using Pupitre.AIRecommendations.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AIRecommendations.Infrastructure.PostgreSQL.Repositories;

internal class AIRecommendationPostgresRepository : EFRepository<AIRecommendation, AIRecommendationId>, IAIRecommendationRepository, IDisposable
{
    private readonly AIRecommendationsDbContext _serviceNameDbContext;
    public AIRecommendationPostgresRepository(AIRecommendationsDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<AIRecommendation>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<AIRecommendation> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.AIRecommendations.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<AIRecommendation> GetAsync(AIRecommendationId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.AIRecommendations
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(AIRecommendation entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.AIRecommendations.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(AIRecommendationId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.AIRecommendations.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(AIRecommendation entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.AIRecommendations.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(AIRecommendation entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(AIRecommendationId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.AIRecommendations.Single(c => c.Id == id.Value), cancellationToken);
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