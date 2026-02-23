using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Assessments.Domain.Entities;
using Pupitre.Assessments.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.Assessments.Infrastructure.Redis.Repositories;

internal class AssessmentRedisRepository : IAssessmentRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<AssessmentRedisRepository> _logger;
    private const string KeyPrefix = "assessment:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public AssessmentRedisRepository(IConnectionMultiplexer redis, ILogger<AssessmentRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(Assessment entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<Assessment> GetAsync(AssessmentId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<Assessment>(json!)!;
    }

    public async Task UpdateAsync(Assessment entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(AssessmentId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(AssessmentId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<Assessment>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Assessment>>(Array.Empty<Assessment>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
