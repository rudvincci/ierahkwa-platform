using System;
using Mamey.CQRS.Queries;
using Pupitre.Ministries.Application.DTO;


namespace Pupitre.Ministries.Application.Queries;

internal class BrowseMinistries : PagedQueryBase, IQuery<PagedResult<MinistryDataDto>?>
{
    public string? Name { get; set; }
}