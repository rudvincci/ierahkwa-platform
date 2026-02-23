using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.Operations.Application.DTO;
using Pupitre.Operations.Application.Queries;
using Pupitre.Operations.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.Operations.Infrastructure.Mongo.Queries;

internal sealed class BrowseOperationsHandler : IQueryHandler<BrowseOperations, PagedResult<OperationMetricDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseOperationsHandler(IMongoDatabase database, IMongoRepository<OperationMetricDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<OperationMetricDto>?> HandleAsync(BrowseOperations query, CancellationToken cancellationToken = default)
    {
        PagedResult<OperationMetricDocument> pagedResult;

        var operationmetricDocuments = _database.GetCollection<OperationMetricDocument>("operations").AsQueryable();
        IEnumerable<OperationMetricDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await operationmetricDocuments.PaginateAsync(query);
            return PagedResult<OperationMetricDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            operationmetricDocuments = operationmetricDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await operationmetricDocuments.PaginateAsync(query);

        return PagedResult<OperationMetricDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseOperations query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



