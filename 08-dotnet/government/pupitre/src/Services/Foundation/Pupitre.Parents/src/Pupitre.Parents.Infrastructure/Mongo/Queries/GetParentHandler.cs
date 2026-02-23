using System;
using Mamey.CQRS.Queries;
using Pupitre.Parents.Application.DTO;
using Pupitre.Parents.Application.Queries;
using Pupitre.Parents.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.Parents.Infrastructure.Mongo.Queries;

internal sealed class GetParentHandler : IQueryHandler<GetParent, ParentDetailsDto>
{
    private const string collectionName = "parents";
    private readonly IMongoDatabase _database;

    public GetParentHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<ParentDetailsDto> HandleAsync(GetParent query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<ParentDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


