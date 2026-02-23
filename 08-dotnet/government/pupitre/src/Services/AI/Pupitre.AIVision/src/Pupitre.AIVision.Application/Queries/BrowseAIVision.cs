using System;
using Mamey.CQRS.Queries;
using Pupitre.AIVision.Application.DTO;


namespace Pupitre.AIVision.Application.Queries;

internal class BrowseAIVision : PagedQueryBase, IQuery<PagedResult<VisionAnalysisDto>?>
{
    public string? Name { get; set; }
}