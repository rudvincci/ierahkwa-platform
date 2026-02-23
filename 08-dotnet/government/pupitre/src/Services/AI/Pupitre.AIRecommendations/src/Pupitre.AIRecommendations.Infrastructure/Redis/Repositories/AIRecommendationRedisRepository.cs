using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.AIRecommendations.Domain.Entities;
using Pupitre.AIRecommendations.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.AIRecommendations.Infrastructure.Redis.Repositories;

internal class AIRecommendationRedisRepository : IAIRecommendationRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<AIRecommendationRedisRepository> _logger;
    private const string KeyPrefix = "airecommendation:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public AIRecommendationRedisRepository(IConnectionMultiplexer redis, ILogger<AIRecommendationRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(AIRecommendation entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<AIRecommendation> GetAsync(AIRecommendationId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<AIRecommendation>(json!)!;
    }

    public async Task UpdateAsync(AIRecommendation entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(AIRecommendationId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(AIRecommendationId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<AIRecommendation>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<AIRecommendation>>(Array.Empty<AIRecommendation>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
