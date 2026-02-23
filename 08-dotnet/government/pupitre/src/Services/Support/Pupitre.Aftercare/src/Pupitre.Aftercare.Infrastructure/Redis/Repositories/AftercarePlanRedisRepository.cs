using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Aftercare.Domain.Entities;
using Pupitre.Aftercare.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.Aftercare.Infrastructure.Redis.Repositories;

internal class AftercarePlanRedisRepository : IAftercarePlanRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<AftercarePlanRedisRepository> _logger;
    private const string KeyPrefix = "aftercareplan:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public AftercarePlanRedisRepository(IConnectionMultiplexer redis, ILogger<AftercarePlanRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(AftercarePlan entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<AftercarePlan> GetAsync(AftercarePlanId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<AftercarePlan>(json!)!;
    }

    public async Task UpdateAsync(AftercarePlan entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(AftercarePlanId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(AftercarePlanId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<AftercarePlan>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<AftercarePlan>>(Array.Empty<AftercarePlan>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
