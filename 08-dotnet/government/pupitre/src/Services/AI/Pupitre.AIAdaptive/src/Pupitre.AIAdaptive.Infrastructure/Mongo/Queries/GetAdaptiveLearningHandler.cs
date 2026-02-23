using System;
using Mamey.CQRS.Queries;
using Pupitre.AIAdaptive.Application.DTO;
using Pupitre.AIAdaptive.Application.Queries;
using Pupitre.AIAdaptive.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.AIAdaptive.Infrastructure.Mongo.Queries;

internal sealed class GetAdaptiveLearningHandler : IQueryHandler<GetAdaptiveLearning, AdaptiveLearningDetailsDto>
{
    private const string collectionName = "aiadaptive";
    private readonly IMongoDatabase _database;

    public GetAdaptiveLearningHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<AdaptiveLearningDetailsDto> HandleAsync(GetAdaptiveLearning query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<AdaptiveLearningDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


