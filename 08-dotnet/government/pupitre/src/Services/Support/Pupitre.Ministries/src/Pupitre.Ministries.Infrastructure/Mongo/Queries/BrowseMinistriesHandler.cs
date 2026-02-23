using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.Ministries.Application.DTO;
using Pupitre.Ministries.Application.Queries;
using Pupitre.Ministries.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.Ministries.Infrastructure.Mongo.Queries;

internal sealed class BrowseMinistriesHandler : IQueryHandler<BrowseMinistries, PagedResult<MinistryDataDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseMinistriesHandler(IMongoDatabase database, IMongoRepository<MinistryDataDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<MinistryDataDto>?> HandleAsync(BrowseMinistries query, CancellationToken cancellationToken = default)
    {
        PagedResult<MinistryDataDocument> pagedResult;

        var ministrydataDocuments = _database.GetCollection<MinistryDataDocument>("ministries").AsQueryable();
        IEnumerable<MinistryDataDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await ministrydataDocuments.PaginateAsync(query);
            return PagedResult<MinistryDataDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            ministrydataDocuments = ministrydataDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await ministrydataDocuments.PaginateAsync(query);

        return PagedResult<MinistryDataDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseMinistries query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



