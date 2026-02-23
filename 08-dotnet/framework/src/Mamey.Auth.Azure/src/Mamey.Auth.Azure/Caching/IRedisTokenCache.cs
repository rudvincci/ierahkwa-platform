namespace Mamey.Auth.Azure.Caching;
using System.Threading.Tasks;
public interface IRedisTokenCache
{
    Task<string> GetCachedTokenAsync(string key);
    Task SetCachedTokenAsync(string key, string token);
}
