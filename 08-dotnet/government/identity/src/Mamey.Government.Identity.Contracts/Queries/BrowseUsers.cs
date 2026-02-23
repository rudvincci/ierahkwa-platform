using System.Runtime.CompilerServices;
using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Queries;

public class BrowseUsers : PagedQueryBase, IQuery<PagedResult<UserDto>?>
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Status { get; set; }
}

