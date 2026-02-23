using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.AIAdaptive.Domain.Entities;
using Pupitre.AIAdaptive.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.AIAdaptive.Infrastructure.Redis.Repositories;

internal class AdaptiveLearningRedisRepository : IAdaptiveLearningRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<AdaptiveLearningRedisRepository> _logger;
    private const string KeyPrefix = "adaptivelearning:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public AdaptiveLearningRedisRepository(IConnectionMultiplexer redis, ILogger<AdaptiveLearningRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(AdaptiveLearning entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<AdaptiveLearning> GetAsync(AdaptiveLearningId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<AdaptiveLearning>(json!)!;
    }

    public async Task UpdateAsync(AdaptiveLearning entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(AdaptiveLearningId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(AdaptiveLearningId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<AdaptiveLearning>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<AdaptiveLearning>>(Array.Empty<AdaptiveLearning>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
