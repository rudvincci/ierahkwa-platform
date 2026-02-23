using System;
using Mamey.CQRS.Queries;
using Pupitre.Accessibility.Application.DTO;


namespace Pupitre.Accessibility.Application.Queries;

internal class BrowseAccessibility : PagedQueryBase, IQuery<PagedResult<AccessProfileDto>?>
{
    public string? Name { get; set; }
}