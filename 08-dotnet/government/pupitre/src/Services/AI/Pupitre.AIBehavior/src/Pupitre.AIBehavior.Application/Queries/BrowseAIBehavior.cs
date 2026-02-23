using System;
using Mamey.CQRS.Queries;
using Pupitre.AIBehavior.Application.DTO;


namespace Pupitre.AIBehavior.Application.Queries;

internal class BrowseAIBehavior : PagedQueryBase, IQuery<PagedResult<BehaviorDto>?>
{
    public string? Name { get; set; }
}