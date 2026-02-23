using System;
using Mamey.CQRS.Queries;
using Pupitre.AIContent.Application.DTO;
using Pupitre.AIContent.Application.Queries;
using Pupitre.AIContent.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.AIContent.Infrastructure.Mongo.Queries;

internal sealed class GetContentGenerationHandler : IQueryHandler<GetContentGeneration, ContentGenerationDetailsDto>
{
    private const string collectionName = "aicontent";
    private readonly IMongoDatabase _database;

    public GetContentGenerationHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<ContentGenerationDetailsDto> HandleAsync(GetContentGeneration query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<ContentGenerationDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


