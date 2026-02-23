using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.AIVision.Domain.Entities;
using Pupitre.AIVision.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.AIVision.Infrastructure.Redis.Repositories;

internal class VisionAnalysisRedisRepository : IVisionAnalysisRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<VisionAnalysisRedisRepository> _logger;
    private const string KeyPrefix = "visionanalysis:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public VisionAnalysisRedisRepository(IConnectionMultiplexer redis, ILogger<VisionAnalysisRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(VisionAnalysis entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<VisionAnalysis> GetAsync(VisionAnalysisId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<VisionAnalysis>(json!)!;
    }

    public async Task UpdateAsync(VisionAnalysis entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(VisionAnalysisId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(VisionAnalysisId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<VisionAnalysis>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<VisionAnalysis>>(Array.Empty<VisionAnalysis>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
