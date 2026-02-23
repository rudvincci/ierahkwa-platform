using System;
using Mamey.CQRS.Queries;
using Pupitre.Parents.Application.DTO;


namespace Pupitre.Parents.Application.Queries;

internal class BrowseParents : PagedQueryBase, IQuery<PagedResult<ParentDto>?>
{
    public string? Name { get; set; }
}