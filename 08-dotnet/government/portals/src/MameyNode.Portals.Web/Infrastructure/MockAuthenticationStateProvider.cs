using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace MameyNode.Portals.Web.Infrastructure;

public class MockAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AuthenticationState _anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));
    private readonly AuthenticationState _authenticated;

    public MockAuthenticationStateProvider()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Mock User"),
            new(ClaimTypes.Email, "mock@mamey.io"),
            new(ClaimTypes.Role, "Admin"),
            new(ClaimTypes.Role, "GovernmentAgent"),
            new(ClaimTypes.Role, "BankAdmin"),
            new(ClaimTypes.Role, "Treasury"),
            new(ClaimTypes.Role, "User"),
            new(ClaimTypes.Role, "Citizen")
        };

        var identity = new ClaimsIdentity(claims, "MockAuth");
        _authenticated = new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(_authenticated);
    }
}





