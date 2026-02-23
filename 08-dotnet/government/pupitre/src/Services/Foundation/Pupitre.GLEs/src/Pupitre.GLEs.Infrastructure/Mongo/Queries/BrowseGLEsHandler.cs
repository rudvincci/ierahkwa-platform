using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.GLEs.Application.DTO;
using Pupitre.GLEs.Application.Queries;
using Pupitre.GLEs.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.GLEs.Infrastructure.Mongo.Queries;

internal sealed class BrowseGLEsHandler : IQueryHandler<BrowseGLEs, PagedResult<GLEDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseGLEsHandler(IMongoDatabase database, IMongoRepository<GLEDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<GLEDto>?> HandleAsync(BrowseGLEs query, CancellationToken cancellationToken = default)
    {
        PagedResult<GLEDocument> pagedResult;

        var gleDocuments = _database.GetCollection<GLEDocument>("gles").AsQueryable();
        IEnumerable<GLEDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await gleDocuments.PaginateAsync(query);
            return PagedResult<GLEDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            gleDocuments = gleDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await gleDocuments.PaginateAsync(query);

        return PagedResult<GLEDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseGLEs query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



