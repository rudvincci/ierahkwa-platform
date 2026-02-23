using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.Parents.Application.DTO;
using Pupitre.Parents.Application.Queries;
using Pupitre.Parents.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.Parents.Infrastructure.Mongo.Queries;

internal sealed class BrowseParentsHandler : IQueryHandler<BrowseParents, PagedResult<ParentDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseParentsHandler(IMongoDatabase database, IMongoRepository<ParentDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<ParentDto>?> HandleAsync(BrowseParents query, CancellationToken cancellationToken = default)
    {
        PagedResult<ParentDocument> pagedResult;

        var parentDocuments = _database.GetCollection<ParentDocument>("parents").AsQueryable();
        IEnumerable<ParentDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await parentDocuments.PaginateAsync(query);
            return PagedResult<ParentDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            parentDocuments = parentDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await parentDocuments.PaginateAsync(query);

        return PagedResult<ParentDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseParents query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



