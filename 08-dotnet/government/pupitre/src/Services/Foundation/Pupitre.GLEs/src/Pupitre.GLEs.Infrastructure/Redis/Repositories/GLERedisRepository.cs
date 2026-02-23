using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.GLEs.Domain.Entities;
using Pupitre.GLEs.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.GLEs.Infrastructure.Redis.Repositories;

internal class GLERedisRepository : IGLERepository
{
    private readonly IDatabase _database;
    private readonly ILogger<GLERedisRepository> _logger;
    private const string KeyPrefix = "gle:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public GLERedisRepository(IConnectionMultiplexer redis, ILogger<GLERedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(GLE entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<GLE> GetAsync(GLEId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<GLE>(json!)!;
    }

    public async Task UpdateAsync(GLE entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(GLEId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(GLEId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<GLE>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<GLE>>(Array.Empty<GLE>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
