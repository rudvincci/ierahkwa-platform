using System;
using Mamey.CQRS.Queries;
using Pupitre.Fundraising.Application.DTO;


namespace Pupitre.Fundraising.Application.Queries;

internal class BrowseFundraising : PagedQueryBase, IQuery<PagedResult<CampaignDto>?>
{
    public string? Name { get; set; }
}