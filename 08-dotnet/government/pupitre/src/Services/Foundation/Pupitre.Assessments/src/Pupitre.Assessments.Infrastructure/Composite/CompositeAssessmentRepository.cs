using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Assessments.Domain.Entities;
using Pupitre.Assessments.Domain.Repositories;
using Pupitre.Assessments.Infrastructure.EF.Repositories;
using Pupitre.Assessments.Infrastructure.Mongo.Repositories;
using Pupitre.Assessments.Infrastructure.Redis.Repositories;

namespace Pupitre.Assessments.Infrastructure.Composite;

internal class CompositeAssessmentRepository : IAssessmentRepository
{
    private readonly AssessmentPostgresRepository _postgresRepo;
    private readonly AssessmentMongoRepository _mongoRepo;
    private readonly AssessmentRedisRepository _redisRepo;
    private readonly ILogger<CompositeAssessmentRepository> _logger;

    public CompositeAssessmentRepository(
        AssessmentPostgresRepository postgresRepo,
        AssessmentMongoRepository mongoRepo,
        AssessmentRedisRepository redisRepo,
        ILogger<CompositeAssessmentRepository> logger)
    {
        _postgresRepo = postgresRepo;
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _logger = logger;
    }

    public async Task AddAsync(Assessment entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.AddAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.AddAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task<Assessment> GetAsync(AssessmentId id, CancellationToken cancellationToken = default)
    {
        try { var r = await _redisRepo.GetAsync(id, cancellationToken); if (r != null) return r; }
        catch { _logger.LogWarning("Redis read failed, trying MongoDB"); }
        try { var m = await _mongoRepo.GetAsync(id, cancellationToken); if (m != null) return m; }
        catch { _logger.LogWarning("MongoDB read failed, trying PostgreSQL"); }
        return await _postgresRepo.GetAsync(id, cancellationToken);
    }

    public async Task UpdateAsync(Assessment entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.UpdateAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.UpdateAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task DeleteAsync(AssessmentId id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        await TryAsync(() => _redisRepo.DeleteAsync(id, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.DeleteAsync(id, cancellationToken), "MongoDB");
    }

    public async Task<bool> ExistsAsync(AssessmentId id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; } catch { }
        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; } catch { }
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    public Task<IReadOnlyList<Assessment>> BrowseAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(cancellationToken);

    private async Task TryAsync(Func<Task> action, string store)
    {
        try { await action(); }
        catch (Exception ex) { _logger.LogWarning(ex, "Propagation to {Store} failed", store); }
    }
}
