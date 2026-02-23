using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Mamey.Identity.Blazor.Services;
using Mamey.Identity.Core;
using System.Security.Claims;

namespace Mamey.Identity.Blazor.Components;

/// <summary>
/// Custom authentication state provider for Mamey Identity in Blazor WebAssembly.
/// </summary>
public class MameyAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IBlazorAuthenticationService _authenticationService;
    private readonly IBlazorTokenService _tokenService;
    private readonly IBlazorUserService _userService;
    private readonly ILogger<MameyAuthenticationStateProvider> _logger;

    public MameyAuthenticationStateProvider(
        IBlazorAuthenticationService authenticationService,
        IBlazorTokenService tokenService,
        IBlazorUserService userService,
        ILogger<MameyAuthenticationStateProvider> logger)
    {
        _authenticationService = authenticationService;
        _tokenService = tokenService;
        _userService = userService;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var authState = await _authenticationService.GetAuthenticationStateAsync();
            
            if (authState.IsAuthenticated && authState.User != null)
            {
                var claims = authState.Claims.ToList();
                
                // Add standard claims
                claims.Add(new Claim(ClaimTypes.NameIdentifier, authState.User.UserId.ToString()));
                claims.Add(new Claim(ClaimTypes.Name, authState.User.Name ?? string.Empty));
                claims.Add(new Claim(ClaimTypes.Email, authState.User.Email ?? string.Empty));
                
                if (authState.User.TenantId.HasValue)
                {
                    claims.Add(new Claim("tenant_id", authState.User.TenantId.Value.ToString()));
                }
                
                if (!string.IsNullOrEmpty(authState.User.Type))
                {
                    claims.Add(new Claim("user_type", authState.User.Type));
                }
                
                if (!string.IsNullOrEmpty(authState.User.Status))
                {
                    claims.Add(new Claim("user_status", authState.User.Status));
                }

                // Add custom claims from user
                if (authState.User.Claims != null)
                {
                    foreach (var claim in authState.User.Claims)
                    {
                        claims.Add(new Claim(claim.Key, claim.Value));
                    }
                }

                var identity = new ClaimsIdentity(claims, "MameyIdentity");
                var principal = new ClaimsPrincipal(identity);
                
                _logger.LogDebug("User {UserId} is authenticated with {ClaimCount} claims", 
                    authState.User.UserId, claims.Count);
                
                return new AuthenticationState(principal);
            }
            else
            {
                _logger.LogDebug("User is not authenticated");
                return new AuthenticationState(new ClaimsPrincipal());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting authentication state");
            return new AuthenticationState(new ClaimsPrincipal());
        }
    }

    /// <summary>
    /// Notifies that the authentication state has changed.
    /// </summary>
    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    /// Signs in a user and notifies of state change.
    /// </summary>
    /// <param name="user">The authenticated user.</param>
    public async Task SignInAsync(AuthenticatedUser user)
    {
        try
        {
            await _userService.StoreUserAsync(user);
            NotifyAuthenticationStateChanged();
            _logger.LogInformation("User {UserId} signed in successfully", user.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing in user {UserId}", user.UserId);
        }
    }

    /// <summary>
    /// Signs out the current user and notifies of state change.
    /// </summary>
    public async Task SignOutAsync()
    {
        try
        {
            await _userService.ClearUserDataAsync();
            await _tokenService.ClearAllTokensAsync();
            NotifyAuthenticationStateChanged();
            _logger.LogInformation("User signed out successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing out user");
        }
    }
}

































