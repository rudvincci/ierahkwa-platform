using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.Accessibility.Application.DTO;
using Pupitre.Accessibility.Application.Queries;
using Pupitre.Accessibility.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.Accessibility.Infrastructure.Mongo.Queries;

internal sealed class BrowseAccessibilityHandler : IQueryHandler<BrowseAccessibility, PagedResult<AccessProfileDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseAccessibilityHandler(IMongoDatabase database, IMongoRepository<AccessProfileDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<AccessProfileDto>?> HandleAsync(BrowseAccessibility query, CancellationToken cancellationToken = default)
    {
        PagedResult<AccessProfileDocument> pagedResult;

        var accessprofileDocuments = _database.GetCollection<AccessProfileDocument>("accessibility").AsQueryable();
        IEnumerable<AccessProfileDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await accessprofileDocuments.PaginateAsync(query);
            return PagedResult<AccessProfileDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            accessprofileDocuments = accessprofileDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await accessprofileDocuments.PaginateAsync(query);

        return PagedResult<AccessProfileDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseAccessibility query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



