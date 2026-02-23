using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.Fundraising.Application.DTO;
using Pupitre.Fundraising.Application.Queries;
using Pupitre.Fundraising.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.Fundraising.Infrastructure.Mongo.Queries;

internal sealed class BrowseFundraisingHandler : IQueryHandler<BrowseFundraising, PagedResult<CampaignDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseFundraisingHandler(IMongoDatabase database, IMongoRepository<CampaignDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<CampaignDto>?> HandleAsync(BrowseFundraising query, CancellationToken cancellationToken = default)
    {
        PagedResult<CampaignDocument> pagedResult;

        var campaignDocuments = _database.GetCollection<CampaignDocument>("fundraising").AsQueryable();
        IEnumerable<CampaignDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await campaignDocuments.PaginateAsync(query);
            return PagedResult<CampaignDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            campaignDocuments = campaignDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await campaignDocuments.PaginateAsync(query);

        return PagedResult<CampaignDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseFundraising query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



