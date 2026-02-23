using System;
using Mamey.CQRS.Queries;
using Pupitre.AIVision.Application.DTO;
using Pupitre.AIVision.Application.Queries;
using Pupitre.AIVision.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.AIVision.Infrastructure.Mongo.Queries;

internal sealed class GetVisionAnalysisHandler : IQueryHandler<GetVisionAnalysis, VisionAnalysisDetailsDto>
{
    private const string collectionName = "aivision";
    private readonly IMongoDatabase _database;

    public GetVisionAnalysisHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<VisionAnalysisDetailsDto> HandleAsync(GetVisionAnalysis query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<VisionAnalysisDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


