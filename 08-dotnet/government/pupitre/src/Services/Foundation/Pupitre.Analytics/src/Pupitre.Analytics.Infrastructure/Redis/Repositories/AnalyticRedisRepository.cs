using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Analytics.Domain.Entities;
using Pupitre.Analytics.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.Analytics.Infrastructure.Redis.Repositories;

internal class AnalyticRedisRepository : IAnalyticRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<AnalyticRedisRepository> _logger;
    private const string KeyPrefix = "analytic:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public AnalyticRedisRepository(IConnectionMultiplexer redis, ILogger<AnalyticRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(Analytic entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<Analytic> GetAsync(AnalyticId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<Analytic>(json!)!;
    }

    public async Task UpdateAsync(Analytic entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(AnalyticId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(AnalyticId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<Analytic>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Analytic>>(Array.Empty<Analytic>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
