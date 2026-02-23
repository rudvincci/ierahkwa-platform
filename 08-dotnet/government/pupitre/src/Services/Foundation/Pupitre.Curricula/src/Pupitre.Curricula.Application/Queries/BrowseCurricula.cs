using System;
using Mamey.CQRS.Queries;
using Pupitre.Curricula.Application.DTO;


namespace Pupitre.Curricula.Application.Queries;

internal class BrowseCurricula : PagedQueryBase, IQuery<PagedResult<CurriculumDto>?>
{
    public string? Name { get; set; }
}