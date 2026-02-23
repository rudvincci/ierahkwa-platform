using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.AIAdaptive.Application.DTO;
using Pupitre.AIAdaptive.Application.Queries;
using Pupitre.AIAdaptive.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.AIAdaptive.Infrastructure.Mongo.Queries;

internal sealed class BrowseAIAdaptiveHandler : IQueryHandler<BrowseAIAdaptive, PagedResult<AdaptiveLearningDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseAIAdaptiveHandler(IMongoDatabase database, IMongoRepository<AdaptiveLearningDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<AdaptiveLearningDto>?> HandleAsync(BrowseAIAdaptive query, CancellationToken cancellationToken = default)
    {
        PagedResult<AdaptiveLearningDocument> pagedResult;

        var adaptivelearningDocuments = _database.GetCollection<AdaptiveLearningDocument>("aiadaptive").AsQueryable();
        IEnumerable<AdaptiveLearningDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await adaptivelearningDocuments.PaginateAsync(query);
            return PagedResult<AdaptiveLearningDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            adaptivelearningDocuments = adaptivelearningDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await adaptivelearningDocuments.PaginateAsync(query);

        return PagedResult<AdaptiveLearningDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseAIAdaptive query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



