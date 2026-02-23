using System;
using Mamey.CQRS.Queries;
using Pupitre.Accessibility.Application.DTO;
using Pupitre.Accessibility.Application.Queries;
using Pupitre.Accessibility.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.Accessibility.Infrastructure.Mongo.Queries;

internal sealed class GetAccessProfileHandler : IQueryHandler<GetAccessProfile, AccessProfileDetailsDto>
{
    private const string collectionName = "accessibility";
    private readonly IMongoDatabase _database;

    public GetAccessProfileHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<AccessProfileDetailsDto> HandleAsync(GetAccessProfile query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<AccessProfileDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


