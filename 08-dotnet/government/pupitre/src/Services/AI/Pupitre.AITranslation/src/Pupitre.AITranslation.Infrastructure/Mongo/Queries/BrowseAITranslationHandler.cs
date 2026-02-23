using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.AITranslation.Application.DTO;
using Pupitre.AITranslation.Application.Queries;
using Pupitre.AITranslation.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.AITranslation.Infrastructure.Mongo.Queries;

internal sealed class BrowseAITranslationHandler : IQueryHandler<BrowseAITranslation, PagedResult<TranslationRequestDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseAITranslationHandler(IMongoDatabase database, IMongoRepository<TranslationRequestDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<TranslationRequestDto>?> HandleAsync(BrowseAITranslation query, CancellationToken cancellationToken = default)
    {
        PagedResult<TranslationRequestDocument> pagedResult;

        var translationrequestDocuments = _database.GetCollection<TranslationRequestDocument>("aitranslation").AsQueryable();
        IEnumerable<TranslationRequestDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await translationrequestDocuments.PaginateAsync(query);
            return PagedResult<TranslationRequestDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            translationrequestDocuments = translationrequestDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await translationrequestDocuments.PaginateAsync(query);

        return PagedResult<TranslationRequestDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseAITranslation query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



