using Mamey.ApplicationName.Modules.Identity.Core.DTO;
using Mamey.CQRS.Queries;

namespace Mamey.ApplicationName.Modules.Identity.Core.Queries;

internal class BrowseUsers : IQuery<PagedResult<ApplicationUserDto>>
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public bool? EmailConfirmed { get; set; }
    public bool? LockoutEnabled { get; set; }
}