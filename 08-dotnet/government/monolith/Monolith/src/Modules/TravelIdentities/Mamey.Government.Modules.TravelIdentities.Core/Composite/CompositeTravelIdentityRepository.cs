using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Tenant.Core.Domain.ValueObjects;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Entities;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;
using Mamey.Government.Modules.TravelIdentities.Core.EF.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.Mongo.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.Redis.Repositories;
using Mamey.Types;
using Microsoft.Extensions.Logging;
using TenantId = Mamey.Types.TenantId;

namespace Mamey.Government.Modules.TravelIdentities.Core.Composite;

internal class CompositeTravelIdentityRepository : ITravelIdentityRepository
{
    private readonly TravelIdentityMongoRepository _mongoRepo;
    private readonly TravelIdentityRedisRepository _redisRepo;
    private readonly TravelIdentityPostgresRepository _postgresRepo;
    private readonly ILogger<CompositeTravelIdentityRepository> _logger;

    public CompositeTravelIdentityRepository(
        TravelIdentityMongoRepository mongoRepo,
        TravelIdentityRedisRepository redisRepo,
        TravelIdentityPostgresRepository postgresRepo,
        ILogger<CompositeTravelIdentityRepository> logger)
    {
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _postgresRepo = postgresRepo;
        _logger = logger;
    }

    public async Task AddAsync(TravelIdentity entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.AddAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.AddAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task UpdateAsync(TravelIdentity entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.UpdateAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.UpdateAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task DeleteAsync(TravelIdentityId id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        await TryAsync(() => _redisRepo.DeleteAsync(id, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.DeleteAsync(id, cancellationToken), "MongoDB");
    }

    public async Task<TravelIdentity?> GetAsync(TravelIdentityId id, CancellationToken cancellationToken = default)
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

    public async Task<IReadOnlyList<TravelIdentity>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        return await _postgresRepo.BrowseAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(TravelIdentityId id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis exists failed, trying Mongo"); }
        
        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo exists failed, trying Postgres"); }
        
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    public async Task<TravelIdentity?> GetByTravelIdentityNumberAsync(TravelIdentityNumber travelIdentityNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromMongo = await _mongoRepo.GetByTravelIdentityNumberAsync(travelIdentityNumber, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo lookup failed, trying Postgres"); }
        
        return await _postgresRepo.GetByTravelIdentityNumberAsync(travelIdentityNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<TravelIdentity>> GetByCitizenAsync(CitizenId citizenId, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromMongo = await _mongoRepo.GetByCitizenAsync(citizenId, cancellationToken);
            if (fromMongo.Any()) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo lookup failed, trying Postgres"); }
        
        return await _postgresRepo.GetByCitizenAsync(citizenId, cancellationToken);
    }

    public async Task<IReadOnlyList<TravelIdentity>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromMongo = await _mongoRepo.GetByTenantAsync(tenantId, cancellationToken);
            if (fromMongo.Any()) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo lookup failed, trying Postgres"); }
        
        return await _postgresRepo.GetByTenantAsync(tenantId, cancellationToken);
    }

    public Task<int> GetCountByYearAsync(TenantId tenantId, int year, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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
