using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Operations.Domain.Entities;
using Pupitre.Operations.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.Operations.Infrastructure.Redis.Repositories;

internal class OperationMetricRedisRepository : IOperationMetricRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<OperationMetricRedisRepository> _logger;
    private const string KeyPrefix = "operationmetric:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public OperationMetricRedisRepository(IConnectionMultiplexer redis, ILogger<OperationMetricRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(OperationMetric entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<OperationMetric> GetAsync(OperationMetricId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<OperationMetric>(json!)!;
    }

    public async Task UpdateAsync(OperationMetric entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(OperationMetricId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(OperationMetricId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<OperationMetric>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<OperationMetric>>(Array.Empty<OperationMetric>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
