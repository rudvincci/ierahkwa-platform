using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Identity.Blazor.Configuration;
using Mamey.Identity.Core;

namespace Mamey.Identity.Blazor.Services;

/// <summary>
/// Service for handling Blazor WebAssembly authentication.
/// </summary>
public class BlazorAuthenticationService : IBlazorAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly IBlazorTokenService _tokenService;
    private readonly IBlazorUserService _userService;
    private readonly BlazorIdentityOptions _options;
    private readonly ILogger<BlazorAuthenticationService> _logger;

    public BlazorAuthenticationService(
        IHttpClientFactory httpClientFactory,
        IBlazorTokenService tokenService,
        IBlazorUserService userService,
        IOptions<BlazorIdentityOptions> options,
        ILogger<BlazorAuthenticationService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("MameyIdentity");
        _tokenService = tokenService;
        _userService = userService;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<BlazorAuthenticationResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var loginRequest = new
            {
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync(_options.LoginEndpoint, loginRequest, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);
                
                if (loginResponse != null && loginResponse.Success)
                {
                    // Store tokens
                    if (!string.IsNullOrEmpty(loginResponse.AccessToken))
                    {
                        await _tokenService.StoreAccessTokenAsync(loginResponse.AccessToken, cancellationToken);
                    }
                    
                    if (!string.IsNullOrEmpty(loginResponse.RefreshToken))
                    {
                        await _tokenService.StoreRefreshTokenAsync(loginResponse.RefreshToken, cancellationToken);
                    }

                    // Store user information
                    if (loginResponse.User != null)
                    {
                        await _userService.StoreUserAsync(loginResponse.User, cancellationToken);
                    }

                    _logger.LogInformation("User {Email} logged in successfully", email);
                    
                    return new BlazorAuthenticationResult
                    {
                        IsSuccess = true,
                        User = loginResponse.User,
                        AccessToken = loginResponse.AccessToken,
                        RefreshToken = loginResponse.RefreshToken,
                        ExpiresAt = loginResponse.ExpiresAt
                    };
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Login failed for user {Email}: {Error}", email, errorContent);
            
            return new BlazorAuthenticationResult
            {
                IsSuccess = false,
                ErrorMessage = "Login failed. Please check your credentials."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Email}", email);
            return new BlazorAuthenticationResult
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred during login. Please try again."
            };
        }
    }

    public async Task<bool> LogoutAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync(_options.LogoutEndpoint, null, cancellationToken);
            
            // Clear local storage regardless of server response
            await _userService.ClearUserDataAsync(cancellationToken);
            await _tokenService.ClearAllTokensAsync(cancellationToken);
            
            _logger.LogInformation("User logged out successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return false;
        }
    }

    public async Task<BlazorAuthenticationResult?> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var refreshToken = await _tokenService.GetRefreshTokenAsync(cancellationToken);
            if (string.IsNullOrEmpty(refreshToken))
            {
                return null;
            }

            var refreshRequest = new
            {
                RefreshToken = refreshToken
            };

            var response = await _httpClient.PostAsJsonAsync(_options.RefreshTokenEndpoint, refreshRequest, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var refreshResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);
                
                if (refreshResponse != null && refreshResponse.Success)
                {
                    // Update tokens
                    if (!string.IsNullOrEmpty(refreshResponse.AccessToken))
                    {
                        await _tokenService.StoreAccessTokenAsync(refreshResponse.AccessToken, cancellationToken);
                    }
                    
                    if (!string.IsNullOrEmpty(refreshResponse.RefreshToken))
                    {
                        await _tokenService.StoreRefreshTokenAsync(refreshResponse.RefreshToken, cancellationToken);
                    }

                    // Update user information
                    if (refreshResponse.User != null)
                    {
                        await _userService.StoreUserAsync(refreshResponse.User, cancellationToken);
                    }

                    _logger.LogDebug("Token refreshed successfully");
                    
                    return new BlazorAuthenticationResult
                    {
                        IsSuccess = true,
                        User = refreshResponse.User,
                        AccessToken = refreshResponse.AccessToken,
                        RefreshToken = refreshResponse.RefreshToken,
                        ExpiresAt = refreshResponse.ExpiresAt
                    };
                }
            }

            _logger.LogWarning("Token refresh failed");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return null;
        }
    }

    public async Task<BlazorAuthenticationState> GetAuthenticationStateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userService.GetCurrentUserAsync(cancellationToken);
            var accessToken = await _tokenService.GetAccessTokenAsync(cancellationToken);
            var refreshToken = await _tokenService.GetRefreshTokenAsync(cancellationToken);

            if (user != null && !string.IsNullOrEmpty(accessToken))
            {
                // Parse token expiration if available
                DateTime? expiresAt = null;
                try
                {
                    var tokenParts = accessToken.Split('.');
                    if (tokenParts.Length == 3)
                    {
                        var payload = JsonSerializer.Deserialize<JsonElement>(Convert.FromBase64String(tokenParts[1]));
                        if (payload.TryGetProperty("exp", out var expElement))
                        {
                            var exp = expElement.GetInt64();
                            expiresAt = DateTimeOffset.FromUnixTimeSeconds(exp).DateTime;
                        }
                    }
                }
                catch
                {
                    // Ignore token parsing errors
                }

                var claims = new List<Claim>();
                if (user.Claims != null)
                {
                    foreach (var claim in user.Claims)
                    {
                        claims.Add(new Claim(claim.Key, claim.Value));
                    }
                }

                return new BlazorAuthenticationState
                {
                    IsAuthenticated = true,
                    User = user,
                    Claims = claims,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = expiresAt
                };
            }

            return new BlazorAuthenticationState
            {
                IsAuthenticated = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting authentication state");
            return new BlazorAuthenticationState
            {
                IsAuthenticated = false
            };
        }
    }

    public async Task<bool> IsAuthenticatedAsync(CancellationToken cancellationToken = default)
    {
        var authState = await GetAuthenticationStateAsync(cancellationToken);
        return authState.IsAuthenticated;
    }

    public async Task<IEnumerable<Claim>> GetUserClaimsAsync(CancellationToken cancellationToken = default)
    {
        var authState = await GetAuthenticationStateAsync(cancellationToken);
        return authState.Claims;
    }

    public async Task<bool> HasClaimAsync(string claimType, string claimValue, CancellationToken cancellationToken = default)
    {
        var claims = await GetUserClaimsAsync(cancellationToken);
        return claims.Any(c => c.Type == claimType && c.Value == claimValue);
    }

    public async Task<bool> IsInRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        return await HasClaimAsync(ClaimTypes.Role, role, cancellationToken);
    }

    private class LoginResponse
    {
        public bool Success { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public AuthenticatedUser? User { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

































