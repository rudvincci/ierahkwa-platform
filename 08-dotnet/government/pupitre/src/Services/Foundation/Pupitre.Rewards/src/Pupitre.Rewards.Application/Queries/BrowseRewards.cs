using System;
using Mamey.CQRS.Queries;
using Pupitre.Rewards.Application.DTO;


namespace Pupitre.Rewards.Application.Queries;

internal class BrowseRewards : PagedQueryBase, IQuery<PagedResult<RewardDto>?>
{
    public string? Name { get; set; }
}