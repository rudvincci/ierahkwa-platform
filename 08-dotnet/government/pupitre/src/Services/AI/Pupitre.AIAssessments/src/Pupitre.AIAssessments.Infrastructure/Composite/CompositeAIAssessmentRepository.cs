using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.AIAssessments.Domain.Entities;
using Pupitre.AIAssessments.Domain.Repositories;
using Pupitre.AIAssessments.Infrastructure.EF.Repositories;
using Pupitre.AIAssessments.Infrastructure.Mongo.Repositories;
using Pupitre.AIAssessments.Infrastructure.Redis.Repositories;

namespace Pupitre.AIAssessments.Infrastructure.Composite;

internal class CompositeAIAssessmentRepository : IAIAssessmentRepository
{
    private readonly AIAssessmentPostgresRepository _postgresRepo;
    private readonly AIAssessmentMongoRepository _mongoRepo;
    private readonly AIAssessmentRedisRepository _redisRepo;
    private readonly ILogger<CompositeAIAssessmentRepository> _logger;

    public CompositeAIAssessmentRepository(
        AIAssessmentPostgresRepository postgresRepo,
        AIAssessmentMongoRepository mongoRepo,
        AIAssessmentRedisRepository redisRepo,
        ILogger<CompositeAIAssessmentRepository> logger)
    {
        _postgresRepo = postgresRepo;
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _logger = logger;
    }

    public async Task AddAsync(AIAssessment entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.AddAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.AddAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task<AIAssessment> GetAsync(AIAssessmentId id, CancellationToken cancellationToken = default)
    {
        try { var r = await _redisRepo.GetAsync(id, cancellationToken); if (r != null) return r; }
        catch { _logger.LogWarning("Redis read failed, trying MongoDB"); }
        try { var m = await _mongoRepo.GetAsync(id, cancellationToken); if (m != null) return m; }
        catch { _logger.LogWarning("MongoDB read failed, trying PostgreSQL"); }
        return await _postgresRepo.GetAsync(id, cancellationToken);
    }

    public async Task UpdateAsync(AIAssessment entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.UpdateAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.UpdateAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task DeleteAsync(AIAssessmentId id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        await TryAsync(() => _redisRepo.DeleteAsync(id, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.DeleteAsync(id, cancellationToken), "MongoDB");
    }

    public async Task<bool> ExistsAsync(AIAssessmentId id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; } catch { }
        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; } catch { }
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    public Task<IReadOnlyList<AIAssessment>> BrowseAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(cancellationToken);

    private async Task TryAsync(Func<Task> action, string store)
    {
        try { await action(); }
        catch (Exception ex) { _logger.LogWarning(ex, "Propagation to {Store} failed", store); }
    }
}
