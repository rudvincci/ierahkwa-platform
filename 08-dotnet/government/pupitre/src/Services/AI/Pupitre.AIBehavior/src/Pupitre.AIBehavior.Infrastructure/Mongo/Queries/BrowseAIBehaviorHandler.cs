using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.AIBehavior.Application.DTO;
using Pupitre.AIBehavior.Application.Queries;
using Pupitre.AIBehavior.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.AIBehavior.Infrastructure.Mongo.Queries;

internal sealed class BrowseAIBehaviorHandler : IQueryHandler<BrowseAIBehavior, PagedResult<BehaviorDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseAIBehaviorHandler(IMongoDatabase database, IMongoRepository<BehaviorDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<BehaviorDto>?> HandleAsync(BrowseAIBehavior query, CancellationToken cancellationToken = default)
    {
        PagedResult<BehaviorDocument> pagedResult;

        var behaviorDocuments = _database.GetCollection<BehaviorDocument>("aibehavior").AsQueryable();
        IEnumerable<BehaviorDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await behaviorDocuments.PaginateAsync(query);
            return PagedResult<BehaviorDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            behaviorDocuments = behaviorDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await behaviorDocuments.PaginateAsync(query);

        return PagedResult<BehaviorDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseAIBehavior query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



