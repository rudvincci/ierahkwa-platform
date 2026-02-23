using System;
using Mamey.CQRS.Queries;
using Pupitre.Educators.Application.DTO;


namespace Pupitre.Educators.Application.Queries;

internal class BrowseEducators : PagedQueryBase, IQuery<PagedResult<EducatorDto>?>
{
    public string? Name { get; set; }
}