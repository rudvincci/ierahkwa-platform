using System.Runtime.CompilerServices;
using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Queries;

public class BrowsePermissions : PagedQueryBase, IQuery<PagedResult<PermissionDto>?>
{
    public string? Resource { get; set; }
    public string? Action { get; set; }
    public string? Status { get; set; }
}

