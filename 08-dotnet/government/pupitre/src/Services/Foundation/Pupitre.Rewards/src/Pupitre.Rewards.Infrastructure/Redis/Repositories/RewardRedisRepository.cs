using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Rewards.Domain.Entities;
using Pupitre.Rewards.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.Rewards.Infrastructure.Redis.Repositories;

internal class RewardRedisRepository : IRewardRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<RewardRedisRepository> _logger;
    private const string KeyPrefix = "reward:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public RewardRedisRepository(IConnectionMultiplexer redis, ILogger<RewardRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(Reward entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<Reward> GetAsync(RewardId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<Reward>(json!)!;
    }

    public async Task UpdateAsync(Reward entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(RewardId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(RewardId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<Reward>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Reward>>(Array.Empty<Reward>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
