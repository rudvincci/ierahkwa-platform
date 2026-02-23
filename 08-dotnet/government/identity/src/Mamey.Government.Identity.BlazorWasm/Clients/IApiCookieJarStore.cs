using System.Net;

namespace Mamey.Government.Identity.BlazorWasm.Clients;

public interface IApiCookieJarStore
{
    Task SetAsync(string userId, CookieContainer jar);
    Task<CookieContainer?> GetAsync(string userId);
    Task ClearAsync(string userId);
}