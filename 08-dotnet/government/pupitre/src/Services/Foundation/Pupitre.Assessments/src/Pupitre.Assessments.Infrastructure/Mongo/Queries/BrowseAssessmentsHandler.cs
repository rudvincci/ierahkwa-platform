using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.Assessments.Application.DTO;
using Pupitre.Assessments.Application.Queries;
using Pupitre.Assessments.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.Assessments.Infrastructure.Mongo.Queries;

internal sealed class BrowseAssessmentsHandler : IQueryHandler<BrowseAssessments, PagedResult<AssessmentDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseAssessmentsHandler(IMongoDatabase database, IMongoRepository<AssessmentDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<AssessmentDto>?> HandleAsync(BrowseAssessments query, CancellationToken cancellationToken = default)
    {
        PagedResult<AssessmentDocument> pagedResult;

        var assessmentDocuments = _database.GetCollection<AssessmentDocument>("assessments").AsQueryable();
        IEnumerable<AssessmentDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await assessmentDocuments.PaginateAsync(query);
            return PagedResult<AssessmentDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            assessmentDocuments = assessmentDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await assessmentDocuments.PaginateAsync(query);

        return PagedResult<AssessmentDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseAssessments query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



