using System;
using Mamey.CQRS.Queries;
using Pupitre.Aftercare.Application.DTO;
using Pupitre.Aftercare.Application.Queries;
using Pupitre.Aftercare.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.Aftercare.Infrastructure.Mongo.Queries;

internal sealed class GetAftercarePlanHandler : IQueryHandler<GetAftercarePlan, AftercarePlanDetailsDto>
{
    private const string collectionName = "aftercare";
    private readonly IMongoDatabase _database;

    public GetAftercarePlanHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<AftercarePlanDetailsDto> HandleAsync(GetAftercarePlan query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<AftercarePlanDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


