using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Ministries.Domain.Entities;
using Pupitre.Ministries.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.Ministries.Infrastructure.Redis.Repositories;

internal class MinistryDataRedisRepository : IMinistryDataRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<MinistryDataRedisRepository> _logger;
    private const string KeyPrefix = "ministrydata:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public MinistryDataRedisRepository(IConnectionMultiplexer redis, ILogger<MinistryDataRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(MinistryData entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<MinistryData> GetAsync(MinistryDataId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<MinistryData>(json!)!;
    }

    public async Task UpdateAsync(MinistryData entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(MinistryDataId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(MinistryDataId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<MinistryData>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<MinistryData>>(Array.Empty<MinistryData>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
