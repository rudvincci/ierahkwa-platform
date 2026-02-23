using System;
using Mamey.CQRS.Queries;
using Pupitre.Rewards.Application.DTO;
using Pupitre.Rewards.Application.Queries;
using Pupitre.Rewards.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.Rewards.Infrastructure.Mongo.Queries;

internal sealed class GetRewardHandler : IQueryHandler<GetReward, RewardDetailsDto>
{
    private const string collectionName = "rewards";
    private readonly IMongoDatabase _database;

    public GetRewardHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<RewardDetailsDto> HandleAsync(GetReward query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<RewardDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


