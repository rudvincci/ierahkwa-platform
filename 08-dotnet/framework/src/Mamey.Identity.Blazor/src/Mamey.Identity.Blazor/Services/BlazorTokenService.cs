using Microsoft.JSInterop;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Identity.Blazor.Configuration;

namespace Mamey.Identity.Blazor.Services;

/// <summary>
/// Service for managing tokens in Blazor WebAssembly using localStorage.
/// </summary>
public class BlazorTokenService : IBlazorTokenService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly BlazorIdentityOptions _options;
    private readonly ILogger<BlazorTokenService> _logger;

    public BlazorTokenService(
        IJSRuntime jsRuntime,
        IOptions<BlazorIdentityOptions> options,
        ILogger<BlazorTokenService> logger)
    {
        _jsRuntime = jsRuntime;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> StoreAccessTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", _options.TokenStorageKey, token);
            _logger.LogDebug("Access token stored successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing access token");
            return false;
        }
    }

    public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", _options.TokenStorageKey);
            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving access token");
            return null;
        }
    }

    public async Task<bool> RemoveAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", _options.TokenStorageKey);
            _logger.LogDebug("Access token removed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing access token");
            return false;
        }
    }

    public async Task<bool> StoreRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", _options.RefreshTokenStorageKey, token);
            _logger.LogDebug("Refresh token stored successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing refresh token");
            return false;
        }
    }

    public async Task<string?> GetRefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", _options.RefreshTokenStorageKey);
            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving refresh token");
            return null;
        }
    }

    public async Task<bool> RemoveRefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", _options.RefreshTokenStorageKey);
            _logger.LogDebug("Refresh token removed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing refresh token");
            return false;
        }
    }

    public async Task<bool> ClearAllTokensAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", _options.TokenStorageKey);
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", _options.RefreshTokenStorageKey);
            _logger.LogDebug("All tokens cleared successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing all tokens");
            return false;
        }
    }

    public async Task<bool> HasAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await GetAccessTokenAsync(cancellationToken);
            return !string.IsNullOrEmpty(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for access token");
            return false;
        }
    }

    public async Task<bool> HasRefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await GetRefreshTokenAsync(cancellationToken);
            return !string.IsNullOrEmpty(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for refresh token");
            return false;
        }
    }

    public async Task<bool> ValidateAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await GetAccessTokenAsync(cancellationToken);
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            // Basic token validation - check if it's a valid JWT format
            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                return false;
            }

            // Check if token is expired
            try
            {
                var payload = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(
                    Convert.FromBase64String(parts[1]));
                
                if (payload.TryGetProperty("exp", out var expElement))
                {
                    var exp = expElement.GetInt64();
                    var expDateTime = DateTimeOffset.FromUnixTimeSeconds(exp).DateTime;
                    return expDateTime > DateTime.UtcNow;
                }
            }
            catch
            {
                // If we can't parse the token, consider it invalid
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating access token");
            return false;
        }
    }
}



































