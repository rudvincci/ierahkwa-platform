using System;
using Mamey.CQRS.Queries;
using Pupitre.Ministries.Application.DTO;
using Pupitre.Ministries.Application.Queries;
using Pupitre.Ministries.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.Ministries.Infrastructure.Mongo.Queries;

internal sealed class GetMinistryDataHandler : IQueryHandler<GetMinistryData, MinistryDataDetailsDto>
{
    private const string collectionName = "ministries";
    private readonly IMongoDatabase _database;

    public GetMinistryDataHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<MinistryDataDetailsDto> HandleAsync(GetMinistryData query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<MinistryDataDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


