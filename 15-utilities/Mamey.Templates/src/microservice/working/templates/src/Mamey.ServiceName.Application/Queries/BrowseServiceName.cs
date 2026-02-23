using System;
using Mamey.CQRS.Queries;
using Mamey.ServiceName.Application.DTO;


namespace Mamey.ServiceName.Application.Queries;

internal class BrowseServiceName : PagedQueryBase, IQuery<PagedResult<EntityNameDto>?>
{
    public string? Name { get; set; }
}