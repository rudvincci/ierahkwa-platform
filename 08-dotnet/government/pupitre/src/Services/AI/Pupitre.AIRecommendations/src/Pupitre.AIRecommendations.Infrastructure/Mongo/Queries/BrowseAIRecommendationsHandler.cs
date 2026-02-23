using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.AIRecommendations.Application.DTO;
using Pupitre.AIRecommendations.Application.Queries;
using Pupitre.AIRecommendations.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.AIRecommendations.Infrastructure.Mongo.Queries;

internal sealed class BrowseAIRecommendationsHandler : IQueryHandler<BrowseAIRecommendations, PagedResult<AIRecommendationDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseAIRecommendationsHandler(IMongoDatabase database, IMongoRepository<AIRecommendationDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<AIRecommendationDto>?> HandleAsync(BrowseAIRecommendations query, CancellationToken cancellationToken = default)
    {
        PagedResult<AIRecommendationDocument> pagedResult;

        var airecommendationDocuments = _database.GetCollection<AIRecommendationDocument>("airecommendations").AsQueryable();
        IEnumerable<AIRecommendationDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await airecommendationDocuments.PaginateAsync(query);
            return PagedResult<AIRecommendationDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            airecommendationDocuments = airecommendationDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await airecommendationDocuments.PaginateAsync(query);

        return PagedResult<AIRecommendationDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseAIRecommendations query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



