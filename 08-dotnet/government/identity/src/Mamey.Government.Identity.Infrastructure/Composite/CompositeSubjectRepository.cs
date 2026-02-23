using System;
using System.Collections.Generic;
using System.Linq;
using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Mongo.Repositories;
using Mamey.Government.Identity.Infrastructure.Redis.Repositories;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Composite;

internal class CompositeSubjectRepository : ISubjectRepository
{
    private readonly SubjectMongoRepository _mongoRepo;
    private readonly SubjectRedisRepository _redisRepo;
    private readonly SubjectPostgresRepository _postgresRepo;
    private readonly ILogger<CompositeSubjectRepository> _logger;

    public CompositeSubjectRepository(
        SubjectMongoRepository mongoRepo,
        SubjectRedisRepository redisRepo,
        SubjectPostgresRepository postgresRepo,
        ILogger<CompositeSubjectRepository> logger)
    {
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _postgresRepo = postgresRepo;
        _logger = logger;
    }

    public async Task AddAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddAsync(subject, cancellationToken);
        Try(() => _redisRepo.AddAsync(subject, cancellationToken));
        Try(() => _mongoRepo.AddAsync(subject, cancellationToken));
    }

    public async Task UpdateAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(subject, cancellationToken);
        Try(() => _redisRepo.UpdateAsync(subject, cancellationToken));
        Try(() => _mongoRepo.UpdateAsync(subject, cancellationToken));
    }

    public async Task DeleteAsync(SubjectId id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        Try(() => _redisRepo.DeleteAsync(id, cancellationToken));
        Try(() => _mongoRepo.DeleteAsync(id, cancellationToken));
    }

    public async Task<Subject> GetAsync(SubjectId id, CancellationToken cancellationToken = default)
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

    public Task<IReadOnlyList<Subject>> BrowseAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(cancellationToken);

    public Task<PagedResult<Subject>> BrowseAsync(Func<Subject, bool> predicate, PagedQueryBase query, CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(predicate, query, cancellationToken);

    public async Task<bool> ExistsAsync(SubjectId id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis exists failed, trying Mongo"); }

        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo exists failed, trying Postgres"); }

        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    public async Task<Subject> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromRedis = await _redisRepo.GetByEmailAsync(email, cancellationToken);
            if (fromRedis != null) return fromRedis;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Mongo"); }

        try
        {
            var fromMongo = await _mongoRepo.GetByEmailAsync(email, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo read failed, trying Postgres"); }

        return await _postgresRepo.GetByEmailAsync(email, cancellationToken);
    }

    public async Task<Subject> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var subject = await GetByEmailAsync(email.Value, cancellationToken);
        return subject;
    }

    public Task<IReadOnlyList<Subject>> GetByStatusAsync(SubjectStatus status, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByStatusAsync(status, cancellationToken);

    public Task<IReadOnlyList<Subject>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByRoleIdAsync(roleId, cancellationToken);

    public Task<IReadOnlyList<Subject>> GetByTagAsync(string tag, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByTagAsync(tag, cancellationToken);

    public Task<IReadOnlyList<Subject>> GetActiveSubjectsAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.GetActiveSubjectsAsync(cancellationToken);

    public Task<IReadOnlyList<Subject>> GetRecentlyAuthenticatedAsync(DateTime since, CancellationToken cancellationToken = default)
        => _postgresRepo.GetRecentlyAuthenticatedAsync(since, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => _postgresRepo.EmailExistsAsync(email, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, SubjectId excludeId, CancellationToken cancellationToken = default)
        => _postgresRepo.EmailExistsAsync(email, excludeId, cancellationToken);

    public Task<bool> HasRoleAsync(SubjectId subjectId, RoleId roleId, CancellationToken cancellationToken = default)
        => _postgresRepo.HasRoleAsync(subjectId, roleId, cancellationToken);

    public Task<bool> HasTagAsync(SubjectId subjectId, string tag, CancellationToken cancellationToken = default)
        => _postgresRepo.HasTagAsync(subjectId, tag, cancellationToken);

    private void Try(Func<Task> action)
    {
        try { action().GetAwaiter().GetResult(); }
        catch (Exception ex) { _logger.LogWarning(ex, "Best-effort propagation failed"); }
    }
}
