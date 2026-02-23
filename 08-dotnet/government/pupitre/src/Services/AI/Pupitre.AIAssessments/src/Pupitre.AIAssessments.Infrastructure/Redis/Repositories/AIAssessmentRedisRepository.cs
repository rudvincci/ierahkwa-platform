using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.AIAssessments.Domain.Entities;
using Pupitre.AIAssessments.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.AIAssessments.Infrastructure.Redis.Repositories;

internal class AIAssessmentRedisRepository : IAIAssessmentRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<AIAssessmentRedisRepository> _logger;
    private const string KeyPrefix = "aiassessment:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public AIAssessmentRedisRepository(IConnectionMultiplexer redis, ILogger<AIAssessmentRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(AIAssessment entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<AIAssessment> GetAsync(AIAssessmentId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<AIAssessment>(json!)!;
    }

    public async Task UpdateAsync(AIAssessment entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(AIAssessmentId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(AIAssessmentId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<AIAssessment>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<AIAssessment>>(Array.Empty<AIAssessment>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
