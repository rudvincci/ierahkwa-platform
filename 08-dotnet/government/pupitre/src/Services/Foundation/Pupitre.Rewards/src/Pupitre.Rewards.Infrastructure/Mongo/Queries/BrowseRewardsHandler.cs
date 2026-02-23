using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.Rewards.Application.DTO;
using Pupitre.Rewards.Application.Queries;
using Pupitre.Rewards.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.Rewards.Infrastructure.Mongo.Queries;

internal sealed class BrowseRewardsHandler : IQueryHandler<BrowseRewards, PagedResult<RewardDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseRewardsHandler(IMongoDatabase database, IMongoRepository<RewardDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<RewardDto>?> HandleAsync(BrowseRewards query, CancellationToken cancellationToken = default)
    {
        PagedResult<RewardDocument> pagedResult;

        var rewardDocuments = _database.GetCollection<RewardDocument>("rewards").AsQueryable();
        IEnumerable<RewardDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await rewardDocuments.PaginateAsync(query);
            return PagedResult<RewardDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            rewardDocuments = rewardDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await rewardDocuments.PaginateAsync(query);

        return PagedResult<RewardDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseRewards query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



