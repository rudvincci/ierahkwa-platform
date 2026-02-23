using Mamey.Azure.Abstractions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace Mamey.Graph.Providers;

public class TokenAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IBlazorAuthenticationService _authenticationService;
    public TokenAuthenticationStateProvider(IJSRuntime jsRuntime,
        IBlazorAuthenticationService authenticationService)
    {
        _jsRuntime = jsRuntime;
        _authenticationService = authenticationService;
    }

    public async Task<string> GetTokenAsync()
        => await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

    public async Task SetTokenAsync(string token)
    {
        if (token == null)
        {
            await _jsRuntime.InvokeAsync<object>("localStorage.removeItem", "authToken");
        }
        else
        {
            await _jsRuntime.InvokeAsync<object>("localStorage.setItem", "authToken", token);
        }

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var accessToken = await GetTokenAsync();
        var identity = new ClaimsIdentity();
        if (!string.IsNullOrEmpty(accessToken))
        {
            await _authenticationService.SaveAccessToken(accessToken);

            identity = new ClaimsIdentity(accessToken.ParseClaimsFromJwt(), "jwt");

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);

        }
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
}

