using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.AIContent.Application.DTO;
using Pupitre.AIContent.Application.Queries;
using Pupitre.AIContent.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.AIContent.Infrastructure.Mongo.Queries;

internal sealed class BrowseAIContentHandler : IQueryHandler<BrowseAIContent, PagedResult<ContentGenerationDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseAIContentHandler(IMongoDatabase database, IMongoRepository<ContentGenerationDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<ContentGenerationDto>?> HandleAsync(BrowseAIContent query, CancellationToken cancellationToken = default)
    {
        PagedResult<ContentGenerationDocument> pagedResult;

        var contentgenerationDocuments = _database.GetCollection<ContentGenerationDocument>("aicontent").AsQueryable();
        IEnumerable<ContentGenerationDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await contentgenerationDocuments.PaginateAsync(query);
            return PagedResult<ContentGenerationDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            contentgenerationDocuments = contentgenerationDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await contentgenerationDocuments.PaginateAsync(query);

        return PagedResult<ContentGenerationDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseAIContent query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



