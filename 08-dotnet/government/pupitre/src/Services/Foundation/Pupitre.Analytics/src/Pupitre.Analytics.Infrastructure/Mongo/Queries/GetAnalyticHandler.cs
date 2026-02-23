using System;
using Mamey.CQRS.Queries;
using Pupitre.Analytics.Application.DTO;
using Pupitre.Analytics.Application.Queries;
using Pupitre.Analytics.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.Analytics.Infrastructure.Mongo.Queries;

internal sealed class GetAnalyticHandler : IQueryHandler<GetAnalytic, AnalyticDetailsDto>
{
    private const string collectionName = "analytics";
    private readonly IMongoDatabase _database;

    public GetAnalyticHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<AnalyticDetailsDto> HandleAsync(GetAnalytic query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<AnalyticDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


