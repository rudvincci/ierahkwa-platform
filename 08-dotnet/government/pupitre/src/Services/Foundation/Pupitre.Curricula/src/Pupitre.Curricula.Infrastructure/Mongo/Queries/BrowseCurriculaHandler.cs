using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.Curricula.Application.DTO;
using Pupitre.Curricula.Application.Queries;
using Pupitre.Curricula.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.Curricula.Infrastructure.Mongo.Queries;

internal sealed class BrowseCurriculaHandler : IQueryHandler<BrowseCurricula, PagedResult<CurriculumDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseCurriculaHandler(IMongoDatabase database, IMongoRepository<CurriculumDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<CurriculumDto>?> HandleAsync(BrowseCurricula query, CancellationToken cancellationToken = default)
    {
        PagedResult<CurriculumDocument> pagedResult;

        var curriculumDocuments = _database.GetCollection<CurriculumDocument>("curricula").AsQueryable();
        IEnumerable<CurriculumDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await curriculumDocuments.PaginateAsync(query);
            return PagedResult<CurriculumDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            curriculumDocuments = curriculumDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await curriculumDocuments.PaginateAsync(query);

        return PagedResult<CurriculumDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseCurricula query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



