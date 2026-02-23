using System.Net.Http.Json;
using Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Dto;
using Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Models;
using Mamey.CQRS.Queries;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Clients;

/// <inheritdoc/>
public class UserApiClient : IUserApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserApiClient> _logger;

    public UserApiClient(HttpClient httpClient, ILogger<UserApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagedResult<UserDto>> GetUsersAsync(
        UserQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            var qs = new Dictionary<string, string>
            {
                ["page"]     = parameters.Page.ToString(),
                ["pageSize"] = parameters.PageSize.ToString(),
                ["sortBy"]   = parameters.SortBy ?? string.Empty,
                ["sortDesc"] = parameters.SortDescending.ToString(),
                ["filter"]   = parameters.Filter ?? string.Empty,
            };
            var url = QueryHelpers.AddQueryString("users", qs);

            // Read the PagedResult<T> exactly once
            var paged = await _httpClient
                .GetFromJsonAsync<PagedResult<UserDto>>(url, cancellationToken)
                .ConfigureAwait(false);

            return paged;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching users with {@Parameters}", parameters);
            throw;
        }
    }
}