using System;
using Mamey.CQRS.Queries;
using Pupitre.AITranslation.Application.DTO;
using Pupitre.AITranslation.Application.Queries;
using Pupitre.AITranslation.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.AITranslation.Infrastructure.Mongo.Queries;

internal sealed class GetTranslationRequestHandler : IQueryHandler<GetTranslationRequest, TranslationRequestDetailsDto>
{
    private const string collectionName = "aitranslation";
    private readonly IMongoDatabase _database;

    public GetTranslationRequestHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<TranslationRequestDetailsDto> HandleAsync(GetTranslationRequest query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<TranslationRequestDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


