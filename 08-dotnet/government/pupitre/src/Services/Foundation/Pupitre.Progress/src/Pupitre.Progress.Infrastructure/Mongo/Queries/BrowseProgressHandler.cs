using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.Progress.Application.DTO;
using Pupitre.Progress.Application.Queries;
using Pupitre.Progress.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.Progress.Infrastructure.Mongo.Queries;

internal sealed class BrowseProgressHandler : IQueryHandler<BrowseProgress, PagedResult<LearningProgressDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseProgressHandler(IMongoDatabase database, IMongoRepository<LearningProgressDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<LearningProgressDto>?> HandleAsync(BrowseProgress query, CancellationToken cancellationToken = default)
    {
        PagedResult<LearningProgressDocument> pagedResult;

        var learningprogressDocuments = _database.GetCollection<LearningProgressDocument>("progress").AsQueryable();
        IEnumerable<LearningProgressDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await learningprogressDocuments.PaginateAsync(query);
            return PagedResult<LearningProgressDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            learningprogressDocuments = learningprogressDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await learningprogressDocuments.PaginateAsync(query);

        return PagedResult<LearningProgressDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseProgress query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



