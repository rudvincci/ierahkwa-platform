using System.Runtime.CompilerServices;
using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Queries;

public class BrowseRoles : PagedQueryBase, IQuery<PagedResult<RoleDto>?>
{
    public string? Name { get; set; }
    public string? Status { get; set; }
}

