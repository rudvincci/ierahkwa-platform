using System;
using Mamey.CQRS.Queries;
using Pupitre.Educators.Application.DTO;
using Pupitre.Educators.Application.Queries;
using Pupitre.Educators.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.Educators.Infrastructure.Mongo.Queries;

internal sealed class GetEducatorHandler : IQueryHandler<GetEducator, EducatorDetailsDto>
{
    private const string collectionName = "educators";
    private readonly IMongoDatabase _database;

    public GetEducatorHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<EducatorDetailsDto> HandleAsync(GetEducator query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<EducatorDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


