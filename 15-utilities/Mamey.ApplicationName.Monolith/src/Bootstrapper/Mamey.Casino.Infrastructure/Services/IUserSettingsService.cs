using System.Security.Claims;
using Mamey.Casino.Domain.Entities;
using Mamey.Casino.Infrastructure.EF;
using Mamey.Persistence.Redis;
using Microsoft.AspNetCore.Http;

namespace Mamey.Casino.Infrastructure.Services;

public interface IUserSettingsService
{
    Task<UserSettings> GetAsync();
    Task SaveAsync(UserSettings settings);
}
public class UserSettingsService : IUserSettingsService
{
    private readonly CasinoDbContext _db;
    private readonly IHttpContextAccessor _http;
    private ICache _cache;

    public UserSettingsService(IHttpContextAccessor http)
        => (_http) = (http);

    private Guid CurrentUserId =>
        Guid.Parse(_http.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<UserSettings> GetAsync() 
    {
        var userSetting = await _cache.GetAsync<UserSettings>("userSettings:" + CurrentUserId) 
                          ?? new UserSettings { UserId = CurrentUserId, IsDarkMode = true};
        return userSetting;
    }

    public async Task SaveAsync(UserSettings settings)
    {
        var currentSettings = await _cache.GetAsync<UserSettings>("userSettings:" + CurrentUserId);
        if (currentSettings == null)
        {
            await _cache.AddToSetAsync("userSettings:" + CurrentUserId, settings);
        }
        else
        {
            await _cache.SetAsync("userSettings:" + CurrentUserId, settings);
        }
    }
}