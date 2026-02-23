using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.AIBehavior.Domain.Entities;
using Pupitre.AIBehavior.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.AIBehavior.Infrastructure.Redis.Repositories;

internal class BehaviorRedisRepository : IBehaviorRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<BehaviorRedisRepository> _logger;
    private const string KeyPrefix = "behavior:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public BehaviorRedisRepository(IConnectionMultiplexer redis, ILogger<BehaviorRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(Behavior entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<Behavior> GetAsync(BehaviorId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<Behavior>(json!)!;
    }

    public async Task UpdateAsync(Behavior entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(BehaviorId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(BehaviorId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<Behavior>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Behavior>>(Array.Empty<Behavior>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
