using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.Analytics.Application.DTO;
using Pupitre.Analytics.Application.Queries;
using Pupitre.Analytics.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.Analytics.Infrastructure.Mongo.Queries;

internal sealed class BrowseAnalyticsHandler : IQueryHandler<BrowseAnalytics, PagedResult<AnalyticDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseAnalyticsHandler(IMongoDatabase database, IMongoRepository<AnalyticDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<AnalyticDto>?> HandleAsync(BrowseAnalytics query, CancellationToken cancellationToken = default)
    {
        PagedResult<AnalyticDocument> pagedResult;

        var analyticDocuments = _database.GetCollection<AnalyticDocument>("analytics").AsQueryable();
        IEnumerable<AnalyticDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await analyticDocuments.PaginateAsync(query);
            return PagedResult<AnalyticDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            analyticDocuments = analyticDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await analyticDocuments.PaginateAsync(query);

        return PagedResult<AnalyticDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseAnalytics query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



