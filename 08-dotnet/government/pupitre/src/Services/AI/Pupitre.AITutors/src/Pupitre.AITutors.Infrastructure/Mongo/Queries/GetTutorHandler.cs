using System;
using Mamey.CQRS.Queries;
using Pupitre.AITutors.Application.DTO;
using Pupitre.AITutors.Application.Queries;
using Pupitre.AITutors.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.AITutors.Infrastructure.Mongo.Queries;

internal sealed class GetTutorHandler : IQueryHandler<GetTutor, TutorDetailsDto>
{
    private const string collectionName = "aitutors";
    private readonly IMongoDatabase _database;

    public GetTutorHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<TutorDetailsDto> HandleAsync(GetTutor query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<TutorDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


