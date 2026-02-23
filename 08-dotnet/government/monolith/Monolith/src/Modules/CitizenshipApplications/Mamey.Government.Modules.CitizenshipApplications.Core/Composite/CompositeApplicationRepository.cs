using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Government.Modules.CitizenshipApplications.Core.EF.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Mongo.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Redis.Repositories;
using Mamey.Types;
using Microsoft.Extensions.Logging;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Composite;

internal class CompositeApplicationRepository : IApplicationRepository
{
    private readonly ApplicationMongoRepository _mongoRepo;
    private readonly ApplicationRedisRepository _redisRepo;
    private readonly ApplicationPostgresRepository _postgresRepo;
    private readonly ILogger<CompositeApplicationRepository> _logger;

    public CompositeApplicationRepository(
        ApplicationMongoRepository mongoRepo,
        ApplicationRedisRepository redisRepo,
        ApplicationPostgresRepository postgresRepo,
        ILogger<CompositeApplicationRepository> logger)
    {
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _postgresRepo = postgresRepo;
        _logger = logger;
    }

    public async Task AddAsync(CitizenshipApplication entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.AddAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.AddAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task UpdateAsync(CitizenshipApplication entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.UpdateAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.UpdateAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task DeleteAsync(AppId id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        await TryAsync(() => _redisRepo.DeleteAsync(id, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.DeleteAsync(id, cancellationToken), "MongoDB");
    }

    public async Task<CitizenshipApplication?> GetAsync(AppId id, CancellationToken cancellationToken = default)
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

    public async Task<IReadOnlyList<CitizenshipApplication>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        return await _postgresRepo.BrowseAsync(cancellationToken);
    }

    public async Task<IList<CitizenshipApplication>> GetAllByApplicationEmail(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromMongo = await _mongoRepo.GetAllByApplicationEmail(email, cancellationToken);
            if (fromMongo.Any()) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo lookup failed, trying Postgres"); }
        
        return await _postgresRepo.GetAllByApplicationEmail(email, cancellationToken);
    }

    public async Task<CitizenshipApplication?> GetByApplicationEmail(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromMongo = await _mongoRepo.GetByApplicationEmail(email, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo lookup failed, trying Postgres"); }
        
        return await _postgresRepo.GetByApplicationEmail(email, cancellationToken);
    }

    public async Task<bool> ExistsAsync(AppId id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis exists failed, trying Mongo"); }
        
        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo exists failed, trying Postgres"); }
        
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    public async Task<CitizenshipApplication?> GetByApplicationNumberAsync(ApplicationNumber applicationNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromMongo = await _mongoRepo.GetByApplicationNumberAsync(applicationNumber, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo lookup failed, trying Postgres"); }
        
        return await _postgresRepo.GetByApplicationNumberAsync(applicationNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<CitizenshipApplication>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromMongo = await _mongoRepo.GetByTenantAsync(tenantId, cancellationToken);
            if (fromMongo.Any()) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo lookup failed, trying Postgres"); }
        
        return await _postgresRepo.GetByTenantAsync(tenantId, cancellationToken);
    }

    public async Task<IReadOnlyList<CitizenshipApplication>> GetByStatusAsync(ApplicationStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromMongo = await _mongoRepo.GetByStatusAsync(status, cancellationToken);
            if (fromMongo.Any()) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo lookup failed, trying Postgres"); }
        
        return await _postgresRepo.GetByStatusAsync(status, cancellationToken);
    }

    public async Task<int> GetCountByYearAsync(TenantId tenantId, int year, CancellationToken cancellationToken = default)
    {
        return await _postgresRepo.GetCountByYearAsync(tenantId, year, cancellationToken);
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
