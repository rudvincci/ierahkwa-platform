using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Lessons.Domain.Entities;
using Pupitre.Lessons.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.Lessons.Infrastructure.Redis.Repositories;

internal class LessonRedisRepository : ILessonRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<LessonRedisRepository> _logger;
    private const string KeyPrefix = "lesson:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public LessonRedisRepository(IConnectionMultiplexer redis, ILogger<LessonRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(Lesson entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<Lesson> GetAsync(LessonId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<Lesson>(json!)!;
    }

    public async Task UpdateAsync(Lesson entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(LessonId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(LessonId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<Lesson>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Lesson>>(Array.Empty<Lesson>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
