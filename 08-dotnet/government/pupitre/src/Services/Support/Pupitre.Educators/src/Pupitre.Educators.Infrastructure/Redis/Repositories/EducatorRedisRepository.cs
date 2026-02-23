using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Educators.Domain.Entities;
using Pupitre.Educators.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.Educators.Infrastructure.Redis.Repositories;

internal class EducatorRedisRepository : IEducatorRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<EducatorRedisRepository> _logger;
    private const string KeyPrefix = "educator:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public EducatorRedisRepository(IConnectionMultiplexer redis, ILogger<EducatorRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(Educator entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<Educator> GetAsync(EducatorId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<Educator>(json!)!;
    }

    public async Task UpdateAsync(Educator entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(EducatorId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(EducatorId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<Educator>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Educator>>(Array.Empty<Educator>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
