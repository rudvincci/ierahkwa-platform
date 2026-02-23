using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.AISafety.Application.DTO;
using Pupitre.AISafety.Application.Queries;
using Pupitre.AISafety.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.AISafety.Infrastructure.Mongo.Queries;

internal sealed class BrowseAISafetyHandler : IQueryHandler<BrowseAISafety, PagedResult<SafetyCheckDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseAISafetyHandler(IMongoDatabase database, IMongoRepository<SafetyCheckDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<SafetyCheckDto>?> HandleAsync(BrowseAISafety query, CancellationToken cancellationToken = default)
    {
        PagedResult<SafetyCheckDocument> pagedResult;

        var safetycheckDocuments = _database.GetCollection<SafetyCheckDocument>("aisafety").AsQueryable();
        IEnumerable<SafetyCheckDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await safetycheckDocuments.PaginateAsync(query);
            return PagedResult<SafetyCheckDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            safetycheckDocuments = safetycheckDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await safetycheckDocuments.PaginateAsync(query);

        return PagedResult<SafetyCheckDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseAISafety query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



