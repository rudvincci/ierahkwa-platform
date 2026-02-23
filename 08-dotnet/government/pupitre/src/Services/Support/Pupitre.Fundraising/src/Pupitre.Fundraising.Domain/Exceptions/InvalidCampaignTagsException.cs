using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.Fundraising.Domain.Exceptions;

internal class InvalidCampaignTagsException : DomainException
{
    public override string Code { get; } = "invalid_campaign_tags";

    public InvalidCampaignTagsException() : base("Campaign tags are invalid.")
    {
    }
}
