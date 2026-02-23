using System;
using Mamey.CQRS.Queries;
using Pupitre.Lessons.Application.DTO;
using Pupitre.Lessons.Application.Queries;
using Pupitre.Lessons.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.Lessons.Infrastructure.Mongo.Queries;

internal sealed class GetLessonHandler : IQueryHandler<GetLesson, LessonDetailsDto>
{
    private const string collectionName = "lessons";
    private readonly IMongoDatabase _database;

    public GetLessonHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<LessonDetailsDto> HandleAsync(GetLesson query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<LessonDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


