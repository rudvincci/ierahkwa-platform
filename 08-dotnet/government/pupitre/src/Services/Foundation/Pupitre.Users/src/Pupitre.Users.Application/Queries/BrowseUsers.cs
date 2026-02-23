using System;
using Mamey.CQRS.Queries;
using Pupitre.Users.Application.DTO;


namespace Pupitre.Users.Application.Queries;

internal class BrowseUsers : PagedQueryBase, IQuery<PagedResult<UserDto>?>
{
    public string? Name { get; set; }
}