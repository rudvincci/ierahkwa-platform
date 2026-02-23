using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Fundraising.Domain.Entities;
using Pupitre.Fundraising.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.Fundraising.Infrastructure.Redis.Repositories;

internal class CampaignRedisRepository : ICampaignRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<CampaignRedisRepository> _logger;
    private const string KeyPrefix = "campaign:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public CampaignRedisRepository(IConnectionMultiplexer redis, ILogger<CampaignRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(Campaign entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<Campaign> GetAsync(CampaignId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<Campaign>(json!)!;
    }

    public async Task UpdateAsync(Campaign entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(CampaignId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(CampaignId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<Campaign>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Campaign>>(Array.Empty<Campaign>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
