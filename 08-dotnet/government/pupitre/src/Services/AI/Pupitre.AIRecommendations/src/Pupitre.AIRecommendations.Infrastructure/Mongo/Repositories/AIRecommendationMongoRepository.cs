using System;
using Mamey.Persistence.MongoDB;
using Pupitre.AIRecommendations.Domain.Repositories;
using Pupitre.AIRecommendations.Domain.Entities;
using Pupitre.AIRecommendations.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.AIRecommendations.Infrastructure.Mongo.Repositories;

internal class AIRecommendationMongoRepository : IAIRecommendationRepository
{
    private readonly IMongoRepository<AIRecommendationDocument, Guid> _repository;

    public AIRecommendationMongoRepository(IMongoRepository<AIRecommendationDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(AIRecommendation airecommendation, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new AIRecommendationDocument(airecommendation));

    public async Task UpdateAsync(AIRecommendation airecommendation, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new AIRecommendationDocument(airecommendation));
    public async Task DeleteAsync(AIRecommendationId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<AIRecommendation>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<AIRecommendation> GetAsync(AIRecommendationId id, CancellationToken cancellationToken = default)
    {
        var airecommendation = await _repository.GetAsync(id.Value);
        return airecommendation?.AsEntity();
    }
    public async Task<bool> ExistsAsync(AIRecommendationId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



