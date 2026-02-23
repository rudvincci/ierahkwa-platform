using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.AISafety.Domain.Entities;
using Pupitre.AISafety.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.AISafety.Infrastructure.Redis.Repositories;

internal class SafetyCheckRedisRepository : ISafetyCheckRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<SafetyCheckRedisRepository> _logger;
    private const string KeyPrefix = "safetycheck:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public SafetyCheckRedisRepository(IConnectionMultiplexer redis, ILogger<SafetyCheckRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(SafetyCheck entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<SafetyCheck> GetAsync(SafetyCheckId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<SafetyCheck>(json!)!;
    }

    public async Task UpdateAsync(SafetyCheck entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(SafetyCheckId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(SafetyCheckId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<SafetyCheck>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<SafetyCheck>>(Array.Empty<SafetyCheck>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
