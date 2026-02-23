using System.Net;
using Mamey.Persistence.Redis;

namespace Mamey.Government.Identity.BlazorWasm.Clients;

public sealed class ApiCookieJarStore : IApiCookieJarStore
{
    public const string KeyPath = "cookies:";
    private readonly ICache _cache;

    public ApiCookieJarStore(ICache cache)
    {
        _cache = cache;
    }

    public Task SetAsync(string userId, CookieContainer jar) => _cache.SetAsync($"{KeyPath}{userId}", jar);
    public async Task<CookieContainer?> GetAsync(string userId) => (await _cache.GetAsync<CookieContainer?>($"{KeyPath}{userId}")) ?? null;
    public Task ClearAsync(string userId) => _cache.DeleteAsync<CookieContainer>($"{KeyPath}{userId}");
}