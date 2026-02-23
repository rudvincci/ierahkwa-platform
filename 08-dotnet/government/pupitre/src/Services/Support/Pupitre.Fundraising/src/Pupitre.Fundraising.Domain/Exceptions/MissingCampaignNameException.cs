using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.Fundraising.Domain.Exceptions;

internal class MissingCampaignNameException : DomainException
{
    public MissingCampaignNameException()
        : base("Campaign name is missing.")
    {
    }

    public override string Code => "missing_campaign_name";
}
