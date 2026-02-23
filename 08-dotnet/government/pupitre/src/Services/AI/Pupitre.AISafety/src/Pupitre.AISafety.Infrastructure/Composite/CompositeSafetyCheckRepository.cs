using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.AISafety.Domain.Entities;
using Pupitre.AISafety.Domain.Repositories;
using Pupitre.AISafety.Infrastructure.EF.Repositories;
using Pupitre.AISafety.Infrastructure.Mongo.Repositories;
using Pupitre.AISafety.Infrastructure.Redis.Repositories;

namespace Pupitre.AISafety.Infrastructure.Composite;

internal class CompositeSafetyCheckRepository : ISafetyCheckRepository
{
    private readonly SafetyCheckPostgresRepository _postgresRepo;
    private readonly SafetyCheckMongoRepository _mongoRepo;
    private readonly SafetyCheckRedisRepository _redisRepo;
    private readonly ILogger<CompositeSafetyCheckRepository> _logger;

    public CompositeSafetyCheckRepository(
        SafetyCheckPostgresRepository postgresRepo,
        SafetyCheckMongoRepository mongoRepo,
        SafetyCheckRedisRepository redisRepo,
        ILogger<CompositeSafetyCheckRepository> logger)
    {
        _postgresRepo = postgresRepo;
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _logger = logger;
    }

    public async Task AddAsync(SafetyCheck entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.AddAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.AddAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task<SafetyCheck> GetAsync(SafetyCheckId id, CancellationToken cancellationToken = default)
    {
        try { var r = await _redisRepo.GetAsync(id, cancellationToken); if (r != null) return r; }
        catch { _logger.LogWarning("Redis read failed, trying MongoDB"); }
        try { var m = await _mongoRepo.GetAsync(id, cancellationToken); if (m != null) return m; }
        catch { _logger.LogWarning("MongoDB read failed, trying PostgreSQL"); }
        return await _postgresRepo.GetAsync(id, cancellationToken);
    }

    public async Task UpdateAsync(SafetyCheck entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.UpdateAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.UpdateAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task DeleteAsync(SafetyCheckId id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        await TryAsync(() => _redisRepo.DeleteAsync(id, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.DeleteAsync(id, cancellationToken), "MongoDB");
    }

    public async Task<bool> ExistsAsync(SafetyCheckId id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; } catch { }
        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; } catch { }
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    public Task<IReadOnlyList<SafetyCheck>> BrowseAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(cancellationToken);

    private async Task TryAsync(Func<Task> action, string store)
    {
        try { await action(); }
        catch (Exception ex) { _logger.LogWarning(ex, "Propagation to {Store} failed", store); }
    }
}
