using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Mamey.ServiceName.Application.DTO;
using Mamey.ServiceName.Application.Queries;
using Mamey.ServiceName.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Mamey.ServiceName.Infrastructure.Mongo.Queries;

internal sealed class BrowseServiceNameHandler : IQueryHandler<BrowseServiceName, PagedResult<EntityNameDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseServiceNameHandler(IMongoDatabase database, IMongoRepository<EntityNameDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<EntityNameDto>?> HandleAsync(BrowseServiceName query, CancellationToken cancellationToken = default)
    {
        PagedResult<EntityNameDocument> pagedResult;

        var entitynameDocuments = _database.GetCollection<EntityNameDocument>("servicename").AsQueryable();
        IEnumerable<EntityNameDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await entitynameDocuments.PaginateAsync(query);
            return PagedResult<EntityNameDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            entitynameDocuments = entitynameDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await entitynameDocuments.PaginateAsync(query);

        return PagedResult<EntityNameDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseServiceName query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



