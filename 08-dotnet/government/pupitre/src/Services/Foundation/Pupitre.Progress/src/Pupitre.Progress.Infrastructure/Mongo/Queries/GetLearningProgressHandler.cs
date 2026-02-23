using System;
using Mamey.CQRS.Queries;
using Pupitre.Progress.Application.DTO;
using Pupitre.Progress.Application.Queries;
using Pupitre.Progress.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.Progress.Infrastructure.Mongo.Queries;

internal sealed class GetLearningProgressHandler : IQueryHandler<GetLearningProgress, LearningProgressDetailsDto>
{
    private const string collectionName = "progress";
    private readonly IMongoDatabase _database;

    public GetLearningProgressHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<LearningProgressDetailsDto> HandleAsync(GetLearningProgress query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<LearningProgressDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


