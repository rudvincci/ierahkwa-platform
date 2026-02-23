using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Accessibility.Domain.Entities;
using Pupitre.Accessibility.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.Accessibility.Infrastructure.Redis.Repositories;

internal class AccessProfileRedisRepository : IAccessProfileRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<AccessProfileRedisRepository> _logger;
    private const string KeyPrefix = "accessprofile:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public AccessProfileRedisRepository(IConnectionMultiplexer redis, ILogger<AccessProfileRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(AccessProfile entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<AccessProfile> GetAsync(AccessProfileId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<AccessProfile>(json!)!;
    }

    public async Task UpdateAsync(AccessProfile entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(AccessProfileId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(AccessProfileId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<AccessProfile>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<AccessProfile>>(Array.Empty<AccessProfile>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
