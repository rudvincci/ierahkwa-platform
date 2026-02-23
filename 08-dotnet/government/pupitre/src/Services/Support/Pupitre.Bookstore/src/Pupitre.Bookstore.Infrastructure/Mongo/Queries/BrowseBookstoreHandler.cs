using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.Bookstore.Application.DTO;
using Pupitre.Bookstore.Application.Queries;
using Pupitre.Bookstore.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.Bookstore.Infrastructure.Mongo.Queries;

internal sealed class BrowseBookstoreHandler : IQueryHandler<BrowseBookstore, PagedResult<BookDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseBookstoreHandler(IMongoDatabase database, IMongoRepository<BookDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<BookDto>?> HandleAsync(BrowseBookstore query, CancellationToken cancellationToken = default)
    {
        PagedResult<BookDocument> pagedResult;

        var bookDocuments = _database.GetCollection<BookDocument>("bookstore").AsQueryable();
        IEnumerable<BookDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await bookDocuments.PaginateAsync(query);
            return PagedResult<BookDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            bookDocuments = bookDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await bookDocuments.PaginateAsync(query);

        return PagedResult<BookDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseBookstore query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



