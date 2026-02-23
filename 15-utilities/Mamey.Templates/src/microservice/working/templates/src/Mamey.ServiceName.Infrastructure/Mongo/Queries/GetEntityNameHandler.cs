using System;
using Mamey.CQRS.Queries;
using Mamey.ServiceName.Application.DTO;
using Mamey.ServiceName.Application.Queries;
using Mamey.ServiceName.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Mamey.ServiceName.Infrastructure.Mongo.Queries;

internal sealed class GetEntityNameHandler : IQueryHandler<GetEntityName, EntityNameDetailsDto>
{
    private const string collectionName = "servicename";
    private readonly IMongoDatabase _database;

    public GetEntityNameHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<EntityNameDetailsDto> HandleAsync(GetEntityName query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<EntityNameDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


