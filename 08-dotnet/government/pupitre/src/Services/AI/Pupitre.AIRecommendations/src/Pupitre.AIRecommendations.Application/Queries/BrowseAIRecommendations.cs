using System;
using Mamey.CQRS.Queries;
using Pupitre.AIRecommendations.Application.DTO;


namespace Pupitre.AIRecommendations.Application.Queries;

internal class BrowseAIRecommendations : PagedQueryBase, IQuery<PagedResult<AIRecommendationDto>?>
{
    public string? Name { get; set; }
}