using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.Aftercare.Application.DTO;
using Pupitre.Aftercare.Application.Queries;
using Pupitre.Aftercare.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.Aftercare.Infrastructure.Mongo.Queries;

internal sealed class BrowseAftercareHandler : IQueryHandler<BrowseAftercare, PagedResult<AftercarePlanDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseAftercareHandler(IMongoDatabase database, IMongoRepository<AftercarePlanDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<AftercarePlanDto>?> HandleAsync(BrowseAftercare query, CancellationToken cancellationToken = default)
    {
        PagedResult<AftercarePlanDocument> pagedResult;

        var aftercareplanDocuments = _database.GetCollection<AftercarePlanDocument>("aftercare").AsQueryable();
        IEnumerable<AftercarePlanDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await aftercareplanDocuments.PaginateAsync(query);
            return PagedResult<AftercarePlanDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            aftercareplanDocuments = aftercareplanDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await aftercareplanDocuments.PaginateAsync(query);

        return PagedResult<AftercarePlanDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseAftercare query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



