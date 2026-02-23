using Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Dto;
using Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Models;
using Mamey.CQRS.Queries;

namespace Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Clients;

/// <summary>
/// HTTP client for Identity API (users).
/// </summary>
public interface IUserApiClient
{
    /// <summary>
    /// Fetches a paged list of users from the server.
    /// </summary>
    Task<PagedResult<UserDto>> GetUsersAsync(UserQueryParameters parameters, CancellationToken cancellationToken);
}