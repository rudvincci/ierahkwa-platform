using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.AISpeech.Application.DTO;
using Pupitre.AISpeech.Application.Queries;
using Pupitre.AISpeech.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.AISpeech.Infrastructure.Mongo.Queries;

internal sealed class BrowseAISpeechHandler : IQueryHandler<BrowseAISpeech, PagedResult<SpeechRequestDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseAISpeechHandler(IMongoDatabase database, IMongoRepository<SpeechRequestDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<SpeechRequestDto>?> HandleAsync(BrowseAISpeech query, CancellationToken cancellationToken = default)
    {
        PagedResult<SpeechRequestDocument> pagedResult;

        var speechrequestDocuments = _database.GetCollection<SpeechRequestDocument>("aispeech").AsQueryable();
        IEnumerable<SpeechRequestDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await speechrequestDocuments.PaginateAsync(query);
            return PagedResult<SpeechRequestDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            speechrequestDocuments = speechrequestDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await speechrequestDocuments.PaginateAsync(query);

        return PagedResult<SpeechRequestDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseAISpeech query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



