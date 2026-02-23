using System;
using Mamey.CQRS.Queries;
using Pupitre.Curricula.Application.DTO;
using Pupitre.Curricula.Application.Queries;
using Pupitre.Curricula.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.Curricula.Infrastructure.Mongo.Queries;

internal sealed class GetCurriculumHandler : IQueryHandler<GetCurriculum, CurriculumDetailsDto>
{
    private const string collectionName = "curricula";
    private readonly IMongoDatabase _database;

    public GetCurriculumHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<CurriculumDetailsDto> HandleAsync(GetCurriculum query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<CurriculumDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


