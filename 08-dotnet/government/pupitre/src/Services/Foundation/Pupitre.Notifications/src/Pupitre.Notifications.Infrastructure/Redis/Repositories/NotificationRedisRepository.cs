using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Notifications.Domain.Entities;
using Pupitre.Notifications.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.Notifications.Infrastructure.Redis.Repositories;

internal class NotificationRedisRepository : INotificationRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<NotificationRedisRepository> _logger;
    private const string KeyPrefix = "notification:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public NotificationRedisRepository(IConnectionMultiplexer redis, ILogger<NotificationRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(Notification entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<Notification> GetAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<Notification>(json!)!;
    }

    public async Task UpdateAsync(Notification entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<Notification>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Notification>>(Array.Empty<Notification>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
