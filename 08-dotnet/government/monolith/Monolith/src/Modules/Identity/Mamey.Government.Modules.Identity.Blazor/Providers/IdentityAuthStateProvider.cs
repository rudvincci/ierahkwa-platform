using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mamey.Government.Modules.Identity.Blazor.Providers;

/// <summary>
/// Authentication state provider for Identity module.
/// Integrates with Authentik OIDC authentication.
/// </summary>
public class IdentityAuthStateProvider : AuthenticationStateProvider
{
    private readonly IIdentityAuthService _authService;

    public IdentityAuthStateProvider(IIdentityAuthService authService)
    {
        _authService = authService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await _authService.GetCurrentUserAsync();
        
        if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
        {
            return new AuthenticationState(user);
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public void NotifyUserAuthenticationChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
