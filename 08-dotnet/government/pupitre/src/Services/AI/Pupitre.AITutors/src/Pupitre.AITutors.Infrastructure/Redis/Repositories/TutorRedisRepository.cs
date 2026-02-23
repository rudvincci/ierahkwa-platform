using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.AITutors.Domain.Entities;
using Pupitre.AITutors.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.AITutors.Infrastructure.Redis.Repositories;

internal class TutorRedisRepository : ITutorRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<TutorRedisRepository> _logger;
    private const string KeyPrefix = "tutor:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public TutorRedisRepository(IConnectionMultiplexer redis, ILogger<TutorRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(Tutor entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<Tutor> GetAsync(TutorId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<Tutor>(json!)!;
    }

    public async Task UpdateAsync(Tutor entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(TutorId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(TutorId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<Tutor>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Tutor>>(Array.Empty<Tutor>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
