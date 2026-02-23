using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Compliance.Domain.Entities;
using Pupitre.Compliance.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.Compliance.Infrastructure.Redis.Repositories;

internal class ComplianceRecordRedisRepository : IComplianceRecordRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<ComplianceRecordRedisRepository> _logger;
    private const string KeyPrefix = "compliancerecord:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public ComplianceRecordRedisRepository(IConnectionMultiplexer redis, ILogger<ComplianceRecordRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(ComplianceRecord entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<ComplianceRecord> GetAsync(ComplianceRecordId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<ComplianceRecord>(json!)!;
    }

    public async Task UpdateAsync(ComplianceRecord entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(ComplianceRecordId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(ComplianceRecordId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<ComplianceRecord>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ComplianceRecord>>(Array.Empty<ComplianceRecord>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
