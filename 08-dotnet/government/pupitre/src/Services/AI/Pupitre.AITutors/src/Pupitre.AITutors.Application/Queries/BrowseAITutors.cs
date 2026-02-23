using System;
using Mamey.CQRS.Queries;
using Pupitre.AITutors.Application.DTO;


namespace Pupitre.AITutors.Application.Queries;

internal class BrowseAITutors : PagedQueryBase, IQuery<PagedResult<TutorDto>?>
{
    public string? Name { get; set; }
}