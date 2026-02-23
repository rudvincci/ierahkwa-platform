using Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Dto;
using Mamey.CQRS.Queries;

namespace Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Services;

/// <summary>
/// Business‑logic façade for user management.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves a paged list of users with filtering/sorting.
    /// </summary>
    Task<PagedResult<UserDto>> GetUsersAsync(
        int page,
        int pageSize,
        string? sortBy,
        bool sortDesc,
        string? filter,
        CancellationToken cancellationToken);
}