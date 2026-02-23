using System;
using Mamey.CQRS.Queries;
using Pupitre.GLEs.Application.DTO;


namespace Pupitre.GLEs.Application.Queries;

internal class BrowseGLEs : PagedQueryBase, IQuery<PagedResult<GLEDto>?>
{
    public string? Name { get; set; }
}