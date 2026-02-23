using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.AIContent.Domain.Entities;
using Pupitre.AIContent.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.AIContent.Infrastructure.Redis.Repositories;

internal class ContentGenerationRedisRepository : IContentGenerationRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<ContentGenerationRedisRepository> _logger;
    private const string KeyPrefix = "contentgeneration:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public ContentGenerationRedisRepository(IConnectionMultiplexer redis, ILogger<ContentGenerationRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(ContentGeneration entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<ContentGeneration> GetAsync(ContentGenerationId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<ContentGeneration>(json!)!;
    }

    public async Task UpdateAsync(ContentGeneration entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(ContentGenerationId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(ContentGenerationId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<ContentGeneration>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ContentGeneration>>(Array.Empty<ContentGeneration>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
