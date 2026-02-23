using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Fundraising.Domain.Entities;

namespace Pupitre.Fundraising.Application.Exceptions;

internal class CampaignAlreadyExistsException : MameyException
{
    public CampaignAlreadyExistsException(CampaignId campaignId)
        : base($"Campaign with ID: '{campaignId.Value}' already exists.")
        => CampaignId = campaignId;

    public CampaignId CampaignId { get; }
}
