using Mamey.Government.Modules.Notifications.Core.Domain.Entities;
using Mamey.Government.Modules.Notifications.Core.Domain.Repositories;
using Mamey.Government.Modules.Notifications.Core.Domain.Types;
using Mamey.Government.Modules.Notifications.Core.EF.Repositories;
using Mamey.Government.Modules.Notifications.Core.Mongo.Repositories;
using Mamey.Government.Modules.Notifications.Core.Redis.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Notifications.Core.Composite;

internal class CompositeNotificationRepository : INotificationRepository
{
    private readonly NotificationsMongoRepository _mongoRepo;
    private readonly NotificationRedisRepository _redisRepo;
    private readonly NotificationPostgresRepository _postgresRepo;
    private readonly ILogger<CompositeNotificationRepository> _logger;

    public CompositeNotificationRepository(
        NotificationsMongoRepository mongoRepo,
        NotificationRedisRepository redisRepo,
        NotificationPostgresRepository postgresRepo,
        ILogger<CompositeNotificationRepository> logger)
    {
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _postgresRepo = postgresRepo;
        _logger = logger;
    }

    public async Task AddAsync(Notification entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.AddAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.AddAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task UpdateAsync(Notification entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.UpdateAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.UpdateAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task DeleteAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        await TryAsync(() => _redisRepo.DeleteAsync(id, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.DeleteAsync(id, cancellationToken), "MongoDB");
    }

    public async Task<Notification?> GetAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        // Try Redis → Mongo → Postgres
        try
        {
            var fromRedis = await _redisRepo.GetAsync(id, cancellationToken);
            if (fromRedis != null) return fromRedis;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetAsync(id, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo read failed, trying Postgres"); }
        
        return await _postgresRepo.GetAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        // Always use PostgreSQL for complex queries
        return await _postgresRepo.BrowseAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis exists failed, trying Mongo"); }
        
        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo exists failed, trying Postgres"); }
        
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    private async Task TryAsync(Func<Task> action, string storeName)
    {
        try 
        { 
            await action(); 
            _logger.LogDebug("Successfully propagated to {StoreName}", storeName);
        }
        catch (Exception ex) 
        { 
            _logger.LogWarning(ex, "Best-effort propagation to {StoreName} failed: {ErrorMessage}", storeName, ex.Message); 
        }
    }
}
