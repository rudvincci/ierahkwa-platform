using System;
using Mamey.CQRS.Queries;
using Pupitre.Aftercare.Application.DTO;


namespace Pupitre.Aftercare.Application.Queries;

internal class BrowseAftercare : PagedQueryBase, IQuery<PagedResult<AftercarePlanDto>?>
{
    public string? Name { get; set; }
}