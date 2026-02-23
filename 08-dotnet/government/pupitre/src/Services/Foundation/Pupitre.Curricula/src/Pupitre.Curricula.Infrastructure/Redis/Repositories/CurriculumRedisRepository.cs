using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Curricula.Domain.Entities;
using Pupitre.Curricula.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.Curricula.Infrastructure.Redis.Repositories;

internal class CurriculumRedisRepository : ICurriculumRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<CurriculumRedisRepository> _logger;
    private const string KeyPrefix = "curriculum:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public CurriculumRedisRepository(IConnectionMultiplexer redis, ILogger<CurriculumRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(Curriculum entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<Curriculum> GetAsync(CurriculumId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<Curriculum>(json!)!;
    }

    public async Task UpdateAsync(Curriculum entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(CurriculumId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(CurriculumId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<Curriculum>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Curriculum>>(Array.Empty<Curriculum>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
