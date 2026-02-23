using Mamey.Government.Modules.Citizens.Core.Domain.Entities;
using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.EF.Repositories;
using Mamey.Government.Modules.Citizens.Core.Mongo.Repositories;
using Mamey.Government.Modules.Citizens.Core.Redis.Repositories;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Citizens.Core.Composite;

internal class CompositeCitizenRepository : ICitizenRepository
{
    private readonly CitizenMongoRepository _mongoRepo;
    private readonly CitizenRedisRepository _redisRepo;
    private readonly CitizenPostgresRepository _postgresRepo;
    private readonly ILogger<CompositeCitizenRepository> _logger;

    public CompositeCitizenRepository(
        CitizenMongoRepository mongoRepo,
        CitizenRedisRepository redisRepo,
        CitizenPostgresRepository postgresRepo,
        ILogger<CompositeCitizenRepository> logger)
    {
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _postgresRepo = postgresRepo;
        _logger = logger;
    }

    public async Task AddAsync(Citizen entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.AddAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.AddAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task UpdateAsync(Citizen entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.UpdateAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.UpdateAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task DeleteAsync(CitizenId id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        await TryAsync(() => _redisRepo.DeleteAsync(id, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.DeleteAsync(id, cancellationToken), "MongoDB");
    }

    public async Task<Citizen?> GetAsync(CitizenId id, CancellationToken cancellationToken = default)
    {
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

    public async Task<IReadOnlyList<Citizen>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        return await _postgresRepo.BrowseAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(CitizenId id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis exists failed, trying Mongo"); }
        
        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo exists failed, trying Postgres"); }
        
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<Citizen>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromMongo = await _mongoRepo.GetByTenantAsync(tenantId, cancellationToken);
            if (fromMongo.Any()) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo lookup failed, trying Postgres"); }
        
        return await _postgresRepo.GetByTenantAsync(tenantId, cancellationToken);
    }

    public async Task<IReadOnlyList<Citizen>> GetByStatusAsync(CitizenshipStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromMongo = await _mongoRepo.GetByStatusAsync(status, cancellationToken);
            if (fromMongo.Any()) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo lookup failed, trying Postgres"); }
        
        return await _postgresRepo.GetByStatusAsync(status, cancellationToken);
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
