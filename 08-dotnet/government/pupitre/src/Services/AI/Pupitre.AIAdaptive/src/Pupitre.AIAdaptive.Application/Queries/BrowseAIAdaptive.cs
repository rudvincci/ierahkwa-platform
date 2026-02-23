using System;
using Mamey.CQRS.Queries;
using Pupitre.AIAdaptive.Application.DTO;


namespace Pupitre.AIAdaptive.Application.Queries;

internal class BrowseAIAdaptive : PagedQueryBase, IQuery<PagedResult<AdaptiveLearningDto>?>
{
    public string? Name { get; set; }
}