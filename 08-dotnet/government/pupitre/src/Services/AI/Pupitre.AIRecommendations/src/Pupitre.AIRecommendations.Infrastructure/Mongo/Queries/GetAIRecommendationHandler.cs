using System;
using Mamey.CQRS.Queries;
using Pupitre.AIRecommendations.Application.DTO;
using Pupitre.AIRecommendations.Application.Queries;
using Pupitre.AIRecommendations.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.AIRecommendations.Infrastructure.Mongo.Queries;

internal sealed class GetAIRecommendationHandler : IQueryHandler<GetAIRecommendation, AIRecommendationDetailsDto>
{
    private const string collectionName = "airecommendations";
    private readonly IMongoDatabase _database;

    public GetAIRecommendationHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<AIRecommendationDetailsDto> HandleAsync(GetAIRecommendation query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<AIRecommendationDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


