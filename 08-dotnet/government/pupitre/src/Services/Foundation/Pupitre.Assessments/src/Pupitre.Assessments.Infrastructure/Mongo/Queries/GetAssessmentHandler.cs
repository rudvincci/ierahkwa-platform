using System;
using Mamey.CQRS.Queries;
using Pupitre.Assessments.Application.DTO;
using Pupitre.Assessments.Application.Queries;
using Pupitre.Assessments.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.Assessments.Infrastructure.Mongo.Queries;

internal sealed class GetAssessmentHandler : IQueryHandler<GetAssessment, AssessmentDetailsDto>
{
    private const string collectionName = "assessments";
    private readonly IMongoDatabase _database;

    public GetAssessmentHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<AssessmentDetailsDto> HandleAsync(GetAssessment query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<AssessmentDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


