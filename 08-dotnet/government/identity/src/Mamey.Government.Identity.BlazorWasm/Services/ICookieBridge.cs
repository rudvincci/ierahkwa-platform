namespace Mamey.Government.Identity.BlazorWasm.Services;

internal interface ICookieBridge
{
    Task IssueCookieAsync(string accessToken);
    Task SignOutCookieAsync();
    ValueTask DisposeAsync();
}