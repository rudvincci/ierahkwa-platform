using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Fundraising.Domain.Entities;

namespace Pupitre.Fundraising.Application.Exceptions;

internal class CampaignNotFoundException : MameyException
{
    public CampaignNotFoundException(CampaignId campaignId)
        : base($"Campaign with ID: '{campaignId.Value}' was not found.")
        => CampaignId = campaignId;

    public CampaignId CampaignId { get; }
}

