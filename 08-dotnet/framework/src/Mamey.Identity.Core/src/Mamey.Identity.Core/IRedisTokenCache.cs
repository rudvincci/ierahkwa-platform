namespace Mamey.Identity.Core;

public interface IRedisTokenCache
{
    Task<string> GetCachedTokenAsync(string key);
    Task SetCachedTokenAsync(string key, string token);
}
