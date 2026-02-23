using Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Clients;
using Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Dto;
using Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Models;
using Mamey.CQRS.Queries;
using Microsoft.Extensions.Logging;

namespace Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Services;

/// <inheritdoc/>
public class UserService : IUserService
{
    private readonly IUserApiClient _api;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserApiClient api, ILogger<UserService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<PagedResult<UserDto>> GetUsersAsync(
        int page,
        int pageSize,
        string? sortBy,
        bool sortDesc,
        string? filter,
        CancellationToken cancellationToken)
    {
        // Additional business rules could go here
        return await _api.GetUsersAsync(new UserQueryParameters
        {
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDescending = sortDesc,
            Filter = filter
        }, cancellationToken);
    }
}