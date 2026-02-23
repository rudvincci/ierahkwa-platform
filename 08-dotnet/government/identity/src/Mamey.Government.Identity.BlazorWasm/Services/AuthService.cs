using System.Security.Claims;
using Mamey.Auth.Identity.Managers;
using Mamey.Government.Identity.BlazorWasm.Clients;
using Mamey.Government.Identity.BlazorWasm.Providers;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.BlazorWasm.Services;

internal class AuthService : IAuthService
{
    private readonly IIdentityApiClient _httpClient;
    private readonly ApiAuthenticationStateProvider _authStateProvider;
    private readonly IIdentityRedirectManager _redirect;
    private readonly ILogger<AuthService> _logger;
    private readonly ICookieBridge _cookieBridge;

    public AuthService(IIdentityRedirectManager redirect, ApiAuthenticationStateProvider authStateProvider, IIdentityApiClient httpClient, ILogger<AuthService> logger, ICookieBridge cookieBridge)
    {
        _redirect = redirect ?? throw new ArgumentNullException(nameof(redirect));
        _authStateProvider = authStateProvider;
        _httpClient = httpClient;
        _logger = logger;
        _cookieBridge = cookieBridge;
        _redirect = redirect;
    }

    // public Task RegisterAsync(Register command)
    // {
    //     throw new NotImplementedException();
    // }

    public async Task<AuthDto?> PasswordSignInAsync(SignInRequest signInRequest, string returnUrl = "/")
    {
        var result = await _httpClient.PasswordSignInAsync(signInRequest);
        if (!result.Succeeded)
        {
            _logger.LogInformation("Invalid login attempt");
            return null;
        }

        // 1) Persist token for API calls (ProtectedLocalStorage)
        await _authStateProvider.SetTokenAsync(result.Value.AccessToken, persist: true);

        // 2) Ask browser to POST to /auth/issue-cookie so the Blazor Server origin sets the cookie
        try
        {
            await _cookieBridge.IssueCookieAsync(result.Value.AccessToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed issuing cookie via browser bridge.");
        }

        // 3) Navigate to the requested page (forceLoad optional)
        _redirect.RedirectTo(returnUrl, forceLoad: true);
        return result.Value;
    }

    public Task<bool> ConfirmEmailAsync(UserId userId, string code)
    {
        throw new NotImplementedException();
    }

    public Task RefreshSignInAsync(UserId userId)
    {
        throw new NotImplementedException();
    }

    public Task<AuthDto?> TwoFactorRecoveryCodeSignInAsync(UserId userId, string code)
    {
        throw new NotImplementedException();
    }

    public async Task LogoutAsync()
    {
        try
        {
            var id = _authStateProvider.CurrentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(id, out var userId)) await _httpClient.LogoutAsync(userId);
        } catch { /* swallow network errors on logout */ }

        await _authStateProvider.LogoutAsync(); // clears storage + notifies
        await _cookieBridge.SignOutCookieAsync();
        _redirect.RedirectTo("/", true);   
    }
}