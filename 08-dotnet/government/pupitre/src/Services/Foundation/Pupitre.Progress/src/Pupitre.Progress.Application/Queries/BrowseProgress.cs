using System;
using Mamey.CQRS.Queries;
using Pupitre.Progress.Application.DTO;


namespace Pupitre.Progress.Application.Queries;

internal class BrowseProgress : PagedQueryBase, IQuery<PagedResult<LearningProgressDto>?>
{
    public string? Name { get; set; }
}