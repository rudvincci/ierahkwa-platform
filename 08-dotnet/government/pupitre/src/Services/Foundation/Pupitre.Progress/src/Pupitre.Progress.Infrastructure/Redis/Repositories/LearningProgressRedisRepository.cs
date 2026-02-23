using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Progress.Domain.Entities;
using Pupitre.Progress.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.Progress.Infrastructure.Redis.Repositories;

internal class LearningProgressRedisRepository : ILearningProgressRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<LearningProgressRedisRepository> _logger;
    private const string KeyPrefix = "learningprogress:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public LearningProgressRedisRepository(IConnectionMultiplexer redis, ILogger<LearningProgressRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(LearningProgress entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<LearningProgress> GetAsync(LearningProgressId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<LearningProgress>(json!)!;
    }

    public async Task UpdateAsync(LearningProgress entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(LearningProgressId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(LearningProgressId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<LearningProgress>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<LearningProgress>>(Array.Empty<LearningProgress>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
