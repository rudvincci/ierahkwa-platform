using Microsoft.JSInterop;

namespace Mamey.Azure.Identity.BlazorWasm.Services;

public interface IBlazorAuthenticationService
{
    Task SaveAccessToken(string accessToken);
    Task<string> GetAccessToken();
}

public class BlazorAuthenticationService : IBlazorAuthenticationService
{
private readonly IJSRuntime _jsRuntime;

public BlazorAuthenticationService(IJSRuntime jsRuntime)
{
    _jsRuntime = jsRuntime;
}

public async Task SaveAccessToken(string accessToken)
{
    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "accessToken", accessToken);
}

public async Task<string> GetAccessToken()
{
    return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");
}

}