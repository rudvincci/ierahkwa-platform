using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.Educators.Application.DTO;
using Pupitre.Educators.Application.Queries;
using Pupitre.Educators.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.Educators.Infrastructure.Mongo.Queries;

internal sealed class BrowseEducatorsHandler : IQueryHandler<BrowseEducators, PagedResult<EducatorDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseEducatorsHandler(IMongoDatabase database, IMongoRepository<EducatorDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<EducatorDto>?> HandleAsync(BrowseEducators query, CancellationToken cancellationToken = default)
    {
        PagedResult<EducatorDocument> pagedResult;

        var educatorDocuments = _database.GetCollection<EducatorDocument>("educators").AsQueryable();
        IEnumerable<EducatorDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await educatorDocuments.PaginateAsync(query);
            return PagedResult<EducatorDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            educatorDocuments = educatorDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await educatorDocuments.PaginateAsync(query);

        return PagedResult<EducatorDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseEducators query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



