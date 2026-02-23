using System;
using Mamey.CQRS.Queries;
using Pupitre.AISpeech.Application.DTO;
using Pupitre.AISpeech.Application.Queries;
using Pupitre.AISpeech.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.AISpeech.Infrastructure.Mongo.Queries;

internal sealed class GetSpeechRequestHandler : IQueryHandler<GetSpeechRequest, SpeechRequestDetailsDto>
{
    private const string collectionName = "aispeech";
    private readonly IMongoDatabase _database;

    public GetSpeechRequestHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<SpeechRequestDetailsDto> HandleAsync(GetSpeechRequest query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<SpeechRequestDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


