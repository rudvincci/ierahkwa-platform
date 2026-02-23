using System;
using Mamey.CQRS.Queries;
using Pupitre.GLEs.Application.DTO;
using Pupitre.GLEs.Application.Queries;
using Pupitre.GLEs.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.GLEs.Infrastructure.Mongo.Queries;

internal sealed class GetGLEHandler : IQueryHandler<GetGLE, GLEDetailsDto>
{
    private const string collectionName = "gles";
    private readonly IMongoDatabase _database;

    public GetGLEHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<GLEDetailsDto> HandleAsync(GetGLE query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<GLEDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


