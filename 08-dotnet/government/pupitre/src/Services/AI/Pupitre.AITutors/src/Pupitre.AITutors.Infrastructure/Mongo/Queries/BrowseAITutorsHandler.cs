using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.AITutors.Application.DTO;
using Pupitre.AITutors.Application.Queries;
using Pupitre.AITutors.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.AITutors.Infrastructure.Mongo.Queries;

internal sealed class BrowseAITutorsHandler : IQueryHandler<BrowseAITutors, PagedResult<TutorDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseAITutorsHandler(IMongoDatabase database, IMongoRepository<TutorDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<TutorDto>?> HandleAsync(BrowseAITutors query, CancellationToken cancellationToken = default)
    {
        PagedResult<TutorDocument> pagedResult;

        var tutorDocuments = _database.GetCollection<TutorDocument>("aitutors").AsQueryable();
        IEnumerable<TutorDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await tutorDocuments.PaginateAsync(query);
            return PagedResult<TutorDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            tutorDocuments = tutorDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await tutorDocuments.PaginateAsync(query);

        return PagedResult<TutorDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseAITutors query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



