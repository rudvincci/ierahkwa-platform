using Mamey.Auth.Identity.Abstractions;
using Mamey.Auth.Identity.Entities;
using Mamey.Persistence.Redis;
using Microsoft.AspNetCore.Identity;

namespace Mamey.Casino.Infrastructure.Stores;

public interface IRefreshTokenStore
{
    Task<string?> GetTokenAsync(Guid userId);

    Task SetTokenAsync(Guid userId, string refreshToken, TimeSpan expiresAt,
        CancellationToken cancellationToken = default);
}
public class RefreshTokenStore : IRefreshTokenStore
{
    private readonly ICache _cache;
    private readonly UserManager<ApplicationUser> _userManager;

    public RefreshTokenStore(ICache cache)
    {
        _cache = cache;
    }

    public async Task<string?> GetTokenAsync(Guid userId)
    {
        var cacheKey = $"user:{userId}:refreshToken";
        return await _cache.GetAsync<string>(cacheKey);
    }

    public Task SetTokenAsync(Guid userId, string refreshToken, TimeSpan expiresAt, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"user:{userId}:refreshToken";
        return  _cache.SetAsync(cacheKey, refreshToken, expiresAt);
    }
}