using Microsoft.JSInterop;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Mamey.Identity.Blazor.Configuration;
using Mamey.Identity.Core;

namespace Mamey.Identity.Blazor.Services;

/// <summary>
/// Service for managing user information in Blazor WebAssembly using localStorage.
/// </summary>
public class BlazorUserService : IBlazorUserService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly BlazorIdentityOptions _options;
    private readonly ILogger<BlazorUserService> _logger;

    public BlazorUserService(
        IJSRuntime jsRuntime,
        IOptions<BlazorIdentityOptions> options,
        ILogger<BlazorUserService> logger)
    {
        _jsRuntime = jsRuntime;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<AuthenticatedUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetStoredUserAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return null;
        }
    }

    public async Task<bool> StoreUserAsync(AuthenticatedUser user, CancellationToken cancellationToken = default)
    {
        try
        {
            var userJson = JsonSerializer.Serialize(user);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", _options.UserInfoStorageKey, userJson);
            _logger.LogDebug("User {UserId} stored successfully", user.UserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing user {UserId}", user.UserId);
            return false;
        }
    }

    public async Task<AuthenticatedUser?> GetStoredUserAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var userJson = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", _options.UserInfoStorageKey);
            
            if (string.IsNullOrEmpty(userJson))
            {
                return null;
            }

            var user = JsonSerializer.Deserialize<AuthenticatedUser>(userJson);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stored user");
            return null;
        }
    }

    public async Task<bool> RemoveUserAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", _options.UserInfoStorageKey);
            _logger.LogDebug("User removed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user");
            return false;
        }
    }

    public async Task<bool> UpdateUserAsync(AuthenticatedUser user, CancellationToken cancellationToken = default)
    {
        try
        {
            return await StoreUserAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", user.UserId);
            return false;
        }
    }

    public async Task<AuthenticatedUser?> RefreshUserAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // This would typically make an API call to refresh user information
            // For now, we'll just return the stored user
            // In a real implementation, you would make an HTTP call to the user info endpoint
            _logger.LogDebug("User refresh requested - returning stored user");
            return await GetStoredUserAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing user");
            return null;
        }
    }

    public async Task<bool> HasStoredUserAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await GetStoredUserAsync(cancellationToken);
            return user != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for stored user");
            return false;
        }
    }

    public async Task<bool> ClearUserDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", _options.UserInfoStorageKey);
            _logger.LogDebug("User data cleared successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing user data");
            return false;
        }
    }
}



































