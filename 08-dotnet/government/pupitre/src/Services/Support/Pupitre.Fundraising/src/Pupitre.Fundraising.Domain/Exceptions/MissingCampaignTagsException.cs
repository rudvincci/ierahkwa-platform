using Mamey.Exceptions;

namespace Pupitre.Fundraising.Domain.Exceptions;

internal class MissingCampaignTagsException : DomainException
{
    public MissingCampaignTagsException()
        : base("Campaign tags are missing.")
    {
    }

    public override string Code => "missing_campaign_tags";
}