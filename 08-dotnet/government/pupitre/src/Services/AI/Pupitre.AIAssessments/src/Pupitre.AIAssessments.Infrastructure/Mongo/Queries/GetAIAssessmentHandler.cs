using System;
using Mamey.CQRS.Queries;
using Pupitre.AIAssessments.Application.DTO;
using Pupitre.AIAssessments.Application.Queries;
using Pupitre.AIAssessments.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.AIAssessments.Infrastructure.Mongo.Queries;

internal sealed class GetAIAssessmentHandler : IQueryHandler<GetAIAssessment, AIAssessmentDetailsDto>
{
    private const string collectionName = "aiassessments";
    private readonly IMongoDatabase _database;

    public GetAIAssessmentHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<AIAssessmentDetailsDto> HandleAsync(GetAIAssessment query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<AIAssessmentDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


