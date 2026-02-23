using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.AIAssessments.Application.DTO;
using Pupitre.AIAssessments.Application.Queries;
using Pupitre.AIAssessments.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.AIAssessments.Infrastructure.Mongo.Queries;

internal sealed class BrowseAIAssessmentsHandler : IQueryHandler<BrowseAIAssessments, PagedResult<AIAssessmentDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseAIAssessmentsHandler(IMongoDatabase database, IMongoRepository<AIAssessmentDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<AIAssessmentDto>?> HandleAsync(BrowseAIAssessments query, CancellationToken cancellationToken = default)
    {
        PagedResult<AIAssessmentDocument> pagedResult;

        var aiassessmentDocuments = _database.GetCollection<AIAssessmentDocument>("aiassessments").AsQueryable();
        IEnumerable<AIAssessmentDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await aiassessmentDocuments.PaginateAsync(query);
            return PagedResult<AIAssessmentDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            aiassessmentDocuments = aiassessmentDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await aiassessmentDocuments.PaginateAsync(query);

        return PagedResult<AIAssessmentDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseAIAssessments query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



