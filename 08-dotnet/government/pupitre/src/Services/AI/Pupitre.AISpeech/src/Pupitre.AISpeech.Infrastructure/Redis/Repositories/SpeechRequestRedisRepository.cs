using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.AISpeech.Domain.Entities;
using Pupitre.AISpeech.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.AISpeech.Infrastructure.Redis.Repositories;

internal class SpeechRequestRedisRepository : ISpeechRequestRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<SpeechRequestRedisRepository> _logger;
    private const string KeyPrefix = "speechrequest:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public SpeechRequestRedisRepository(IConnectionMultiplexer redis, ILogger<SpeechRequestRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(SpeechRequest entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<SpeechRequest> GetAsync(SpeechRequestId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<SpeechRequest>(json!)!;
    }

    public async Task UpdateAsync(SpeechRequest entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(SpeechRequestId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(SpeechRequestId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<SpeechRequest>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<SpeechRequest>>(Array.Empty<SpeechRequest>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
