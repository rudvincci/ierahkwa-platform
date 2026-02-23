using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.Compliance.Application.DTO;
using Pupitre.Compliance.Application.Queries;
using Pupitre.Compliance.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.Compliance.Infrastructure.Mongo.Queries;

internal sealed class BrowseComplianceHandler : IQueryHandler<BrowseCompliance, PagedResult<ComplianceRecordDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseComplianceHandler(IMongoDatabase database, IMongoRepository<ComplianceRecordDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<ComplianceRecordDto>?> HandleAsync(BrowseCompliance query, CancellationToken cancellationToken = default)
    {
        PagedResult<ComplianceRecordDocument> pagedResult;

        var compliancerecordDocuments = _database.GetCollection<ComplianceRecordDocument>("compliance").AsQueryable();
        IEnumerable<ComplianceRecordDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await compliancerecordDocuments.PaginateAsync(query);
            return PagedResult<ComplianceRecordDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            compliancerecordDocuments = compliancerecordDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await compliancerecordDocuments.PaginateAsync(query);

        return PagedResult<ComplianceRecordDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseCompliance query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



