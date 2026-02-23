using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.AIVision.Application.DTO;
using Pupitre.AIVision.Application.Queries;
using Pupitre.AIVision.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.AIVision.Infrastructure.Mongo.Queries;

internal sealed class BrowseAIVisionHandler : IQueryHandler<BrowseAIVision, PagedResult<VisionAnalysisDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseAIVisionHandler(IMongoDatabase database, IMongoRepository<VisionAnalysisDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<VisionAnalysisDto>?> HandleAsync(BrowseAIVision query, CancellationToken cancellationToken = default)
    {
        PagedResult<VisionAnalysisDocument> pagedResult;

        var visionanalysisDocuments = _database.GetCollection<VisionAnalysisDocument>("aivision").AsQueryable();
        IEnumerable<VisionAnalysisDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await visionanalysisDocuments.PaginateAsync(query);
            return PagedResult<VisionAnalysisDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            visionanalysisDocuments = visionanalysisDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await visionanalysisDocuments.PaginateAsync(query);

        return PagedResult<VisionAnalysisDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseAIVision query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



