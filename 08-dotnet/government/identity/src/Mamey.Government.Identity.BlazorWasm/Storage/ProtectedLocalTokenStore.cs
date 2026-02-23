using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Mamey.Government.Identity.BlazorWasm.Storage;

public sealed class ProtectedLocalTokenStore : ITokenStore
{
    private const string Key = "auth.accessToken";
    private readonly ProtectedLocalStorage _pls;

    public ProtectedLocalTokenStore(ProtectedLocalStorage pls) => _pls = pls;

    public async Task SaveAsync(string token)
    {
        try { await _pls.SetAsync(Key, token); }
        catch (InvalidOperationException)
        {
            // prerender / static render – ignore; caller should retry after circuit established
        }
    }

    public async Task<string?> GetAsync()
    {
        try
        {
            var result = await _pls.GetAsync<string>(Key);
            return result.Success ? result.Value : null;
        }
        catch (InvalidOperationException)
        {
            // "JS interop cannot be issued… statically rendered"
            return null; // defer until circuit established
        }
    }

    public async Task ClearAsync()
    {
        try { await _pls.DeleteAsync(Key); }
        catch (InvalidOperationException) { /* ignore during prerender */ }
    }
}