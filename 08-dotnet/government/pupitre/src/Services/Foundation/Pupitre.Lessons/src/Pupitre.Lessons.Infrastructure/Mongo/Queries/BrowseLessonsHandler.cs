using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.Lessons.Application.DTO;
using Pupitre.Lessons.Application.Queries;
using Pupitre.Lessons.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.Lessons.Infrastructure.Mongo.Queries;

internal sealed class BrowseLessonsHandler : IQueryHandler<BrowseLessons, PagedResult<LessonDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseLessonsHandler(IMongoDatabase database, IMongoRepository<LessonDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<LessonDto>?> HandleAsync(BrowseLessons query, CancellationToken cancellationToken = default)
    {
        PagedResult<LessonDocument> pagedResult;

        var lessonDocuments = _database.GetCollection<LessonDocument>("lessons").AsQueryable();
        IEnumerable<LessonDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await lessonDocuments.PaginateAsync(query);
            return PagedResult<LessonDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            lessonDocuments = lessonDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await lessonDocuments.PaginateAsync(query);

        return PagedResult<LessonDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseLessons query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



