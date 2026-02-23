using System;
using Mamey.CQRS.Queries;
using Pupitre.Users.Application.DTO;
using Pupitre.Users.Application.Queries;
using Pupitre.Users.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.Users.Infrastructure.Mongo.Queries;

internal sealed class GetUserHandler : IQueryHandler<GetUser, UserDetailsDto>
{
    private const string collectionName = "users";
    private readonly IMongoDatabase _database;

    public GetUserHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<UserDetailsDto> HandleAsync(GetUser query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<UserDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


