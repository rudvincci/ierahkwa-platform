using System;
using Mamey.CQRS.Queries;
using Pupitre.Compliance.Application.DTO;
using Pupitre.Compliance.Application.Queries;
using Pupitre.Compliance.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.Compliance.Infrastructure.Mongo.Queries;

internal sealed class GetComplianceRecordHandler : IQueryHandler<GetComplianceRecord, ComplianceRecordDetailsDto>
{
    private const string collectionName = "compliance";
    private readonly IMongoDatabase _database;

    public GetComplianceRecordHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<ComplianceRecordDetailsDto> HandleAsync(GetComplianceRecord query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<ComplianceRecordDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


