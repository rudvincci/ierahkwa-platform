using Microsoft.JSInterop;

namespace Mamey.Government.Identity.BlazorWasm.Services;

internal sealed class CookieBridge : IAsyncDisposable, ICookieBridge
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    // ABSOLUTE path = always resolves, even on /Account/Login, /Account/Manage, etc.
    private const string ModulePath =
        "/_content/Mamey.FutureWampumId.Identity.BlazorWasm/js/auth.module.js";

    public CookieBridge(IJSRuntime js) => _js = js;

    private async ValueTask<IJSObjectReference> GetModuleAsync()
    {
        _module ??= await _js.InvokeAsync<IJSObjectReference>("import", ModulePath);
        return _module;
    }

    public async Task IssueCookieAsync(string accessToken)
    {
        var m = await GetModuleAsync();
        await m.InvokeVoidAsync("issueCookie", accessToken);
    }

    public async Task SignOutCookieAsync()
    {
        var m = await GetModuleAsync();
        await m.InvokeVoidAsync("signOutCookie");
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            try { await _module.DisposeAsync(); } catch { /* ignore */ }
        }
    }
}