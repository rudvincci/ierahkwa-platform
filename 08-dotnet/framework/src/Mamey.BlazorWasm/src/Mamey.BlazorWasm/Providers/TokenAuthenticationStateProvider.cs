//using System.Net.Http.Headers;
//using System.Security.Claims;
//using System.Text.Json;
//using Mamey.Http;
//using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.JSInterop;

//namespace Mamey.BlazorWasm.Providers;

//public class TokenAuthenticationStateProvider : AuthenticationStateProvider
//{
//    private readonly IJSRuntime _jsRuntime;
//    private readonly IBlazorAuthenticationService _authenticationService;
//    public TokenAuthenticationStateProvider(IJSRuntime jsRuntime,
//        IBlazorAuthenticationService authenticationService)
//    {
//        _jsRuntime = jsRuntime;
//        _authenticationService = authenticationService;
//    }

//    public async Task<string> GetTokenAsync()
//        => await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

//    public async Task SetTokenAsync(string token)
//    {
//        if (token == null)
//        {
//            await _jsRuntime.InvokeAsync<object>("localStorage.removeItem", "authToken");
//        }
//        else
//        {
//            await _jsRuntime.InvokeAsync<object>("localStorage.setItem", "authToken", token);
//        }

//        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
//    }

//    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//    {
//        var accessToken = await GetTokenAsync();
//        var identity = new ClaimsIdentity();
//        if (!string.IsNullOrEmpty(accessToken))
//        {
//            await _authenticationService.SaveAccessToken(accessToken);

//            identity = new ClaimsIdentity(Extensions.ParseClaimsFromJwt(accessToken), "jwt");

//            var user = new ClaimsPrincipal(identity);
//            return new AuthenticationState(user);

//        }
//        return new AuthenticationState(new ClaimsPrincipal(identity));
//    }
//}
//public static class AuthenticatedHttpClientExtensions
//{
//    public static async Task<T> GetJsonAsync<T>(this IHttpClient httpClient, string url, AuthenticationHeaderValue authorization)
//    {
//        var request = new HttpRequestMessage(HttpMethod.Get, url);
//        request.Headers.Authorization = authorization;

//        var response = await httpClient.SendAsync(request);
//        var responseBytes = await response.Content.ReadAsByteArrayAsync();
//        return JsonSerializer.Deserialize<T>(responseBytes, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
//    }
//}