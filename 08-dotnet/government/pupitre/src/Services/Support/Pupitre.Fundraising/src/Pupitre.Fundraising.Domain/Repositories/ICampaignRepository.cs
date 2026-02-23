using System;
using Pupitre.Fundraising.Domain.Entities;
using Mamey.Types;

namespace Pupitre.Fundraising.Domain.Repositories;

internal interface ICampaignRepository
{
    Task AddAsync(Campaign campaign, CancellationToken cancellationToken = default);
    Task UpdateAsync(Campaign campaign, CancellationToken cancellationToken = default);
    Task DeleteAsync(CampaignId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Campaign>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<Campaign> GetAsync(CampaignId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(CampaignId id, CancellationToken cancellationToken = default);
}
