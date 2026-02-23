using System;
using Mamey.Persistence.MongoDB;
using Pupitre.Fundraising.Domain.Repositories;
using Pupitre.Fundraising.Domain.Entities;
using Pupitre.Fundraising.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.Fundraising.Infrastructure.Mongo.Repositories;

internal class CampaignMongoRepository : ICampaignRepository
{
    private readonly IMongoRepository<CampaignDocument, Guid> _repository;

    public CampaignMongoRepository(IMongoRepository<CampaignDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Campaign campaign, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new CampaignDocument(campaign));

    public async Task UpdateAsync(Campaign campaign, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new CampaignDocument(campaign));
    public async Task DeleteAsync(CampaignId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<Campaign>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<Campaign> GetAsync(CampaignId id, CancellationToken cancellationToken = default)
    {
        var campaign = await _repository.GetAsync(id.Value);
        return campaign?.AsEntity();
    }
    public async Task<bool> ExistsAsync(CampaignId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



