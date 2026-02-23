using System;
using Mamey.CQRS.Queries;
using Pupitre.Fundraising.Application.DTO;
using Pupitre.Fundraising.Application.Queries;
using Pupitre.Fundraising.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.Fundraising.Infrastructure.Mongo.Queries;

internal sealed class GetCampaignHandler : IQueryHandler<GetCampaign, CampaignDetailsDto>
{
    private const string collectionName = "fundraising";
    private readonly IMongoDatabase _database;

    public GetCampaignHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<CampaignDetailsDto> HandleAsync(GetCampaign query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<CampaignDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


