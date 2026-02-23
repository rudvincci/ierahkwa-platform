using System;
using Mamey.CQRS.Queries;
using Pupitre.AIBehavior.Application.DTO;
using Pupitre.AIBehavior.Application.Queries;
using Pupitre.AIBehavior.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.AIBehavior.Infrastructure.Mongo.Queries;

internal sealed class GetBehaviorHandler : IQueryHandler<GetBehavior, BehaviorDetailsDto>
{
    private const string collectionName = "aibehavior";
    private readonly IMongoDatabase _database;

    public GetBehaviorHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<BehaviorDetailsDto> HandleAsync(GetBehavior query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<BehaviorDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


