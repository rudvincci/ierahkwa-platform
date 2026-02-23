using System;
using Mamey.CQRS.Queries;
using Pupitre.Operations.Application.DTO;
using Pupitre.Operations.Application.Queries;
using Pupitre.Operations.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.Operations.Infrastructure.Mongo.Queries;

internal sealed class GetOperationMetricHandler : IQueryHandler<GetOperationMetric, OperationMetricDetailsDto>
{
    private const string collectionName = "operations";
    private readonly IMongoDatabase _database;

    public GetOperationMetricHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<OperationMetricDetailsDto> HandleAsync(GetOperationMetric query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<OperationMetricDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


