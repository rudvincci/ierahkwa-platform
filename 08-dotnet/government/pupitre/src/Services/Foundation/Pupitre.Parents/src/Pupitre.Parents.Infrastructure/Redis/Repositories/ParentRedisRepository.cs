using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Parents.Domain.Entities;
using Pupitre.Parents.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.Parents.Infrastructure.Redis.Repositories;

internal class ParentRedisRepository : IParentRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<ParentRedisRepository> _logger;
    private const string KeyPrefix = "parent:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public ParentRedisRepository(IConnectionMultiplexer redis, ILogger<ParentRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(Parent entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<Parent> GetAsync(ParentId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<Parent>(json!)!;
    }

    public async Task UpdateAsync(Parent entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(ParentId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(ParentId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<Parent>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Parent>>(Array.Empty<Parent>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
