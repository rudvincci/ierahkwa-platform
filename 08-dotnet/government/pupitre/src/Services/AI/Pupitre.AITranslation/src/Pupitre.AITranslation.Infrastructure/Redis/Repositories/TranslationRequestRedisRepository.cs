using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.AITranslation.Domain.Entities;
using Pupitre.AITranslation.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.AITranslation.Infrastructure.Redis.Repositories;

internal class TranslationRequestRedisRepository : ITranslationRequestRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<TranslationRequestRedisRepository> _logger;
    private const string KeyPrefix = "translationrequest:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public TranslationRequestRedisRepository(IConnectionMultiplexer redis, ILogger<TranslationRequestRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(TranslationRequest entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<TranslationRequest> GetAsync(TranslationRequestId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<TranslationRequest>(json!)!;
    }

    public async Task UpdateAsync(TranslationRequest entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(TranslationRequestId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(TranslationRequestId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<TranslationRequest>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<TranslationRequest>>(Array.Empty<TranslationRequest>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
