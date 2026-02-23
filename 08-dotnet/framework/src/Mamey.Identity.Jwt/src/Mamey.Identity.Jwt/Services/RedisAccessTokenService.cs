using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using StackExchange.Redis;
using Mamey.Identity.Core;

namespace Mamey.Identity.Jwt.Services;

internal class RedisAccessTokenService : IAccessTokenService, IRedisTokenCache
{
    private readonly IDatabase _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TimeSpan _expires;

    public RedisAccessTokenService(IDatabase cache, IHttpContextAccessor httpContextAccessor, TimeSpan expires)
    {
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
        _expires = expires;
    }

    /// <inheritdoc />
    public Task<bool> IsCurrentActiveToken()
        => IsActiveAsync(GetCurrentAsync());

    /// <inheritdoc />
    public Task DeactivateCurrentAsync()
        => DeactivateAsync(GetCurrentAsync());

    /// <inheritdoc />
    public async Task<bool> IsActiveAsync(string token)
        => string.IsNullOrWhiteSpace(await GetCachedTokenAsync(GetKey(token)));

    /// <inheritdoc />
    public Task DeactivateAsync(string token)
        => _cache.StringSetAsync(GetKey(token), "revoked");

    /// <inheritdoc />
    private string GetCurrentAsync()
    {
        var authorizationHeader = _httpContextAccessor
            .HttpContext.Request.Headers["authorization"];

        return authorizationHeader == StringValues.Empty
            ? string.Empty
            : authorizationHeader.Single().Split(' ').Last();
    }

    /// <inheritdoc />
    private static string GetKey(string token) => $"blacklisted-tokens:{token}";

    /// <inheritdoc />
    public async Task<string?> GetCachedTokenAsync(string key)
        => await _cache?.StringGetAsync(key);

    /// <inheritdoc />
    public Task SetCachedTokenAsync(string key, string token)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
        }

        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentException($"'{nameof(token)}' cannot be null or empty.", nameof(token));
        }

        // Adjust expiration based on your needs
        var expiration = TimeSpan.FromMinutes(60); // TODO: Add configuration. 

        return _cache.StringSetAsync(key, token, expiration);
    }
}
