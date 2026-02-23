using System;
using Mamey.CQRS.Queries;
using Pupitre.AISafety.Application.DTO;
using Pupitre.AISafety.Application.Queries;
using Pupitre.AISafety.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.AISafety.Infrastructure.Mongo.Queries;

internal sealed class GetSafetyCheckHandler : IQueryHandler<GetSafetyCheck, SafetyCheckDetailsDto>
{
    private const string collectionName = "aisafety";
    private readonly IMongoDatabase _database;

    public GetSafetyCheckHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<SafetyCheckDetailsDto> HandleAsync(GetSafetyCheck query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<SafetyCheckDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


