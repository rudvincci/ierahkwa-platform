using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Mamey.Portal.Web.Auth;

public sealed class MockAuthStateProvider : AuthenticationStateProvider
{
    private readonly MockAuthSession _session;

    public MockAuthStateProvider(MockAuthSession session)
    {
        _session = session;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!_session.IsAuthenticated)
        {
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, _session.UserName),
            new(ClaimTypes.Role, _session.Role),
            new("tenant", _session.Tenant),
        };

        var identity = new ClaimsIdentity(claims, authenticationType: "Mock");
        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }

    public void NotifyChanged()
        => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}




